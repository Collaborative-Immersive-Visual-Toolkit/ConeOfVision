using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;

    public static T Instance {

        get {

            if (_instance ==null) 
            {

                T[] results = Resources.FindObjectsOfTypeAll<T>();

                if (results.Length == 0)
                {

                    Debug.Log("[SingletonScriptableObject] failed to find object of type: " + typeof(T).ToString());
                    return null;
                }
                else if (results.Length > 1) {

                    Debug.Log("[SingletonScriptableObject] result length is longer than 1 for object of type: " + typeof(T).ToString());
                    return null;
                }

                _instance = results[0];
            }
            return _instance;
        }
    }
}
