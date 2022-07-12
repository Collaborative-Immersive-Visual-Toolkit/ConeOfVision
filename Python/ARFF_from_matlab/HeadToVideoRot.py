import numpy as np
from YZXrotation import YZXrotation
"""% PYTHON port from  Matlab
% HeadToVideoRot.m
%
% Calculates the rotation of the head vector to the provided video vector.
"""

def HeadToVideoRot(headVec, headAngleRads, videoVec):
    headToRef = YZXrotation(headVec, -headAngleRads)
    videoToRef = YZXrotation(videoVec, 0)
    rot = np.matmul(np.array(videoToRef), np.transpose(headToRef))
    return rot
