using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public int damage = 20;
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
    }
}
