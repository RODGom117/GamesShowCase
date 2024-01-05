using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public HeroStats playerStats; // Reference to the scriptable object
    public Transform playerCamera; // Reference to the player's camera
    
    //Camera and Mouse
    public float mouseSensitivity = 2.0f; // Mouse sensitivity for camera movement
    [SerializeField]
    private float maxVerticalRotation = 90f;  // Maximum camera rotation angle (in degrees)
    [SerializeField]
    private float minVerticalRotation = -90f; // Minimum camera rotation angle (in degrees)

    
    private CharacterController characterController;
    private Vector3 moveDirection;

    private Vector3 _cameraRelativeMovement;
    private float verticalVelocity;
    private float rotX = 0; // Current camera rotation around the X-axis
    private float currentSpeed;
    private bool isSprinting;
    
    private Animator animator;
    
    
    //Handling Rotation
    public float rotationFactorPerFrame = 1.0f;
    private Vector2 moveInput;
    private Vector3 localMovementDirection;

    // Photon
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Camera camera;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        camera.gameObject.SetActive(photonView.IsMine);
        if (playerStats != null)
        {
            Debug.Log("PlayerStats successfully loaded");
        }
        else
        {
            Debug.LogError(
                "Failed to load the scriptable object. Check the path and ensure it's in the Resources folder.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HandleMovement()
    {
        //HandleRotation();
        //Movement();

        HandleMovementInput();
        HandleMouseLook();
        

    }
    
    private void HandleRotation()
    {

            Vector3 positionToLookAt;

            positionToLookAt.x = _cameraRelativeMovement.x;
            //We will never need to rotate around the y-axis.
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = _cameraRelativeMovement.z;

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);

    }
    private void HandleMovementInput()
    {
 
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // Use the camera's forward and right vectors to calculate movement direction
            Vector3 inputDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
            inputDirection.Normalize();

            // Sprinting toggle
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isSprinting = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                isSprinting = false;
            }

            // Gradual sprint acceleration
            float targetSpeed = isSprinting
                ? playerStats.movementAttributes.movementSpeed * playerStats.movementAttributes.sprintSpeedMultiplier
                : playerStats.movementAttributes.movementSpeed;

            // Only update the currentSpeed when there's input
            if (inputDirection != Vector3.zero)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed,
                    playerStats.movementAttributes.sprintGradualAcceleration * Time.deltaTime);
            }
            else
            {
                // If there's no input, set currentSpeed to 0 to stop the character from moving
                currentSpeed = 0f;
            }

            // Apply movement speed
            moveDirection = inputDirection * currentSpeed;

            // Apply gravity
            ApplyGravity();

            // Jumping
            if (characterController.isGrounded)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    verticalVelocity = playerStats.movementAttributes.jumpHeight;
                }
            }

            // Apply vertical velocity
            moveDirection.y = verticalVelocity;

            //_cameraRelativeMovement = ConvertToCameraSpace(moveDirection);
            // Move the character
            characterController.Move(moveDirection * Time.deltaTime);

            // Update the 'isWalking' and 'isRunning' parameters in the Animator
            // Set the Animator parameters
            animator.SetFloat("HorizontalSpeed", horizontalInput);
            animator.SetFloat("VerticalSpeed", verticalInput);
    }

    /*private void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput);
        inputDirection = transform.TransformDirection(inputDirection);
        inputDirection.Normalize();

        // Sprinting toggle
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isSprinting = false;
        }

        // Gradual sprint acceleration
        float targetSpeed = isSprinting
            ? playerStats.movementAttributes.movementSpeed * playerStats.movementAttributes.sprintSpeedMultiplier
            : playerStats.movementAttributes.movementSpeed;


        // Only update the currentSpeed when there's input
        if (inputDirection != Vector3.zero)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed,
                playerStats.movementAttributes.sprintGradualAcceleration * Time.deltaTime);
        }
        else
        {
            // If there's no input, set currentSpeed to 0 to stop the character from moving
            currentSpeed = 0f;
        }

        // Apply movement speed
        moveDirection = inputDirection * currentSpeed;

        // Update the 'Speed' parameter in the Animator
        animator.SetFloat("ForwardSpeed", currentSpeed);

        // Apply gravity
        ApplyGravity();

        // Jumping
        if (characterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = playerStats.movementAttributes.jumpHeight;
            }
        }

        // Apply vertical velocity
        moveDirection.y = verticalVelocity;
        
        _cameraRelativeMovement = ConvertToCameraSpace(moveDirection);
        // Move the character
        characterController.Move(_cameraRelativeMovement * Time.deltaTime);
    }*/


    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= playerStats.movementAttributes.gravitySpeed * Time.deltaTime;
        }
        else
        {
            verticalVelocity = -playerStats.movementAttributes.gravitySpeed * Time.deltaTime;
        }
    }


    private Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        //Get forward and right vectors of camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x  * cameraRight;

        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        
        return vectorRotatedToCameraSpace;
    }
    private void HandleMouseLook()
    {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Rotate the character (Y-axis)
            transform.Rotate(Vector3.up * mouseX);

            // Rotate the camera (X-axis)
            rotX -= mouseY;
            rotX = Mathf.Clamp(rotX, minVerticalRotation, maxVerticalRotation); // Limit camera rotation to prevent flipping

            // Apply camera rotation
            playerCamera.localRotation = Quaternion.Euler(rotX, 0, 0);

    }
}
