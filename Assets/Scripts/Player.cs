using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float upDownRange = 90;
    public float jumpSpeed = 5;
    public float downSpeed = 5;

    private Vector3 speed;
    private float forwardSpeed;
    private float sideSpeed;

    private float rotLeftRight;
    private float rotUpDown;
    private float verticalRotation = 0f;

    private float verticalVelocity = 0f;

    private CharacterController cc;

    [SerializeField] private Transform strikingRangeTransform;

    void Start()
    {
       // Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        FPMove();
        FPRotate();

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f)), out RaycastHit hit, 10.0f))
        {
            strikingRangeTransform.gameObject.SetActive(true);
            strikingRangeTransform.position = hit.point;
        }
        else
        {
            strikingRangeTransform.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f)), out hit, 10.0f))
            {
                World.DeformationChunk(hit.point, 0.3f);
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f)), out hit, 10.0f))
            {
                World.DeformationChunk(hit.point, -0.3f);
            }
        }
    }


    //Player의 x축, z축 움직임을 담당 
    void FPMove()
    {

        forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
        sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        //if (cc.isGrounded && Input.GetButtonDown("Jump"))
        //    verticalVelocity = jumpSpeed;
        //if (!cc.isGrounded)
        //    verticalVelocity = downSpeed;


        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);
        speed = transform.rotation * speed;

        cc.Move(speed * Time.deltaTime);
    }

    //Player의 회전을 담당
    void FPRotate()
    {
        //좌우 회전
        rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0f, rotLeftRight, 0f);

        //상하 회전
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
