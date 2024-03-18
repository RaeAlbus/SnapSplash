using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TakePictureTEST : MonoBehaviour
{
    public float raycastWidth = 0.6f;
    public float raycastDistance = 30;

    public float pictureThreshold = 30;

    GameObject[] objectsToTakePicturesOf;
    private bool cameraEquipped;

    void Start()
    {
        cameraEquipped = false;
        objectsToTakePicturesOf = GameObject.FindGameObjectsWithTag("Fish");
    }

    void Update()
    {
        //if (LevelManager.isDiving)
        //{
            if (Input.GetKey(KeyCode.Mouse1))
            {
                //LevelManager.Instance.UseCameraUI();
                cameraEquipped = true;
            }
            else
            {
                //LevelManager.Instance.UsePlayerUI();
                cameraEquipped = false;
            }

            if (cameraEquipped)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))// && LevelManager.storageLeft > 0)
                {
                    CapturePic();
                }
            }
        //}

    }

    void CapturePic()
    {
        /*
        Debug.Log("Pic taken");
        // Cast a sphere along the line from the camera
        RaycastHit[] hits = Physics.SphereCastAll(Camera.main.transform.position, raycastWidth, Camera.main.transform.forward, raycastDistance);

        // Process the hits
        foreach (RaycastHit hit in hits)
        {
            // Handle the hit
            if (hit.collider.CompareTag("Fish"))
            {
                FishController fish = hit.collider.GetComponent<FishController>();
                if (fish != null)
                {
                    float fishValue = fish.fishValue;
                    //LevelManager.Instance.addFishValue(fishValue);
                    Debug.Log("Fish hit! Value: " + fishValue);
                    //Debug.Log("Pics left: " + LevelManager.storageLeft);
                }
            }
        }
        */
        if(LevelManager.storageLeft != 0)
        {
            SoundManager.Instance.PlayCameraSFX();
        } else {
            SoundManager.Instance.PlayNoStorageSFX();
        }

        foreach (GameObject objectToTakePictureOf in objectsToTakePicturesOf)
        {
            Vector3 directionToObject = objectToTakePictureOf.transform.position - transform.position;
            if (Vector3.Magnitude(directionToObject) < 30)
            {
                float angleToFish = Vector3.Dot(directionToObject.normalized, transform.forward);
                if (angleToFish > pictureThreshold)
                {
                    FishController fish = objectToTakePictureOf.GetComponent<FishController>();
                    float fishValue = fish.fishValue;
                    if (angleToFish > .9975)
                    {
                        fishValue *= 1;
                    }
                    else if (angleToFish > .995) {
                        fishValue *= .9f;
                    }
                    else if (angleToFish > .9925)
                    {
                        fishValue *= .75f;
                    }
                    else
                    {
                        fishValue *= .5f;
                    }
                    //LevelManager.Instance.addFishValue(fishValue);
                    Debug.Log("Fish hit! Value: " + fishValue);
                }
            }
        }
    }
}
