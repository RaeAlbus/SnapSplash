using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightBehavior : MonoBehaviour
{
    public Light flashlight;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && LevelManager.hasFlashlight)
        {
            SoundManager.Instance.PlayFlashlightSFX();
            flashlight.enabled = !flashlight.enabled;
        }  
    }
}
