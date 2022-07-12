""" python PORTING
% DetectBlinks360.m
%
% This function detects blinks by using intervals of noise in the provided ARFF
% data together with saccade detection. For every noise interval searches on
% both directions (forward and backwards in time) and if it finds a saccade
% within a given time distance it labels the noise, the saccade interval, and
% the samples in between as blink.
%
% input:
%   data        - data from the ARFF file
%   metadata    - metadata from the ARFF file
%   attributes  - attributes from the ARFF file
%   typeOfMotion- 1 -> eye FOV, 2 -> eye+head
%   params      - parameters to use for saccade detection
%
% output:
%   result      - logical vector with same length as data and true for every sample that is part of a blink
"""

def DetectBlinks360(data, metadata, attributes, typeOfMotion, params):
    #-----------------------------------------------------------------------------------
    # local functions
    #-----------------------------------------------------------------------------------

    # function UpdateResult:
    # It searches on both sides of the noise intervals for blinks.

    def UpdateResult():
        # search backwards
        searchIndex = startIndex
        saccadeFound = false
        while (searchIndex > 0):
            if (data[startIndex][timeInd]-data[searchIndex][timeInd] > c_searchRange and saccadeFound==false):
                break
            
            if (saccades[searchIndex] and saccadeFound==false):
                saccadeFound = true

            if (not saccades[searchIndex] and saccadeFound==true):
                result[searchIndex+1:startIndex] = 1
                break
        
            searchIndex = searchIndex-1
        
        # search forward
        searchIndex = endIndex
        saccadeFound = false
        while (searchIndex <= data.shape[0]):
            if (data[searchIndex][timeInd]-data[endIndex][timeInd] > c_searchRange and saccadeFound==false):
                break

            if (saccades[searchIndex] and saccadeFound==false):
                saccadeFound = true

            if (not saccades[searchIndex] and saccadeFound==true):
                result[endIndex+1:searchIndex] = 1
                break
        
            searchIndex = searchIndex+1



    # initialize search interval on both sides of the blink in us
    c_searchRange = 40000
    c_minConf = 0.5

    timeInd = GetAttPositionArff(attributes, 'time')
    confInd = GetAttPositionArff(attributes, 'confidence')
    
    sh = data.shape
    noise = np.zeros((sh[0],1), dtype=bool)
    noise[data[:][confInd] < c_minConf] = 1

    saccades = DetectSaccades360(data, metadata, attributes, typeOfMotion, params)

    #initially
    result = noise;

    #search for noise indices
    isNoiseActive = 0;
    startIndex = -1;
    endIndex = -1;

    for noiseIndex in range(sh[0]):
        if (isNoiseActive == 0 and noise(noiseIndex) == 1):
            isNoiseActive = 1
            startIndex = noiseIndex

        if (isNoiseActive == 1 and noise(noiseIndex) == 0):
            isNoiseActive = 0
            endIndex = noiseIndex-1
            UpdateResult()


    return result 

function result = DetectBlinks360(data, metadata, attributes, typeOfMotion, params)

end
