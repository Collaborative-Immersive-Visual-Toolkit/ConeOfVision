using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Launcher : MonoBehaviourPunCallbacks, IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback
{
    private GameObject localAvatar;

    private GameObject localObserver;

    public GameObject localAvatarsMenu;

    public GameObject cone;

    public bool voiceDebug = true;

    public AvatarBehaviourRecorder avatarRecorder;

    public RemoteAvatarsManager ram;

    PhotonView photonView;

    public string RoomName;

    private void Awake()
    {
        Resources.LoadAll("ScriptableObjects");

        if (MasterManager.GameSettings.Observer)
        {
            Connect();
        }
    }

    public void Connect()
    {
        
        Debug.Log("[PUN] connecting to server");

        PhotonNetwork.AuthValues = new AuthenticationValues();

        if (MasterManager.GameSettings.Observer) {
            PhotonNetwork.NickName ="Observer";
            PhotonNetwork.AuthValues.UserId = "1";
        }
        else
        {
            PhotonNetwork.NickName = MasterManager.GameSettings.Nickname;                
        }
     
        PhotonNetwork.GameVersion = MasterManager.GameSettings.Gameversion;
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster() {

        Debug.Log("[PUN] connected to server");

        Debug.Log("[PUN] connected with Nickname: " + PhotonNetwork.LocalPlayer.NickName + "\n UserID: " + PhotonNetwork.LocalPlayer.UserId);

        //Debug.Log("[PUN] joining room " + MasterManager.GameSettings.RoomName);
        Debug.Log("[PUN] joining room " + RoomName);

        RoomOptions options = new RoomOptions();
        options.PublishUserId = true;
        //PhotonNetwork.JoinOrCreateRoom(MasterManager.GameSettings.RoomName, options, TypedLobby.Default);
        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }

    void IMatchmakingCallbacks.OnJoinedRoom()
    {

        Debug.Log("[PUN] joined room " + PhotonNetwork.CurrentRoom);

        /*if (!firstconnection) reconnectPhotonViews();

        firstconnection = false;*/

        StartCoroutine(waitForSdkManagerInstantiated());
    }

    private IEnumerator waitForSdkManagerInstantiated()
    {

        //get audiosource from the localavatar       
        while (OvrAvatarSDKManager.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (MasterManager.GameSettings.Observer) ObserverInstantiation();
        else 
            InstantiateLocalAvatar();
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == MasterManager.GameSettings.InstantiateVrAvatarEventCode)
        {
            InstantiateRemoteAvatar(photonEvent);
        }
        else if (photonEvent.Code == MasterManager.GameSettings.InstantiateObserverEventCode) {

            InstantiateRemoteObserver(photonEvent);
        }
    }

    /*public override void OnDisconnected(DisconnectCause cause)
    {
        
        Debug.Log("[PUN] Disconnected -->" + cause);

        Debug.Log("[PUN] reconnecting to server");

        PhotonNetwork.ReconnectAndRejoin();
    }

    public override void OnLeftRoom() {
        
        Debug.Log("[PUN] LeftRoom" );

        OnConnectedToMaster();
    }
    */




    //AVATAR
    //local avatar
    void InstantiateLocalAvatar()
    {

        Debug.Log("[PUN] instantiate LocalAvatar");

        GameObject player = GameObject.Find("OVRPlayerController");

        //check if an avatar attached to the OVR player controller already exist
        Transform attachedLocalAvatar = player.transform.FindDeepChild("LocalAvatar");
        if (attachedLocalAvatar != null) Destroy(attachedLocalAvatar.gameObject);
       
        photonView = player.AddComponent<PhotonView>();//Add a photonview to the OVR player controller 
        PhotonTransformView photonTransformView = player.AddComponent<PhotonTransformView>();//Add a photonTransformView to the OVR player controller 
        photonTransformView.m_SynchronizeRotation = false;
        photonView.ObservedComponents = new List<Component>();
        photonView.ObservedComponents.Add(photonTransformView);
        photonView.Synchronization = ViewSynchronization.UnreliableOnChange; // set observeoption to unreliableonchange

        //instantiate the local avatr
        GameObject TrackingSpace = GameObject.Find("TrackingSpace");
        localAvatar = Instantiate(Resources.Load("LocalAvatar"), TrackingSpace.transform.position, TrackingSpace.transform.rotation, TrackingSpace.transform) as GameObject;
        PhotonAvatarView photonAvatrView = localAvatar.GetComponent<PhotonAvatarView>();
        photonAvatrView.photonView = photonView;
        photonAvatrView.ovrAvatar = localAvatar.GetComponent<OvrAvatar>();
        photonView.ObservedComponents.Add(photonAvatrView);


        if (PhotonNetwork.AllocateViewID(photonView))
        {

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            // request to all other clients in the network to create a remote 
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.InstantiateVrAvatarEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);

            //OvrAvatar ovrAvatar = localAvatar.GetComponent<OvrAvatar>();
            //ovrAvatar.oculusUserID = MasterManager.GameSettings.UserID;

            Debug.Log("[PUN] LocalAvatar instantiatiation triggered now waiting for OVRAvatar to initialize");

            OvrAvatar.LocalAvatarInstantiated += LocalAvatarInstantiated;
        }
        else
        {
            Debug.LogError("[PUN] Failed instantiate LocalAvatar, Failed to allocate a ViewId.");

            Destroy(localAvatar);
        }
    }

    private void LocalAvatarInstantiated() {


        if (MasterManager.GameSettings._devmode)
        {

            if (localAvatarsMenu == null)
            {

                GameObject player = GameObject.Find("OVRPlayerController");

                Transform t = player.transform.FindDeepChild("panels");

                localAvatarsMenu = t.gameObject;
            }

            localAvatarsMenu.SetActive(true);
        }

        StartCoroutine(PhotonVoiceInstantiationForLocalAvatar());

    }
   
    private IEnumerator PhotonVoiceInstantiationForLocalAvatar()
    {
       
        Debug.Log("[PUN] OVRAvatar completed instantiation of LocalAvatar now we setup voice by adding Speaker,Recorder,VoiceView ");

        //get audiosource from the localavatar       
        while (localAvatar.GetComponentInChildren<AudioSource>() == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        AudioSource audioSource = localAvatar.GetComponentInChildren<AudioSource>();

        //////get the ovr 
        while (audioSource.gameObject.GetComponent<OVRLipSyncContext>() == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        OVRLipSyncContext LipSyncContext = audioSource.gameObject.GetComponent<OVRLipSyncContext>();
        LipSyncContext.audioSource = audioSource;
        if (voiceDebug) LipSyncContext.audioLoopback = true; // Don't use mic.
        else LipSyncContext.audioLoopback = false;
        LipSyncContext.skipAudioSource = false;

        ////add speaker to the element which holds the audio source 
        Speaker speaker = audioSource.gameObject.AddComponent<Speaker>();

        ////add recorder to the element that has the photonView
        Recorder recorder = photonView.gameObject.AddComponent<Recorder>();
        recorder.DebugEchoMode = true;

        ////add Photonvoice view to the local avatar
        PhotonVoiceView voiceView = photonView.gameObject.AddComponent<PhotonVoiceView>();
        voiceView.RecorderInUse = recorder;
        voiceView.SpeakerInUse = speaker;
        voiceView.SetupDebugSpeaker = true;

        ////start transmission 
        yield return voiceView.RecorderInUse.TransmitEnabled = true;
        voiceView.RecorderInUse.StartRecording();


    }

    //remote Avatar
    private void InstantiateRemoteAvatar(EventData photonEvent)
    {

        //sender 
        Player player = PhotonNetwork.CurrentRoom.Players[photonEvent.Sender];


        //check if a remote avatar still exist and clean up 

        GameObject oldRemoteAvatar = GameObject.Find(player.NickName);

        //if an avatar is still there reassociate
        if (oldRemoteAvatar != null)
        {
            PhotonView olphotonView = oldRemoteAvatar.GetComponent<PhotonView>();
            olphotonView.ViewID = (int)photonEvent.CustomData;
            return;
        }


        Debug.Log("[PUN] Instantiatate an avatar for user " + player.NickName + "\n with user ID " + player.UserId);

        GameObject remoteAvatar = Instantiate(Resources.Load("RemoteAvatar")) as GameObject;
       
        remoteAvatar.name = player.NickName;

        PhotonView photonView = remoteAvatar.GetComponent<PhotonView>();
        photonView.ViewID = (int)photonEvent.CustomData;

        //OvrAvatar ovrAvatar = remoteAvatar.GetComponent<OvrAvatar>();
        //ovrAvatar.oculusUserID = player.UserId;

        OvrAvatar.RemoteAvatarInstantiated += OvrAvatar_RemoteAvatarInstantiated;

        Debug.Log("[PUN] RemoteAvatar instantiated");

        //PhotonVoiceView pvv = remoteAvatar.GetComponent<PhotonVoiceView>();


    }

    private GameObject OvrAvatar_RemoteAvatarInstantiated(GameObject rA)
    {

        ram.AddToList(rA);

        Debug.Log("[PUN] RemoteAvatar instantiated");

        InstantiateRemoteUiHelpers(rA);

        return rA;
    }

    private void InstantiateRemoteUiHelpers(GameObject remoteAvatar) {

       

        //instanstiate cone
        Transform Head = DeepChildSearch(remoteAvatar, "head_JNT");
        var newcone = Instantiate(cone, Head, false);
        newcone.transform.parent = Head.transform;
        newcone.name = "VisualCone";
        newcone.layer = 10;

        Debug.Log("[PUN] RemoteAvatar UI helpers instanstiated");
    }


    //OBSERVER
    //local Observer
    void ObserverInstantiation()
    {
        Debug.Log("[PUN] instantiate Local Observer");
     
        //instantiate the local avatar      
        localObserver = Instantiate(Resources.Load("ObserverCamera")) as GameObject;
        photonView = localObserver.GetComponent<PhotonView>();

        //enable observer camera
        localObserver.tag = "MainCamera";


        if (PhotonNetwork.AllocateViewID(photonView))
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.InstantiateObserverEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);

            Debug.Log("[PUN] Local Observer instantiated");

            //enablePhotonVoice()
            StartCoroutine(PhotonVoiceInstantiationForLocalObserver());

        }
        else
        {
            Debug.LogError("[PUN] Failed instantiate Local Observer, Failed to allocate a ViewId.");

            Destroy(localObserver);
        }

        //destroy the player controller 
        Destroy(GameObject.Find("OVRPlayerController"));

        //destroy the player cone
        GameObject octagon = GameObject.Find("cone");
        if (octagon != null) {
            cone c = octagon.GetComponent<cone>();
            c.disabled = true;
        }

        //destroy UiHelpers
        DestroyImmediate(GameObject.Find("UIHelpersModified"));

        //enable observer recorder
        avatarRecorder.enabled = true;

        //create a folder for saving the data
        DataFolderCreation();
    }

    private IEnumerator PhotonVoiceInstantiationForLocalObserver()
    {
        
        Debug.Log("[PUN] setup voice for observer by adding Speaker,Recorder,VoiceView ");

        //add audio source
        AudioSource audioSource = localObserver.GetComponent<AudioSource>();

        ////add speaker to the element which holds the audio source 
        Speaker speaker = audioSource.gameObject.AddComponent<Speaker>();

        ////add recorder to the element that has the photonView
        Recorder recorder = localObserver.gameObject.AddComponent<Recorder>();
        recorder.DebugEchoMode = false;

        ////add Photonvoice view to the local avatar
        PhotonVoiceView voiceView = localObserver.gameObject.AddComponent<PhotonVoiceView>();
        voiceView.RecorderInUse = recorder;
        voiceView.SpeakerInUse = speaker;
        voiceView.SetupDebugSpeaker = true;

        ////start transmission 
        yield return voiceView.RecorderInUse.TransmitEnabled = true;
        voiceView.RecorderInUse.StartRecording();


    }

    //Remote observer 
    private void InstantiateRemoteObserver(EventData photonEvent)
    {

        //sender 
        Player player = PhotonNetwork.CurrentRoom.Players[photonEvent.Sender];

        Debug.Log("[PUN] Instantiatate Observer for user " + player.NickName + "\n with user ID " + player.UserId);

        GameObject remoteObserver = Instantiate(Resources.Load("RemoteObserver")) as GameObject;

        PhotonView photonView = remoteObserver.GetComponent<PhotonView>();
        photonView.ViewID = (int)photonEvent.CustomData;

        Debug.Log("[PUN] Remote Observer Instantiated");

    }

    private void DataFolderCreation() {

        string path = MasterManager.GameSettings.DataFolder + "\\";

        if(!Directory.Exists(path)) Directory.CreateDirectory(path);

    }

    private GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    public Transform DeepChildSearch(GameObject g, string childName)
    {

        Transform child = null;

        for (int i = 0; i < g.transform.childCount; i++)
        {

            Transform currentchild = g.transform.GetChild(i);

            if (currentchild.gameObject.name == childName)
            {

                return currentchild;
            }
            else
            {

                child = DeepChildSearch(currentchild.gameObject, childName);

                if (child != null) return child;
            }

        }

        return null;
    }

    void OnDisable()
    {
        PhotonNetwork.Disconnect();
    }

}
