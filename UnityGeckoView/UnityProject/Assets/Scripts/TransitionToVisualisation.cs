using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionToVisualisation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (TransitionManager.Instance.isWaiting)
        {
            TransitionManager.Instance.setExperimentView();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
