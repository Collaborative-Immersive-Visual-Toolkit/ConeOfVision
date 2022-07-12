using System.IO;
using UnityEngine;


public class RecorderObject:MonoBehaviour
{
    FileStream stream;

    public bool recording = false;


#if UNITY_EDITOR

    /*void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (!recording)
            {
                Debug.Log("[start recording");
                StartRecording();
            }
               
            else
            {
                Debug.Log("[stop recording");
                SaveAndCloseFile();
            }
                
        }
    }*/

    public void StartRecording()
    {

        Debug.Log("[voice recording object] Open File Stream");

        recording = true;

        //Create and open file for the stream in RemoteVoiceAdded handler.

        string fileName = generateFilename();

        string path = MasterManager.GameSettings.DataFolder + "\\" + fileName + ".wav";

        stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

    }

    public string generateFilename()
    {

        string fileName = gameObject.transform.parent.gameObject.name.ToString();

        return fileName;
    }

    public void SaveAndCloseFile()
    {
        recording = false;

        //Save and close the file in RemoteVoiceRemoved handler.
        if (stream != null) stream.Close();

        Debug.Log("[voice recording object] Closing File Stream");
    }

    public void WriteFrameAudioData(float[] obj)
    {
        if (stream != null)  stream.AppendWaveData(obj);
    }

    public void OnApplicationQuit()
    {
        SaveAndCloseFile();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (recording) WriteFrameAudioData(data);
    }
#endif

}
