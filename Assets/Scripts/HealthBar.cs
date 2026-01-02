using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    public void SetHealth(float current, float max)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(current / max);
        }
    }
}
