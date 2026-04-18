using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public int damage = 20;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
        }
    }
}
