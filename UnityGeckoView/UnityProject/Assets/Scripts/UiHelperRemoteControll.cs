using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class UiHelperRemoteControll : MonoBehaviourPun
{



    public void Update()
    {


#if UNITY_EDITOR

        if (Input.GetKeyDown("9"))
        {
            object[] data = new object[] { 1 };
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.UiHelperSwitch, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

        }
        else if (Input.GetKeyDown("0"))
        {
            object[] data = new object[] { 2 };
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.UiHelperSwitch, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

        }
        else if (Input.GetKeyDown("q"))
        {
            Debug.Log("ConeOff");
            object[] data = new object[] {};
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.ConeOff, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

        }
        else if (Input.GetKeyDown("w"))
        {
            object[] data = new object[] {};
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.ConeOn, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

        }
        else if (Input.GetKeyDown("e"))
        {
            object[] data = new object[] {};
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.OwnConeOn, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

        }

#endif


    }


}
