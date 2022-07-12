using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class remote_reorientation_manager : MonoBehaviourPun
{
    private float angle;
    Transform Head;

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
        if (obj.Code == MasterManager.GameSettings.Reorient)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[1] == gameObject.name)
            {

                angle = (float)data[0];

                ReorientAvatar(angle);

            }

        }

    }

    public void ReorientAvatar(float angle)
    {

        if (Head == null ) Head = DeepChildSearch(this.gameObject, "head_JNT");

        if (Head != null )
        {

            this.transform.RotateAround(Head.position, Vector3.up, angle);


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

}
