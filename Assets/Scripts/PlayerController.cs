using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Underwater")]
    public float movementSpeedUnderwater = 3f;
    public float agilityUnderwater = 3;

    [Header("Abovewater")]
    public float moveSpeedAbovewater = 5.9f;
    public float jumpHeightAbovewater = 2.9f;
    public float gravityAbovewater = 16.5f;
    public float airControlAbovewater = 3.4f;

    [Header("Fainting")]
    public float fallSpeed = 1.5f;
    public float rotationSpeed = 10f;   


    Vector3 input, moveDirection, moveDirectionUnderwater;
    private Camera playerCamera;
    private CharacterController characterController;
    private Rigidbody rb;

    void Start()
    {
        // Assuming the camera is a child of the player
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        rb = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        if (!LevelManager.isDiving)
        {
            ControlPlayerAbovewater();
        } else
        {
            if (!LevelManager.isLevelLost)
            {
                ControlPlayerUnderwater();
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

    void ControlPlayerUnderwater()
    {
        // Player movement based on camera direction
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        input = (playerCamera.transform.right * horizontal + playerCamera.transform.forward * vertical).normalized;
        input *= movementSpeedUnderwater;

        //rb.AddForce(movementDirection * movementSpeedUnderwater);
        moveDirectionUnderwater = Vector3.Lerp(moveDirectionUnderwater, input, agilityUnderwater * Time.deltaTime);
        characterController.Move(moveDirectionUnderwater * Time.deltaTime);
    }

    void ControlPlayerAbovewater()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;
        input *= moveSpeedAbovewater;

        if (characterController.isGrounded)
        {
            moveDirection = input;
            //we can jump
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = Mathf.Sqrt(2 * jumpHeightAbovewater * gravityAbovewater);
            }
            else
            {
                moveDirection.y = 0.0f;
            }

        }
        else
        {
            //we are midair
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControlAbovewater * Time.deltaTime);
        }
        moveDirection.y -= gravityAbovewater * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }
}
