using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class LaserNetworkEvent : MonoBehaviourPun
{

    public void RaiseLaserChangeEvent(object[] data)
    {

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.LaserPointerChange, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }


}
