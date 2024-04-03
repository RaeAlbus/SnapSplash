using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePictureTEST : MonoBehaviour
{
    public float raycastWidth = 0.6f;
    public float raycastDistance = 30;

    public float pictureThreshold = 30;

    private bool cameraEquipped;

    public Image crosshair;

    void Start()
    {
        cameraEquipped = false;
    }

    void Update()
    {
        if (LevelManager.isDiving)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                LevelManager.Instance.UseCameraUI();
                cameraEquipped = true;
            }
            else
            {
                LevelManager.Instance.UsePlayerUI();
                cameraEquipped = false;
            }

            if (cameraEquipped)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
                {
                    if (hit.collider.CompareTag("Fish"))
                    {
                        crosshair.transform.localScale = Vector3.Lerp(crosshair.transform.localScale, new Vector3(0.7f, 0.7f, 1f), Time.deltaTime * 4);
                    }
                    else
                    {
                        crosshair.transform.localScale = Vector3.Lerp(crosshair.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 4);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (LevelManager.storageLeft != 0)
                    {
                        CapturePic();
                    }
                    else
                    {
                        SoundManager.Instance.PlayNoStorageSFX();
                    }
                }
            }
        }
    }

    void CapturePic()
    {
        LevelManager.storageLeft -= 1;
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
        SoundManager.Instance.PlayCameraSFX();

        foreach (GameObject objectToTakePictureOf in LevelManager.fishInScene)
        {
            Vector3 directionToObject = objectToTakePictureOf.transform.position - transform.position;
            if (Vector3.Magnitude(directionToObject) < 30)
            {
                float angleToFish = Vector3.Dot(directionToObject.normalized, transform.forward);
                if (angleToFish > pictureThreshold)
                {
                    float fishValue;
                    if (objectToTakePictureOf.CompareTag("Shark"))
                    {
                        SharkAI shark = objectToTakePictureOf.GetComponent<SharkAI>();
                        fishValue = shark.sharkValue;
                        shark.StunShark();
                    }
                    else 
                    {
                        fishValue = objectToTakePictureOf.GetComponent<FishController>().fishValue;
                    }
                    
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
                    LevelManager.Instance.addFishValue(fishValue);
                    Debug.Log("Fish hit! Value: " + fishValue);
                }
            }
        }
    }
}
