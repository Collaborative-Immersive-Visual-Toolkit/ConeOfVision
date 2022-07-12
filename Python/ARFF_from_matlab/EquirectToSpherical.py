import math 
"""% PYTHON port from  Matlab
% EquirectToSpherical.m
%
% This function converts equirectangular coordinates to spherical coordinates.
"""
def EquirectToSpherical(xEq, yEq, widthPx, heightPx):
    horRads = (xEq *2 * math.pi) / widthPx
    verRads = (yEq * math.pi) / heightPx

    return horRads, verRads