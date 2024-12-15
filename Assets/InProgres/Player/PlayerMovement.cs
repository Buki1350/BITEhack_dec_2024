using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isSneaking;

    [SerializeField] float mouseSensitivity = 0.8f;
    [SerializeField] Transform playerCameraHolder; 
    [SerializeField] GameObject playerModel;
    [SerializeField] Animator animator;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 5.0f;
    //private float jumpHeight = 3.0f;
    private float gravityValue = -30.0f;
    
    private bool isMovingRight;
    private bool isMovingLeft;
    private bool isMovingForward;
    private bool isMovingBackwards;
    private bool isJumping;

    private float sneakingSpeedMod=0.75f;
    

    private Vector3 previousMousePosition = Vector3.zero;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>() == null ? gameObject.AddComponent<CharacterController>() : gameObject.GetComponent<CharacterController>();
        controller.minMoveDistance = 0.0f;
        
        previousMousePosition = Input.mousePosition;
        
        Cursor.lockState = CursorLockMode.Locked;


    }
   

    void Update()
    {

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0f;
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        //HERE VARIABLES FOR ANIMATION
        isMovingRight = Input.GetKey(KeyCode.D);
        isMovingLeft = Input.GetKey(KeyCode.A);
        isMovingForward = Input.GetKey(KeyCode.W);
        isMovingBackwards = Input.GetKey(KeyCode.S);
        //=====================================
        
        Vector3 move = new Vector3(horizontalInput, 0, verticalInput);

        
        move = playerCameraHolder.transform.right * move.x + playerCameraHolder.transform.forward * move.z;
        move.y = 0;
        if (Input.GetButton("Sneak"))
        {
            UnityEngine.Debug.Log(isSneaking);
            controller.Move(move * (Time.deltaTime * playerSpeed)*sneakingSpeedMod);
            isSneaking = true;
        }
        else
        {
            controller.Move(move * (Time.deltaTime * playerSpeed));
            isSneaking = false;
        }


        
        /*
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            isJumping = true;
        }
        else
            isJumping = false;
        */

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Get the current camera rotation
        Vector3 cameraRotation = playerCameraHolder.eulerAngles;

        // Adjust the vertical rotation (pitch) using the mouse Y input
        float verticalRotation = cameraRotation.x - Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Clamp the vertical rotation between -5 and 25 degrees
        if (verticalRotation > 180) verticalRotation -= 360; // Handle angles greater than 180
        verticalRotation = Mathf.Clamp(verticalRotation, -5, 25);

        // Update the camera's rotation
        playerCameraHolder.eulerAngles = new Vector3(verticalRotation, cameraRotation.y + Input.GetAxis("Mouse X") * mouseSensitivity, 0);

        previousMousePosition = Input.mousePosition;
        
        playerModel.transform.eulerAngles = new Vector3(playerModel.transform.eulerAngles.x, playerCameraHolder.eulerAngles.y, playerModel.transform.transform.eulerAngles.z);

        //HERE SPACE FOR ANIMATION VARIABLES AND STUFF IDK

        UnityEngine.Debug.Log(Input.GetAxis("Vertical"));
        if (Input.GetAxis("Vertical") == 0) { animator.SetFloat("X_movement", 0); }
        else if (Input.GetAxis("Vertical") > 0) { animator.SetFloat("X_movement", 1); }
        else { animator.SetFloat("X_movement", -1);}

        if (Input.GetAxis("Horizontal") == 0) { animator.SetFloat("Y_movement", 0); }
        else if (Input.GetAxis("Horizontal") > 0) { animator.SetFloat("Y_movement", -1); }
        else { animator.SetFloat("Y_movement", 1); }
        //==============================================
    }
}
