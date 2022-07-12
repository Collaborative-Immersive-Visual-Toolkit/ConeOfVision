
from cv2 import CV_32SC1
import pandas as pd
import matplotlib.pyplot as plt
import matplotlib
import numpy as np
from scipy.ndimage.filters import gaussian_filter
from shapely.geometry import Point
from shapely.geometry.polygon import Polygon
import matplotlib.pyplot as plt
from matplotlib.path import Path
from matplotlib.patches import PathPatch
from matplotlib.collections import PatchCollection

GIW = pd.read_pickle("GIW.pkl") 
SGaze = pd.read_pickle("SGaze.pkl")
Gaze360 =  pd.read_pickle("360_VR_gaze.pkl")

columns=["GazeFoVDegreesX",
        "GazeFoVDegreesY",
        "headVelX",
        "headVelY",
        "headVelAng",
        "headVelMagnitude",
        "headAccX",
        "headAccY",
        "headAccAng",
        "headAccMagnitude",
        "user",
        "task",
        "usage"]


strs = ["90%","80%","70%","60%","50%","40%","30%"]
manual_locations = [(0, -22.3),(0, -22.3), (0, -16.4), (0, -13.1), (0, -10.7), (0, -9.1), (0, -7.5)]
percentages = [.9,.8,.7,.6,.5,.4,.3]
skip=[1,3,5,6]

px=[]
py=[]
rpx=[]
rpy=[]



# function that generates the percentile treshold used to create the contours
def generateBounds(grid,percentages=percentages):

    values=[]

    for val in percentages:
        percentage = 1 - (val)
        epsilon = 0.01

        current = 0
        value = np.mean(grid)
        i = grid.min()
        j = grid.max()
        while True:
            partial = grid[grid <= value]
            current = np.sum(partial)/grid.sum()

            #input([current, value, np.sum(partial)])
            if (current > percentage + epsilon):
                j = value
                value = (i + value)*0.5


                #print("to dec:", value)
            elif (current < percentage - epsilon):
                i = value
                value = (j + value)*0.5

                #print("to inc:", value)
            else:
                break

        values.append(value)

    return values

# Generate a 3D surface
def surface3D(grid, gridx, gridy, bounds, axe, min = -1.2,max= 1.2 ):

    #normalize all data
    gridx = gridx/gridx.max()
    gridy = gridy/gridy.max()
    grid = gaussian_filter(grid, sigma=9)
    bounds = bounds/grid.max()
    grid = grid/grid.max()
    gridx, gridy = np.meshgrid(gridx, gridy)

    cs = axe.contour(gridx, gridy, grid, [bounds[2]], linewidths=3, colors="k", linestyles="solid")

    axe.grid(False)
    axe.xaxis.set_pane_color((1.0, 1.0, 1.0, 0.0))
    axe.yaxis.set_pane_color((1.0, 1.0, 1.0, 0.0))
    axe.zaxis.set_pane_color((1.0, 1.0, 1.0, 0.0))
    axe.set_zticks([])
    axe.set_xticks(np.linspace(-1, 1, 5))
    axe.set_yticks(np.linspace(-1, 1, 5))
    axe.set_xticklabels(np.linspace(min, max, 5))
    axe.set_yticklabels(np.linspace(min, max, 5))
    axe.view_init(elev=25, azim=45)
    #fig.colorbar(surf, shrink=0.5, aspect=5)
    #axe.plot_surface(gridx, gridy, grid, cmap=matplotlib.cm.coolwarm, linewidth=0, antialiased=True )

# Generate grid and bounds
def generateGridAndBounds(x,y, resolution = 512, min = -1.2,max= 1.2 ):

    grid, gridx, gridy = coordinatestoGrid(x,y, resolution, min , max)
    bounds = generateBounds(grid)

    return grid, gridx, gridy, bounds

#plot a heatmap
def plotheatmap(grid, gridx, gridy, bounds, axe, min = -1.2,max= 1.2 ):

    #colormaps
    cmap = matplotlib.cm.coolwarm #plt.cm.jet  # define the colormap
    cmaplist = [cmap(i) for i in range(cmap.N)]
    cmap = matplotlib.colors.LinearSegmentedColormap.from_list( 'Custom cmap', cmaplist, cmap.N)
    norm = matplotlib.colors.BoundaryNorm(bounds, cmap.N) #discrete map based on bounds

    #plotmesh
    axe.set_xlim([min,max])
    axe.set_ylim([min,max])
    mesh  = axe.pcolormesh(gridx, gridy, grid, cmap=cmap, norm=norm, shading='gouraud',zorder=0) # discrete

    #colorbar
    # norm= matplotlib.colors.Normalize(vmin=0, vmax=1)
    # sm = plt.cm.ScalarMappable(cmap=cmap, norm=norm)
    # sm.set_array([])
    # N = 10

    # strs = [ f'{item:.1f}' for item in np.linspace(0,1,8)]

    # # Add colorbar, make sure to specify tick locations to match desired ticklabels
    # cbar = plt.colorbar(sm,  boundaries=bounds)
    # cbar.ax.set_yticklabels(strs)  # vertically oriented colorbar

    cmap = matplotlib.cm.coolwarm
    norm = matplotlib.colors.Normalize(vmin=0, vmax=1)

    cbar = plt.colorbar(matplotlib.cm.ScalarMappable(norm=norm, cmap=cmap))
    cbar.ax.tick_params(labelsize=13)
    # ticklabs = cbar.ax.get_yticklabels()
    # cbar.ax.set_yticklabels(ticklabs, fontsize=13)

# Generate conoturs
def generateContours(grid, gridx, gridy, bounds, axe, smoothing=15, strs=strs, manual_locations=manual_locations, skip=skip):

    smoothgrid = gaussian_filter(grid, sigma=smoothing)

    # boundstobeplotted = [ item for index,item in enumerate(bounds) if index not in skip]
    # strtobeplotted = [ item for index,item in enumerate(strs) if index not in skip]
    # manual_locationstobeplotted = [ item for index,item in enumerate(manual_locations) if index not in skip]

    CS=[]

    for i,b in enumerate(bounds):
        if i not in skip: alpha = 1
        else:alpha=0
        CS1 = axe.contour(gridx, gridy, smoothgrid, [b],  colors="w", linestyles="solid", alpha=alpha)
        fmt = {}
        for l, s in zip(CS1.levels, [strs[i]]):
            fmt[l] = s
        plt.clabel(CS1,  fontsize=13, fmt=fmt, manual=[manual_locations[i]]) # manual=True
        CS.append(CS1)
    #CS = axe.contour(gridx, gridy, smoothgrid, bounds,  colors="k", linestyles="solid", alpha=1)

    # fmt = {}
    # for l, s in zip(CS.levels, strs):
    #     fmt[l] = s


    

    return CS

# Save a conotur as a npy np
def SaveContours(contour):

    contours =[]

    for cs in contour.allsegs:

        polygon = Polygon(cs[0])
        simplified = polygon.simplify(0.5, preserve_topology=False)
        list(polygon.exterior.coords)
        points = np.array(list(polygon.exterior.coords))
        contours.append(points)
        #simplified = simplified.buffer(50, join_style=1).buffer(-50.0, join_style=1)
        #plot_polygon(axe, simplified, facecolor=(0., 0., 0., 0.), edgecolor='k')

    with open('contour.npy', 'wb') as f:
        np.save(f, contours)

# Plots a Polygon to pyplot `ax`
def plot_polygon(ax, poly, **kwargs):
    path = Path.make_compound_path(
        Path(np.asarray(poly.exterior.coords)[:, :2]),
        *[Path(np.asarray(ring.coords)[:, :2]) for ring in poly.interiors])

    patch = PathPatch(path, **kwargs)
    collection = PatchCollection([patch], **kwargs)

    ax.add_collection(collection, autolim=True)
    ax.autoscale_view()
    return collection

# transform points to a grid of cells with an associated percentage of points contained
def coordinatestoGrid(x,y, resolution,min,max):

    gridx = np.linspace(min, max, resolution+1)
    gridy = np.linspace(min, max, resolution+1)
    grid, _, _ = np.histogram2d(x, y, bins=[gridx, gridy])
    grid = grid.transpose()

    return grid/np.sum(grid), gridx[:-1], gridy[:-1]

#check how much data from a specific dataset is contined within a generated set of contours
def testContours(dataset,contour,skip=skip):

    count=0
    values=[]


    for i,cs in enumerate(contour):
        
        cs = cs.allsegs

        if( len(cs[0][0])>3):
            polygon = Polygon(cs[0][0])

            total = 10000 #len(SGaze)
            count=0

            u1filtered = dataset.sample(n=total, random_state=1)
            u1filtered = u1filtered[np.logical_and(u1filtered["GazeFoVDegreesX"]>-50,u1filtered["GazeFoVDegreesX"]<50)]
            u1filtered = u1filtered[np.logical_and(u1filtered["GazeFoVDegreesY"]>-50,u1filtered["GazeFoVDegreesY"]<50)]

            for index, row  in u1filtered.iterrows():

                x = row['GazeFoVDegreesX']
                y = row['GazeFoVDegreesY']
                point = Point(x,y)
                if polygon.contains(point):
                    count+=1

            stringValue = f"{((count/total)*100):.1f}%"
            values.append(stringValue)
            print(str(strs[i])+' captures -> '+stringValue)

        count+=1

    return values

# filter data to remove gaze samples beyond the 50 and -50 degree angles
# as they are mechanically impossible for the eyes thus probably wrong
def filterData(data):

    filtered = data
    filtered = filtered[np.logical_and(filtered["GazeFoVDegreesX"]>-50,filtered["GazeFoVDegreesX"]<50)]
    filtered = filtered[np.logical_and(filtered["GazeFoVDegreesY"]>-50,filtered["GazeFoVDegreesY"]<50)]
    x = filtered['GazeFoVDegreesX'].values.tolist()
    y = filtered['GazeFoVDegreesY'].values.tolist()

    return x,y


# fig = plt.figure()
# axes = plt.axes(projection='3d')
# plt.tight_layout()

# x,y = filterData(Gaze360)
# grid1, gridx1, gridy1, bounds1 = generateGridAndBounds(x,y, 256, min = -50,max= 50)
# surface3D(grid1, gridx1, gridy1, bounds1, axes, min = -50,max= 50)
# plt.show()



fig = plt.figure(figsize=(5, 4), dpi=100)
axes = fig.add_subplot(111)
axes.tick_params(axis='both', which='major', labelsize=13)
plt.tight_layout()

x,y = filterData(Gaze360)
grid1, gridx1, gridy1, bounds1 = generateGridAndBounds(x,y, 256, min = -50,max= 50)
plotheatmap(grid1, gridx1, gridy1, bounds1, axes, min = -50,max= 50 )
CS = generateContours(grid1, gridx1, gridy1, bounds1, axes, smoothing=17)
plt.show()


#######

values = testContours(SGaze,CS)


fig = plt.figure(figsize=(5, 4), dpi=100)
axes = fig.add_subplot(111)
axes.tick_params(axis='both', which='major', labelsize=13)
plt.tight_layout()

x2,y2 = filterData(SGaze)
grid2, gridx2, gridy2, bounds2 = generateGridAndBounds(x2,y2, 256, min = -50,max= 50)
plotheatmap(grid2, gridx2, gridy2, bounds2, axes, min = -50,max= 50 )
CS = generateContours(grid1, gridx1, gridy1, bounds1, axes, smoothing=17, strs=values)
plt.show()

########

values = testContours(GIW,CS)

fig = plt.figure(figsize=(5, 4), dpi=100)
axes = fig.add_subplot(111)
axes.tick_params(axis='both', which='major', labelsize=13)
plt.tight_layout()

x2,y2 = filterData(GIW)
grid2, gridx2, gridy2, bounds2 = generateGridAndBounds(x2,y2, 256, min = -50,max= 50)
plotheatmap(grid2, gridx2, gridy2, bounds2, axes, min = -50,max= 50 )
CS = generateContours(grid1, gridx1, gridy1, bounds1, axes, smoothing=17, strs=values)
plt.show()



