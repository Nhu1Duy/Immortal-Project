using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image instantFill;
    public Image delayFill;

    public float smoothSpeed = 3f;

    private float targetFill = 1f;
    private Transform target;
    private Vector3 offset;

    public void Setup(Transform targetTransform, Vector3 positionOffset)
    {
        target = targetTransform;
        offset = positionOffset;
        targetFill = 1f;
        instantFill.fillAmount = 1f;
        delayFill.fillAmount = 1f;
    }

    public void UpdateBar(float current, float max)
    {
        targetFill = Mathf.Clamp01(current / max);
        instantFill.fillAmount = targetFill;
        instantFill.color = Color.Lerp(Color.red, Color.green, targetFill);
    }

    void Update()
    {
        if (delayFill.fillAmount > targetFill)
        {
            delayFill.fillAmount = Mathf.Lerp(
                delayFill.fillAmount,
                targetFill,
                Time.deltaTime * smoothSpeed
            );

            if (Mathf.Abs(delayFill.fillAmount - targetFill) < 0.001f)
                delayFill.fillAmount = targetFill;
        }
        else
        {
            delayFill.fillAmount = targetFill;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.identity;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}