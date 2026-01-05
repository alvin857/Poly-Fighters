using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    
    [Header("Starting Currency")]
    public int startingCurrency = 1000;
    
    private int currentCurrency;
    
    public event Action<int> OnCurrencyChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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
        if (!CanAfford(amount))
            return false;
        
        currentCurrency -= amount;
        OnCurrencyChanged?.Invoke(currentCurrency);
        return true;
    }
    
    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }
    
    public void ResetCurrency()
    {
        currentCurrency = startingCurrency;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }
}