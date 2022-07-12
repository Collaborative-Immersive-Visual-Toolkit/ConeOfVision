using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;

public class EndOfTrialRemote : MonoBehaviourPun
{

    public UnityEvent OnEndOfTrialEvent;

    public bool invoked = false;

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
        if (obj.Code == MasterManager.GameSettings.EndOfTrialEvent)
        {

            InvokeEndOfTrial();

        }

    }

    public void InvokeEndOfTrial()
    {

        if (invoked)
        {
            return;
        }
        else
        {
            invoked = false;
            OnEndOfTrialEvent.Invoke();
            Destroy(gameObject);
        }
    }

}
