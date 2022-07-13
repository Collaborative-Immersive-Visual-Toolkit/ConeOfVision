

-----------
1 - The first step consist in downloading the 3 dataset and generate the pkl containing gaze data in head local coordinate
-----------

-----------
360 VR gaze


A - Download the dataset https://gin.g-node.org/ioannis.agtzidis/360_em_dataset, alternatively donwload the processed pkl file from https://drive.google.com/file/d/1SMKngXkOhGDXMXUnIoKQe_3lXSA3UJDH/view?usp=sharing and skip step B and C 

B - (only downloads the 360_em_dataset/gaze folder)

C - python generateParameters.py --path [SGazepath] --type txt --output [SGaze.pkl]


------------------------
Gaze in the wild Dataset


1 - Download dataset from #http://www.cis.rit.edu/~rsk3900/gaze-in-wild/, alternatively donwload the processed pkl file from https://drive.google.com/file/d/1irzn4J2xQrkkI68ILrI2uU6hz_KP_sIZ/view?usp=sharing and skip step B and C 

2 - (only downloads the Extracted_Data folder and only keep the ProcessData folder from each of the 4 subdirectory)

3 - python generateParameters.py --path [giwpath] --type mat --output [GIW.pkl]


--------------------------------
S gaze dataset 


1 - Download dataset from https://cranehzm.github.io/SGaze.html, alternatively donwload the processed pkl file from https://drive.google.com/file/d/1QFmXJjakkOJJMkqEGedsmxdTgTUDIYbI/view?usp=sharing and skip step B and C 

2 - (only downloads the Extracted_Data folder and only keep the ProcessData folder from each of the 4 subdirectory)

3 - python generateParameters.py --path [SGazepath] --type txt --output [SGaze.pkl]


------------------------------------------------------------------------------------------------
2 - The second step consist in comparing them by running the 360_VR_gaze.pkl
------------------------------------------------------------------------------------------------
![image](https://user-images.githubusercontent.com/7544912/178710412-2609ad1f-dbd0-4088-8f68-14601546ffb6.png)

------------------------------------------------------------------------------------------------
3 - If you want to extract the contours for use in the unity application use afm_10_contours.py
------------------------------------------------------------------------------------------------
![image](https://user-images.githubusercontent.com/7544912/178710542-899e8354-7689-4586-9511-958854979cef.png)
![image](https://user-images.githubusercontent.com/7544912/178714374-ac665a4d-a1c3-4084-8ed5-8fa388116af3.png)

In order to use the extracted contour in the Unity application you will need to reformat the json file with only one of the extracted contours

use as a template the following file ConeOfVision\UnityGeckoView\UnityProject\Assets\Resources\vectors_triangles_cone_20_77.json and do not alter the triangle list 



