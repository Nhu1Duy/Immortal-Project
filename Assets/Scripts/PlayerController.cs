using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private GameObject attackHitbox;

    private Rigidbody2D rb;
    private Animator anim;
    private HealthSystem health;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private Vector2 lastMoveDir = Vector2.down;
    private float attackTimer;
    private bool isAttacking;
    private bool isDead;

    private static readonly int HashMoveX = Animator.StringToHash("MoveX");
    private static readonly int HashMoveY = Animator.StringToHash("MoveY");
    private static readonly int HashIsMoving = Animator.StringToHash("IsMoving");
    private static readonly int HashAttack = Animator.StringToHash("Attack");
    private static readonly int HashDie = Animator.StringToHash("Die");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<HealthSystem>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Attack.performed += OnAttackPerformed;

        if (health != null)
            health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Attack.performed -= OnAttackPerformed;

        inputActions.Disable();

        if (health != null)
            health.OnDeath -= HandleDeath;
    }
    private void OnMove(InputAction.CallbackContext ctx)
    {
        Debug.Log("OnMove CALLED: " + ctx.ReadValue<Vector2>());
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (isDead) return;

        if (moveInput.sqrMagnitude > 0.01f) lastMoveDir = moveInput.normalized;

        UpdateAnimator();
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (isDead || isAttacking) return;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimator()
    {
        bool moving = moveInput.sqrMagnitude > 0.01f;
        anim.SetFloat(HashMoveX, lastMoveDir.x);
        anim.SetFloat(HashMoveY, lastMoveDir.y);
        anim.SetBool(HashIsMoving, moving && !isAttacking);
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (isDead || attackTimer > 0f) return;

        isAttacking = true;
        attackTimer = attackCooldown;
        PositionHitbox();
        anim.SetTrigger(HashAttack);
    }

    private void PositionHitbox()
    {
        if (attackHitbox == null) return;
        attackHitbox.transform.localPosition = lastMoveDir.normalized * 0.7f;
    }

    public void EnableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }
    public void OnAttackEnd()
    {
        Debug.Log("Attack End CALLED");
        isAttacking = false;

        inputActions.Enable();
    }

    private void HandleDeath()
    {
        Debug.Log("HANDLE DEATH CALLED");
        if (isDead) return;
        isDead = true;

        anim.SetTrigger(HashDie);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        inputActions.Disable();
        Invoke(nameof(TriggerGameOver), 1.5f);
    }

    private void TriggerGameOver()
    {
        GameManager.Instance.GameOver();
    }
    public int AttackDamage => attackDamage;
}