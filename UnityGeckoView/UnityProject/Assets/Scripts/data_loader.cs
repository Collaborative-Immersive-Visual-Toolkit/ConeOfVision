
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class data_loader : MonoBehaviourPun
{

    [SerializeField]
    public GameObject[] dataprefabs;

    [SerializeField]
    public bool[] record;

    GameObject currentData;

    bool recordCurrentdata;

    private Queue<GameObject> dataPrefabsQueue;

    private Queue<bool> databoolQueue;

    public AvatarBehaviourRecorder avatarRecorder;

    public RemoteAvatarsManager ram;
    /*
    [Tooltip("Gamepad button to act as gaze click")]
    public OVRInput.Button joyPadClickButton = OVRInput.Button.One;*/

    //public RecorderAll recorderAll;

    private void Start()
    {
        dataPrefabsQueue = new Queue<GameObject>();
        databoolQueue = new Queue<bool>();

        foreach (GameObject g in dataprefabs) dataPrefabsQueue.Enqueue(g);

        foreach (bool g in record) databoolQueue.Enqueue(g);
    }

    IEnumerator LoadNext() {

        if (!this.enabled) yield break;

        if(currentData!=null)  Destroy(currentData);

        if (dataPrefabsQueue.Count > 0) {
            
            currentData = Instantiate(dataPrefabsQueue.Dequeue());

            recordCurrentdata = databoolQueue.Dequeue();

            if (!currentData.activeSelf) currentData.SetActive(true);

            IgnoreCollistion();

        }

    }

    public void IgnoreCollistion() { 

        CharacterController charCon = FindObjectOfType<CharacterController>();

        if (charCon == null) return;

        Collider[] colliders = currentData.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders) Physics.IgnoreCollision(charCon, c);

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) )
            Next();
    }

    public void Next() {

#if UNITY_EDITOR
        /*if(partecipantsVoiceRecorder!=null) partecipantsVoiceRecorder.StartRecording();*/
#endif  
        if (dataPrefabsQueue.Count > 0)
        {

            StartCoroutine(LoadNext());

            RaiseNetworkEvent();

#if UNITY_EDITOR
            RecordEvent();
#endif  
            

        }
        else {

            StartCoroutine(LoadNext());

#if UNITY_EDITOR
            StopRecorder();
#endif  

        }

    }

    public void RecordEvent() {

        if (recordCurrentdata) avatarRecorder.NewData(currentData.name);

        else avatarRecorder.closeWriter();

    }

    public void StopRecorder()
    {

        avatarRecorder.enabled=false;
    }

    public void RaiseNetworkEvent() {

        object[] data = new object[] { };

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.NextDataDisplay, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientEventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientEventReceived;
    }

    private void NetworkingClientEventReceived(EventData obj)
    {
        if (obj.Code == MasterManager.GameSettings.NextDataDisplay)
        {

            StartCoroutine(LoadNext());

            object[] datas = (object[])obj.CustomData;

        }
    }



}


