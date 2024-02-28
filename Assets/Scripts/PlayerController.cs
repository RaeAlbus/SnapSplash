using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Transform switchTransform;
    private Camera playerCamera;
    private CharacterController characterController;
    private Rigidbody rb;

    public float fallSpeed = 1.5f;
    public float rotationSpeed = 10f;

    private LevelManager levelManager;

    private const float DISTANCE_TO_INTERACT = 5f;


    void Start()
    {
        // Assuming the camera is a child of the player
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        levelManager = FindObjectOfType<LevelManager>();
        rb = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        // Switch To Surface if close to switch interactable
        if (Input.GetKeyDown(KeyCode.Space) && !LevelManager.isLevelLost)
        {
            float distanceToSwitch = Vector3.Distance(transform.position, switchTransform.position);
            if (distanceToSwitch < DISTANCE_TO_INTERACT)
            {
                levelManager.SwitchScene();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!LevelManager.isLevelLost)
        {
            ControlPlayer();
        }
        else
        {
            // have the player start falling
            characterController.Move(Vector3.down * fallSpeed * Time.deltaTime);

            // rotate the player like they are passing out
            transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }
    }

    void ControlPlayer()
    {
        // Player movement based on camera direction
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movementDirection = playerCamera.transform.forward * vertical + playerCamera.transform.right * horizontal;
        
        rb.AddForce(movementDirection * movementSpeed);
    }
}
