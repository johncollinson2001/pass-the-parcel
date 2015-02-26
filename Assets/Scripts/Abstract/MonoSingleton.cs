using UnityEngine;
using System.Collections.Generic;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(T)) as T;

                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString());
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}