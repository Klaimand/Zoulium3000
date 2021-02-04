using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class KLD_PlayerController : SerializedMonoBehaviour
{

    [SerializeField]
    Transform axisTransform;
    Rigidbody rb;
    CapsuleCollider col;

    public enum ControllerMode
    {
        GRAVITY,
        NO_GRAVITY
    }

    [SerializeField]
    ControllerMode controllerMode;

    [SerializeField]
    float speed = 10f;

    [SerializeField, Header("Jump")]
    float jumpSpeed = 10f;
    [SerializeField] float fallMultiplier = 2f; //more means a faster fall
    [SerializeField] float lowJumpMultiplier = 3f; //more means a lower minimal jump

    [SerializeField] LayerMask groundLayer;
    [SerializeField, ReadOnly] bool m_isGrounded = false;

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
        DoPlayerMove();
        CheckPlayerJump();

        m_isGrounded = isGrounded();
    }

    private void FixedUpdate()
    {
        CheckFall();
    }

    void DoPlayerMove()
    {
        float xSpeed = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed * 100f;
        float zSpeed = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed * 100f;
        Vector3 flatSpeedVector = axisTransform.right * xSpeed + axisTransform.forward * zSpeed;
        rb.velocity = new Vector3(flatSpeedVector.x, rb.velocity.y, flatSpeedVector.z);
    }

    void CheckPlayerJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity += Vector3.up * jumpSpeed;
        }
    }

    bool isGrounded()
    {
        float radius = col.radius * 0.9f;
        return Physics.CheckSphere(transform.position + Vector3.up * radius * 0.9f, radius, groundLayer);
    }

    void CheckFall()
    {
        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))  //check if we're jumping and gaining height
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
