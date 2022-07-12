using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class stickyCircle : MonoBehaviour
{
    private List<Vector3> stickyPointsList;
    float[] pos = { 0f, 0.125f, 0.25f, 0.375f, 0.5f, 0.625f, 0.75f, 0.875f, 1f, 1.125f, 1.25f, 1.375f, 1.5f, 1.625f, 1.75f, 1.875f };
    public Vector3[] _circlePos = new Vector3[16];
    private bool updateMaterial = true;
    public Vector3[] circlePos {

        get { 
            return _circlePos; 
        }
        set {

            _circlePos = value;
            _circlePos = new Vector3[16];
        }
    }

    LineRenderer lineRenderer;
    private float timeLeft;
    private int counter=0;
    public Vector3 center;
    private float from = 0.001f;
    private float to = 0.999f;
    private float howfar = 0f;
    private float middle = 1f;
    private bool redraw = true;
    public float alpha;
    public Material[] Visible;
    public Material[] NonVisible;
    public bool insideOtherCone;

    public bool stickyCircleActive = true;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

    }
    
    void Update()
    {
        if (!stickyCircleActive) return;

        if (stickyPointsList != null && stickyPointsList.Count > 6 && redraw)
        {
            create();
            redraw = false;
        }

        if (lineRenderer.positionCount > 0)
        {
            if (howfar > 1f) howfar = 0f;
            howfar += 0.005f;

            middle = Mathf.Lerp(from, to, howfar);

            UpdateMaterial();

            lineRenderer.materials[0].SetFloat("_Middle", middle);
          
        }

        if (timeLeft > -1f)
        {
            countdown();
        }


    }

    public void capture(Vector3 point)
    {

        if (stickyPointsList == null)
        {
            stickyPointsList = new List<Vector3>();
            counter = 3;
        }

        //limit the number of sample to plus or minus 3 per second 
        counter += 1;
        if (counter > 3)
        {
            stickyPointsList.Add(point);
            counter = 0;
            redraw = true;
        }

        if (stickyPointsList.Count == 15) {

            stickyPointsList.RemoveRange(0, 1);
        }

    }

    public void cleanList() {

        stickyPointsList =null;
        lineRenderer.positionCount = 0;        
        timeLeft = -1f;
    }
   
    public void create() {
   
            GenerateCircle();
            updateLineRender();
            alpha = 1;
            lineRenderer.materials[0].SetFloat("_Alpha", alpha);
    }

    private void GenerateCircle()
    {
        circlePos = new Vector3[16];

        if (stickyPointsList.Count < 6) return;

        center = GetAveragePoint();
        float radius = StdDev(center);
        Vector3 normal = Normal(stickyPointsList[0], stickyPointsList[2], stickyPointsList[5]);

        //create one ortogonal vector to the normal vector i 
        //  (0,z,−y) 
        Vector3 i = new Vector3(0f, normal.z, -normal.y);
        // (−z,0,x) 
        if(i.magnitude==0) i = new Vector3(-normal.z, 0f, normal.x);
        // (y,−x, 0)
        if (i.magnitude == 0) i = new Vector3(normal.y, -normal.x, 0f);

        //normalize ortogonal vectors
        i = i.normalized;

        //create vector j ortogonal to both normal and i 
        Vector3 j = Vector3.Cross(normal, i).normalized;

        //create points for line renderer
        //cosθi +sinθj
        for (int c = 0; c < 16; c++)
        {
            circlePos[c] = center +  i * radius * (float)Math.Cos(pos[c]*Math.PI) + j * radius * (float)Math.Sin(pos[c] * Math.PI);
        }

    }

    Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
    {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }

    private Vector3 GetAveragePoint()
    {

        Vector3 average = Vector3.zero;

        foreach (Vector3 p in stickyPointsList) average += p;

        return average / stickyPointsList.Count;

    }

    private float Mean(float[] list)
    {

        float mean = 0f;

        foreach (float p in list) mean += p;

        return mean / list.Length;

    }

    private float SumOfSquares(float[] list, float mean)
    {

        float[] sos = new float[list.Length];

        for (int i = 0; i < list.Length; i++)
        {
            float value = list[i];
            sos[i] = (value - mean) * (value - mean);

        }

        return Mean(sos);

    }

    private float StdDev(Vector3 center)
    {
        float[] values = new float[stickyPointsList.Count];

        for (int i = 0; i < stickyPointsList.Count; i++)
        {
            Vector3 distance = center - stickyPointsList[i];

            values[i] = distance.magnitude;
        }

        // Get the mean.
        float mean = Mean(values);

        // Get the sum of the squares of the differences
        // between the values and the mean.
        //var sum_of_squares = SumOfSquares(values, mean);

        //return (float)Math.Sqrt(sum_of_squares / values.Length);

        return mean;
    }

    private void updateLineRender()
    {

        //alpha
        lineRenderer.positionCount = circlePos.Length;
        lineRenderer.SetPositions(circlePos);

    }

    public void DestroySlowly() {

        timeLeft = 3f;
    }

    private void UpdateMaterial()
    {
        if (!updateMaterial) return;
        if (insideOtherCone)  lineRenderer.materials = Visible;         
        else lineRenderer.materials = NonVisible;
     
    }
    public void toggleMaterialUpdate() {
        updateMaterial = !updateMaterial;

    }

    private void countdown() {
        
        timeLeft -= Time.deltaTime;

        if (timeLeft < 0)
        {
            lineRenderer.positionCount = 0;
            center = Vector3.zero;
            zerocriclepos();
        }
        else {
            alpha = timeLeft / 3f;
            lineRenderer.materials[0].SetFloat("_Alpha", alpha);  
        }

    }

    private void zerocriclepos()
    {

        circlePos = new Vector3[16];

        for (int i = 0; i < 16; i++) {

            circlePos[i] = Vector3.zero;

        }

    }

    public void toggleStickyCircleVisibility() {

        stickyCircleActive = !stickyCircleActive;

        if (!stickyCircleActive) {

            lineRenderer.positionCount = 0;
            center = Vector3.zero;
            zerocriclepos();

        }
    }


}