
using System.IO;
using UnityEngine;

public class RecorderAll : MonoBehaviour
{
    FileStream stream;

    public bool recOutput = false;

    public void StartRecording()
    {

        Debug.Log("[voice recording object] Open File Stream");


        //Create and open file for the stream in RemoteVoiceAdded handler.

        string fileName = "all";

        string path = MasterManager.GameSettings.DataFolder + "\\" + fileName + ".wav";

        stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        recOutput = true;

    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (recOutput)
        {
            stream.AppendWaveData(data); //audio data is interlaced
        }
    }

    public void SaveAndCloseFile()
    {
        //Save and close the file in RemoteVoiceRemoved handler.
        stream.Close();

        Debug.Log("[voice recording object] Closing File Stream");
    }


    private void OnApplicationQuit()
    {
        SaveAndCloseFile();
    }


}