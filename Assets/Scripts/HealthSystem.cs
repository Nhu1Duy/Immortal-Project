using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHP = 100;
    public int currenHP;
    public event System.Action OnDeadth;
    void Start()
    {
        currenHP = maxHP;
    }
    void Update()
    {
        
    }
    public void TakeDamage(int amount)
    {
        currenHP -= amount;
        if(currenHP <= 0)
        {
            OnDeadth.Invoke();
            Die();
        }
    }
    public void Die()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
