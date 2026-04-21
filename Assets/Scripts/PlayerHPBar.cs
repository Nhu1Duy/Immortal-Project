using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color colorFull = Color.green;
    [SerializeField] private Color colorMid = Color.yellow;
    [SerializeField] private Color colorLow = Color.red;

    private HealthSystem playerHealth;

    public void Start()
    {
        FindPlayer();
    }
    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p == null) return;
 
        playerHealth = p.GetComponent<HealthSystem>();
        if (playerHealth == null) return;
 
        playerHealth.OnHPChanged += UpdateBar;
 
        UpdateBar(playerHealth.CurrentHP, playerHealth.MaxHP);
    }
    private void OnDestroy()
    {
        if (playerHealth != null) playerHealth.OnHPChanged -= UpdateBar;
    }
    private void UpdateBar(int current, int max)
    {
        if (hpSlider == null) return;
 
        float pct = (float)current / max;
        hpSlider.value = pct;
        if (fillImage != null)
        {
            if (pct > 0.6f)
                fillImage.color = colorFull;
            else if (pct > 0.3f)
                fillImage.color = colorMid;
            else
                fillImage.color = colorLow;
        }
    }
}
