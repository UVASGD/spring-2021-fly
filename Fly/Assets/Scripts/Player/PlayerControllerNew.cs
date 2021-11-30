using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerNew : MonoBehaviour
{
    private Rigidbody rb; // Rigidbody component (physics)

    // The model is a child of the "true" plane.
    // Parent handles actual left-right rotation, while child/model visualizes angle of attack and banking
    // Separation of these important to handle gimbal lock, and overall makes it easier to program.
    public Transform model; 

    [Header("Init")]
    public float initialSpeed = 10f;
    public float liftCoefficient = 1f;
    public float dragCoefficient = 1f;

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
    public Vector3 thrust;
    public Vector3 drag;
    public Vector3 lift;
    public Vector3 weight;

    [Space]

    public float speed;
    public float thrustSpeed;
    public float direction; //0-360 degrees
    public float angleOfAttack; // dip/climb
    public float roll; // banking
    public float pitch; // turning
    public float velocityDecrease;
    public float fuel;
    //public float drag;
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
        //drag = 0.02f * dynamicsMultiplier / designMultiplier;
        bounceCount = Mathf.RoundToInt(spunkMultiplier);

        rb.velocity = Vector3.forward * speed;
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

        // Get player input if flying, otherwise set to 0
        roll = stalling || !flying ? 0f : Input.GetAxis("Horizontal"); // z = roll
        pitch = stalling || !flying ? 0f : Input.GetAxis("Vertical"); // x = pitch

        direction += roll * rollSpeed * Time.fixedDeltaTime;
        angleOfAttack -= pitch * pitchSpeed * Time.fixedDeltaTime;
        angleOfAttack = Mathf.Clamp(angleOfAttack, -80f, 80f);

        transform.eulerAngles = new Vector3(angleOfAttack, direction, 0f);
        model.localEulerAngles = new Vector3(0f, 0f, -roll * 45f);

        float forwardSpeedSqrHalf = Mathf.Max(0f, Vector3.Project(rb.velocity, transform.forward).sqrMagnitude / 2f);
        float wingArea = 1 + Mathf.Abs(angleOfAttack) / 40f;
        weight = Physics.gravity * rb.mass;
        lift = transform.up * liftCoefficient * forwardSpeedSqrHalf * wingArea;
        drag = -transform.forward * dragCoefficient * forwardSpeedSqrHalf;
        thrust = Vector3.zero; // TODO
        rb.AddForce(weight + lift + drag + thrust);


        //rb.velocity = transform.forward * speed;
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
