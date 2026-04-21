using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private PlayerController owner;

    private void Awake()
    {
        owner = GetComponentInParent<PlayerController>();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        HealthSystem enemyHP = other.GetComponent<HealthSystem>();
        if (enemyHP == null)
            enemyHP = other.GetComponentInParent<HealthSystem>();

        if (enemyHP != null && !enemyHP.IsDead)
        {
            int dmg = owner != null ? owner.AttackDamage : 25;
            enemyHP.TakeDamage(dmg);
            Rigidbody2D enemyRb = other.GetComponentInParent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockDir = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockDir * 3f, ForceMode2D.Impulse);
            }
        }
    }
}