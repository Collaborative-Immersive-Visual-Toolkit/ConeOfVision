import math
import numpy as np
"""% PYTHON port from  Matlab
% YZXrotation.m
%
% This function calculates the rotation matrix to the coordinate system of
% reference.
"""
def YZXrotation(vec, tiltRads):
    theta = math.asin(vec[1])
    psi = 0
    if (math.fabs(theta) < math.pi/2 - 0.01):
        psi = math.atan2(vec[2], vec[0])

    rot = np.zeros(shape=(3,3))
    rot[0][0] = math.cos(theta)*math.cos(psi)
    rot[0][1] = -math.sin(theta)
    rot[0][2] = math.cos(theta)*math.sin(psi)
    rot[1][0] = math.cos(tiltRads)*math.sin(theta)*math.cos(psi) + math.sin(tiltRads)*math.sin(psi)
    rot[1][1] = math.cos(tiltRads)*math.cos(theta)
    rot[1][2] = math.cos(tiltRads)*math.sin(theta)*math.sin(psi) - math.sin(tiltRads)*math.cos(psi)
    rot[2][0] = math.sin(tiltRads)*math.sin(theta)*math.cos(psi) - math.cos(tiltRads)*math.sin(psi)
    rot[2][1] = math.sin(tiltRads)*math.cos(theta)
    rot[2][2] = math.sin(tiltRads)*math.sin(theta)*math.sin(psi) + math.cos(tiltRads)*math.cos(psi)
    return rot


