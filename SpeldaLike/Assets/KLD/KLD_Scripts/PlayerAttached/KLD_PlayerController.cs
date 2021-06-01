using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class KLD_PlayerController : SerializedMonoBehaviour
{
    #region Variables

    //[SerializeField] PlayerInput playerInput;
    [SerializeField] Transform axisTransform;
    [SerializeField] Transform playerGroundPoint;
    [SerializeField] Transform dampedGroundPoint;
    [SerializeField] float groundPointDampingSpeed = 0.5f;
    [SerializeField] float groundPointDampingMaxSpeed = 2f;
    float yVelocity = 0f;
    Rigidbody rb;
    CapsuleCollider col;
    Camera mainCamera;

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

    Vector3 flatSpeedVector; //non normalized direction where the player moves
    Vector2 axisVector; //normalized direction where the player moves

    bool LT_GetKey = false;
    bool LT_GetKeyDown = false;

    bool RT_GetKey = false;
    bool RT_GetKeyDown = false;

    [SerializeField, Header("Movement")]
    float speed = 10f;

    [SerializeField, Header("Ground Detection"), Range(0.5f, 1f)]
    float sphereRadiusMultiplier = 0.9f;
    [SerializeField] float maxSlopeAngle = 30f;
    [SerializeField] PhysicMaterial noFrictionMat = null;
    [SerializeField] PhysicMaterial frictionMat = null;
    bool groundDetectionDisabled = false;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask groundPointLayer;

    Vector3 lastGroundedPosition = Vector3.zero;

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
    //[SerializeField] float steepSlopeLockedAngle = 90f;

    //[SerializeField, ReadOnly] bool m_isGrounded = false;

    [SerializeField, Header("PowerJump")]
    float powerJumpSpeed = 30f;
    [SerializeField] float powerJumpLoadTime = 0.5f;
    float curPowerJumpLoadTime = 0f;
    [SerializeField] Vector2 powerJumpDrag = new Vector2(0.04f, 0.1f);
    [SerializeField] float powerJumpFallMultiplier = 1.02f;
    [SerializeField] float powerJumpHorizontalSpeed = 10f;
    [SerializeField] float maxPowerJumpAirSpeed = 20f;
    [SerializeField] float powerJumpAddAirSpeed = 20f;
    [SerializeField] GameObject powerJumpAttackPrefab;
    bool justPowerJumped = false;

    [SerializeField, Header("Grappling Hook")]
    float gh_time = 5f;
    float gh_curTime = 0f;
    Vector3 gh_startPos;
    [SerializeField] AnimationCurve gh_speedCurve;
    [SerializeField] LayerMask anchorDetectionRayMask;
    KLD_Anchor[] anchors;
    [SerializeField] float maxAnchorDist = 30f;
    [ReadOnly, SerializeField] KLD_Anchor selectedAnchor;
    [ReadOnly, SerializeField] KLD_Anchor grabbedAnchor;
    //List<KLD_Anchor> anchorsListBuffer = new List<KLD_Anchor>();
    [SerializeField] float maxAnchorAngle = 60f;
    [SerializeField] Transform grapplingStartPoint;
    [SerializeField] GameObject grapplingLrObject;
    LineRenderer lr;
    [SerializeField] float mastodonteGrabDuration = 0.95f;
    float curMastodonteGrabTime;

    [SerializeField, Header("Dodge")]
    float dodgeForce = 10f;
    [SerializeField] float dodgeMinJoystickMagnitude = 0.5f;
    [SerializeField] float dodgeTime = 1f;
    float dodgeCurTime = 0f;
    [SerializeField] float dodgeDrag = 0.05f;
    [SerializeField] float dodgeCooldown = 1f;
    float dodgeCurCooldown = 0f;

    [SerializeField, Header("Attack")]
    float attackComboCooldown = 3f;
    [SerializeField] float attackComboLoseTime = 1f;
    float timeSinceLastAttack = 0f;
    float timeSinceLastCombo = 0f;
    enum Attack { DEFAULT, FIRST_ATTACK, SECOND_ATTACK, THIRD_ATTACK };
    Attack curAttack = Attack.DEFAULT;
    float[] attacksTime = { 0.4f, 0.5f, 1.23f };
    int attackState = 0;
    bool attackBuffer = false;
    [SerializeField] float attackBufferLenght = 0.3f;
    [SerializeField] GameObject[] attacksFxPrefabs;
    [SerializeField] GameObject[] attacksZonePrefabs;
    [SerializeField] GameObject attackZonePrefab;

    [SerializeField]
    enum PlayerState
    {
        IDLE, //0
        RUNNING, //1
        JUMPING, //2
        FALLING, //3

        POWERCROUCHING, //4
        POWERJUMPING, //5
        POWERFALLING, //6

        FLOATING, //7

        GRAPPLING, //8
        GRAPPLING_GRABBED, //9

        DODGING, //10

        FORCED_IDLE, //11

        GRAPPLING_PULLING //12
    };
    [SerializeField, Space(10)] PlayerState curPlayerState = PlayerState.IDLE;

    [SerializeField, Header("NO GRAVITY CONTROLLER"), Space(20)]
    float ng_impulseForce = 3f;
    [SerializeField] float ng_verticalImpulseForce = 3f;
    [SerializeField] float ng_maxHorizontalSpeed = 10f;
    [SerializeField] float ng_maxVerticalSpeed = 5f;
    [SerializeField] float ng_rotationThreshold = 0.1f;
    [SerializeField] float ng_lockedAngle = 135f;
    [SerializeField] bool ng_lockHorizontalSpeed = true;
    [SerializeField] bool ng_lockVerticalSpeed = true;

    [SerializeField, Header("Animation"), Space(20)]
    Animator animator = null;
    int animState = 0;

    //CAPACITES
    [SerializeField]
    public enum PowerUp { DEFAULT, POWERJUMP, GRAPPLING_HOOK };
    [SerializeField] HashSet<PowerUp> curPowerUps = new HashSet<PowerUp>();
    [SerializeField] List<PowerUp> startPowerUps = new List<PowerUp>(); //I must use an init list because hashset clears on start

    #endregion

    #region Monobehaviour Voids

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lr = grapplingLrObject.GetComponent<LineRenderer>();
        grapplingLrObject.SetActive(false);

        mainCamera = Camera.main;
        UpdatePlayerGroundPointPosition();
        dampedGroundPoint.position = playerGroundPoint.position;
        GiveStartPups();
        anchors = GetAnchors();

        timeSinceLastCombo = 99f;
        timeSinceLastAttack = 99f;
    }

    // Update is called once per frame
    void Update()
    {
        DoTriggerInputProcessing();

        UpdatePlayerState();

        UpdatePlayerGroundPointPosition();
        UpdateDampedGroundPointPosition();

        UpdatePlayerAnimationState();

        ResetPlayerOnVoidFall(); //hard debug

        dodgeCurCooldown -= Time.deltaTime;
        AttackCooldownsManager();
    }

    private void FixedUpdate()
    {
        //axis
        DoDeadZoneRawAxis();
        DoTimedAxis();
        DoNoGravityTimedAxis();

        CalculateAxisVector();

        DoPlayerBehavior();
    }

    private void OnEnable()
    {
        GameEvents.Instance.onGravityDisable += OnGravityDisable;
        GameEvents.Instance.onGravityEnable += OnGravityEnable;

        GameEvents.Instance.onDialogStart += OnDialogStart;
        GameEvents.Instance.onDialogEnd += OnDialogEnd;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onGravityDisable -= OnGravityDisable;
        GameEvents.Instance.onGravityEnable -= OnGravityEnable;

        GameEvents.Instance.onDialogStart -= OnDialogStart;
        GameEvents.Instance.onDialogEnd -= OnDialogEnd;
    }

    #endregion

    #region Events

    void OnGravityEnable(int _id)
    {
        if (controllerMode != ControllerMode.GRAVITY)
        {
            controllerMode = ControllerMode.GRAVITY;
            curPlayerState = PlayerState.FALLING;
            rb.useGravity = true;
        }
    }

    void OnGravityDisable(int _id)
    {
        if (controllerMode != ControllerMode.NO_GRAVITY)
        {
            controllerMode = ControllerMode.NO_GRAVITY;
            curPlayerState = PlayerState.FLOATING;
            rb.useGravity = false;
        }
    }

    void OnDialogStart()
    {
        curPlayerState = PlayerState.FORCED_IDLE;
    }

    void OnDialogEnd()
    {
        curPlayerState = PlayerState.IDLE;
    }

    #endregion

    #region StateMachine

    void UpdatePlayerState()
    {
        if (curPlayerState == PlayerState.IDLE) //________________________________IDLE
        {
            if (CheckPlayerJump())
            {
                curPlayerState = PlayerState.JUMPING;
            }

            if (timedAxis.magnitude != 0f)
            {
                curPlayerState = PlayerState.RUNNING;
            }

            if (!isGrounded())
            {
                curPlayerState = PlayerState.FALLING;
            }

            if ((Input.GetButtonDown("Crouch") || LT_GetKeyDown) && HavePowerUp(PowerUp.POWERJUMP))
            {
                curPowerJumpLoadTime = 0f;
                curPlayerState = PlayerState.POWERCROUCHING;
                col.material = frictionMat;
            }

            if ((Input.GetButtonDown("Grapple") || RT_GetKeyDown) && HavePowerUp(PowerUp.GRAPPLING_HOOK) && selectedAnchor != null)
            {
                curPlayerState = PlayerState.GRAPPLING;
                grapplingLrObject.SetActive(true);
                gh_curTime = 0f;
                gh_startPos = transform.position;
                grabbedAnchor = selectedAnchor;

                if (selectedAnchor.isOnEnemy)
                {
                    curPlayerState = PlayerState.GRAPPLING_PULLING;
                    curMastodonteGrabTime = 0f;
                }
                return;
            }

            if (CheckDodge())
            {
                curPlayerState = PlayerState.DODGING;
            }

            CheckAttack();
        }
        else if (curPlayerState == PlayerState.RUNNING) //_______________________RUNNING
        {
            if (CheckPlayerJump())
            {
                curPlayerState = PlayerState.JUMPING;
            }

            if (timedAxis.magnitude == 0f)
            {
                curPlayerState = PlayerState.IDLE;
            }

            if (!isGrounded())
            {
                curPlayerState = PlayerState.FALLING;
            }

            if ((Input.GetButtonDown("Grapple") || RT_GetKeyDown) && HavePowerUp(PowerUp.GRAPPLING_HOOK) && selectedAnchor != null)
            {
                curPlayerState = PlayerState.GRAPPLING;
                grapplingLrObject.SetActive(true);
                gh_curTime = 0f;
                gh_startPos = transform.position;
                grabbedAnchor = selectedAnchor;

                if (selectedAnchor.isOnEnemy)
                {
                    curPlayerState = PlayerState.GRAPPLING_PULLING;
                    curMastodonteGrabTime = 0f;
                }
                return;
            }

            if (CheckDodge())
            {
                curPlayerState = PlayerState.DODGING;
            }

            CheckAttack();
        }
        else if (curPlayerState == PlayerState.JUMPING) //_______________________JUMPING
        {
            if (rb.velocity.y < 0f)
            {
                curPlayerState = PlayerState.FALLING;
            }

            CheckPlayerJump();

            if ((Input.GetButtonDown("Grapple") || RT_GetKeyDown) && HavePowerUp(PowerUp.GRAPPLING_HOOK) && selectedAnchor != null)
            {

                curPlayerState = PlayerState.GRAPPLING;
                grapplingLrObject.SetActive(true);
                gh_curTime = 0f;
                gh_startPos = transform.position;
                grabbedAnchor = selectedAnchor;

                if (selectedAnchor.isOnEnemy)
                {
                    curPlayerState = PlayerState.GRAPPLING_PULLING;
                    curMastodonteGrabTime = 0f;
                }
                return;
            }

            GroundedRunningIdleCheck();
        }
        else if (curPlayerState == PlayerState.FALLING) //_______________________FALLING
        {
            if (CheckPlayerJump())
            {
                curPlayerState = PlayerState.JUMPING;
            }

            if ((Input.GetButtonDown("Grapple") || RT_GetKeyDown) && HavePowerUp(PowerUp.GRAPPLING_HOOK) && selectedAnchor != null)
            {
                curPlayerState = PlayerState.GRAPPLING;
                grapplingLrObject.SetActive(true);
                gh_curTime = 0f;
                gh_startPos = transform.position;
                grabbedAnchor = selectedAnchor;

                if (selectedAnchor.isOnEnemy)
                {
                    curPlayerState = PlayerState.GRAPPLING_PULLING;
                    curMastodonteGrabTime = 0f;
                }
                return;
            }

            GroundedRunningIdleCheck();
        }
        else if (curPlayerState == PlayerState.POWERCROUCHING) //________________POWERCROUCHING
        {
            if (!Input.GetButton("Crouch") && !LT_GetKey)
            {
                curPlayerState = PlayerState.IDLE;
                col.material = noFrictionMat;
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (curPowerJumpLoadTime >= powerJumpLoadTime)
                {
                    PowerJump();
                    curPlayerState = PlayerState.POWERJUMPING;
                    //print("Power Jumped");
                    justPowerJumped = true;
                    StartCoroutine(WaitAndDisableJustPowerJumped());
                    col.material = noFrictionMat;
                }
            }

        }
        else if (curPlayerState == PlayerState.POWERJUMPING) //__________________POWERJUMPING
        {
            if (rb.velocity.y < 0f)
            {
                curPlayerState = PlayerState.POWERFALLING;
                //print("powerfell");
            }

            if ((Input.GetButtonDown("Grapple") || RT_GetKeyDown) && HavePowerUp(PowerUp.GRAPPLING_HOOK) && selectedAnchor != null)
            {
                curPlayerState = PlayerState.GRAPPLING;
                grapplingLrObject.SetActive(true);
                gh_curTime = 0f;
                gh_startPos = transform.position;
                grabbedAnchor = selectedAnchor;

                if (selectedAnchor.isOnEnemy)
                {
                    curPlayerState = PlayerState.GRAPPLING_PULLING;
                    curMastodonteGrabTime = 0f;
                }
                return;
            }

            if (!justPowerJumped)
            {
                GroundedRunningIdleCheck();
            }
        }
        else if (curPlayerState == PlayerState.POWERFALLING) //___________________POWERFALLING
        {
            if ((Input.GetButtonDown("Grapple") || RT_GetKeyDown) && HavePowerUp(PowerUp.GRAPPLING_HOOK) && selectedAnchor != null)
            {
                curPlayerState = PlayerState.GRAPPLING;
                grapplingLrObject.SetActive(true);
                gh_curTime = 0f;
                gh_startPos = transform.position;
                grabbedAnchor = selectedAnchor;

                if (selectedAnchor.isOnEnemy)
                {
                    curPlayerState = PlayerState.GRAPPLING_PULLING;
                    curMastodonteGrabTime = 0f;
                }
                return;
            }

            //GroundedRunningIdleCheck();
            if (isGrounded() && !groundDetectionDisabled)
            {
                Instantiate(powerJumpAttackPrefab, transform.position, Quaternion.identity);
                if (timedAxis.magnitude != 0f)
                {
                    curPlayerState = PlayerState.RUNNING;
                }
                else
                {
                    curPlayerState = PlayerState.IDLE;
                }
            }
        }
        else if (curPlayerState == PlayerState.FLOATING) //_______________________FLOATING
        {

        }
        else if (curPlayerState == PlayerState.GRAPPLING) //______________________GRAPPLING
        {
            if (CheckGrabbed())
            {
                curPlayerState = PlayerState.GRAPPLING_GRABBED;
                rb.isKinematic = true;
                grapplingLrObject.SetActive(false);
            }

            if (!Input.GetButton("Grapple") && !RT_GetKey)
            {
                curPlayerState = PlayerState.IDLE;
                grabbedAnchor = null;
                grapplingLrObject.SetActive(false);
                UpdatePlayerState();
            }
        }
        else if (curPlayerState == PlayerState.GRAPPLING_GRABBED) //______________GRAPPLING_GRABBED
        {
            if (!Input.GetButton("Grapple") && !RT_GetKey)
            {
                rb.isKinematic = false;
                curPlayerState = PlayerState.IDLE;
                grabbedAnchor = null;
                UpdatePlayerState();
            }

            if (Input.GetButtonDown("Jump") && selectedAnchor != null)
            {
                rb.isKinematic = false;
                curPlayerState = PlayerState.GRAPPLING;
                grapplingLrObject.SetActive(true);
                gh_curTime = 0f;
                gh_startPos = transform.position;
                grabbedAnchor = selectedAnchor;
            }
        }
        else if (curPlayerState == PlayerState.DODGING) //___________________________DODGING
        {
            dodgeCurTime += Time.deltaTime;

            if (dodgeCurTime >= dodgeTime)
            {
                curPlayerState = PlayerState.IDLE;
                UpdatePlayerState();
            }
        }
        else if (curPlayerState == PlayerState.GRAPPLING_PULLING) //______________GRAPPLING_PULLING
        {
            if (curMastodonteGrabTime >= mastodonteGrabDuration)
            {
                curPlayerState = PlayerState.IDLE;
                UpdatePlayerState();
            }
            else if (!Input.GetButton("Grapple") && !RT_GetKey)
            {
                curPlayerState = PlayerState.IDLE;
                grabbedAnchor = null;
                grapplingLrObject.SetActive(false);
                UpdatePlayerState();
            }
        }
    }

    void DoPlayerBehavior()
    {
        switch (curPlayerState)
        {
            case PlayerState.IDLE:
                DoPlayerMove();
                ChangePlayerMaterial();
                DoPlayerRotation();
                UpdateSelectedAnchorIfPup();
                break;

            case PlayerState.RUNNING:
                DoPlayerMove();
                ChangePlayerMaterial();
                DoPlayerRotation();
                UpdateSelectedAnchorIfPup();
                break;

            case PlayerState.JUMPING:
                DoPlayerMove();
                //DoPlayerRotation();
                DoPlayerVelocityRotation();
                CheckFall();
                UpdateSelectedAnchorIfPup();
                break;

            case PlayerState.FALLING:
                DoPlayerMove();
                //DoPlayerRotation();
                DoPlayerVelocityRotation();
                CheckFall();
                UpdateSelectedAnchorIfPup();
                break;

            case PlayerState.POWERCROUCHING:
                DoPlayerRotation();
                curPowerJumpLoadTime += Time.deltaTime;
                break;

            case PlayerState.POWERJUMPING:
                DoPlayerPowerJumpMove(false); //moved to fixed
                //DoPlayerMove();
                //DoPlayerRotation();
                DoPlayerVelocityRotation();
                //CheckFall();
                UpdateSelectedAnchorIfPup();
                break;

            case PlayerState.POWERFALLING:
                DoPlayerPowerJumpMove(true);
                //DoPlayerMove();
                //DoPlayerRotation();
                DoPlayerVelocityRotation();
                //CheckFall();
                UpdateSelectedAnchorIfPup();
                break;

            case PlayerState.FLOATING:
                DoPlayerNoGravityMove();
                DoPlayerNoGravityRotation();
                UpdateSelectedAnchorIfPup();
                break;

            case PlayerState.GRAPPLING:
                lr.SetPosition(0, grapplingStartPoint.position);
                lr.SetPosition(1, selectedAnchor.transform.position + Vector3.up * 1.5f);
                break;

            case PlayerState.GRAPPLING_GRABBED:
                UpdateSelectedAnchorIfPup();
                DoGrabbedRotation();
                break;

            case PlayerState.DODGING:
                DoPlayerDodge();
                break;

            case PlayerState.FORCED_IDLE:
                //do nothing we cant move
                rb.velocity = Vector3.zero;
                break;

            case PlayerState.GRAPPLING_PULLING:
                rb.velocity = Vector3.zero;
                curMastodonteGrabTime += Time.fixedDeltaTime;
                if (grabbedAnchor != null)
                {
                    DoGrabbedRotation();
                }
                break;

            default:
                Debug.LogError("WHATTTTTATATATATTA");
                break;

        }
    }

    void GroundedRunningIdleCheck()
    {
        if (isGrounded() && !groundDetectionDisabled)
        {
            if (timedAxis.magnitude != 0f)
            {
                curPlayerState = PlayerState.RUNNING;
            }
            else
            {
                curPlayerState = PlayerState.IDLE;
            }
        }
    }

    #endregion

    #region Axis Computing

    void DoDeadZoneRawAxis()
    {
        rawAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //OLD INPUT

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

    void DoTriggerInputProcessing()
    {
        bool frameLT = Input.GetAxisRaw("LeftTrigger") >= 0.9f;
        LT_GetKeyDown = frameLT && !LT_GetKey;
        LT_GetKey = frameLT;

        bool frameRT = Input.GetAxisRaw("RightTrigger") >= 0.9f;
        RT_GetKeyDown = frameRT && !RT_GetKey;
        RT_GetKey = frameRT;
    }

    #endregion

    void CalculateAxisVector()
    {
        Vector2 clampedTimedAxis = timedAxis;
        if (clampedTimedAxis.sqrMagnitude > 1f)
        {
            clampedTimedAxis.Normalize();
        }

        float xSpeed = clampedTimedAxis.x * Time.fixedDeltaTime * speed * 30f;
        float zSpeed = clampedTimedAxis.y * Time.fixedDeltaTime * speed * 30f;

        flatSpeedVector = axisTransform.right * xSpeed + axisTransform.forward * zSpeed;
        axisVector = new Vector2(flatSpeedVector.x, flatSpeedVector.z).normalized;
    }

    void DoPlayerMove()
    {
        //float xSpeed = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed * 30f;
        //float zSpeed = Input.GetAxis("Vertical") * Time.fixedDeltaTime * speed * 30f;

        //calc axis v

        if (isGrounded())
        {
            //rb.velocity = new Vector3(flatSpeedVector.x, rb.velocity.y, flatSpeedVector.z);

            Vector3 wantedVector = Vector3.ProjectOnPlane(new Vector3(flatSpeedVector.x, rb.velocity.y, flatSpeedVector.z), GetSlopeNormal());

            if (rb.velocity.y >= jumpSpeed * 0.85f)
            {
                wantedVector.y = rb.velocity.y;
            }

            rb.velocity = wantedVector;
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
                if (!isOnSteepSlope())
                //|| (isOnSteepSlope() && Vector3.Angle(inputDirectionVector, FlatAndNormalize(GetSlopeNormal())) > steepSlopeLockedAngle))
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

    void ChangePlayerMaterial()
    {
        if (isGrounded() && Vector3.Angle(Vector3.up, GetSlopeNormal()) > 5f && timedAxis == Vector2.zero)
        {
            col.material = frictionMat;
        }
        else
        {
            col.material = noFrictionMat;
        }
    }


    bool CheckPlayerJump()
    {
        if (Input.GetButtonDown("Jump") || jumpBuffer)
        {
            if (isGrounded())
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.velocity += Vector3.up * jumpSpeed;
                StartCoroutine(WaitAndApplyHorizontalJumpForce());

                jumpBuffer = false;

                return true;
            }
            else if (Input.GetButtonDown("Jump"))
            {
                jumpBuffer = true;
                StartCoroutine(WaitAndDebufferJump());
            }
            //print("jumped while idle");
        }
        return false;
    }

    IEnumerator DisableGroundDetection(int _fixedFrames)
    {
        groundDetectionDisabled = true;
        for (int i = 0; i < _fixedFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        groundDetectionDisabled = false;
    }

    IEnumerator WaitAndApplyHorizontalJumpForce()
    {
        yield return new WaitForFixedUpdate();
        //rb.velocity += new Vector3(timedAxis.x, 0f, timedAxis.y) * jumpHorizontalAddedForce;
        rb.velocity += ((axisTransform.right * timedAxis.x) + (axisTransform.forward * timedAxis.y)) * jumpHorizontalAddedForce;
    }

    IEnumerator WaitAndDebufferJump()
    {
        yield return new WaitForSeconds(jumpBufferDuration);
        jumpBuffer = false;
    }

    void PowerJump()
    {
        StartCoroutine(DisableGroundDetection(2));

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.velocity += Vector3.up * powerJumpSpeed;
        StartCoroutine(WaitAndApplyPowerJumpHorizontalForce()); //Horizontal force if needed

        //print("Power Jumped");
    }

    IEnumerator WaitAndApplyPowerJumpHorizontalForce()
    {
        yield return new WaitForFixedUpdate();
        rb.velocity += ((axisTransform.right * timedAxis.x) + (axisTransform.forward * timedAxis.y)).normalized * powerJumpHorizontalSpeed;
    }

    IEnumerator WaitAndDisableJustPowerJumped()
    {
        yield return new WaitForSeconds(0.1f);
        justPowerJumped = false;
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

        if (detectGround && isSlopeCorrect)
        {
            lastGroundedPosition = transform.position;
        }

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
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
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

    void DoPlayerVelocityRotation()
    {
        Vector3 v = rb.velocity;
        v.y = 0f;

        float o = 0.1f;

        if (v.sqrMagnitude > o * o)
        {
            float angleToLook = Vector3.SignedAngle(Vector3.forward, v, Vector3.up);
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
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 200f, groundPointLayer);
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

    void UpdateDampedGroundPointPosition()
    {
        float startY = dampedGroundPoint.position.y;
        float endY = playerGroundPoint.position.y;
        float curY = Mathf.SmoothDamp(startY, endY, ref yVelocity, groundPointDampingSpeed, groundPointDampingMaxSpeed, Time.deltaTime);
        dampedGroundPoint.position = new Vector3(playerGroundPoint.position.x, curY, playerGroundPoint.position.z);
    }

    void DoPlayerPowerJumpMove(bool isFalling)
    {
        float dragX = 1f - powerJumpDrag.x;
        float dragY = 1f - powerJumpDrag.y;

        float yVelocity = (isFalling ? rb.velocity.y * powerJumpFallMultiplier : rb.velocity.y * dragY);

        rb.velocity = new Vector3(rb.velocity.x * dragX, yVelocity, rb.velocity.z * dragX);

        Vector3 horizontalMagnitude = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (Mathf.Abs(horizontalMagnitude.magnitude) < maxPowerJumpAirSpeed)
        {
            //rb.AddForce(new Vector3(deadZonedRawAxis.x, 0f, deadZonedRawAxis.y) * addAirSpeed);
            Vector3 inputDirectionVector = ((axisTransform.right * deadZonedRawAxis.x) + (axisTransform.forward * deadZonedRawAxis.y)).normalized;
            if (!isOnSteepSlope())
            {
                rb.AddForce(inputDirectionVector * powerJumpAddAirSpeed);
            }
        }
        else if (Mathf.Abs(horizontalMagnitude.magnitude) > maxPowerJumpAirSpeed)
        {
            Vector3 maxHorizontalSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).normalized * maxPowerJumpAirSpeed;
            rb.velocity = new Vector3(maxHorizontalSpeed.x, rb.velocity.y, maxHorizontalSpeed.z);
        }
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
            //print(Vector3.Angle(rbHorizontalMagnitude, forceDirectionVector));
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



        if (rb.velocity.y < ng_maxVerticalSpeed && Input.GetButton("Jump"))
        {
            rb.AddForce(Vector3.up * ng_verticalImpulseForce, ForceMode.Force);
        }
        else if (rb.velocity.y > -ng_maxVerticalSpeed && (Input.GetButton("Crouch") || LT_GetKey))
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

    Vector3 FlatAndNormalize(Vector3 _vectorToFlat)
    {
        Vector3 v = new Vector3(_vectorToFlat.x, 0f, _vectorToFlat.z).normalized;
        return v;
    }


    bool HavePowerUp(PowerUp _powerUp)
    {
        return curPowerUps.Contains(_powerUp);
    }

    public void GivePowerUp(PowerUp _powerUpToGive)
    {
        curPowerUps.Add(_powerUpToGive);
    }

    void GiveStartPups()
    {
        foreach (var pup in startPowerUps)
        {
            GivePowerUp(pup);
        }
    }

    KLD_Anchor[] GetAnchors()
    {
        GameObject[] anchorObjs = GameObject.FindGameObjectsWithTag("Anchor");

        if (anchorObjs.Length == 0)
            return null;

        KLD_Anchor[] anchorsBuffer = new KLD_Anchor[anchorObjs.Length];

        for (int i = 0; i < anchorObjs.Length; i++)
        {
            anchorsBuffer[i] = anchorObjs[i].GetComponent<KLD_Anchor>();
        }

        return anchorsBuffer;
    }

    void UpdateSelectedAnchorIfPup()
    {

        if (!HavePowerUp(PowerUp.GRAPPLING_HOOK))
            return;

        float minAngle = 999f;
        int minAngleIndex = 0;

        for (int i = 0; i < anchors.Length; i++)
        {
            if (anchors[i] == null)
                continue;

            Vector3 playerToAnchor = anchors[i].transform.position - transform.position;

            if (playerToAnchor.sqrMagnitude < maxAnchorDist * maxAnchorDist) //dist player-anchor check
            {
                Vector3 cameraToAnchor = anchors[i].transform.position - mainCamera.transform.position;
                float ptaMagnitude = playerToAnchor.magnitude;
                float ctaMagnitude = cameraToAnchor.magnitude;

                if (!Physics.Raycast(transform.position, playerToAnchor, ptaMagnitude, anchorDetectionRayMask)) //ray player-anchor
                {
                    if (!Physics.Raycast(mainCamera.transform.position, cameraToAnchor, ctaMagnitude, anchorDetectionRayMask)) //ray camera-anchor
                    {

                        Vector3 anchorDirection = playerToAnchor;

                        if (curPlayerState == PlayerState.GRAPPLING_GRABBED)
                        {
                            anchorDirection = Vector3.ProjectOnPlane(anchorDirection, axisTransform.forward);
                        }
                        else
                        {
                            anchorDirection.y = 0f; //maybe remove this A TESTER
                        }

                        Vector3 referenceDirection = curPlayerState == PlayerState.GRAPPLING_GRABBED ?
                                                                        (Vector3)axisVector :
                                                                        transform.forward;

                        if (referenceDirection == Vector3.zero)
                            continue;

                        Debug.DrawRay(transform.position + Vector3.up * 2f, referenceDirection, Color.red);
                        Debug.DrawRay(transform.position + Vector3.up * 2f, anchorDirection, Color.green);

                        float curAngle = Vector3.Angle(referenceDirection, anchorDirection);

                        if (curAngle < minAngle && anchors[i].curState != KLD_Anchor.AnchorState.GRABBED)
                        {
                            minAngle = curAngle;
                            minAngleIndex = i;
                        }
                    }
                }
            }
        }

        selectedAnchor = minAngle < maxAnchorAngle ? anchors[minAngleIndex] : null;

        for (int i = 0; i < anchors.Length; i++)
        {
            if (anchors[i] == grabbedAnchor)
            {
                anchors[i].curState = KLD_Anchor.AnchorState.GRABBED;
            }
            else if (selectedAnchor != null && i == minAngleIndex)
            {
                anchors[i].curState = KLD_Anchor.AnchorState.SELECTED;
            }
            else
            {
                anchors[i].curState = KLD_Anchor.AnchorState.FREE;
            }
        }

    }

    bool CheckGrabbed()
    {
        //transform.position = selectedAnchor.transform.position;

        transform.position = Vector3.Lerp(gh_startPos, grabbedAnchor.transform.position, gh_speedCurve.Evaluate(gh_curTime / gh_time));

        gh_curTime += Time.deltaTime;

        bool done = gh_curTime >= gh_time;

        if (done)
            transform.position = grabbedAnchor.transform.position;

        return done;

    }

    void DoGrabbedRotation()
    {
        transform.LookAt(grabbedAnchor.transform.position + grabbedAnchor.transform.forward);
    }

    /// <summary>
    /// Check if dodge input is pressed and apply velocity
    /// </summary>
    /// <returns></returns>
    bool CheckDodge()
    {
        if (Input.GetButtonDown("Dodge") && timedAxis.sqrMagnitude > dodgeMinJoystickMagnitude * dodgeMinJoystickMagnitude && dodgeCurCooldown < 0f)
        {
            rb.velocity += ((axisTransform.right * timedAxis.x) + (axisTransform.forward * timedAxis.y)).normalized * dodgeForce;
            dodgeCurTime = 0f;
            dodgeCurCooldown = dodgeCooldown;
            return true;
        }

        return false;
    }

    void DoPlayerDodge()
    {
        float drag = 1f - dodgeDrag;

        Vector3 vel;
        vel = rb.velocity;
        vel.x *= drag;
        vel.z *= drag;

        rb.velocity = vel;
    }

    void CheckAttack()
    {
        if (Input.GetButtonDown("Attack") || attackBuffer)
        {
            bool didAttack = false;
            //if (curAttack == Attack.DEFAULT && timeSinceLastCombo >= attacksTime[2] + attackComboCooldown)
            if (curAttack == Attack.DEFAULT && timeSinceLastCombo >= attackComboCooldown)
            {
                curAttack = Attack.FIRST_ATTACK;
                //InstantiateAttackVFX(0);
                //Instantiate(attackZonePrefab, transform.position, transform.rotation, transform);
                timeSinceLastAttack = 0f;
                didAttack = true;
                attackBuffer = false;
            }
            else if (curAttack != Attack.DEFAULT && curAttack != Attack.THIRD_ATTACK)
            {
                int i = (int)curAttack - 1;
                if (timeSinceLastAttack > attacksTime[i] && timeSinceLastAttack < attacksTime[i] + attackComboLoseTime)
                {
                    curAttack = (Attack)(i + 2);
                    //InstantiateAttackVFX(i);
                    //Instantiate(attackZonePrefab, transform.position, transform.rotation, transform);
                    timeSinceLastAttack = 0f;
                    didAttack = true;
                    attackBuffer = false;
                }

                if (!didAttack && !attackBuffer)
                {
                    attackBuffer = true;
                }
            }

            if (!didAttack && !attackBuffer && curAttack == Attack.DEFAULT)
            {
                attackBuffer = true;
                StartCoroutine(WaitAndDisableAttackBuffer());
            }
        }
    }

    IEnumerator WaitAndDisableAttackBuffer()
    {
        yield return new WaitForSeconds(attackBufferLenght);
        if (curAttack == Attack.DEFAULT)
        {
            attackBuffer = false;
        }
    }

    void AttackCooldownsManager()
    {
        if (curAttack != Attack.DEFAULT && curAttack != Attack.THIRD_ATTACK)
        {
            if (timeSinceLastAttack > attacksTime[(int)curAttack - 1] + attackComboLoseTime)
            {
                curAttack = Attack.DEFAULT;
                timeSinceLastCombo = 0f;
            }
        }
        else if (curAttack == Attack.THIRD_ATTACK)
        {
            if (timeSinceLastAttack > attacksTime[2])
            {
                curAttack = Attack.DEFAULT;
                timeSinceLastCombo = 0f;
            }
        }

        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastCombo += Time.deltaTime;
    }

    public void InstantiateAttackVFX(int _attackIndex)
    {
        Instantiate(attacksFxPrefabs[_attackIndex], transform.position, transform.rotation, transform);
    }

    public void InstantiateAttack(int _attackIndex)
    {
        Instantiate(attacksZonePrefabs[_attackIndex], transform.position, transform.rotation, transform);
    }

    public int GetPlayerState()
    {
        return (int)curPlayerState;
    }

    public void InvokeMastodonteAnchorEvent()
    {
        grabbedAnchor?.onGrab.Invoke();
        grabbedAnchor = null;
    }

    #region Animation

    void UpdatePlayerAnimationState()
    {
        animator?.SetInteger("lastState", animState);

        animState = (int)curPlayerState;

        animator?.SetInteger("playerState", animState);

        //attack
        animator?.SetInteger("LastAttackState", attackState);

        attackState = (int)curAttack;

        animator?.SetInteger("AttackState", attackState);

    }

    #endregion

    #region Debug

    void ResetPlayerOnVoidFall()
    {
        if (transform.position.y < -1000f)
        {
            rb.velocity = Vector3.zero;
            transform.position = lastGroundedPosition + Vector3.up;
        }
    }

    #endregion

}
