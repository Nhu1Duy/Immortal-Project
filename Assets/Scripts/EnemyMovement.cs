using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    private int facingDirection = 1; 
    private EnemyState enemyState;
    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (enemyState == EnemyState.Run && player != null)
        {
            if ((player.position.x > transform.position.x && facingDirection == -1) ||
                (player.position.x < transform.position.x && facingDirection == 1))
            {
                Flip();
            }

            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);
        }
    }

    void Flip()
    {
        facingDirection *= -1; 
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
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
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    void ChangeState(EnemyState newState)
    {
        enemyState = newState;

        if (enemyState == EnemyState.Run)
        {
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
        
        if (enemyState == EnemyState.Attack)
        {
            anim.SetTrigger("Attack");
        }
    }
}

public enum EnemyState
{
    Idle,
    Run,
    Attack
}