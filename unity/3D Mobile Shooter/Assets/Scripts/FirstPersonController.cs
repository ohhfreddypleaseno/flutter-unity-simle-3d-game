using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    // References
    public Transform cameraTransform;
    public CharacterController characterController;

    // Player settings
    public float cameraSensitivity;
    public float moveSpeed;
    public float moveInputDeadZone;

    // Touch detection
    int leftFingerId, rightFingerId;
    float halfScreenWidth;

    // Camera control
    Vector2 lookInput;
    float cameraPitch;

    // Player movement
    Vector2 moveTouchStartPosition;
    Vector2 moveInput;

    Vector2 movementDirection;
    Vector2 viewDirection;

    // Start is called before the first frame update
    void Start()
    {
        // id = -1 means the finger is not being tracked
        leftFingerId = -1;
        rightFingerId = -1;

        // only calculate once
        halfScreenWidth = Screen.width / 2;

        // calculate the movement input dead zone
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    // Update is called once per frame
    void Update() {
        Move();
        LookAround();
    }

    void GetTouchInput() {
        // Iterate through all the detected touches
        for (int i = 0; i < Input.touchCount; i++)
        {

            Touch t = Input.GetTouch(i);

            // Check each touch's phase
            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        // Start tracking the left finger if it was not previously being tracked
                        leftFingerId = t.fingerId;

                        // Set the start position for the movement control finger
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        // Start tracking the rightfinger if it was not previously being tracked
                        rightFingerId = t.fingerId;
                    }

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:

                    if (t.fingerId == leftFingerId)
                    {
                        // Stop tracking the left finger
                        leftFingerId = -1;
                        Debug.Log("Stopped tracking left finger");
                    }
                    else if (t.fingerId == rightFingerId)
                    {
                        // Stop tracking the right finger
                        rightFingerId = -1;
                        Debug.Log("Stopped tracking right finger");
                    }

                    break;
                case TouchPhase.Moved:

                    // Get input for looking around
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId) {

                        // calculating the position delta from the start position
                        moveInput = t.position - moveTouchStartPosition;
                    }

                    break;
                case TouchPhase.Stationary:
                    // Set the look input to zero if the finger is still
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    // Set view direction
    // Processed by LookAround()
    void ChangeDirectionOfView(string direction) {
        viewDirection = parseStringToVector(direction).deltaPosition * cameraSensitivity * Time.deltaTime;
    }

    void LookAround() {
        if (viewDirection != Vector2.zero) {
        // vertical (pitch) rotation
        cameraPitch = Mathf.Clamp(cameraPitch - viewDirection.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transform.Rotate(transform.up, viewDirection.x);
        }
    }

    // Set movement direction
    // Processed by Move()
    void ChangeDirectionOfMove(string direction) {
        movementDirection = parseStringToVector(direction).normalized * moveSpeed * Time.deltaTime;
    }


    // Check if [movementDirection] not empty
    // Move in provided direction
    void Move() {
        if (movementDirection != Vector2.zero) {
            characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);      
        }
    }

    // Parser "0 1" to Vector2(x: 0, y: 1)
    Vector2 parseStringToVector(string direction) {
        string[] xyVals = direction.Split(' ');
        float aXPosition = float.Parse(xyVals[0]);
        float aYPosition = float.Parse(xyVals[1]);  
        return new Vector2(aXPosition, aYPosition);
    }
}