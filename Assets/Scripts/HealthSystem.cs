using System;
using NUnit.Framework;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 100;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    public event System.Action OnDeath;
    public event System.Action<int, int> OnHPChanged;

    private int currentHP;
    private bool isDead;
    private Color originalColor;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public bool IsDead => isDead;
    public float HPPercent => (float)currentHP / maxHP;

    public void Awake()
    {
        currentHP = maxHP;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0) return;
        currentHP = Math.Max(0, currentHP - amount);
        OnHPChanged?.Invoke(currentHP, maxHP);
        if (spriteRenderer != null) StartCoroutine(DamageFlash());
        if (currentHP == 0) Die();
    }
    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }
    public void SetMaxHP(int newMax, bool healToFull = false)
    {
        maxHP = Mathf.Max(1, newMax);
        currentHP = healToFull ? maxHP : Mathf.Min(currentHP, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
    }
    private System.Collections.IEnumerator DamageFlash() { 
        spriteRenderer.color = damageFlashColor; 
        yield return new WaitForSeconds(flashDuration); 
        if (spriteRenderer != null) spriteRenderer.color = originalColor; 
    }
}
