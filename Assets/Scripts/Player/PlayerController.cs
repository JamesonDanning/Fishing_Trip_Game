using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   public struct Inputs
    {
        public bool interact;
        public Vector3 moveDirection;
        public Vector2 rawDirection;
        public bool usingMouse;
        public Vector2 mousePos;
    }
    public Inputs inputs;

    Rigidbody rb;
    public Transform cam;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float accelerationAmount = 5;





    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
    }




    void FixedUpdate() 
    {
       DoMovement();
    }



    void OnMove(InputValue value)
    {
        inputs.rawDirection = value.Get<Vector2>();
        inputs.moveDirection = new Vector3(inputs.rawDirection.x, 0, inputs.rawDirection.y).normalized;        
    }

    void DoMovement()
    {
        //rotates player in direction moving
        if (inputs.moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputs.moveDirection.x, inputs.moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.velocity = Vector3.Lerp(rb.velocity, moveDir.normalized * speed * Time.deltaTime, accelerationAmount); 
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, accelerationAmount);
        }
       
    }


    

    
}
