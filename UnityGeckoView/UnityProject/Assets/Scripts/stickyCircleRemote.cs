using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class stickyCircleRemote : MonoBehaviourPun
{

    public float alpha;
    public Vector3[] pos;
    LineRenderer lineRenderer;
    RaycastHit hit1 = new RaycastHit();
    RaycastHit hit2 = new RaycastHit();
    public int layerMask;
    public GameObject reorient;
    ReorientManager rom;
    float timeToGo;
    public Transform target;
    private Transform Head;
    private Transform RemoteHead;
    private OVRPlayerController ovrpc;
    private GameObject player;
    private bool renable = false;
    public Material[] Visible;
    public Material[] NonVisible;
    public Vector3 center;
    public bool circleVisible;
    private float from = 0.001f;
    private float to = 0.999f;
    private float howfar = 0f;
    private float middle = 1f;

    void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            
            timeToGo = Time.fixedTime + .2f;

            if (reorient != null)
            {
                if (!checkIfInside(GetAveragePoint()) && alpha>0.1)
                {
                    reorient.SetActive(true);
                }
                else
                {
                    reorient.SetActive(false);
                }
            }
        }



    }

    private void Update()
    {

        if (renable && ovrpc != null)
        {
            renable = false;
            ovrpc.enabled = true;
        }

        if (alpha < 0.1f)
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void Awake()
    {
        layerMask = LayerMask.GetMask("");
        lineRenderer = GetComponent<LineRenderer>();
        reorient = GameObject.Find("ReOrientButtonPlaceholder");
        if (reorient != null)
        {

            rom = reorient.GetComponent<ReorientManager>();
            reorient = rom.g;
            rom.scr = this;
        }
        timeToGo = Time.fixedTime + .2f;

    }

    public void updateLineRender(Vector3[] circlePos)
    {

        pos = circlePos;
        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions(pos);

    }

    public void UpdateAlpha(float circleAlpha)
    {

        alpha = circleAlpha;
        lineRenderer.materials[0].SetFloat("_Alpha", alpha);
        

    }

    public Vector3 GetAveragePoint()
    {

        Vector3 average = Vector3.zero;

        foreach (Vector3 p in pos) average += p;

        center = average / pos.Length;

        return center;

    }

    bool checkIfInside(Vector3 point)
    {

        hit1 = new RaycastHit();
        hit2 = new RaycastHit();

        Vector3 direction = new Vector3(0, 1, 0);

        if (Physics.Raycast(point, direction, out hit1, 6f, layerMask) &&
            Physics.Raycast(point, -direction, out hit2, 6f, layerMask))
        {
            if (hit1.transform.name == hit2.transform.name) return true;
        }

        return false;
    }

    public void ReorientAvatar() {

        if (player==null) player = GameObject.Find("OVRPlayerController");

        if (Head == null && player != null) Head = DeepChildSearch(player, "head_JNT");

        if(RemoteHead == null) RemoteHead = DeepChildSearch(this.transform.parent.gameObject, "head_JNT");

        if (Head != null && player != null )
        {

            Vector3 target = GetAveragePoint();

            //initialize Teleportation
            ovrpc =  player.GetComponent<OVRPlayerController>();
            ovrpc.enabled = false; //disable ovrpc

            //get distance of remote avatar from circle 
            Vector3 translateVector =  target - player.transform.position;
            translateVector = new Vector3(translateVector.x, 0f, translateVector.z); //make translation to xz plane
            translateVector = translateVector.normalized * 1.5f; //set ditsance to 1.5 m
            player.transform.position += translateVector; //translate

            //check that remote player and player are not too close
            if (RemoteHead != null) 
            {
                Vector3 distanceBetweenPlayers = Head.position - RemoteHead.position;

                if (distanceBetweenPlayers.magnitude<1f) {

                    player.transform.position += (distanceBetweenPlayers.normalized * (1f - distanceBetweenPlayers.magnitude));
                }

            }

            //rotate
            Vector3 localTarget = Head.InverseTransformPoint(target);
            float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            player.transform.RotateAround(Head.position, Vector3.up, angle);

            ///todo send rotation and translation to remote player 

            object[] data = new object[] { angle, PhotonNetwork.NickName };
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.Reorient, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

            renable = true;
        }




    }

    public Transform DeepChildSearch(GameObject g, string childName)
    {

        Transform child = null;

        for (int i = 0; i < g.transform.childCount; i++)
        {

            Transform currentchild = g.transform.GetChild(i);

            if (currentchild.gameObject.name == childName)
            {

                return currentchild;
            }
            else
            {

                child = DeepChildSearch(currentchild.gameObject, childName);

                if (child != null) return child;
            }

        }

        return null;
    }

    public void UpdateMaterial(float alpha)
    {

        if (alpha < 1) {
            lineRenderer.materials = Visible;
            circleVisible = true;
        }
        else {
            lineRenderer.materials = NonVisible;
            circleVisible = false;
        }

        if (lineRenderer.positionCount > 0)
        {
            if (howfar > 1f) howfar = 0f;
            howfar += 0.005f;

            middle = Mathf.Lerp(from, to, howfar);

            lineRenderer.materials[0].SetFloat("_Middle", middle);

        }
    }

}
