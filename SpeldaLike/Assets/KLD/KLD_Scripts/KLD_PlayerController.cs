using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class KLD_PlayerController : SerializedMonoBehaviour
{

    [SerializeField] PlayerInput playerInput;
    [SerializeField] Transform axisTransform;
    [SerializeField] Transform playerGroundPoint;
    Rigidbody rb;
    CapsuleCollider col;

    public enum ControllerMode
    {
        GRAVITY,
        NO_GRAVITY
    }

    [SerializeField]
    ControllerMode controllerMode;

    //AXIS
    [SerializeField, Header("Axis")] float axisDeadZoneMagnitude = 0.1f;
    Vector2 rawAxis; //raw unity axis
    Vector2 deadZonedRawAxis; //raw axis that are only 0 or 1
    Vector2 timedAxis = Vector2.zero; //axis that are timed with acceleration and deceleration times
    [SerializeField] float accelerationTime = 0.3f;
    [SerializeField] float decelerationTime = 0.2f;
    Vector2 ng_timedAxis = Vector2.zero; //axis that are timed with acceleration and deceleration times
    [SerializeField] float ng_accelerationTime = 0.3f;
    [SerializeField] float ng_decelerationTime = 0.2f;
    [Tooltip("When the axis is less than this, zero it")]
    [SerializeField] float axisZeroingDeadzone = 0.05f;
    [SerializeField] bool snapAxis = true;

    Vector2 axisVector; //normalized direction where the player moves

    [SerializeField, Header("Movement")]
    float speed = 10f;

    [SerializeField, Header("Ground Detection"), Range(0.5f, 1f)]
    float sphereRadiusMultiplier = 0.9f;
    [SerializeField] float maxSlopeAngle = 30f;


    [SerializeField, Header("Jump")]
    float jumpSpeed = 10f;
    [SerializeField, Tooltip("More means a faster fall")] float fallMultiplier = 2f; //more means a faster fall
    [SerializeField, Tooltip("More means a lower minimal jump")] float lowJumpMultiplier = 3f; //more means a lower minimal jump
    bool jumpBuffer = false;
    [SerializeField] float jumpBufferDuration = 0.2f;
    [SerializeField] float normalJumpDrag = 0.05f;
    [SerializeField] float maxAirSpeed = 10f;
    [SerializeField] float addAirSpeed = 1f;
    [SerializeField] float jumpHorizontalAddedForce = 3f;
    [SerializeField] float steepSlopeLockedAngle = 90f;

    [SerializeField] LayerMask groundLayer;
    [SerializeField, ReadOnly] bool m_isGrounded = false;

    [SerializeField, Header("NO GRAVITY CONTROLLER"), Space(20)]
    float ng_impulseForce = 3f;
    [SerializeField] float ng_verticalImpulseForce = 3f;
    [SerializeField] float ng_maxHorizontalSpeed = 10f;
    [SerializeField] float ng_maxVerticalSpeed = 5f;
    [SerializeField] float ng_rotationThreshold = 0.1f;
    [SerializeField] float ng_lockedAngle = 135f;
    [SerializeField] bool ng_lockHorizontalSpeed = true;
    [SerializeField] bool ng_lockVerticalSpeed = true;
    [SerializeField] Transform[] ng_reactors = null;

    [SerializeField, Header("Animation"), Space(20)]
    float idleVelocityThreshold = 0.1f;
    public enum PlayerState { IDLE, RUNNING, NO_GRAVITY, JUMPING, FALLING };
    [SerializeField] PlayerState playerAnimationState = PlayerState.IDLE;
    [SerializeField] Animator animator = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //CheckPlayerJump();
        UpdatePlayerGroundPointPosition();

        UpdatePlayerAnimationState();

        //DoPlayerNoGravityReactorsSize();
    }

    private void FixedUpdate()
    {
        //axis
        DoDeadZoneRawAxis();
        DoTimedAxis();
        DoNoGravityTimedAxis();

        if (controllerMode == ControllerMode.GRAVITY)
        {
            DoPlayerMove();
            DoPlayerRotation();

            m_isGrounded = isGrounded();

            CheckPlayerJump(false);
            CheckFall();
        }
        else if (controllerMode == ControllerMode.NO_GRAVITY)
        {
            DoPlayerNoGravityMove();
            DoPlayerNoGravityRotation();
        }


    }

    private void OnEnable()
    {
        GameEvents.Instance.onGravityDisable += OnGravityDisable;
        GameEvents.Instance.onGravityEnable += OnGravityEnable;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onGravityDisable -= OnGravityDisable;
        GameEvents.Instance.onGravityEnable -= OnGravityEnable;
    }

    #region Inputs Callbacks

    public void OnMovement(InputAction.CallbackContext value)
    {
        rawAxis = value.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started && controllerMode == ControllerMode.GRAVITY)
        {
            CheckPlayerJump(true);
        }
    }

    #endregion

    #region Events

    void OnGravityEnable(int _id)
    {
        if (controllerMode != ControllerMode.GRAVITY)
        {
            controllerMode = ControllerMode.GRAVITY;
            rb.useGravity = true;
        }
    }

    void OnGravityDisable(int _id)
    {
        if (controllerMode != ControllerMode.NO_GRAVITY)
        {
            controllerMode = ControllerMode.NO_GRAVITY;
            rb.useGravity = false;
        }
    }

    #endregion

    void DoDeadZoneRawAxis()
    {
        //rawAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); OLD INPUT

        float hori = (Mathf.Abs(rawAxis.x) >= axisDeadZoneMagnitude ? 1f : 0f) * Mathf.Sign(rawAxis.x);
        float vert = (Mathf.Abs(rawAxis.y) >= axisDeadZoneMagnitude ? 1f : 0f) * Mathf.Sign(rawAxis.y);

        deadZonedRawAxis = new Vector2(hori, vert);
    }

    void DoTimedAxis()
    {

        float hori = timedAxis.x;
        float vert = timedAxis.y;

        if (deadZonedRawAxis.x != 0f)
        {
            hori += deadZonedRawAxis.x > 0f ? 1f / accelerationTime * Time.fixedDeltaTime : 1f / accelerationTime * -Time.fixedDeltaTime;
            if (snapAxis && hori != 0f && (Mathf.Sign(hori) != Mathf.Sign(deadZonedRawAxis.x)))
            {
                hori = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(hori) >= axisZeroingDeadzone)
            {
                hori += hori > 0f ? 1f / decelerationTime * -Time.fixedDeltaTime : 1f / decelerationTime * Time.fixedDeltaTime;
            }
            else
            {
                hori = 0f;
            }
        }

        if (deadZonedRawAxis.y != 0f)
        {
            vert += deadZonedRawAxis.y > 0f ? 1f / accelerationTime * Time.fixedDeltaTime : 1f / accelerationTime * -Time.fixedDeltaTime;
            if (snapAxis && vert != 0f && (Mathf.Sign(vert) != Mathf.Sign(deadZonedRawAxis.y)))
            {
                vert = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(vert) >= axisZeroingDeadzone)
            {
                vert += vert > 0f ? 1f / decelerationTime * -Time.fixedDeltaTime : 1f / decelerationTime * Time.fixedDeltaTime;
            }
            else
            {
                vert = 0f;
            }
        }

        hori = Mathf.Clamp(hori, -1f, 1f);
        vert = Mathf.Clamp(vert, -1f, 1f);

        timedAxis = new Vector2(hori, vert);
    }

    void DoNoGravityTimedAxis()
    {

        float hori = timedAxis.x;
        float vert = timedAxis.y;

        if (deadZonedRawAxis.x != 0f)
        {
            hori += deadZonedRawAxis.x > 0f ? 1f / ng_accelerationTime * Time.fixedDeltaTime : 1f / ng_accelerationTime * -Time.fixedDeltaTime;
            if (snapAxis && hori != 0f && (Mathf.Sign(hori) != Mathf.Sign(deadZonedRawAxis.x)))
            {
                hori = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(hori) >= axisZeroingDeadzone)
            {
                hori += hori > 0f ? 1f / ng_decelerationTime * -Time.fixedDeltaTime : 1f / ng_decelerationTime * Time.fixedDeltaTime;
            }
            else
            {
                hori = 0f;
            }
        }

        if (deadZonedRawAxis.y != 0f)
        {
            vert += deadZonedRawAxis.y > 0f ? 1f / ng_accelerationTime * Time.fixedDeltaTime : 1f / ng_accelerationTime * -Time.fixedDeltaTime;
            if (snapAxis && vert != 0f && (Mathf.Sign(vert) != Mathf.Sign(deadZonedRawAxis.y)))
            {
                vert = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(vert) >= axisZeroingDeadzone)
            {
                vert += vert > 0f ? 1f / ng_decelerationTime * -Time.fixedDeltaTime : 1f / ng_decelerationTime * Time.fixedDeltaTime;
            }
            else
            {
                vert = 0f;
            }
        }

        hori = Mathf.Clamp(hori, -1f, 1f);
        vert = Mathf.Clamp(vert, -1f, 1f);

        ng_timedAxis = new Vector2(hori, vert);
    }

    void DoPlayerMove()
    {
        //float xSpeed = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed * 30f;
        //float zSpeed = Input.GetAxis("Vertical") * Time.fixedDeltaTime * speed * 30f;
        float xSpeed = timedAxis.x * Time.fixedDeltaTime * speed * 30f;
        float zSpeed = timedAxis.y * Time.fixedDeltaTime * speed * 30f;

        Vector3 flatSpeedVector = axisTransform.right * xSpeed + axisTransform.forward * zSpeed;
        axisVector = new Vector2(flatSpeedVector.x, flatSpeedVector.z).normalized;

        if (isGrounded())
        {
            rb.velocity = new Vector3(flatSpeedVector.x, rb.velocity.y, flatSpeedVector.z);
        }
        else
        {
            float drag = 1f - normalJumpDrag;
            rb.velocity = new Vector3(rb.velocity.x * drag, rb.velocity.y, rb.velocity.z * drag);

            Vector3 horizontalMagnitude = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (Mathf.Abs(horizontalMagnitude.magnitude) < maxAirSpeed)
            {
                //rb.AddForce(new Vector3(deadZonedRawAxis.x, 0f, deadZonedRawAxis.y) * addAirSpeed);
                Vector3 inputDirectionVector = ((axisTransform.right * deadZonedRawAxis.x) + (axisTransform.forward * deadZonedRawAxis.y)).normalized;
                if (!isOnSteepSlope())//|| (isOnSteepSlope() &&
                //Vector3.Angle(inputDirectionVector, FlatAndNormalize(GetSlopeNormal())) > steepSlopeLockedAngle))
                //A VERIFIER
                {
                    rb.AddForce(inputDirectionVector * addAirSpeed);
                }
            }
            else if (Mathf.Abs(horizontalMagnitude.magnitude) > maxAirSpeed)
            {
                Vector3 maxHorizontalSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).normalized * maxAirSpeed;
                rb.velocity = new Vector3(maxHorizontalSpeed.x, rb.velocity.y, maxHorizontalSpeed.z);
            }

        }

    }

    void CheckPlayerJump(bool _calledByInput)
    {
        if (isGrounded() && (_calledByInput || jumpBuffer))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity += Vector3.up * jumpSpeed;
            StartCoroutine(WaitAndApplyHorizontalJumpForce());

            jumpBuffer = false;
        }
        else if (_calledByInput)
        {
            jumpBuffer = true;
            StartCoroutine(WaitAndDebufferJump());
        }
    }

    IEnumerator WaitAndApplyHorizontalJumpForce()
    {
        yield return new WaitForFixedUpdate();
        rb.velocity += new Vector3(timedAxis.x, 0f, timedAxis.y) * jumpHorizontalAddedForce;
    }

    IEnumerator WaitAndDebufferJump()
    {
        yield return new WaitForSeconds(jumpBufferDuration);
        jumpBuffer = false;
    }

    bool isGrounded()
    {
        float radius = col.radius * sphereRadiusMultiplier;
        bool detectGround = Physics.CheckSphere(transform.position + Vector3.up * radius * 0.9f, radius, groundLayer);

        RaycastHit hit;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, groundLayer);
        //bool isSlopeCorrect = hit.normal
        bool isSlopeCorrect = Vector3.Angle(Vector3.up, hit.normal) <= maxSlopeAngle;
        //print(Vector3.Angle(Vector3.up, hit.normal));
        return detectGround && isSlopeCorrect;
    }

    bool isOnSteepSlope()
    {
        float radius = col.radius * 1.2f;
        bool detectGround = Physics.CheckSphere(transform.position + Vector3.up * radius * 0.9f, radius, groundLayer);

        RaycastHit hit;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, groundLayer);
        bool isSlopeSteep = Vector3.Angle(Vector3.up, hit.normal) > maxSlopeAngle;

        return detectGround && isSlopeSteep;
    }

    Vector3 GetSlopeNormal()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f, groundLayer);
        return hit.normal;
    }

    void CheckFall()
    {
        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        //else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))  //check if we're jumping and gaining height
        else if (rb.velocity.y > 0 && playerInput.actions.FindAction("Jump").phase == InputActionPhase.Waiting)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void DoPlayerRotation()
    {
        if (timedAxis.magnitude > 0.1f)
        {
            float angleToLook = Vector3.SignedAngle(Vector3.forward, new Vector3(axisVector.x, 0f, axisVector.y), Vector3.up);
            //print(axisVector + "\n" + angleToLook);
            transform.rotation = Quaternion.Euler(0f, angleToLook, 0f);
        }
    }

    void UpdatePlayerGroundPointPosition()
    {
        /*
        if (isGrounded())
        {
            playerGroundPoint.position = transform.position;
        }
        else
        {*/
        RaycastHit hit;
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 200f, groundLayer);
        if (hit.point != Vector3.zero)
        {
            playerGroundPoint.position = hit.point;
        }
        else
        {
            playerGroundPoint.position = transform.position;
        }
        //}
    }

    void DoPlayerNoGravityMove()
    {
        Vector2 rbHorizontalMagnitude = new Vector2(rb.velocity.x, rb.velocity.z);
        Vector3 forceDirectionVector = axisTransform.right * deadZonedRawAxis.x + axisTransform.forward * deadZonedRawAxis.y;
        if (forceDirectionVector.magnitude > 1f)
        {
            forceDirectionVector.Normalize();
        }
        if (rbHorizontalMagnitude.magnitude < ng_maxHorizontalSpeed)
        {

            //rb.AddForce(new Vector3(deadZonedRawAxis.x, 0f, deadZonedRawAxis.y) * ng_impulseForce, ForceMode.Force);
            rb.AddForce(forceDirectionVector * ng_impulseForce, ForceMode.Force);
        }
        else
        {
            print(Vector3.Angle(rbHorizontalMagnitude, forceDirectionVector));
            if (forceDirectionVector != Vector3.zero && Vector3.Angle(rbHorizontalMagnitude, forceDirectionVector) > ng_lockedAngle)
            {
                rb.AddForce(forceDirectionVector * ng_impulseForce, ForceMode.Force);
                //print("addedforce");
            }
            rbHorizontalMagnitude = rbHorizontalMagnitude.normalized * ng_maxHorizontalSpeed;
            if (ng_lockHorizontalSpeed)
            {
                rb.velocity = new Vector3(rbHorizontalMagnitude.x, rb.velocity.y, rbHorizontalMagnitude.y); //capping vel
            }
        }



        if (rb.velocity.y < ng_maxVerticalSpeed && playerInput.actions.FindAction("Jump").phase == InputActionPhase.Performed)
        {
            rb.AddForce(Vector3.up * ng_verticalImpulseForce, ForceMode.Force);
        }
        else if (rb.velocity.y > -ng_maxVerticalSpeed && playerInput.actions.FindAction("Crouch").phase == InputActionPhase.Performed)
        {
            rb.AddForce(Vector3.down * ng_verticalImpulseForce, ForceMode.Force);
        }

        if (ng_lockVerticalSpeed && Mathf.Abs(rb.velocity.y) > ng_maxVerticalSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sign(rb.velocity.y) * ng_maxVerticalSpeed, rb.velocity.z);
        }
    }

    void DoPlayerNoGravityRotation()
    {
        if (rb.velocity.magnitude > ng_rotationThreshold)
        {
            //float angleToLook = Vector3.SignedAngle(Vector3.forward, new Vector3(axisVector.x, 0f, axisVector.y), Vector3.up);
            float angleToLook = Vector3.SignedAngle(Vector3.forward, rb.velocity, Vector3.up);
            //print(axisVector + "\n" + angleToLook);
            transform.rotation = Quaternion.Euler(0f, angleToLook, 0f);
        }
    }

    //RIGHT
    //LEFT
    //UP
    //DOWN
    //FWD
    //BWD
    void DoPlayerNoGravityReactorsSize()
    {
        if (controllerMode == ControllerMode.NO_GRAVITY)
        {
            //Vector3 forceDirectionVector = axisTransform.right * ng_timedAxis.x + axisTransform.forward * ng_timedAxis.y;
            Vector3 forceDirectionVector = new Vector3(ng_timedAxis.x, 0f, ng_timedAxis.y);
            if (forceDirectionVector.magnitude > 1f)
            {
                forceDirectionVector.Normalize();
            }
            for (int i = 0; i < 6; i += 2)
            {
                if (Mathf.Abs(forceDirectionVector[i / 2]) > 0.05f)
                {
                    int negInd = forceDirectionVector[i / 2] > 0f ? 1 : 0;
                    ng_reactors[i + negInd].localScale = new Vector3(1f, 1f, Mathf.Abs(forceDirectionVector[i / 2]));
                    ng_reactors[i + (1 - negInd)].localScale = new Vector3(1f, 1f, 0f);
                }
            }
        }
        else
        {
            foreach (Transform obj in ng_reactors)
            {
                obj.localScale = new Vector3(1f, 1f, 0f);
            }
        }
    }

    Vector3 FlatAndNormalize(Vector3 _vectorToFlat)
    {
        Vector3 v = new Vector3(_vectorToFlat.x, 0f, _vectorToFlat.z).normalized;
        return v;
    }

    #region Animation

    void UpdatePlayerAnimationState()
    {
        if (controllerMode == ControllerMode.GRAVITY)
        {
            if (isGrounded())
            {
                if (new Vector2(rb.velocity.x, rb.velocity.z).magnitude >= idleVelocityThreshold)
                {
                    playerAnimationState = PlayerState.RUNNING;
                }
                else
                {
                    playerAnimationState = PlayerState.IDLE;
                }
            }
            else
            {
                playerAnimationState = rb.velocity.y > 0f ? PlayerState.JUMPING : PlayerState.FALLING;
            }
        }
        else
        {
            playerAnimationState = PlayerState.NO_GRAVITY;
        }

        //animator?.SetInteger("playerState", (int)playerAnimationState);
    }

    #endregion

}
