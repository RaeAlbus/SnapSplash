using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    private Camera playerCamera;
    private CharacterController characterController;

    public float fallSpeed = 1.5f;
    public float rotationSpeed = 10f;

    void Start()
    {
        // Assuming the camera is a child of the player
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {

        if (!LevelManager.isLevelLost)
        {
            // Player movement based on camera direction
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movementDirection = playerCamera.transform.forward * vertical + playerCamera.transform.right * horizontal;

            // Optionally, you can add jumping logic or other actions here
            characterController.Move(movementDirection.normalized * movementSpeed * Time.deltaTime);
        }
        else
        {
            // have the player start falling
            characterController.Move(Vector3.down * fallSpeed * Time.deltaTime);

            // rotate the player like they are passing out
            transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }
    }
}
