""" PYTHON PORTING
% function DetectSaccades360.m
%
% This function detects saccades from the provided data. It is based on the
% saccade detector described in Dorr, Michael, et al. "Variability of eye
% movements when viewing dynamic natural scenes." Journal of vision 10.10
% (2010): 28-28.
%
% input:
%   data        - data from the ARFF file
%   metadata    - metadata from the ARFF file
%   attributes  - attributes from the ARFF file
%   typeOfMotion- 1 -> eye FOV, 2 -> eye+head
%   params      - parameters to use for saccade detection
%
% output:
%   result      - logical vector with the same length as data and true where a saccade is detected
%
% params format:
% params is a data structure with the following fields
% 
% params.tolerance;
% params.thresholdOnsetFast;
% params.thresholdOnsetSlow;
% params.thresholdOffset;
% params.maxSpeed;
% params.minDuration;
% params.maxDuration;
% params.velIntegrationInterv;
% params.minConfidence
"""

def DetectSaccades360(data, metadata, attributes, typeOfMotion, params):
    c_tolerance = params['tolerance']
    c_thresholdOnsetFast = params['thresholdOnsetFast']
    c_thresholdOnsetSlow = params['thresholdOnsetSlow']
    c_thresholdOffset = params['thresholdOffset']
    c_maxSpeed = params['maxSpeed']
    c_minDuration = params['minDuration']
    c_maxDuration = params['maxDuration']
    c_velIntegrationInterv = params['velIntegrationInterv']
    c_minConf = params['minConfidence']

    timeInd = GetAttPositionArff(attributes, 'time')
    xInd = GetAttPositionArff(attributes, 'x')
    yInd = GetAttPositionArff(attributes, 'y')
    confInd = GetAttPositionArff(attributes, 'confidence')

    #initialize result
    sh = data.shape
    result = np.zeros((sh[0],1), dtype=bool)
    if(sh[0] < 10):
        return None

    eyeFovVec, eyeHeadVec, headVec = GetCartVectors(data, metadata, attributes);
    if(typeOfMotion == 1):
        vecList = eyeFovVec
    elif(typeOfMotion == 2):
        vecList = eyeHeadVec
    elif(typeOfMotion == 3):
        vecList = headVec
    else:
        print('Uknown motion')
        exit(0)

    speed = GetSpeed(vecList, data[:,timeInd]);

    # create glitch array
    glitch = np.zeros((sh[0],1));
    glitch[data[:][xInd] > (1+c_tolerance)*metadata.width_px] = 1;
    glitch[data[:][xInd] < -c_tolerance*metadata.width_px] = 1;
    glitch[data[:][yInd] > (1+c_tolerance)*metadata.height_px] = 1;
    glitch[data[:][yInd] < -c_tolerance*metadata.height_px] = 1;
    glitch[data[:][confInd] < c_minConf] = 0.75;

    isSaccActive = 0;
    onsetSlowIndex = 1;

    #todo
    for i in range(sh[0]):
        # not in glitch
        if (glitch[i] == 0): # check here @@todo i>glitch ?
            if (isSaccActive == 0):
                # if speed less than onset slow move index
                if (speed[i] < c_thresholdOnsetSlow):
                    onsetSlowIndex = i+1

                # saccade above fast threshold but below physically impossible
                if (speed[i] > c_thresholdOnsetFast and speed[i] < c_maxSpeed):
                    isSaccActive = 1
                    
                    #allocate all samples from onset slow as saccade
                    result[onsetSlowIndex:i] = 1

            # if within saccade check for termination cases otherwise make sample part of saccade
            if (isSaccActive == 1):
                # saccade termination cases
                if (speed(i) < c_thresholdOffset):
                    isSaccActive = 0

                    #check for minDuration
                    if (data[i][timeInd]-data[onsetSlowIndex][timeInd] < c_minDuration):
                        result[onsetSlowIndex:i-1] = 0
                    
                    continue # skip rest of the loop
                

                if (data[i][timeInd]-data[onsetSlowIndex][timeInd] > c_maxDuration):
                    isSaccActive = 0
                    continue
                

                # check if onset and current point are the same
                if (i-onsetSlowIndex < 1):
                    continue
                
                meanVel = np.mean(speed[onsetSlowIndex:i-1])

                if (meanVel < c_thresholdOnsetSlow):
                    isSaccActive = 0

                    # check for minDuration
                    if (data[i][timeInd]-data[onsetSlowIndex][timeInd] < c_minDuration):
                        result[onsetSlowIndex:i-1] = 0
                    
                    continue
                
                result[i] = 1
        else:
            onsetSlowIndex = i+1
    return result