using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHelperCondition : MonoBehaviour
{
    public bool helpersOn;

    // Start is called before the first frame update
    void Start()
    {
        GameObject g =  GameObject.FindGameObjectWithTag("uihelpers");

        if (g != null) 
        {
            UiHelperManager uhm = g.GetComponentInChildren<UiHelperManager>();

            if (uhm != null) 
            {

                if (helpersOn)
                {
                    if (!uhm.MaterialActive) uhm.toggleMaterialsUpdate();
                    if (!uhm.StickyCircleActive) uhm.toggleStickyCircle();
                }
                else 
                {
                    if (uhm.MaterialActive) uhm.toggleMaterialsUpdate();
                    if (uhm.StickyCircleActive) uhm.toggleStickyCircle();
                }

            }
        }
        else 
        {
            Debug.LogError("UiHelpers Not Found Cannot alter the experimental conditions");
        }


    }

   
}
