using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 0.8f;
    [SerializeField] Transform playerCameraHolder; 
    [SerializeField] GameObject playerModel;
    [SerializeField] Animator animator;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 5.0f;
    private float jumpHeight = 3.0f;
    private float gravityValue = -30.0f;
    
    private bool isMovingRight;
    private bool isMovingLeft;
    private bool isMovingForward;
    private bool isMovingBackwards;
    private bool isJumping;

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
        controller.Move(move * (Time.deltaTime * playerSpeed));
        
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            isJumping = true;
        }
        else
            isJumping = false;
        
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        
        playerCameraHolder.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * mouseSensitivity;
        previousMousePosition = Input.mousePosition;
        
        playerModel.transform.eulerAngles = new Vector3(playerModel.transform.eulerAngles.x, playerCameraHolder.eulerAngles.y, playerModel.transform.transform.eulerAngles.z);
        
        //HERE SPACE FOR ANIMATION VARIABLES AND STUFF IDK
        
        
        //==============================================
    }
}
