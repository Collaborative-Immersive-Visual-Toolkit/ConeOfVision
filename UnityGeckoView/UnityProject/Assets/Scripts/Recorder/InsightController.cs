using Photon.Pun;
using UnityEngine.UI;


public class InsightController : MonoBehaviourPun
{

    bool recording = false;

    public Text label;

    public void ToggleRecording()
    {

        object[] data = new object[] { PhotonNetwork.NickName };


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

}
