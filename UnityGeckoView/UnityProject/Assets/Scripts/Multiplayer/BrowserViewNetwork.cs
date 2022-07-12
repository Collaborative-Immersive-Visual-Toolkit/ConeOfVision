using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class BrowserViewNetwork : MonoBehaviourPun
{

    public void RaiseAddLongTapNetworkEvent(object[] data)
    {
        
        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.AddLongTap, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

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
        if (obj.Code == MasterManager.GameSettings.AddLongTap)
        {

            //StartCoroutine(LoadNext());

            object[] data = (object[])obj.CustomData;

            if ((string)data[2] == gameObject.transform.parent.gameObject.name) {

                gameObject.SendMessage("ReceivedAddLongTapNetworkEvent", data);

            }

            
        }
    }
}
