﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody rigid;
    

    [SerializeField]
    private float rollSpeed = 50;

    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private LayerMask ground;

    public bool flying;
    // Start is called before the first frame update
    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
        flying = false;
    }

    void Update(){
        if (Input.GetButtonDown("Fly") && !Grounded()) {
            flying = true;
            rigid.useGravity = false;
            transform.rotation = Quaternion.identity;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //Check for ground
        if(Grounded() &! flying) {
            Vector3 floorNormal = GetFloorNormal();
            
             // Slow down when no input recieved
            if (SimpleInput.GetAxis("Vertical") == 0.0f && SimpleInput.GetAxis("Horizontal") == 0.0f && rigid.velocity.magnitude > 0.0f){
                rigid.velocity = Vector3.Lerp(rigid.velocity, Vector3.zero, rollSpeed * 0.1f * Time.deltaTime);
            }

            else {
                Vector3 forward = Vector3.Cross(mainCamera.transform.right, floorNormal);
                Vector3 forwardApply = forward * SimpleInput.GetAxis("Vertical");
                Vector3 rightApply = SimpleInput.GetAxis("Horizontal") * mainCamera.transform.right;
                Debug.DrawLine(transform.position, transform.position + ((forwardApply + rightApply) * rollSpeed), Color.white, 0.5f);
                //Zoom
                rigid.AddForce((forwardApply + rightApply) * rollSpeed);
            }
        }
        else if (flying) {
            float roll = SimpleInput.GetAxis("Horizontal") / Time.timeScale;
            float tilt = SimpleInput.GetAxis("Vertical") / Time.timeScale;
            //Use root(2) to counter magnitudes
            float yaw = (transform.right + Vector3.up).magnitude - 1.414214f;

            if (tilt != 0){
                transform.Rotate(transform.right, tilt * Time.deltaTime * 10, Space.World);
            }
            if (roll != 0){
                transform.Rotate(transform.forward, roll * Time.deltaTime * -10, Space.World);
            }
            if (yaw != 0){
                transform.Rotate(Vector3.up, yaw * Time.deltaTime * 15, Space.World);
            }

            //Gravity
            rigid.velocity -= Vector3.up * Time.deltaTime;
        }
    }

    public bool Grounded()
    {
        return Physics.CheckSphere(transform.position - (Vector3.up * 0.5f), 1, ground);
    }

    public Vector3 GetFloorNormal()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, ground))
        {
            return(hit.normal);
        }
        else return(Vector3.zero);
    }
}
