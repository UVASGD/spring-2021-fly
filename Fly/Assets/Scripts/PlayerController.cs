using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Transform model;

    [Header("Init")]
    public float initialSpeed = 10f;

    [Header("Roll")]
    public float rollSpeed = 1f;
    public float maxRollDeg = 45f;
    
    [Header("Pitch")]
    public float pitchSpeed = 1f;
    public float maxPitchDeg = 45f;

    [Header("Stall")]
    public float stallStartSpeed = 1f;
    public float stallStopSpeed = 3f;
    public float stallStopAngle = -45f;
    public float stallPitchModifier = 2f;
    public float stallGravityModifier = 2f;
    public bool stalling;
    public UnityEvent<bool> StallEvent;

    [Header("Status")]
    public bool flying = true;
    [HideInInspector]
    public UnityEvent DieEvent;

    [Header("Debug")]
    public Vector3 velocity;
    public Vector3 localForward;
    public Vector3 localRight;
    public Vector3 localUp;

    [Space]

    public float speed;
    public float direction; //0-360 degrees
    public float angleOfAttack;
    public float roll;
    public float pitch;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        model = transform.GetChild(0);
        speed = initialSpeed;
        angleOfAttack = 0f;
        direction = 0f;
        stalling = false;
        flying = true;
    }

    private void FixedUpdate()
    {
        if (!flying)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            return;
        }

        if (stalling)
        {
            if (speed > stallStopSpeed)
            {
                stalling = false;
                StallEvent?.Invoke(false);
            }
        }
        else
        {
            if (speed < stallStartSpeed)
            {
                stalling = true;
                StallEvent?.Invoke(true);
            }
        }
        
        roll = stalling || !flying ? 0f : Input.GetAxis("Horizontal"); // z = roll
        pitch = stalling || !flying ? 0f : Input.GetAxis("Vertical"); // x = pitch 

        direction += roll * rollSpeed * Time.fixedDeltaTime;
        localForward = Quaternion.AngleAxis(direction, Vector3.up) * Vector3.forward;
        localRight = Vector3.Cross(localForward, Vector3.up);

        angleOfAttack -= 5f * Time.fixedDeltaTime * (stalling ? 20f : 1f);
        angleOfAttack -= pitch * pitchSpeed * Time.fixedDeltaTime;
        angleOfAttack = Mathf.Clamp(angleOfAttack, -80f, 80f);

        if (angleOfAttack < 0f)
        {
            speed -= angleOfAttack / 45f * Time.fixedDeltaTime * (stalling ? 10f : 1f);
        }
        else
        {
            speed -= angleOfAttack / 20f * Time.fixedDeltaTime;
        }
        speed = Mathf.Max(0.1f, speed);

        velocity = Quaternion.AngleAxis(angleOfAttack, localRight) * localForward * speed;
        rb.velocity = velocity;

        localUp = Quaternion.AngleAxis(-roll * maxRollDeg, velocity) * Vector3.up;
        model.rotation = Quaternion.LookRotation(velocity, localUp);

        Debug.DrawRay(transform.position, velocity, Color.yellow);
    }

    private void OnCollisionEnter(Collision collision)
    {
        flying = false;
        DieEvent?.Invoke();
     
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collided With: " + collision.gameObject.tag);
        //make these generic later :)
        //if (collision.gameObject.name == "PowerUp(Clone)")
        //{
        //    Destroy(collision.gameObject);
        //    speed += 10;
        //}
        //if (collision.gameObject.name == "PowerDown(Clone)")
        //{
        //    Destroy(collision.gameObject);
        //    speed -= 5;
        //}

        if (collision.gameObject.tag == "PowerUp")
        {
            PowerUpFields fields = collision.gameObject.GetComponent<PowerUpFields>();
            speed += fields.effect;
            Destroy(collision.gameObject);
        }
    }
}
