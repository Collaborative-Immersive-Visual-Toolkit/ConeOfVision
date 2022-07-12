import numpy as np
import math
"""% PYTHON port from  Matlab
% SphericalTocart.m
%
% This function converts spherical coordinates to cartesian coordinates.
"""

def SphericalToCart(horRads, verRads):

    vec = np.zeros(shape = (3))
    vec[0] = math.sin(verRads) * math.cos(horRads)
    vec[1] = math.cos(verRads)
    vec[2] = math.sin(verRads) * math.sin(horRads)
    return vec


def CartToSpherical(v):

    xy = v[0]**2 + v[2]**2
    horRads = np.arctan2(v[2], v[0])
    verRads = np.arctan2(np.sqrt(xy), v[1])
    return horRads,verRads
