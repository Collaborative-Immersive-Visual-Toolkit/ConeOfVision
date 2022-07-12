"""% PYTHON port from  Matlab
% function GetAttPositionArff:
%
% Gets a list of attributes returned from LoadArff and an attribute name to
% search.  If it finds the attribute returns its index otherwise it can raise
% an error.
%
% input:
%   arffAttributes  - attribute list returned from LoadArff
%   attribute       - attribute to search
%   check           - (optional) boolean to check if attribute exists. Default is true
%
% output:
%   attIndex        - index attribute of the attribute in the list if it was found. 
%                     Returns 0 if it wasn't found
"""

def GetAttPositionArff(arffAttributes, attribute):
    try:
        attIndex = 0
        for i in range(len(arffAttributes)):
            if (arffAttributes[i][0].lower() == attribute.lower()):
                attIndex = i;
        return attIndex
    except Exception as e:
        print('Error in Loading Attribute: ', attribute)
        exit()