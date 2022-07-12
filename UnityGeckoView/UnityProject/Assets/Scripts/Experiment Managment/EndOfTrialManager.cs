using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;

public class EndOfTrialManager : MonoBehaviourPun
{
    public UnityEvent OnEndOfTrialEvent;

    public bool invoked = false;

    public bool fix;

    public void Start()
    {
        if (!MasterManager.GameSettings._devmode && !fix)
        {

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {

                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
       
    }

    public void InvokeEndOfTrial() {

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

            object[] data = (object[])obj.CustomData;


            //angle = (float)data[0];

            InvokeEndOfTrial();

        }

    }

}
