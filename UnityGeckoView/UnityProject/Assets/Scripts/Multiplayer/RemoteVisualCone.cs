using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteVisualCone : MonoBehaviour
{

    public bool visible = true;
    public LineRenderer lineRenderer;
    public Vector3[] pos;
    public List<Vector2> uvpos;
    public bool userVisible = true;
    public bool otherUserVisible = true;
    //gradient 
    private float from = 0.001f;
    private float to = 0.999f;
    private float howfar = 0f;
    private bool direction = true;
    private float alpha = 1f;
    public RaycastHit[] hits;

    // Bit shift the index of the layer (8) to get a bit mask
    // public int layerMask = 1 << 9;
    private static int octagon;
    private static int inverseOctagon;
    private int layerMask;

    private Transform head;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        octagon = 1 << LayerMask.NameToLayer("octagon");
        inverseOctagon = 1 << LayerMask.NameToLayer("inverseOctagon");
        layerMask = octagon | inverseOctagon;

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
        if (obj.Code == MasterManager.GameSettings.VisualConeChange)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[5] == gameObject.transform.parent.gameObject.name)
            {
                
                userVisible = (bool)data[3]; //user choice to see his own cone
                otherUserVisible = (bool)data[4];
                if (visible)UpdateVisualCone(data[0],data[1], data[2]);

            }

        }
    }

    private void UpdateVisualCone(object positions,object alpha, object middle)
    {

        pos = (Vector3[])positions;

        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions(pos);

        lineRenderer.materials[0].SetFloat("_Middle", (float)middle);
        lineRenderer.materials[0].SetFloat("_Alpha", (float)alpha);

        if (MasterManager.GameSettings.Observer)   CalculateUvPositions(positions);
    }

    private void CalculateUvPositions(object positions)
    {
        pos = (Vector3[])positions;
        int i = pos.Length;
        hits = new RaycastHit[i];
        uvpos = new List<Vector2>();

        i = pos.Length;

        while (i > 0)
        {
            i--;
            if (Physics.Raycast(Vector3.zero, pos[i], out hits[i], 10f, layerMask))
            {
                if (hits[i].collider.gameObject.name == "octagon")
                {
                    uvpos.Add(hits[i].textureCoord);
                }
              
            }

        }
    }
    
    public void SwitchVis()
    {

        visible = !visible;

        if (!visible) { clearLineRender(lineRenderer); } 


    }

    public void clearLineRender(LineRenderer lr)
    {
        lineRenderer.positionCount = 0;

    }

    private Transform DeepChildSearch(GameObject g, string childName)
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

}
