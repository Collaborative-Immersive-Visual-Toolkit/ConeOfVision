using System.IO;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor;
#endif


public class AvatarBehaviourRecorder : MonoBehaviour
{
    string path;

    StreamWriter writer;

    string line;

    public int frameRate = 25;

    float startTime;
    
    float currentTime;

    Char[] remove = new Char[] { ' ', '(', ')' };

    GameObject cube;

    public string fileName;

    public RemoteAvatarsManager ram;

    private bool recording = false;

    string PlayerPos;
    string HeadPos;
    string HeadForward;
    string HeadUp;
    string HeadCone;
    string HeadConeVisibility;
    string OtherHeadConeVisibility;
    string ControllerRPos;
    string ControllerREAng;
    string ControllerLPos;
    string ControllerLEAng;
    string PointerPos;
    string PointerVis;
    string StickyCircle;
    string StickyCircleVis;
    string Relocate;
    string Userspeaking;
    string UserInsight;
    static string null3 = "null,null,null";

    private float nextSampleTime = 0.0f;
    public float sampleFrequency = 0.04f;


#if UNITY_EDITOR
    private RecorderWindow GetRecorderWindow()
    {
        return (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
    }
#endif
    void Update()
    {

#if UNITY_EDITOR

        if (Time.unscaledTime > nextSampleTime)
        {
            nextSampleTime += sampleFrequency;

            if (ram.inputs.Count > 1 && ram.inputs.Count <= 2 && !recording)
            {

               Record();

            }
            else if (ram.inputs.Count > 2)
            {
                Debug.Log("there are too many users logged");
            }


            if (writer == null) return;

            currentTime = Time.unscaledTime - startTime;

            line = currentTime.ToString("F3");

            foreach (inputs i in ram.inputs)
            {
                PlayerPos = i.gameObject.transform.position.ToString("F3");
                HeadPos = i.LocalHead == null ? "null,null,null" : i.LocalHead.position.ToString("F3");               
                HeadForward = i.LocalHead == null ? "null,null,null" : i.LocalHead.forward.ToString("F3");
                //HeadUp = i.LocalHead == null ? "null" : i.LocalHead.up.ToString("F3");
                HeadCone = i.Cone == null ? "null" : Vector2ArrayToString(i.Cone.uvpos.ToArray());
                HeadConeVisibility = i.Cone.userVisible ? "1" : "0";
                OtherHeadConeVisibility = i.Cone.otherUserVisible ? "1" : "0";
                ControllerRPos = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.position.ToString("F3");
                ControllerREAng = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.eulerAngles.ToString("F3");
                ControllerLPos = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.position.ToString("F3");
                ControllerLEAng = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.eulerAngles.ToString("F3");
                PointerPos = i.Pointer._endPoint == Vector3.zero || i.Pointer.isUI ? "null,null,null" : i.Pointer._endPoint.ToString("F3");
                PointerVis = i.Pointer.insideOtherCone && PointerPos  != "null,null,null" ? "1" : "0";
                StickyCircle = i.StickyCircle.GetAveragePoint() == Vector3.zero ? "null,null,null" : i.StickyCircle.center.ToString("F3");
                StickyCircleVis = i.StickyCircle.circleVisible ? "1" : "0";
                Userspeaking = i.Speaking.isSpeaking ? "1" : "0";
                UserInsight = i.Insights.recording ? "1" : "0";

                line += "," + PlayerPos.Trim(remove) + "," +
                              HeadPos.Trim(remove) + "," + HeadForward.Trim(remove) + "," + 
                              HeadCone.Trim(remove) + "," + HeadConeVisibility.Trim(remove) + "," + OtherHeadConeVisibility.Trim(remove) + "," +
                              ControllerRPos.Trim(remove) + "," + ControllerREAng.Trim(remove) + "," +
                              ControllerLPos.Trim(remove) + "," + ControllerLEAng.Trim(remove) + "," +
                              PointerPos.Trim(remove) + "," + PointerVis.Trim(remove) + "," +
                              StickyCircle.Trim(remove) + "," + StickyCircleVis.Trim(remove) + "," +
                              Userspeaking.Trim(remove) + "," + UserInsight.Trim(remove);

            }

            writer.WriteLine(line);
        }
#endif
    }

    private static string Vector2ArrayToString(Vector2[] array) {

        string sarray = "";

        foreach(Vector2 a in array) sarray += a.ToString("F3");

        return sarray.Replace(',', ';').Replace(')',':').Replace("(","");
    }
   
    private void Record()
    {
        
            recording = true;
            this.NewData(fileName);
            StartUnityRecorder();
        
    }

    public void NewData(string name) {

        if (writer != null) closeWriter();

        path = MasterManager.GameSettings.DataFolder +"\\" + name + ".csv";
        writer = new StreamWriter(path, true);

        writer.WriteLine("time, " +
            "U1PosX,U1PosY,U1PosZ,U1HeadX,U1HeadY,U1HeadZ,U1HeadForwardX,U1HeadForwardY,U1LocalHeadForwardZ,U1HeadCone,U1HeadConeVisibility,U1OtherHeadConeVisibility," +
            "U1ControllerRX,U1ControllerRY,U1ControllerRZ,U1ControllerEulerRX,U1ControllerEulerRY,U1ControllerEulerRZ," +
            "U1ControllerLX,U1ControllerLY,U1ControllerLZ,U1ControllerEulerLX,U1ControllerEulerLY,U1ControllerEulerLZ," +
            "U1PointerX,U1PointerY,U1PointerZ,U1PointerVis,U1StickyPointerX,U1StickyPointerY,U1StickyPointerZ,U1StickyPointerVis,U1Speaking,U1InsightRecording," +
            "U2PosX,U2PosY,U2PosZ,U2HeadX,U2HeadY,U2HeadZ,U2HeadForwardX,U2HeadForwardY,U2LocalHeadForwardZ,U2HeadCone,U2HeadConeVisibility,U2OtherHeadConeVisibility," +
            "U2ControllerRX,U2ControllerRY,U2ControllerRZ,U2ControllerEulerRX,U2ControllerEulerRY,U2ControllerEulerRZ," +
            "U2ControllerLX,U2ControllerLY,U2ControllerLZ,U2ControllerEulerLX,U2ControllerEulerLY,U2ControllerEulerLZ," +
            "U2PointerX,U2PointerY,U2PointerZ,U2PointerVis,U2StickyPointerX,U2StickyPointerY,U2StickyPointerZ,U2StickyPointerVis,U2Speaking,U2InsightRecording");

        startTime = Time.unscaledTime;

    }

    void OnDisable()
    {
        closeWriter();
        StopUnityRecorder();
    }

    void OnApplicationQuit()
    {
        closeWriter();
        StopUnityRecorder();
    }

    public void closeWriter() {

        recording = false;

        if (writer != null) writer.Close();

        writer = null;

    }


    public void StartUnityRecorder() {
#if UNITY_EDITOR
        RecorderWindow recorderWindow = GetRecorderWindow();

        if (!recorderWindow.IsRecording())
        {
            recorderWindow.StartRecording();
        }

        foreach (inputs i in ram.inputs)
        {
            i.VoiceRecorder.StartRecording();
        }

#endif
    }

    public void StopUnityRecorder() {
#if UNITY_EDITOR
        RecorderWindow recorderWindow = GetRecorderWindow();
        if (recorderWindow.IsRecording())
            recorderWindow.StopRecording();

        foreach (inputs i in ram.inputs)
        {
            i.VoiceRecorder.SaveAndCloseFile();
        }
#endif
    }

}
