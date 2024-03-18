using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    private static PlayerSingleton singletonInstance;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (singletonInstance == null)
        {
            singletonInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
