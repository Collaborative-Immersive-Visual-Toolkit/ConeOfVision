using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReorientManager : MonoBehaviour
{

    public GameObject g;

    public stickyCircleRemote scr;

    public void reorient() {

        scr.ReorientAvatar();
    }

 
}
