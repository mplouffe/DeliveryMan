using lvl_0;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent (typeof(Controller2D))]
[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float m_movementSpeed;

    [SerializeField]
    private float m_movementDeadSpot;

    [SerializeField]
    private float m_accelerationTimeGrounded;

    [Header("Jumping")]
    [SerializeField]
    private float m_jumpHeight;

    [SerializeField]
    private float m_timeToJumpApex;

    [SerializeField]
    private float m_accelerationTimeAirborne;

    [Header("Climbing")]
    [SerializeField]
    private float m_climbingSpeed;

    private InputActions m_inputActions;
    private Vector2 m_moveInput;
    
    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;

    private bool m_isMoving;
    private bool m_isFacingLeft;
    private bool m_isJumping;

    //
    private Controller2D controller;

    private float m_gravity;
    private float m_jumpVelocity;
    private Vector3 m_velocity;
    private float m_velocityXSmoothing;


    private void Awake()
    {
        m_inputActions = new InputActions();

        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();

        m_isFacingLeft = false;
        m_isMoving = false;

        m_gravity = -(2 * m_jumpHeight) / (m_timeToJumpApex * m_timeToJumpApex);
        m_jumpVelocity = Mathf.Abs(m_gravity) * m_timeToJumpApex;
    }

    private void Update()
    {
        if (GameManager.Instance.GameIsRunning)
        {
            m_moveInput = m_inputActions.Game.Move.ReadValue<Vector2>();
            UpdateMovement();
            UpdateAnimator();
        }
    }

    private void OnEnable()
    {
        m_inputActions.Game.Enable();
        m_inputActions.Game.Jump.performed += OnJump;
        m_inputActions.Game.Pause.performed += OnPause;
        m_inputActions.Game.Escape.performed += OnEscape;
    }

    private void OnDisable()
    {
        m_inputActions.Game.Jump.performed -= OnJump;
        m_inputActions.Game.Disable();
    }

    private void UpdateMovement()
    {
        if (controller.collisions.below)
        {
            m_velocity.y = 0;
            controller.collisions.jumping = false;
        }

        float targetVelocityX = m_moveInput.x * m_movementSpeed;
        m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVelocityX, ref m_velocityXSmoothing, (controller.collisions.below ? m_accelerationTimeGrounded : m_accelerationTimeAirborne));
        m_velocity.y += m_gravity * Time.deltaTime;
        controller.Move(m_velocity * Time.deltaTime);
        
    }

    private void UpdateAnimator()
    {

        if (m_moveInput.x > m_movementDeadSpot && m_isFacingLeft)
        {
            m_spriteRenderer.flipX = false;
            m_isFacingLeft = false;
        }
        else if (m_moveInput.x < -m_movementDeadSpot && !m_isFacingLeft)
        {
            m_spriteRenderer.flipX = true;
            m_isFacingLeft = true;
        }

        if (controller.collisions.jumping && !m_isJumping)
        {
            m_animator.SetTrigger("Jump");
            m_isJumping = true;
        }
        else if (!controller.collisions.jumping && m_isJumping)
        {
            m_isJumping = false;
            if (Mathf.Abs(m_moveInput.x) > 0)
            {
                m_animator.SetTrigger("Run");
                m_isMoving = true;
            }
            else
            {
                m_animator.SetTrigger("Idle");
                m_isMoving = false;
            }
        }
        else
        {
            if (!m_isJumping)
            {
                if ((Mathf.Abs(m_moveInput.x) > 0) && !m_isMoving)
                {
                    m_animator.SetTrigger("Run");
                    m_isMoving = true;
                }
                else if (!(Mathf.Abs(m_moveInput.x) > 0f) && m_isMoving)
                {
                    m_animator.SetTrigger("Idle");
                    m_isMoving = false;
                }
            }
        }
    }

    private void OnJump(CallbackContext context)
    {
        if (controller.collisions.below && !controller.collisions.jumping)
        {
            m_velocity.y = m_jumpVelocity;
            controller.Move(m_velocity * Time.deltaTime);
            controller.collisions.jumping = true;
        }
    }

    private void OnPause(CallbackContext context)
    {
        GameManager.Instance.Pause();
    }

    private void OnEscape(CallbackContext context)
    {
        GameManager.Instance.Escape();
    }
}
