
# Collaborative VR experiment 

![Picture1](https://user-images.githubusercontent.com/7544912/133037100-d9a50031-dccc-4b62-a40c-b537ce8f8d3c.gif)

This unity application collects behavioural data (head, hands and verbal communication) of participants performing an exploratory data analysisi tasks

the application support a max of two 


# Build the application

If you need info to build the application for Oculus Quest Android follow this link https://developer.oculus.com/documentation/unity/unity-build/?locale=en_GB

# Multiplayer

The application supports multiplayers using the PUN2 Photon Unity Network plugin you need a free account to run it https://www.photonengine.com/pun

And create a Photon Cloud App 

![Picture2](https://github.com/Collaborative-Immersive-Visual-Toolkit/ConeOfVision/blob/master/photonserversettings.JPG)

The application support two modes:

1 - Observer mode: this is the experimenter moderator and can be runned by a desktop computer, in this modality the experimenter data will be collected ; you can run the observer mode directly from the unity editor without the need to build the application for a different platform 

2 - User mode: this is the headset unity android modality you need to either build and deploy or you could use the oculus link to test it out (however bare in mind that the browser plugin only works in android)

To switch between modalities you have to use the GameSettings scriptable object and thick or untick the Observer boolean (see image below)

![Picture2](https://github.com/Collaborative-Immersive-Visual-Toolkit/ConeOfVision/blob/master/UnityGeckoView/scriptable.JPG)
