from matplotlib import colors
import numpy as np
from numpy.lib.shape_base import split
import matplotlib.pyplot as plt
from scipy.ndimage.filters import gaussian_filter
import math
import os
import argparse
import glob
import pandas as pd
import sys
import scipy.io
import random 

sys.path.append('./ARFF_from_matlab')
from LoadArff import LoadArff, filterBy, getColumn
from GetCartVectors import GetCartVectors, GetAngles
import PlaneLineIntersection as pi
from HeadToVideoRot import HeadToVideoRot
from RotatePoint import RotatePoint
from SphericalToCart import CartToSpherical

zup=[]
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

data= pd.DataFrame(columns=columns)

def LoadData(file):

    global data

    if ".arff" in file:
        datafile = LoadDataFromArff(file)
    elif ".csv" in file:
        datafile = LoadDataFromCSV(file)
    elif ".txt" in file:
        datafile = LoadDataFromCSV(file)
    elif ".mat" in file:
        datafile = LoadDataFromMAT(file)
    else:
        return

    data= data.append(datafile, ignore_index=True)

    print(data.shape)

def LoadDataFromCSV(file):

    print('Processing ', file)

    columns=["GazeFoVDegreesX","GazeFoVDegreesY","SalX","SalY","HeadVelX","HeadVelY","HeadAccX","HeadAccY","HeadAccMeanX","HeadAccMeanY","HeadVelStdX","HeadVelStdY"]

    datafile= pd.DataFrame(columns=columns)

    print('Processing ', file) 
    
    datafile = pd.read_csv (file, sep="\t", header=None)
    datafile = datafile.drop([12], axis=1)
    datafile.columns = columns

    return datafile

def LoadDataFromArff(file):
    
    user = int(file.split('\\')[-1].split("_")[0])
    task = int(file.split('\\')[-1].split("_")[1])
    #usage = 0 if int(user)<=10 else 1
    usage = 0 if np.random.uniform()<0.7 else 1
    
    global zup
    zup=np.array([0,-1])

    print('Processing ', file)
    da, me,at,re,com = LoadArff(file)
    confidence = getColumn("confidence", at, da)
    head_angle = getColumn("angle_deg_head", at, da)
    time = getColumn('time', at, da)

    time = [x/100000 for x in time]   # transfrom from milliseconds RICALCULATE!!!

    HeadRads, GazeRads, GazeFoVRads = GetAngles(da, me, at)

    #translation to center
    x =[x-math.pi if x>0 else x+math.pi for x in GazeFoVRads[:,0]]
    y = (GazeFoVRads[:,1]-math.pi/2)*-1
    GazeFoVRads =np.stack((x, y), axis=-1)

    datafile = computeVelocityAccelerationHead(time,HeadRads, GazeFoVRads,user,task,usage)

    return datafile


# def LoadDataFromTXT(file):

#     global zup
#     zup=np.array([0,1])

#     print('Processing ', file)

#     data = pd.read_csv(file, sep="\t", header=None)
#     data.columns=['timestamp','Frame_Number','Head_Long_Deg','Head_Lat_Deg','Eye_In_Head_ScreenX','Eye_In_Head_ScreenY', 'Gaze_In_World_Long_Deg','Gaze_In_World_Lat_Deg']

#     data['Eye_In_Head_ScreenX'] = (data['Eye_In_Head_ScreenX'] * 108.77) - 108.77/2
#     data['Eye_In_Head_ScreenY'] = (data['Eye_In_Head_ScreenY'] * 111.53) - 111.53/2

#     initialTime = data['timestamp'][0]
#     data['timestamp'] -= initialTime
#     data['timestamp'] /= 1000

#     data[['Head_Long_Deg','Head_Lat_Deg']] *= (math.pi /180)
#     data[['Eye_In_Head_ScreenX','Eye_In_Head_ScreenY']] *= (math.pi /180)

#     datafile = computeVelocityAccelerationHead(data['timestamp'] ,data[['Head_Long_Deg','Head_Lat_Deg']].values, data[['Eye_In_Head_ScreenX','Eye_In_Head_ScreenY']].values)

#     return datafile

def LoadDataFromMAT(file):

    user = int(file.split('\\')[-1].split("_")[1])
    task = int(file.split('\\')[-1].split("_")[-1].split('.')[0])
    usage = 0 if int(user)<=10 else 1

    print('Processing ', file)

    global zup
    zup=np.array([0,1])
    downasampleratio=3
    mat = scipy.io.loadmat(file)
    time = mat['ProcessData'][0][0][3].reshape(-1)
    time = time[::downasampleratio]

    # #HeadRads
    HeadVector = mat['ProcessData'][0][0][7][0][0][1][::downasampleratio]
    HeadRads = np.array([ CartToSpherical(v) for v in HeadVector])
    HeadRads = np.array([ [v[0]-math.pi/2,(v[1]-math.pi/2)*-1] for v in HeadRads])
    
    #GazeFoVRads
    GazeFoVvector = mat['ProcessData'][0][0][8][0][0][2][::downasampleratio]
    GazeFoVRads = [ CartToSpherical(v) for v in GazeFoVvector]
    GazeFoVRads = np.array([ [v[0]-math.pi/2,(v[1]-math.pi/2)*-1] for v in GazeFoVRads])

    datafile = computeVelocityAccelerationHead(time,HeadRads, GazeFoVRads,user,task,usage)

    return datafile

def computeVelocityAccelerationHead(time, HeadRads, GazeFoVRads,user,task,usage):

    dfout = pd.DataFrame(columns=columns)

    #translation to center
    x = np.degrees(GazeFoVRads[:,0])
    y = np.degrees(GazeFoVRads[:,1])
    GazeFoVDegrees =np.stack((x, y), axis=-1)

    # fig = plt.figure()
    # plt.scatter(GazeFoVDegrees[:,0],GazeFoVDegrees[:,1] ,s=0.1)
    # plt.show()

    headDeg = np.degrees(HeadRads)
    headVel, headVelX, headVelY, headVelAng, headVelMagnitude = derivate2D(headDeg,time)
    headAcc, headAccX, headAccY, headAccAng, headAccMagnitude = derivate2D(headVel,time)

    # num_bins = 100
    # n, bins, patches = plt.hist(headVelAng, num_bins, facecolor='blue', alpha=0.5)
    # plt.show()

    array =np.array([
    GazeFoVDegrees[:,0],
    GazeFoVDegrees[:,1],
    headVelX,
    headVelY,
    headVelAng,
    headVelMagnitude,
    headAccX,
    headAccY,
    headAccAng,
    headAccMagnitude,
    [user]*len(GazeFoVDegrees),
    [task]*len(GazeFoVDegrees),
    [usage]*len(GazeFoVDegrees)])

    s = array.shape
    array = np.transpose(array)

    df2 = pd.DataFrame(array, columns=columns)
    dfout = dfout.append(df2)

    print(dfout.shape)

    return dfout

def derivate2D(val,time):

    DerX= np.gradient(val[:,0],time)
    DerY= np.gradient(val[:,1],time)
    Der = np.stack((DerX, DerY), axis=-1)
    Angle = angle(Der)
    Magnitude = np.apply_along_axis(np.linalg.norm, 1, Der)

    return Der,DerX,DerY,Angle,Magnitude

def angle(array):
    return np.array([ Angle2D(*v) for v in array])

def Angle2D(x,y):
    global zup
    return ((angle_between(np.array([x,y]), zup)/math.pi)*180)*np.sign(x)

def angle_between(v1, v2):
    v1_u = unit_vector(v1)
    v2_u = unit_vector(v2)
    return np.arccos(np.clip(np.dot(v1_u, v2_u), -1.0, 1.0))

def unit_vector(vector):
    """ Returns the unit vector of the vector.  """
    return vector / np.linalg.norm(vector)

def main(args):
    global data

    pathname = args.path + "/**/*." + args.type
    pathoutput = args.output
    files = glob.glob(pathname, recursive=True)

    for c, f in enumerate(files[:]):
        LoadData(f)

    # X=data["GazeFoVDegreesX"]
    # Y=data["GazeFoVDegreesY"]
    # fig = plt.figure()
    # plt.scatter(X,Y ,s=0.1)
    # plt.show()
    # fig = plt.figure()
    # plt.scatter(Y,X ,s=0.1)
    # plt.show()

    data.to_pickle(pathoutput, compression='infer', protocol=4)#, storage_options=None)

def dir_path(string):
    if os.path.isdir(string):
        return string
    else:
        raise NotADirectoryError(string)

def dir_path_create(string):

    fullpath = os.path.abspath(string)
    path = os.path.dirname(fullpath)

    if os.path.isdir(path):
        #raise Exception("directory already existing either delete directory manually or change output directory")
        return string
    else:
        os.makedirs(path)
        return string

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--path", type=dir_path, required=True)
    parser.add_argument("--type", type=str, required=True)
    parser.add_argument("--output", type=dir_path_create, required=True)
    args = parser.parse_args()
    main(args)

# --path 'C:\\Users\\Riccardo\\OneDrive - Imperial College London\\PURE Data Upload' --type csv
# --path 'C:\\Users\\Riccardo\\OneDrive - Imperial College London\\360_em_dataset\\gaze' --type arff --output AccelerationData\\AccVelarff.pkl
# --path 'C:\\Users\\Riccardo\\OneDrive - Imperial College London\\360_em_dataset\\gaze' --type txt --output AccelerationData\\AccVelarff.pkl

#python generateParameters.py --path 'C:\Users\Riccardo\OneDrive - Imperial College London\GIW' --type mat --output GIW.pkl
#python generateParameters.py --path 'C:\Users\Riccardo\OneDrive - Imperial College London\SGaze' --type txt --output SGaze.pkl
#python generateParameters.py --path 'C:\Users\rb1619\Documents\GitHub\S-Gaze-Dataset\Data' --type txt --output SGaze.pkl
# --path 'C:\\Users\\dannox\\Desktop\\GazeVR\\GIW' --type mat --output AccelerationData\\AccVelarff.pkl
#python generateParameters.py --path C:\Users\rb1619\Documents\GitHub\Gaze-in-the-wild-dataset --type mat --output C:\Users\rb1619\Documents\GitHub\ARFF_toolkit\GIW.pkl