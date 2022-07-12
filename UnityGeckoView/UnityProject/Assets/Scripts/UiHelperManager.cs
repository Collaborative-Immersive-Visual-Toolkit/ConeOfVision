using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class UiHelperManager : MonoBehaviourPun
{

    private cone own;
    private cone[] remote;
    public LaserPointerModified lpm;

    public bool MaterialActive = true;

    public bool StickyCircleActive = true;

    public void toggleOwnCone()
    {

        GameObject obj = GameObject.FindGameObjectWithTag("owncone");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            c.SwitchVis();

        }

    }

    public void SwitchOnOwnCone()
    {

        GameObject obj = GameObject.FindGameObjectWithTag("owncone");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            if(!c.visible) c.SwitchVis();

        }

    }

    public void SwitchOffOwnCone()
    {

        GameObject obj = GameObject.FindGameObjectWithTag("owncone");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            if (c.visible) c.SwitchVis();

        }

    }

    public void toggleOthersCone()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("remoteavatar");

        foreach (GameObject obj in objs)
        {

            RemoteVisualCone rvc = obj.GetComponentInChildren<RemoteVisualCone>();
            if (rvc != null)
            {
                rvc.SwitchVis();
               
            }

            /*cone c = obj.GetComponentInChildren<cone>();
            if (c != null) c.SwitchVis();*/
        }


        ///this is just for recording purposes 
        GameObject objc = GameObject.FindGameObjectWithTag("owncone");

        if (objc!= null)
        {

            cone c = objc.GetComponent<cone>();
            c.SwitchOthersVis();

        }
    }

    public void SwitchOnOthersCone()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("remoteavatar");

        foreach (GameObject obj in objs)
        {

            RemoteVisualCone rvc = obj.GetComponentInChildren<RemoteVisualCone>();
            if (rvc != null)
            {
                if(!rvc.visible) rvc.SwitchVis();

            }

        }


        ///this is just for recording purposes 
        GameObject objc = GameObject.FindGameObjectWithTag("owncone");

        if (objc != null)
        {

            cone c = objc.GetComponent<cone>();
            if( !c.otherVisible) c.SwitchOthersVis();

        }
    }

    public void SwitchOffOthersCone()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("remoteavatar");

        foreach (GameObject obj in objs)
        {

            RemoteVisualCone rvc = obj.GetComponentInChildren<RemoteVisualCone>();
            if (rvc != null)
            {
                if (rvc.visible) rvc.SwitchVis();

            }

        }


        ///this is just for recording purposes 
        GameObject objc = GameObject.FindGameObjectWithTag("owncone");

        if (objc != null)
        {

            cone c = objc.GetComponent<cone>();
            if (c.otherVisible) c.SwitchOthersVis();

        }
    }

    public void toggleMaterialsUpdate()
    {

        MaterialActive = lpm.toggleMaterialUpdateReturn();

    }

    public void toggleStickyCircle()
    {

        StickyCircleActive = lpm.toggleStickyReturn();
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
        if (obj.Code == MasterManager.GameSettings.UiHelperSwitch)
        {

            object[] data = (object[])obj.CustomData;

            if ((int)data[0] == 1)
            {
                Load1();
            }
            else if ((int)data[0] == 2)
            {
                Load2();
            }

        }
        else if (obj.Code == MasterManager.GameSettings.ConeOff)
        {
            SwitchOffOthersCone();
            SwitchOffOwnCone();

        }
        else if (obj.Code == MasterManager.GameSettings.ConeOn)
        {
            SwitchOnOthersCone();
            SwitchOffOwnCone();
        }
        else if (obj.Code == MasterManager.GameSettings.OwnConeOn)
        {
            SwitchOnOthersCone();
            SwitchOnOwnCone();
        }

    }

    private void Load1()
    {

        if (!MaterialActive) MaterialActive = lpm.toggleMaterialUpdateReturn();
        if (!StickyCircleActive) StickyCircleActive = lpm.toggleStickyReturn();

  
    }

    private void Load2()
    {

        if (MaterialActive) MaterialActive = lpm.toggleMaterialUpdateReturn();
        if (StickyCircleActive) StickyCircleActive = lpm.toggleStickyReturn();
        

    }
}