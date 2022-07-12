using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;

public class EndOfTrialController : MonoBehaviourPun
{
    private void Update()
    {
        if (Input.GetKeyDown("n"))
        {
            RaiseNetworkEventEvent();
        }
    }

    private void RaiseNetworkEventEvent()
    {
        object[] data = new object[] {  };

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.EndOfTrialEvent, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }


}
