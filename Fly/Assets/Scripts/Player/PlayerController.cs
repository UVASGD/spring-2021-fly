using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb; // Rigidbody component (physics)

    // The model is a child of the "true" plane.
    // Parent handles actual left-right rotation, while child/model visualizes angle of attack and banking
    // Separation of these important to handle gimbal lock, and overall makes it easier to program.
    public Transform model; 
    // need initialized plane Type

    [Header("Init")]
    public float initialSpeed = 10f;

    [Header("Roll")]
    public float rollSpeed = 1f; // maximum speed of turning
    public float maxRollDeg = 45f; // maximum turn angle
    
    [Header("Pitch")]
    public float pitchSpeed = 1f; // maximum speed of dipping and climbing
    public float maxPitchDeg = 45f; // maximum turn angle for dipping and climbing

    [Header("Stall")]
    public float stallStartSpeed = 1f; // threshold from going from flying to stalling
    public float stallStopSpeed = 3f; // once in stalling, threshold to get back to flying
    public float stallPitchModifier = 2f; // how much faster to lose pitch when stalling
    public float stallGravityModifier = 2f; // how much faster to lose altitude when stalling
    public bool stalling; // whether or not the plane is stalling
    public UnityEvent<bool> OnStall; // event to invoke arbitrary functions when starting or stopping stalling

    [Header("Status")]
    public bool flying = true; // true = in the air, false = grounded
    [HideInInspector]
    public UnityEvent OnDeath; // event to invoke arbitrary functions upon death/crash

    [Header("Debug")]
    public Vector3 velocity;
    public Vector3 localForward; // always points towards plane's nose
    public Vector3 localRight; // always points towards plane's right wing
    public Vector3 localUp; // always perpendicular to the above two

    [Space]

    public float speed;
    public float thrustSpeed;
    public float direction; //0-360 degrees
    public float angleOfAttack; // dip/climb
    public float roll; // banking
    public float pitch; // turning
    public float velocityDecrease;
    public float fuel;

    private void Start()
    {
        // Initialize variables and references
        rb = GetComponent<Rigidbody>();
        speed = initialSpeed;
        angleOfAttack = 0f;
        direction = 0f;
        stalling = false;
        flying = true;
        velocityDecrease = 1f;
        fuel = 100f;
    }

    private void FixedUpdate()
    {
        if (model == null) return;

        // Disable all input if plane is grounded
        if (!flying)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            return;
        }

        // Logic for when to change between stalling and flying states
        if (stalling)
        {
            if (speed > stallStopSpeed)
            {
                stalling = false;
                OnStall?.Invoke(false);
            }
        }
        else
        {
            if (speed < stallStartSpeed)
            {
                stalling = true;
                OnStall?.Invoke(true);
            }
        }
        
        // Get player input if flying, otherwise set to 0
        roll = stalling || !flying ? 0f : Input.GetAxis("Horizontal"); // z = roll
        pitch = stalling || !flying ? 0f : Input.GetAxis("Vertical"); // x = pitch 

        if (Input.GetKeyDown(KeyCode.Space)) {
            thrustSpeed = speed;
            speed = 106f;
            Debug.Log(speed);
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            speed = thrustSpeed;
        }

        direction += roll * rollSpeed * Time.fixedDeltaTime; // how much to turn this frame
        localForward = Quaternion.AngleAxis(direction, Vector3.up) * Vector3.forward; // update nose
        localRight = Vector3.Cross(localForward, Vector3.up); // update wings

        angleOfAttack -= 5f * Time.fixedDeltaTime * (stalling ? 20f : 1f); // automatically dip plane's nose over time. Increased if stalling.
        angleOfAttack -= pitch * pitchSpeed * Time.fixedDeltaTime; // adjust angle of attack based on user input
        angleOfAttack = Mathf.Clamp(angleOfAttack, -80f, 80f); // clamp plane so they cant go straight up or down (causes camera glitches otherwise)

        // Increase speed if pointing down, decrease speed if pointing up
        if (angleOfAttack < 0f)
        {
            speed -= angleOfAttack / 45f * Time.fixedDeltaTime * (stalling ? 10f : 1f);
        }
        else
        {
            speed -= angleOfAttack / 20f * Time.fixedDeltaTime;
        }
        speed = Mathf.Max(0.1f, speed); // Cap minimum speed in flight to 0.1f (very small forward speed), otherwise causes undefined behavior

        velocity = Quaternion.AngleAxis(angleOfAttack, localRight) * localForward * speed; // rotate target velocity (direction) by angle of attack
        rb.velocity = velocity; // apply velocity

        if (fuel <= 0)
        {
            //TODO change so varies with Type
            transform.position += new Vector3(0, -.10f, 0);
            //Debug.Log(Type.Classic.getWeight());
            if (velocityDecrease < 8)
            {
                velocity += velocityDecrease * Vector3.down;
                velocityDecrease += .01f;
            }
        }
        if (fuel > 0)
        {
            fuel -= .05f;
        }

        //Debug.Log("fuel: " + fuel + ", velocityDecrease: " + velocityDecrease + ", speed: " + speed);

        // Rotate model for banking
        localUp = Quaternion.AngleAxis(-roll * maxRollDeg, velocity) * Vector3.up;
        model.rotation = Quaternion.LookRotation(velocity, localUp);

        Debug.DrawRay(transform.position, velocity, Color.yellow); // Draw a line indicating velocity in the Scene view for debugging purposes
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Might need more advanced logic later, but for now, if you hit anything solid, you die.
        flying = false;
        OnDeath?.Invoke();
    }
    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("Collided With: " + collision.gameObject.tag);
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
        if (collision.CompareTag("Finish"))
        {
            GameManager.instance.UnlockNextLevel();
            SceneManager.instance.LoadScene(0);
        }
        else if (collision.CompareTag("PowerUp"))
        {
            PowerUpFields fields = collision.gameObject.GetComponent<PowerUpFields>();
            speed += fields.effect;
            Destroy(collision.gameObject);
        }
    }
}
