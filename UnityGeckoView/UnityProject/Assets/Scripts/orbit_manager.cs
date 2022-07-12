using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbit_manager : MonoBehaviour
{
    OVRGrabbable grabbableComponent;

    GameObject orbit;

    bool inOrbit = false;

    // Start is called before the first frame update
    void Start()
    {
        grabbableComponent = gameObject.GetComponent<OVRGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inOrbit) Orbiting();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("orbit")) {

            orbit = other.gameObject;

            OVRGrabber grabber = grabbableComponent.grabbedBy.GetComponent<OVRGrabber>();

            grabber.ForceRelease(grabbableComponent);

            inOrbit = true;

        }
        
    }

    private void Orbiting()
    {
        
        gameObject.transform.RotateAround(orbit.transform.position, orbit.transform.up, 1f);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.name.Contains("orbit"))
    //    {

    //        inOrbit = false;

    //        orbit = null;
    //    }
    //}

 
}
