using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IMovementDetector
{
    [Header("Movement")]
    public bool canMove = true;
    public float walkSpeed = 4f;
    public float sprintMultiplier = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    [SerializeField] private float distanceThreshold = 5f;

    private Vector3 _prevPos;
    private float _distanceBuffer;

    [Header("References")]
    [SerializeField] private Animator animator;
    public Transform cameraTransform;

    [Header("Special Action")]
    public KeyCode specialKey = KeyCode.C;
    public KeyCode dodgeKey = KeyCode.X;

    [Header("Bounce Settings")]
    public float bounceForwardForce = 10f;
    public float bounceUpwardForce = 5f;
    public float bounceDuration = 0.5f;

    [Header("Dodge Settings")]
    public float dodgeSpeed = 12f;
    public float dodgeDuration = 0.2f;

    [HideInInspector] 
    public CharacterController controller;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isSpecialActive = false;

    private Vector3 lastMoveDirection = Vector3.zero;
    private Vector3 _moveDirection = Vector3.zero; 

    private bool isSprinting = false;               

    [SerializeField] private PlayerStat statSystem;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        _prevPos = transform.position;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleSpecialAction();
        HandleDodge();
        ApplyGravity();
        TrackMovementXP();
    }

    public bool IsMoving()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        return Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f;
    }

    void HandleMovement()
    {
        if (!canMove || isSpecialActive)
        {
            animator.SetFloat("xMove", 0);
            animator.SetFloat("yMove", 0);
            animator.SetBool("Directional_Basic_Movement", false);
            _moveDirection = Vector3.zero;
            return;
        }

        isGrounded = controller.isGrounded;

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * inputZ + camRight * inputX;

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        float speed = walkSpeed * (isSprinting ? sprintMultiplier : 1f);
        speed *= statSystem.speedMultiplier;

        if (move.magnitude > 0.1f)
            lastMoveDirection = move.normalized;

        _moveDirection = move * speed;

        controller.Move(_moveDirection * Time.deltaTime);

        Vector3 localMove = transform.InverseTransformDirection(move);
        animator.SetFloat("xMove", localMove.x);
        animator.SetFloat("yMove", localMove.z);
        animator.SetBool("Directional_Basic_Movement", move.magnitude > 0.1f);
    }

    public float GetCurrentSpeed()
    {
        Vector3 horizontal = new Vector3(_moveDirection.x, 0, _moveDirection.z);
        return horizontal.magnitude;
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

    private void TrackMovementXP()
    {
        Vector3 delta = transform.position - _prevPos;
        delta.y = 0f;
        _prevPos = transform.position;

        _distanceBuffer += delta.magnitude;

        while (_distanceBuffer >= distanceThreshold)
        {
            statSystem.RegisterSpeedAction();
            _distanceBuffer -= distanceThreshold;
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isSpecialActive)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    void HandleSpecialAction()
    {
        if (Input.GetKeyDown(specialKey) && !isSpecialActive)
        {
            StartCoroutine(BounceAction());
            statSystem.RegisterDashAction();
        }
    }

    IEnumerator BounceAction()
    {
        isSpecialActive = true;

        float timer = 0f;
        Vector3 bounceDirection = cameraTransform.forward;
        bounceDirection.y = 0;
        bounceDirection.Normalize();

        Vector3 initialVelocity = bounceDirection * bounceForwardForce * statSystem.dashDistanceMultiplier;
        initialVelocity += Vector3.up * bounceUpwardForce;

        while (timer < bounceDuration)
        {
            controller.Move(initialVelocity * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isSpecialActive = false;
    }

    void HandleDodge()
    {
        if (Input.GetKeyDown(dodgeKey) && !isSpecialActive)
        {
            StartCoroutine(Dodge());
            statSystem.RegisterDashAction();
        }
    }

    IEnumerator Dodge()
    {
        isSpecialActive = true;
        float timer = 0f;

        if (lastMoveDirection == Vector3.zero)
        {
            Debug.Log("No movement direction — dodge cancelled.");
            isSpecialActive = false;
            yield break;
        }

        Vector3 dodgeVelocity = lastMoveDirection * dodgeSpeed * statSystem.dashDistanceMultiplier;

        while (timer < dodgeDuration)
        {
            controller.Move(dodgeVelocity * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isSpecialActive = false;
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    #region Save & Load
    public void Save(ref PlayerSaveLocation data)
    {
        data.Position = transform.position;
    }

    public void Load(PlayerSaveLocation data)
    {
        transform.position = data.Position;
    }
    #endregion
}

[System.Serializable]
public struct PlayerSaveLocation
{
    public Vector3 Position;
}
