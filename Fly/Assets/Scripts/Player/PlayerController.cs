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
    public float drag;
    public int bounceCount;

    #region UPGRADEABLE VARIABLES
    private float chonkMultiplier;
    private float dynamicsMultiplier;
    private float rocketScienceMultiplier;
    private float gritMultiplier;
    private float spunkMultiplier;
    private float designMultiplier;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Initialize variables and references
    public void Init()
    {
        //change back to 1
        speed = initialSpeed * designMultiplier;
        angleOfAttack = 0f;
        direction = 0f;
        stalling = false;
        flying = true;
        velocityDecrease = 1f * dynamicsMultiplier * designMultiplier;
        fuel = 100f;
        drag = 0.02f * dynamicsMultiplier / designMultiplier;
        bounceCount = Mathf.RoundToInt(spunkMultiplier);

    }

    private void FixedUpdate()
    {
        if (model == null || !GameManager.instance.runManager.runStarted) return;

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

        direction += roll * rollSpeed * Time.fixedDeltaTime; // how much to turn this frame
        localForward = Quaternion.AngleAxis(direction, Vector3.up) * Vector3.forward; // update nose
        localRight = Vector3.Cross(localForward, Vector3.up); // update wings

        angleOfAttack -= 5f * chonkMultiplier * Time.fixedDeltaTime * (stalling ? 20f : 1f); // automatically dip plane's nose over time. Increased if stalling.
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

        speed -= drag;

        velocity = Quaternion.AngleAxis(angleOfAttack, localRight) * localForward * speed; // rotate target velocity (direction) by angle of attack

        if (Input.GetKey(KeyCode.Space))
        {
            if (fuel > 0f)
            {
                fuel -= 25f / rocketScienceMultiplier * Time.deltaTime;
                velocity += velocity.normalized * thrustSpeed * Time.fixedDeltaTime;
            }
            else
            {
                fuel = 0f;
            }
        }

        velocity -= velocity.normalized * velocityDecrease * Time.fixedDeltaTime;

        rb.velocity = velocity; // apply velocity

        // Rotate model for banking
        localUp = Quaternion.AngleAxis(-roll * maxRollDeg, velocity) * Vector3.up;
        model.rotation = Quaternion.LookRotation(velocity, localUp);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print($"Collision with {collision.gameObject.name} ({collision.gameObject.tag})");
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (bounceCount == 0)
            {
                flying = false;
                Player.instance.cameraController.SetDeadCam();
                OnDeath?.Invoke();
            }
            else
            {
                angleOfAttack *= -1;
                velocity.y *= -1;
                if (velocity.magnitude < 10f)
                {
                    velocity = velocity.normalized * 10f;
                }
                bounceCount--;
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            flying = false;
            Player.instance.cameraController.SetDeadCam();
            OnDeath?.Invoke();
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Finish"))
        {
            GameManager.instance.UnlockNextLevel();
            GameManager.instance.runManager.CompleteRun();
        }
        else if (collision.CompareTag("PowerUp"))
        {
            PowerUpFields fields = collision.gameObject.GetComponent<PowerUpFields>();

            if (fields && !fields.persistent)
            {
                speed += fields.effect;
                Destroy(collision.gameObject);
                if (fields.persistent)
                {
                }
                else
                {
                    speed += fields.effect;
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            PowerUpFields fields = other.gameObject.GetComponent<PowerUpFields>();
            if (fields && fields.persistent)
            {
                speed -= fields.effect / gritMultiplier * Time.deltaTime;

            }
        }
    }

    public void SyncUpgrades()
    {
        chonkMultiplier = UpgradeManager.instance.GetUpgradeValue(TieredUpgrade.Type.Chonk);
        rocketScienceMultiplier = UpgradeManager.instance.GetUpgradeValue(TieredUpgrade.Type.RocketScience);
        gritMultiplier = UpgradeManager.instance.GetUpgradeValue(TieredUpgrade.Type.Grit);
        spunkMultiplier = UpgradeManager.instance.GetUpgradeValue(TieredUpgrade.Type.Spunk);
        dynamicsMultiplier = UpgradeManager.instance.GetUpgradeValue(TieredUpgrade.Type.Dynamics);
        designMultiplier = UpgradeManager.instance.GetUpgradeValue(TieredUpgrade.Type.Design);
    } 
}
