using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePicture : MonoBehaviour
{

    public GameObject levelManager;
    private bool cameraEquipped;

    private LevelManager levelManagerScript;

    void Start()
    {
        levelManagerScript = levelManager.GetComponent<LevelManager>();
        cameraEquipped = false;
    }

    void Update()
    {
        
        if(Input.GetKey(KeyCode.Mouse1))
        {
         //   levelManagerScript.UseCameraUI();
            cameraEquipped = true;
        } 
        else
        {
         //   levelManagerScript.UsePlayerUI();
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
                Debug.Log("Fish hit");
                Debug.Log("Pics left: " + LevelManager.storageLeft);
            }

        }

    }
}
