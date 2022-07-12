using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteLaser : MonoBehaviourPun
{
    public GameObject Pointer;
    public enum LaserBeamBehavior
    {
        On,        // laser beam always on
        Off,        // laser beam always off
        OnWhenHitTarget,  // laser beam only activates when hit valid target
    }
    private LaserBeamBehavior laserBeamBehavior;
    private LineRenderer lineRenderer;

    public GameObject stickyPointerPrefab;
    private GameObject stickyPointer;

    public bool insideOtherCone;

    private Vector3 _startPoint;
    public Vector3 _endPoint;
    private bool _hitTarget;
    private bool sticky;
    private bool reorient;
    public bool isUI;

    private Color c;

    private Vector3[] circlepos;
    private float circlealpha;

    public Material[] Visible;
    public Material[] NonVisible;
    public stickyCircleRemote circle;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        //c = lineRenderer.materials[0].color;
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientEventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientEventReceived;
    }

    private void NetworkingClientEventReceived(EventData obj)
    {
        if (obj.Code == MasterManager.GameSettings.LaserPointerChange)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[9] == gameObject.transform.parent.gameObject.name)
            {

                _startPoint = (Vector3)data[0];
                _endPoint = (Vector3)data[1];
                _hitTarget = (bool)data[2];
                isUI = (bool)data[3];
                sticky = (bool)data[4];
                laserBeamBehavior = (LaserBeamBehavior)data[5];
                insideOtherCone = (bool)data[6];
                circlepos = (Vector3[])data[7];
                circlealpha = (float)data[8];

                UpdatePointer();
                UpdateLaserBeam();
                //UpdateStickyPointer();
                UpdateMaterial();
                UpdateStickyCircle();



            }

        }
 
    }

    private void UpdatePointer() {

        Pointer.transform.position = _endPoint;
    }
    
    private void UpdateLaserBeam()
    {
        if (laserBeamBehavior == LaserBeamBehavior.Off)
        {
            _endPoint = Vector3.zero;

            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                Pointer.SetActive(false);         
            }

            return;
        }
        else if (laserBeamBehavior == LaserBeamBehavior.On)
        {
            lineRenderer.SetPosition(0, _startPoint);
            lineRenderer.SetPosition(1, _endPoint);          
        }
        else if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            if (_hitTarget)
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                    Pointer.SetActive(true);
                }

                lineRenderer.SetPosition(0, _startPoint);
                lineRenderer.SetPosition(1, _endPoint);
            }
            else
            {
                _endPoint = Vector3.zero;

                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    Pointer.SetActive(false);                
                }
            }
        }
    }

    private void UpdateMaterial()
    {

            if (insideOtherCone) lineRenderer.materials= Visible;
            else lineRenderer.materials = NonVisible;
        
    }

    private void UpdateStickyPointer() {

        if (stickyPointer == null)
        {
            stickyPointer = Instantiate(stickyPointerPrefab, _endPoint, Quaternion.identity);
        }

        if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            stickyPointer.transform.position = _endPoint;
        }

        stickyPointer.SetActive(sticky);
        
    }

    private void UpdateStickyCircle()
    {

        if(circlepos != circle.pos) circle.updateLineRender(circlepos);
        if (circlealpha != circle.alpha)
        {
            circle.UpdateMaterial(circlealpha);
            circle.UpdateAlpha(circlealpha);
        }

    }


}
