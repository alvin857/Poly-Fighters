using UnityEngine;
using TMPro;
using System.Collections;

public class GoldTextRedShift : MonoBehaviour
{
    [Header("References")]
    public TMP_Text goldText;

    [Header("Spend Flash")]
    public Color spendColor = new Color(1f, 0.4f, 0.4f);
    public float shiftDuration = 0.15f;

    private VertexGradient cachedGradient;

    void Awake()
    {
        cachedGradient = new VertexGradient(
            goldText.colorGradient.topLeft,
            goldText.colorGradient.topRight,
            goldText.colorGradient.bottomLeft,
            goldText.colorGradient.bottomRight
        );
    }

    void Start()
    {
        CurrencyManager.Instance.OnCurrencyChanged += UpdateText;
        CurrencyManager.Instance.OnCurrencySpent += OnSpend;

        UpdateText(CurrencyManager.Instance.GetCurrency());
    }

    void OnDestroy()
    {
        if (CurrencyManager.Instance == null) return;

        CurrencyManager.Instance.OnCurrencyChanged -= UpdateText;
        CurrencyManager.Instance.OnCurrencySpent -= OnSpend;
    }

    void UpdateText(int amount)
    {
        goldText.text = amount.ToString();
    }

    void OnSpend(int spentAmount)
    {
        StopAllCoroutines();
        StartCoroutine(RedShift());
    }

    IEnumerator RedShift()
    {
        goldText.enableVertexGradient = false;

        Color startColor = goldText.color;
        float t = 0f;

        while (t < shiftDuration)
        {
            t += Time.deltaTime;
            goldText.color = Color.Lerp(startColor, spendColor, t / shiftDuration);
            yield return null;
        }

        goldText.enableVertexGradient = true;
        goldText.colorGradient = cachedGradient;
        goldText.color = Color.white;
    }
}