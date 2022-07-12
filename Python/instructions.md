-----------
The first step consist in downloading the 3 dataset and generate the pkl containing gaze data in head local coordinate
-----------

-----------
360 VR gaze


1 - Download the dataset https://gin.g-node.org/ioannis.agtzidis/360_em_dataset

2 - (only downloads the 360_em_dataset/gaze folder)

3 - python generateParameters.py --path [SGazepath] --type txt --output [SGaze.pkl]

4 - Alternatively donwload the processed pkl file from https://drive.google.com/file/d/1SMKngXkOhGDXMXUnIoKQe_3lXSA3UJDH/view?usp=sharing


------------------------
Gaze in the wild Dataset


1 - Download dataset from #http://www.cis.rit.edu/~rsk3900/gaze-in-wild/

2 - (only downloads the Extracted_Data folder and only keep the ProcessData folder from each of the 4 subdirectory)

3 - python generateParameters.py --path [giwpath] --type mat --output [GIW.pkl]

4 - Alternatively donwload the processed pkl file from https://drive.google.com/file/d/1irzn4J2xQrkkI68ILrI2uU6hz_KP_sIZ/view?usp=sharing


--------------------------------
S gaze dataset 


1 - Download dataset from https://cranehzm.github.io/SGaze.html

2 - (only downloads the Extracted_Data folder and only keep the ProcessData folder from each of the 4 subdirectory)

3 - python generateParameters.py --path [SGazepath] --type txt --output [SGaze.pkl]

4 - Alternatively donwload the processed pkl file from https://drive.google.com/file/d/1QFmXJjakkOJJMkqEGedsmxdTgTUDIYbI/view?usp=sharing


------------------------------------------------------------------------------------------------
The second step consist in comparing them by running the 360_VR_gaze.pkl
------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------
If you want to extract the constours for use in the unity application use afm_10_contours.py
------------------------------------------------------------------------------------------------
