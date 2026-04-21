using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Canvas canvas;

    private HealthSystem health;
    private Camera mainCam;

    private void Awake()
    {
        health = GetComponentInParent<HealthSystem>();
        mainCam = Camera.main;

        if (canvas != null) canvas.worldCamera = mainCam;
    }
    private void Start()
    {
        if (health == null) return;

        health.OnHPChanged += OnHPChanged;

        if (hpSlider != null)
        {
            hpSlider.value = 1f;
            hpSlider.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if (health != null)
            health.OnHPChanged -= OnHPChanged;
    }
    private void LateUpdate()
    {
        if (mainCam == null) return;
        transform.rotation = mainCam.transform.rotation;
    }
    private void OnHPChanged(int current, int max)
    {
        if (hpSlider == null) return;

        float pct = (float)current / max;
        hpSlider.value = pct;

        hpSlider.gameObject.SetActive(pct < 1f && pct > 0f);
    }
}
