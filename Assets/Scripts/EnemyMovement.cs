using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    public float attackRange = 2f;
    public int damage = 10;
    private int lastAttackLoop = -1;

    private int facingDirection = 1;
    private EnemyState enemyState;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    private float lastAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);

        switch (enemyState)
        {
            case EnemyState.Run:
                Chase(distance);
                break;
            case EnemyState.Attacking:
                HandleAttack(distance);
                break;
        }
    }

    void Chase(float distance)
    {
        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attacking);
            return;
        }

        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
            Flip();

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        anim.SetFloat("MoveX", direction.x);
        anim.SetFloat("MoveY", direction.y);
    }

    void HandleAttack(float distance)
    {
        rb.linearVelocity = Vector2.zero;

        if (distance > attackRange + 0.2f)
            ChangeState(EnemyState.Run);
    }



    public void PerformAttack()
    {
        if (player == null) return;
        if (Vector2.Distance(transform.position, player.position) > attackRange + 0.3f) return;

        int currentLoop = (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (currentLoop == lastAttackLoop) return;
        lastAttackLoop = currentLoop;

        player.GetComponent<HealthSystem>()?.TakeDamage(damage);
    }

    void Flip()
    {
        facingDirection *= -1;
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(-s.x, s.y, s.z);
    }

    void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;
        enemyState = newState;

        anim.SetBool("IsMoving", false);
        anim.SetBool("Attacking", false);

        switch (newState)
        {
            case EnemyState.Run:
                anim.SetBool("IsMoving", true);
                break;
            case EnemyState.Attacking:
                anim.SetBool("Attacking", true);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            ChangeState(EnemyState.Run);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("MoveX", 0);
            anim.SetFloat("MoveY", 0);
            ChangeState(EnemyState.Idle);
        }
    }
}

public enum EnemyState { Idle, Run, Attacking }