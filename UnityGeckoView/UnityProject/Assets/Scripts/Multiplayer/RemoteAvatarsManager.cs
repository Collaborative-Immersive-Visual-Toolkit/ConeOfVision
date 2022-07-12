using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAvatarsManager : MonoBehaviour
{
    public List<inputs> inputs;
    public List<GameObject> List;

    private void Awake()
    {
        List = new List<GameObject>();
        inputs = new List<inputs>();
    }

    public void AddToList(GameObject avatar) {

        bool added = false;

        for (int i = 0; i < List.Count; i++)
        {

            if (List[i].name == avatar.name)
            {

                List[i] = avatar;
                inputs[i] = avatar.GetComponent<inputs>();
                added = true;

            }
            
        }

        if (!added) 
        {

            List.Add(avatar);
            inputs.Add(avatar.GetComponent<inputs>());

        }


    }

    private void Update()
    {
        if (List.Count > 0)
        {
            for (int i = 0; i < List.Count; i++)
            {

                if (List[i] == null)
                {
                    inputs.RemoveAt(i);
                    List.RemoveAt(i);
                }
            }
        }
    }

    public void switchVisibility() {

        
                foreach (GameObject g in List)
                {

                    Transform t = DeepChildSearch(g, "VisualCone");

                    t.GetComponent<RemoteVisualCone>().visible = !t.GetComponent<RemoteVisualCone>().visible;


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


}
