using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static Dictionary<string, DontDestroyOnLoad> singletons = new Dictionary<string, DontDestroyOnLoad>();
    public string singletonName;
    void Start()
    {
        if(singletons.ContainsKey(singletonName))
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            singletons[singletonName] = this;
        }
    }

}
