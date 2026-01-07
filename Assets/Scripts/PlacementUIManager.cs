using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlacementUIManager : MonoBehaviour
{
    [Header("References")]
    public UnitPlacer unitPlacer;
    public CurrencyManager currencyManager;
    
    [Header("UI Elements")]
    public TextMeshProUGUI currencyText;
    public Transform unitButtonContainer;
    public GameObject unitButtonPrefab;
    
    [Header("Unit List")]
    public UnitData[] availableUnits;
    
    void Start()
    {
        CreateUnitButtons();
        
        if (currencyManager != null)
        {
            currencyManager.OnCurrencyChanged += UpdateCurrencyDisplay;
            UpdateCurrencyDisplay(currencyManager.GetCurrency());
        }
    }
    
    void CreateUnitButtons()
    {
        foreach (UnitData unit in availableUnits)
        {
            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnUnitButtonClicked(unit));
            
            Image icon = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null && unit.unitIcon != null)
            {
                icon.sprite = unit.unitIcon;
            }
            
            TextMeshProUGUI nameText = buttonObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = unit.unitName;
            }
            
            TextMeshProUGUI costText = buttonObj.transform.Find("Cost")?.GetComponent<TextMeshProUGUI>();
            if (costText != null)
            {
                costText.text = unit.cost.ToString();
            }
        }
    }
    
    void OnUnitButtonClicked(UnitData unit)
    {
        unitPlacer.SelectUnit(unit);
    }
    
    void UpdateCurrencyDisplay(int amount)
    {
        if (currencyText != null)
        {
            currencyText.text = $"Gold: {amount}";
        }
    }
    
    public void OnClearAllPressed()
    {
        unitPlacer.ClearAllUnits();
        currencyManager.ResetCurrency();
    }
    
    public void OnDeselectPressed()
    {
        unitPlacer.DeselectUnit();
    }
    
    void OnDestroy()
    {
        if (currencyManager != null)
        {
            currencyManager.OnCurrencyChanged -= UpdateCurrencyDisplay;
        }
    }
}
