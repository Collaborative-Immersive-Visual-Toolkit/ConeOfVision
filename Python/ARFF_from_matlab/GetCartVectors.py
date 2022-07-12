import numpy as np
import math
from GetAttPositionArff import GetAttPositionArff
from EquirectToSpherical import EquirectToSpherical
from SphericalToCart import SphericalToCart, CartToSpherical
from RotatePoint import RotatePoint
from HeadToVideoRot import HeadToVideoRot

"""% PYTHON port from  Matlab
% GetCartVectors.m
%
% This function returns an nx3 arrays of cartesian vectors for the eye direction
% within the FOV of the headset, the eye direction in the world coordinates and
% the head direction in the world.
%
% input:
%   data        - ARFF data
%   metadata    - metadata of ARFF data
%   attributes  - attributes of ARFF data
%
% output:
%   eyeFovVec   - nx3 (x,y,z) array corresponding to FOV vector of gaze centered 
%                 around the middle of the video corresponding to (1,0,0) vector
%   eyeHeadVec  - nx3 (x,y,z) array corresponding to the direction of the eye in 
%                 the world. That is the head+eye position
%   headVec     - nx3 (x,y,z) array corresponding to the direction of the head in
%                 the world
"""
def GetCartVectors(data, metadata, attributes):
    c_xName = 'x'
    c_yName = 'y'
    c_xHeadName = 'x_head'
    c_yHeadName = 'y_head'
    c_angleHeadName = 'angle_deg_head'
    
    xInd = GetAttPositionArff(attributes, c_xName)
    yInd = GetAttPositionArff(attributes, c_yName)
    xHeadInd = GetAttPositionArff(attributes, c_xHeadName)
    yHeadInd = GetAttPositionArff(attributes, c_yHeadName)
    angleHeadInd = GetAttPositionArff(attributes, c_angleHeadName)
    
    sh = (len(data),3) #check data type
    eyeFovVec = np.zeros(shape=sh)
    eyeHeadVec = np.zeros(shape=sh)
    headVec = np.zeros(shape=sh)
    
    for ind in range(len(data)):
        horHeadRads, verheadRads = EquirectToSpherical(data[ind][xHeadInd], data[ind][yHeadInd], metadata['width_px'], metadata['height_px'])
        curHeadVec = SphericalToCart(horHeadRads, verheadRads)
        headVec[ind][:] = curHeadVec

        angleHeadRads = data[ind][angleHeadInd] * math.pi / 180
        videoVec = [-1, 0, 0] # middle of the video is considered its center

        rot = HeadToVideoRot(curHeadVec, angleHeadRads, videoVec)
        rot = np.transpose(rot)

        horGazeRads, verGazeRads = EquirectToSpherical(data[ind][xInd], data[ind][yInd], metadata['width_px'], metadata['height_px'])
        gazeVec = SphericalToCart(horGazeRads, verGazeRads)
        eyeHeadVec[ind][:] = gazeVec

        gazeWithinVec = RotatePoint(rot, gazeVec)

        eyeFovVec[ind][:] = gazeWithinVec[:]
    
    return eyeFovVec, eyeHeadVec, headVec

def GetAngles(data, metadata, attributes):
    c_xName = 'x'
    c_yName = 'y'
    c_xHeadName = 'x_head'
    c_yHeadName = 'y_head'
    c_angleHeadName = 'angle_deg_head'
    
    xInd = GetAttPositionArff(attributes, c_xName)
    yInd = GetAttPositionArff(attributes, c_yName)
    xHeadInd = GetAttPositionArff(attributes, c_xHeadName)
    yHeadInd = GetAttPositionArff(attributes, c_yHeadName)
    angleHeadInd = GetAttPositionArff(attributes, c_angleHeadName)
    
    sh = (len(data),3) #check data type
    headVec = np.zeros(shape=sh)
    eyeHeadVec = np.zeros(shape=sh)
    eyeFovVec = np.zeros(shape=sh)

    sh = (len(data),2) #check data type
    HeadRads = np.zeros(shape=sh)
    GazeRads = np.zeros(shape=sh)
    GazeFoVRads = np.zeros(shape=sh)
    
    for ind in range(len(data)):
        HeadRads[ind][:] = EquirectToSpherical(data[ind][xHeadInd], data[ind][yHeadInd], metadata['width_px'], metadata['height_px'])
        curHeadVec = SphericalToCart(HeadRads[ind][0], HeadRads[ind][1])
        headVec[ind][:] = curHeadVec

        angleHeadRads = data[ind][angleHeadInd] * math.pi / 180
        videoVec = [-1, 0, 0] # middle of the video is considered its center

        rot = HeadToVideoRot(curHeadVec, angleHeadRads, videoVec)
        rot = np.transpose(rot)

        GazeRads[ind][:] = EquirectToSpherical(data[ind][xInd], data[ind][yInd], metadata['width_px'], metadata['height_px'])
        gazeVec = SphericalToCart(GazeRads[ind][0], GazeRads[ind][1])
        eyeHeadVec[ind][:] = gazeVec

        gazeWithinVec = RotatePoint(rot, gazeVec)

        eyeFovVec[ind][:] = gazeWithinVec[:]
        
        GazeFoVRads[ind][:] = CartToSpherical(eyeFovVec[ind])

    return HeadRads, GazeRads, GazeFoVRads

if __name__ == "__main__":
    from LoadArff import LoadArff
    f = 'C:\\Users\\dannox\\Desktop\\GazeVR\\360_em_dataset\\ground_truth\\test\\012_10_eiffel_tower_-wJl-zFqA2A.arff'
    da, me,at,re,com = LoadArff(f)
    eFV, eHV, hV = GetCartVectors(da, me, at)
    print(np.array(eFV))
    print(np.array(eHV))
    print(np.array(hV))