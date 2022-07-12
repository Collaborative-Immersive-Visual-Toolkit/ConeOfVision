using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class speaking : MonoBehaviour
{
    public AudioSource audioSource;

    float[] clipSampleData = new float[1024];
    public bool isSpeaking = false;
    public float minimumLevel = 0.001f;

    void Update()
    {
#if UNITY_EDITOR
        audioSource.GetSpectrumData(clipSampleData, 0, FFTWindow.Rectangular);
        float currentAverageVolume = clipSampleData.Average();

        if (currentAverageVolume > minimumLevel)
        {
            isSpeaking = true;
        }
        else if (isSpeaking)
        {
            isSpeaking = false;
            //volume below level, but user was speaking before. So user stopped speaking
        }

#endif
    }

}

