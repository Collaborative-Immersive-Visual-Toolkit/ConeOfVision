using Photon.Pun;
using ExitGames.Client.Photon;


public class Insights : MonoBehaviourPun
{

    public  bool recording = false;

#if UNITY_EDITOR


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
        if (obj.Code == MasterManager.GameSettings.StartRecordAudio)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[0] == gameObject.name) recording = true;

        }
        else if (obj.Code == MasterManager.GameSettings.StopRecordAudio)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[0] == gameObject.name) recording = false;

        }
    }


#endif


}
