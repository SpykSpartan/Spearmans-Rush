using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the rectangle Image that fills/depletes.")]
    public Image fillImage;

    [Tooltip("Assign the object that has the healthSystem script (e.g., Player).")]
    public healthSystem targetHealth;

    [Header("Optional Settings")]
    public bool smoothTransition = true;
    public float smoothSpeed = 5f;

    private RectTransform fillRect;
    private float initialWidth;
    private float targetWidth;

    private void Start()
    {
        if (fillImage == null)
        {
            Debug.LogError("HealthBar: No fill image assigned!");
            return;
        }

        fillRect = fillImage.GetComponent<RectTransform>();
        initialWidth = fillRect.sizeDelta.x;

        if (targetHealth == null)
        {
            Debug.LogError("HealthBar: No target health assigned!");
            return;
        }

        targetHealth.OnHealthChanged += OnHealthChanged;
        OnHealthChanged(targetHealthMax(), targetHealthMax());
    }

    private void OnDestroy()
    {
        if (targetHealth != null)
            targetHealth.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int current, int max)
    {
        float ratio = (float)current / max;
        targetWidth = initialWidth * ratio;
    }

    private void Update()
    {
        if (fillRect == null) return;

        Vector2 size = fillRect.sizeDelta;

        if (smoothTransition)
        {
            size.x = Mathf.Lerp(size.x, targetWidth, Time.deltaTime * smoothSpeed);
        }
        else
        {
            size.x = targetWidth;
        }

        fillRect.sizeDelta = size;
    }

    private int targetHealthMax()
    {
        return targetHealth != null ? targetHealth.maxHealth : 1;
    }
}