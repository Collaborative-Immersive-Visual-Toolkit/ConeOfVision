from IsNomAttribute import IsNomAttribute
from GetNomAttValue import GetNomAttValue
"""% PYTHON port from  Matlab
% LoadArff.m
%
% Thi funciton loads data from an ARFF file and returns the data, metadata,
% attributes, relation and comments. All returned strings are lower case.
%
% input:
%   arffFile    - path to ARFF file to read
%   
% output:
%   data        - data stored in the ARFF file
%   metadata    - structure holding metadta in the form: metadata.{width_px, height_px, width_mm, height_mm, distance_mm} -1 if not available. Extra metadata are stored in  metadata.extra, which is an nx2 cell array holding name-value pairs
%   attributes  - nx2 cell array with attribute names and types, where n is the number of attributes
%   relation    - relation described in ARFF
%   comments    - nx1 cell array containing one comment line per cell
"""

metadata = {'width_px':-1, 
            'height_px':-1,
            'width_mm' : -1,
            'height_mm' : -1,
            'distance_mm' : -1,
            'extra' : []
}

def getColumn(key, attributes, data):
    index = -1
    for c,i in enumerate(attributes):
        if(i[0].lower() == key):
            index = c
            break

    if(index == -1):
        return data
    
    before = len(data)
    data = [i[c] for i in data]
    
    return data

def filterBy(key, acceptedValue, attributes, data):
    index = -1
    for c,i in enumerate(attributes):
        if(i[0].lower() == key):
            index = c
            break

    if(index == -1):
        return data
    
    before = len(data)
    data = [i for i in data if(i[c] == acceptedValue)]
    print('before filtering:', before, " After filtering:", len(data))
    return data

def LoadArff(arffFile):
    # initialize data
    data = []
    # initialize metadata
    metadata['width_px'] = -1
    metadata['height_px'] = -1
    metadata['width_mm'] = -1
    metadata['height_mm'] = -1
    metadata['distance_mm'] = -1
    metadata['extra'] = []
    attributes = []
    relation = ''
    comments = []
    
    # nominal attribute handling
    nomMat = []
    nomMaps = []
    
    # read header
    numOfHeaderLines = 0
    with open(arffFile, 'r') as f:
        lines = f.readlines()
        for line in lines:
            words = line.split(' ')
            # check for relation 
            if(len(words)>1 and words[0].lower().strip() =='@relation'):
                relation = words[1].lower().strip()
            # check for width_px
            elif (len(words)>2 and words[0].lower().strip() =='%@metadata' and words[1].lower().strip() == 'width_px'):
                metadata['width_px'] =float( words[2].strip())
            # check for height_px
            elif (len(words)>2 and words[0].lower().strip() == '%@metadata' and words[1].lower().strip() =='height_px'):
                metadata['height_px'] =float( words[2].strip())
            # check for width_mm
            elif (len(words)>2 and words[0].lower().strip() == '%@metadata' and words[1].lower().strip() =='width_mm'):
                 metadata['width_mm'] = float( words[2].strip())
            # check for height_mm
            elif (len(words)>2 and words[0].lower().strip() == '%@metadata' and words[1].lower().strip() =='height_mm'):
                metadata['height_mm'] = float( words[2].strip())
            # check for distance_mm
            elif (len(words)>2 and words[0].lower().strip() == '%@metadata' and words[1].lower().strip() =='distance_mm'):
                metadata['distance_mm'] = float( words[2].strip())
            # process the rest of the metadata
            elif (len(words)>2 and words[0].lower().strip() == '%@metadata'):
                metadata['extra'].append([words[1], words[2].strip()])
            # check for attributes
            elif (len(words)>2 and words[0].lower().strip() == '@attribute'):
                attributes.append([words[1].lower().strip(), words[2].strip()])
                isNom, nominalMap, _ = IsNomAttribute(line)
                nomMat.append(isNom)
                if (isNom):
                    nomMaps.append(nominalMap)
                    attributes[-1][1] = GetNomAttValue(line)
                else:
                    nomMaps.append([])
            # check if it is a comment
            elif (len(line)>0 and line[0] == '%'):
                comments.append(line.strip())
            # check if data has been reached
            elif (len(words)>0 and words[0].lower().strip() == '@data'):
                break
            
            numOfHeaderLines = numOfHeaderLines+1
        
        numAtts = len(attributes)

            
        lines = [[el.strip() for el in v.split(',')] for v in lines[numOfHeaderLines+1:]]
        nomIndices = [i for i, v in enumerate(nomMat) if v]

        for i, line in enumerate(lines):
            for c,item in enumerate(line):
                if(c not in nomIndices):
                    lines[i][c] = float(item)
                else:
                    lines[i][c] = nomMaps[c][item]
        data = lines

    return data, metadata, attributes, relation, comments

if __name__ == "__main__":
    #f = 'C:\\Users\\dannox\\Desktop\\GazeVR\\360_em_dataset\\gaze\\014\\014_01_park_6eLyo2Y7mZU.arff'
    f = 'C:\\Users\\dannox\\Desktop\\GazeVR\\360_em_dataset\\ground_truth\\test\\012_10_eiffel_tower_-wJl-zFqA2A.arff'
    LoadArff(f)