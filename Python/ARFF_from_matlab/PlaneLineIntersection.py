#!/bin/python
from __future__ import print_function
import numpy as np

debug=False

def PlaneLineIntersection(planeNormal, planePoint, rayDirection, rayPoint, epsilon=1e-6):
 
    ndotu = planeNormal.dot(rayDirection)
    if abs(ndotu) < epsilon:
        return None
        raise RuntimeError("no intersection or line is within plane")

    w = rayPoint - planePoint
    si = -planeNormal.dot(w) / ndotu
    Psi = w + si * rayDirection + planePoint
    return Psi

def PlaneLineIntersectionFixed(v):
    
    planeNormal = np.array([1, 0, 0])
    planePoint = np.array([-1, 0, 0]) #Any point on the plane
    rayPoint = np.array([0, 0, 0]) #Any point along the ray
    temp = []
    Psi=None

    rayDirection = np.array([v[0],v[1],v[2]])
    try:
        Psi = PlaneLineIntersection(planeNormal, planePoint, rayDirection, rayPoint)
        if Psi is not None:
            Psi = Psi.tolist()
    except Exception as e:
        if debug: 
            print(e)

    return Psi

def PlaneLineIntersectionFixedArray(Array, planeNormal = np.array([1, 0, 0]), planePoint = np.array([-1, 0, 0]), rayPoint = np.array([0, 0, 0]) ):
    
    newarray=[]
        
    for v in Array:
        rayDirection = np.array([v[0],v[1],v[2]])
        try:
            Psi = PlaneLineIntersection(planeNormal, planePoint, rayDirection, rayPoint)
            if Psi is not None:
                Psi = Psi.tolist()
                newarray.append(Psi)
            else:
                newarray.append([None,None,None])
        except Exception as e:
            if debug: 
                print(e)

    return newarray
 
if __name__=="__main__":
    #Define plane
    planeNormal = np.array([0, 0, 1])
    planePoint = np.array([0, 0, 5]) #Any point on the plane
 
    #Define ray
    rayDirection = np.array([0, -1, -1])
    rayPoint = np.array([0, 0, 10]) #Any point along the ray
 
    Psi = PlaneLineIntersection(planeNormal, planePoint, rayDirection, rayPoint)
    print ("intersection at", Psi)

