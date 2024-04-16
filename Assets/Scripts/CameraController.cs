using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform playerBody;

    public static float mouseSensitivity = 175f;

    float pitch = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = transform.parent.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerPrefs.GetInt("mouseSensitivity", 175);
    }

    // Update is called once per frame
    void Update()
    {
        if(!ShopKeeperBehavior.inShop)
        {
            float moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            //yaw
            playerBody.Rotate(Vector3.up * moveX);

            //pitch
            pitch -= moveY;

            pitch = Mathf.Clamp(pitch, -90f, 90f);
            transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
    }

    public static void UpdateMouseSensitivity(int newSensitivity)
    {
        PlayerPrefs.SetInt("mouseSensitivity", newSensitivity);
        mouseSensitivity = newSensitivity;
    }
}
