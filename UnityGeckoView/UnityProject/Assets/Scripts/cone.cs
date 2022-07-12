using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;

public class cone : MonoBehaviourPun
{
    public bool disabled = false;
    public Transform head;
    public TextAsset jsonTextFile;
    private ConeVectors c;
    public LineRenderer lr;
    private List<Vector3> OldPositions;
    public bool visible = true;
    public bool otherVisible = true;
    public ConeRendering r = ConeRendering.intersection;
    public MeshFilter mf;
    public MeshRenderer mr;

    public bool simulated = false;

    // Start is called before the first frame update
    void Start()
    {
        //Load text from a JSON file (Assets/Resources/Text/jsonFile01.json)
        //var jsonTextFile = Resources.Load<TextAsset>("vectors_cone_20_77");

        if (MasterManager.GameSettings.Observer && !simulated)
        {
            lr.positionCount = 0;
            return;
        }

        if (head == null && !simulated) {

            GameObject parent =  GameObject.Find("OVRPlayerController");

            head =  DeepChildSearch(parent, "CenterEyeAnchor");

        }  //the simulated cone does not require an avatar, but will use the transform of the object 
        // also simulated does not send any event trough the network 
        else if (simulated)
        {

            head = this.transform;

        }

        c = ConeVectors.CreateFromJSON(jsonTextFile);

        c.init(head,lr);

      
    }

    void prepareMeshFilterRenderer() {

        GameObject g = DeepChildSearch(head.gameObject, "cone_mesh_render_here").gameObject;

        mf = g.gameObject.AddComponent<MeshFilter>();
        mr = g.gameObject.AddComponent<MeshRenderer>();

        Material[] a = new Material[1];
        a[0] = Resources.Load("Materials/coneMaterial", typeof(Material)) as Material;
        mr.materials = a;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (MasterManager.GameSettings.Observer && !simulated)
        {
            return;
        }

        c.ComputeRaycast();

        if (r == ConeRendering.intersection)
        {
            if (!lr.enabled) lr.enabled = true;
            if (mr.enabled) mr.enabled = false;
            c.updateLineRender(lr);
        }
        else if (r == ConeRendering.solid)
        {
            if (lr.enabled) lr.enabled = false;
            if (!mr.enabled) mr.enabled = true;
            c.updateMesh(mf);
        }



        if (OldPositions != c.positions)
        {
            var data = c.SerializeData();
            RaiseVisualConeChangeEvent(data);
        }
        OldPositions = c.positions;

    }

    public void SwitchVis()
    {
        visible = !visible;

        c.visible = visible;

        if (!visible)
        {
            r = ConeRendering.none;
            c.clearLineRender(lr);

        }
        else {
            r = ConeRendering.intersection;
        }
    }

    public void SwitchOthersVis()
    {
        otherVisible = !otherVisible;
        c.othervisible = otherVisible;
    }

    public void RaiseVisualConeChangeEvent(object[] data)
    {
        if (simulated) return; //if is a simulated cone without an avatar we do not need to send events 

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.VisualConeChange, data, Photon.Realtime.RaiseEventOptions.Default, SendOptions.SendReliable);

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

[System.Serializable]
public class ConeVectors
{
    public Vector3[] vectorsList;
    
    public int[] trianglesList;

    public Transform head;

    public Vector3[] directions;

    public RaycastHit[] hits;

    public RaycastHit currenthit;

    public List<Vector3> positions;

    public List<Vector2> uvpositions;

    public float distances;

    public bool visible = true;
    public bool othervisible = true;

    //gradient 
    private float from = 0.001f;
    private float to= 0.999f;
    private float howfar = 0f;
    private bool direction = true;
    private float alpha = 1f;
    private float middle = 1f;

    // Bit shift the index of the layer (8) to get a bit mask
    // public int layerMask = 1 << 9;
    private static int octagon;
    private static int inverseOctagon;
    private int layerMask;

    public void init(Transform h, LineRenderer lr)
    {

        head = h;

        directions = new Vector3[vectorsList.Length];

        hits = new RaycastHit[vectorsList.Length];

        octagon = 1 << LayerMask.NameToLayer("octagon");
        inverseOctagon = 1 << LayerMask.NameToLayer("inverseOctagon");
        layerMask = octagon | inverseOctagon;

    }

    public static ConeVectors CreateFromJSON(TextAsset jsonString)
    {
        return JsonUtility.FromJson<ConeVectors>(jsonString.ToString());
    }

    public void ComputeWorldToLocal()
    {

        for (int i = 0; i < vectorsList.Length; i++)
        {

            directions[i] = head.localToWorldMatrix * vectorsList[i];

        }

    }

    public void ComputeRaycast()
    {

        ComputeWorldToLocal();

        int i = vectorsList.Length;
        hits = new RaycastHit[i];
        positions = new List<Vector3>();
        uvpositions = new List<Vector2>();
        distances = 0;

        while (i > 0)
        {
            i--;        
            if (Physics.Raycast(head.position, directions[i], out hits[i], 8f, layerMask))
            {
                if (hits[i].collider.gameObject.name == "inverse") {

                    if (hits[i].point.y > 1) {

                        positions.Add(new Vector3(hits[i].point.x,1.959f, hits[i].point.z));                       

                    }
                    else {

                        positions.Add(new Vector3(hits[i].point.x,0.625f, hits[i].point.z));
                    }
                    
                }
                else
                {
                    
                    positions.Add(hits[i].point);
                }              
                distances+=hits[i].distance;
            }

        }

    }

    public void updateLineRender(LineRenderer lr) {

        //alpha
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());

        alpha = 1f - ((distances / positions.Count) / 6.5f);

        lr.materials[0].SetFloat("_Alpha", alpha);

        ///gradient
        /*if (howfar < 0.1f) direction = true;
        else if (howfar > 0.9f) direction = false;

        if (direction) howfar += 0.01f;
        else howfar -= 0.01f;*/

        if (howfar > 1f) howfar = 0f;
        howfar += 0.005f;

        middle = Mathf.Lerp(from, to, howfar);

        lr.materials[0].SetFloat("_Middle", middle);

    }

    public void RaycastForward()
    {     
        int i = vectorsList.Length;
        RaycastHit hit = new RaycastHit();
   
        if (Physics.Raycast(head.position, head.forward, out hit,6f, layerMask))
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = hit.point;
            sphere.transform.localScale = new Vector3(.1f,.1f,.1f) ;
            currenthit = hit;
        }     

    }

    public object[] SerializeData() {

        //object[] data = new object[] { positions.ToArray(), c.a, PhotonNetwork.NickName };

        object[] data = new object[] { positions.ToArray(), alpha, middle, visible, othervisible, PhotonNetwork.NickName };

        return data;
    }

    public void clearLineRender(LineRenderer lr)
    {
       
            lr.positionCount = 0;
        

    }

    public void clearMeshRenderer(MeshRenderer mr) {

        mr.enabled = false;
    }

    public void updateMesh(MeshFilter mf) {

        Vector3[] newVertices = new Vector3[21];

        newVertices[0] = head.transform.position;

        for (int j = 1; j < 21; j++)  newVertices[j] = positions[j - 1];

        for (int j = 0; j < 21; j++) newVertices[j] = mf.gameObject.transform.InverseTransformPoint(newVertices[j]);

        if (mf.mesh.vertexCount == 0)
        {
            Mesh mesh = new Mesh();
            mf.mesh = mesh;
            mesh.vertices = newVertices;
            mesh.triangles = trianglesList;
        }
        else
        {
            mf.mesh.vertices = newVertices;
        }

    }

}

public enum ConeRendering
{
    solid,
    intersection,
    none,
}