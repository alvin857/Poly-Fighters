using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Starting Currency")]
    public int startingCurrency = 1000;

    private int currentCurrency;

    public event Action<int> OnCurrencyChanged;

    public event Action<int> OnCurrencySpent;

    public event Action<int> OnCurrencyGained;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentCurrency = startingCurrency;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }

    public int GetCurrency()
    {
        return currentCurrency;
    }

    public bool CanAfford(int cost)
    {
        return currentCurrency >= cost;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount <= 0)
            return false;

        if (!CanAfford(amount))
            return false;

        currentCurrency -= amount;

        OnCurrencyChanged?.Invoke(currentCurrency);
        OnCurrencySpent?.Invoke(amount);

        return true;
    }

    public void AddCurrency(int amount)
    {
        if (amount <= 0)
            return;

        currentCurrency += amount;

        OnCurrencyChanged?.Invoke(currentCurrency);
        OnCurrencyGained?.Invoke(amount);
    }

    public void ResetCurrency()
    {
        currentCurrency = startingCurrency;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }
}