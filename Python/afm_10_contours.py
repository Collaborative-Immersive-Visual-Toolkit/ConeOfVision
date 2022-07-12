
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
import math
import json


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

strs = ["100%","90%","80%","70%","60%","50%","40%","30%","20%","10%"]
manual_locations = [(0, -22.3),(0, -22.3),(0, -22.3), (0, -16.4), (0, -13.1), (0, -10.7), (0, -9.1),(0, -9.1),(0, -1.1),(0, -1.1)]
percentages = [1,.9,.8,.7,.6,.5,.4,.3,.2,.1]
dictionaryNames=["p1","p09","p08","p07","p06","p05","p04","p03","p02","p01"]


skip=[]

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

def saveContoursJson(data,name):
    
    with open(name, 'w') as f:
        json.dump(data, f,indent=2)


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


def angleContour(c):

    contour = c[0][0]
    center = np.mean(contour, axis=0)
    keepangles =  np.arange(-180, 181, 360/20)
    angles=[]
    returnarray =[]
    x1, y1 = center

    for i in contour:
        x,y = i
        xc=x-center[0]
        yc=y-center[1]
        angles.append(Angle2D(xc,yc))

    for angle in keepangles:
        index,number = find_nearest(angle,angles)
        #print(str(angle)+" "+str(number))
        returnarray.append(contour[index])
        x2, y2 = contour[index]
        #plt.plot(x1, y1, x2, y2, marker = 'o')
        #plt.axline((x1, y1), (x2, y2))

    return np.array(returnarray).flatten().tolist()

def Angle2D(x,y):

    zup=np.array([0,-1])

    return ((angle_between(np.array([x,y]), zup)/math.pi)*180)*np.sign(x)

def angle_between(v1, v2):
    """ Returns the angle in radians between vectors 'v1' and 'v2'::

            >>> angle_between((1, 0, 0), (0, 1, 0))
            1.5707963267948966
            >>> angle_between((1, 0, 0), (1, 0, 0))
            0.0
            >>> angle_between((1, 0, 0), (-1, 0, 0))
            3.141592653589793
    """
    v1_u = unit_vector(v1)
    v2_u = unit_vector(v2)
    return np.arccos(np.clip(np.dot(v1_u, v2_u), -1.0, 1.0))

def unit_vector(vector):
    """ Returns the unit vector of the vector.  """
    return vector / np.linalg.norm(vector)

def find_nearest(value, array):
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return idx, array[idx]

def reduceContours(contours):

    dict={}
    for i,c in enumerate(contours):
        dict[str(dictionaryNames[i])] = angleContour(c.allsegs)
    
    return dict

fig = plt.figure(figsize=(5, 4), dpi=100)
axes = fig.add_subplot(111)
axes.tick_params(axis='both', which='major', labelsize=13)
plt.tight_layout()

x,y = filterData(Gaze360)
grid1, gridx1, gridy1, bounds1 = generateGridAndBounds(x,y, 256, min = -50,max= 50)
plotheatmap(grid1, gridx1, gridy1, bounds1, axes, min = -50,max= 50 )
CS = generateContours(grid1, gridx1, gridy1, bounds1, axes, smoothing=17)
plt.show()

data = reduceContours(CS)
saveContoursJson(data,"afm.json")

fig, ax = plt.subplots()

for d in data:
    datatoplot = np.array(data[d]).reshape(21,2);
    plt.plot(datatoplot[:,0],datatoplot[:,1])
    for i, point in enumerate(datatoplot):
        ax.annotate(str(i), (point[0], point[1]))


ax.set_xlim([-50,50])
ax.set_ylim([-50,50])
plt.show()
