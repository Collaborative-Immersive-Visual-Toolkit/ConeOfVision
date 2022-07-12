"""% PYTHON port from  Matlab
% IsNomAttribute.m
%
% This function checks if an attribute is of nominal type and returns true along
% with nominal and numeric maps. Otherwise it returns false.
%
% input:
%   attDatatype - the part that describes the attribute after its name
%
% output:
%   isNom       - boolean value denoting if nominal
%   nominalMap  - mapping of nominal values to doubles as in an C++ enumeration
%   numericMap  - mapping of doubles to nominal values
"""
def IsNomAttribute(attDatatype):
    openCurl = attDatatype.find('{')
    closeCurl = attDatatype.find('}')
    if (openCurl == -1  and closeCurl == -1):
        isNom = False
        nominalMap = {}
        numericMap = {}
        return isNom, nominalMap, numericMap
        
    assert(openCurl > 0), 'Invalid attribute datatype ' + attDatatype
    assert(closeCurl > 0), 'Invalid attribute datatype ' + attDatatype
    attDatatype = attDatatype[openCurl+1:closeCurl]

    # remove spaces from nominal
    attDatatype = attDatatype.strip().replace(' ','')

    keys = attDatatype.split(',');
    
    nominalMap = {k:v for k,v in zip(keys,range(len(keys)))}
   
    # probably in python is already ok
    # convert to simple when we have single key. Otherwise the type is invalid for map creation
    #if (len(keys) == 1)
    #    keys = string(keys);
    
    numericMap = {v:k for k,v in zip(keys,range(len(keys)))}
    isNom = True;

    return isNom, nominalMap, numericMap
