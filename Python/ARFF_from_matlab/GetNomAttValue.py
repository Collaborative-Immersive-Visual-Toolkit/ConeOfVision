"""% PYTHON port from  Matlab
% GetNomAttValue.m
%
% This function returns the value of a nominal attribute in its correct form without 
% spaces.
%
% input:
%   attDatatype - the part that describes the attribute after its name
%
% output:
%   attValue    - nominal attribute in its correct form
"""

def GetNomAttValue(attDatatype):
    openCurl = attDatatype.find('{')
    closeCurl = attDatatype.find('}')
    if (openCurl == ''  and closeCurl == ''):
        isNom = False
        nominalMap = {}
        numericMap = {}
        return
        
    assert(openCurl > 0), 'Invalid attribute datatype ' + attDatatype
    assert(closeCurl > 0), 'Invalid attribute datatype ' + attDatatype
    attValue = attDatatype[openCurl+1:closeCurl]
    attValue = attValue.strip().replace(' ','')
    
    return attValue