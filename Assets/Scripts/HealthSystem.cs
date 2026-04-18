using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHP = 100;
    public int currenHP;
    public GameObject healthBarPrefab;
    public Vector3 barOffset = new Vector3(0, -0.7f, 0);
    private HealthBar healthBar;
    void Start()
    {
        currenHP = maxHP;
        if (healthBarPrefab != null)
        {
            var barObj = Instantiate(healthBarPrefab);
            healthBar = barObj.GetComponent<HealthBar>();
            healthBar.Setup(transform, barOffset);
            healthBar.UpdateBar(currenHP, maxHP);
        }
    }
    void Update()
    {

    }
    public void TakeDamage(int amount)
    {
        currenHP = Mathf.Max(0, currenHP - amount);
        healthBar?.UpdateBar(currenHP, maxHP);
        if (currenHP <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
