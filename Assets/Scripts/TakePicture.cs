using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePicture : MonoBehaviour
{

    private bool cameraEquipped;


    void Start()
    {
        cameraEquipped = false;
    }

    void Update()
    {
        if(LevelManager.isDiving)
        {
            if(Input.GetKey(KeyCode.Mouse1))
            {
                LevelManager.Instance.UseCameraUI();
                cameraEquipped = true;
            } 
            else
            {
                LevelManager.Instance.UsePlayerUI();
                cameraEquipped = false;
            }

            if(cameraEquipped)
            {               
                if(Input.GetKeyDown(KeyCode.Mouse0) && LevelManager.storageLeft > 0)
                {
                    CapturePic();
                }
            }
        }
    
    }

    void CapturePic()
    {
        Debug.Log("Pic taken");
        LevelManager.storageLeft -= 1;  
        RaycastHit hit;

        // If pointing at a FISH!
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {

            if(hit.collider.CompareTag("Fish"))
            {
                FishController fish = hit.collider.GetComponent<FishController>();
                if (fish != null)
                {
                    float fishValue = fish.fishValue;
                    LevelManager.Instance.addFishValue(fishValue);
                    Debug.Log("Fish hit! Value: " + fishValue);
                    Debug.Log("Pics left: " + LevelManager.storageLeft);
                }
            }

        }

    }
}
