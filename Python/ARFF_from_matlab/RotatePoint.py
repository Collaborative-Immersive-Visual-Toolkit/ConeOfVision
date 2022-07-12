import numpy as np
"""% PYTHON port from  Matlab
% RotatePoint.m
%
% Rotates a point by multiplying from the left with the provided rotation matrix.
"""

def RotatePoint(rot, vec):
    try:
        sh = vec.shape
        vec = np.transpose(vec)
            
        rotVec = np.matmul(rot, vec)
        return rotVec
    except:
        exit('Error in multiplying matrix/vector')
