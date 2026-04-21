using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState { Idle, Chase, Attack, Dead }

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Combat")]
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackCooldown = 1.2f;

    [Header("Patrol")]
    [SerializeField] private bool doPatrol = false;
    [SerializeField] private float patrolRange = 3f;
    [SerializeField] private float patrolSpeed = 1.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private HealthSystem health;

    private EnemyState state = EnemyState.Idle;
    private Transform player;
    private float attackTimer;
    private Vector2 patrolTarget;
    private Vector3 spawnPos;

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
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        spawnPos = transform.position;
        patrolTarget = (Vector2)spawnPos + Random.insideUnitCircle * patrolRange;

        if (health != null)
            health.OnDeath += HandleDeath;
        else
            Debug.LogWarning($"[EnemyAI] {gameObject.name}: HealthSystem không tìm thấy!", this);
        attackTimer = attackCooldown;
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    private Vector2 moveDir; 

    private void Update()
    {
        if (state == EnemyState.Dead) return;
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case EnemyState.Idle:
                anim.SetBool(HashIsMoving, false);
                moveDir = Vector2.zero;
                if (distToPlayer <= detectionRange)
                    state = EnemyState.Chase;
                else if (doPatrol)
                    Patrol();
                break;

            case EnemyState.Chase:
                if (distToPlayer <= attackRange)
                {
                    state = EnemyState.Attack;
                    moveDir = Vector2.zero;
                }
                else if (distToPlayer > detectionRange * 1.5f)
                {
                    state = EnemyState.Idle;
                    moveDir = Vector2.zero;
                }
                else
                    ChasePlayer();
                break;

            case EnemyState.Attack:
                moveDir = Vector2.zero;
                anim.SetBool(HashIsMoving, false);

                if (distToPlayer > attackRange)
                    state = EnemyState.Chase;
                else if (attackTimer <= 0f)
                    PerformAttack();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (state == EnemyState.Dead) return;
        if (moveDir.sqrMagnitude > 0.01f)
            rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
    }

    private void ChasePlayer()
    {
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        moveDir = dir * moveSpeed;

        anim.SetFloat(HashMoveX, dir.x);
        anim.SetFloat(HashMoveY, dir.y);
        anim.SetBool(HashIsMoving, true);
        FlipByDirection(dir.x);
    }

    private void Patrol()
    {
        Vector2 dir = (patrolTarget - (Vector2)transform.position).normalized;
        float dist = Vector2.Distance(transform.position, patrolTarget);

        if (dist < 0.2f)
        {
            patrolTarget = (Vector2)spawnPos + Random.insideUnitCircle * patrolRange;
            moveDir = Vector2.zero;
        }
        else
        {
            moveDir = dir * patrolSpeed;
            anim.SetFloat(HashMoveX, dir.x);
            anim.SetFloat(HashMoveY, dir.y);
            anim.SetBool(HashIsMoving, true);
            FlipByDirection(dir.x);
        }
    }

    private void FlipByDirection(float dirX)
    {
        if (dirX > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (dirX < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void PerformAttack()
    {
        attackTimer = attackCooldown;
        anim.SetTrigger(HashAttack);

        HealthSystem playerHP = player.GetComponent<HealthSystem>();
        if (playerHP != null && !playerHP.IsDead)
        {
            playerHP.TakeDamage(attackDamage);
        }
    }

    private void HandleDeath()
    {
        state = EnemyState.Dead;

        anim.SetTrigger(HashDie);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;

        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}