/*
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Realtime;

public class PartecipantsVoiceRecorder : MonoBehaviourPun
{
    BinaryWriter writer;

    List<RecorderObject> list = new List<RecorderObject>();

    bool recording = false;

    public Text label;
        
    private void OnEnable()
    {
#if UNITY_EDITOR
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientEventReceived;
        PhotonVoiceNetwork.Instance.RemoteVoiceAdded += HandleRemoteVoicAdded;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientEventReceived;
        PhotonVoiceNetwork.Instance.RemoteVoiceAdded -= HandleRemoteVoicAdded;
#endif
    }

    private void NetworkingClientEventReceived(EventData obj)
    {
        if (obj.Code == MasterManager.GameSettings.StartRecordAudio)
        {

            object[] data = (object[])obj.CustomData;

            StartRecordingByPlayerId(data[0]);

        }
        else if (obj.Code == MasterManager.GameSettings.StopRecordAudio) 
        {

            object[] data = (object[])obj.CustomData;

            StopRecordingByPlayerId(data[0]);

        }
    }

    private void HandleRemoteVoicAdded(RemoteVoiceLink obj)
    {

        Debug.Log("[voice recording] Create a new voice recorded object linked to the remote voice link");

        RecorderObject ro = new RecorderObject();

        ro.obj = obj;



        list.Add(ro);

    }

    public void ToggleRecording() {

        object[] data = new object[] { PhotonNetwork.LocalPlayer.UserId };


        if (recording) 
        {
            recording = false;
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.StopRecordAudio, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
            label.text = "Insight Record";
        }
        else 
        {
            recording = true;
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.StartRecordAudio, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
            label.text = "Stop Record Insight";
        }

    }

    public void StartRecordingByPlayerId(object playerId)
    {

        if (recording) return;

     
        Player player =null;
        
        GameObject go;

        foreach (Player p in PhotonNetwork.PlayerList) {


            if (p.UserId == (string)playerId) {

                player = p;

                Debug.Log(p.ActorNumber);
        

            }

        }

        if (player == null) return;


        foreach (RecorderObject r in list)
        {
            if (r.obj.PlayerId == player.ActorNumber)
            {
                r.g = GameObject.Find(player.NickName);
                r.StartRecording();
            } 
                
        }
            
        recording = true;

    }

    public void StopRecordingByPlayerId(object playerId)
    {

        if (!recording) return;

        Player player = null;

        foreach (Player p in PhotonNetwork.PlayerList)
        {

            if (p.UserId == (string)playerId)
            {

                player = p;
            }

        }

        if (player == null) return;

        foreach (RecorderObject r in list)
        {
            if (r.obj.PlayerId == player.ActorNumber)
            {
               
                r.SaveAndCloseFile();
            }

        }

        recording = false;

    }

    public void OnApplicationQuit()
    {
        foreach (RecorderObject r in list)
            r.SaveAndCloseFile();
    }

   
}
*/