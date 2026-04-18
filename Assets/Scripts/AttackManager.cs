using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public int damage = 20;
    private BoxCollider2D hitBox;
    void Awake()
    {
        hitBox = GetComponent<BoxCollider2D>();
        hitBox.enabled = false;
    }
    public void EnableHitbox()
    {
        hitBox.enabled = true;
    }
    public void DisableHitbox()
    {
        hitBox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HealthSystem enemyHealth = collision.GetComponent<HealthSystem>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
