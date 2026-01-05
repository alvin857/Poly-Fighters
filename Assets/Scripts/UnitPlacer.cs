using UnityEngine;
using System.Collections.Generic;

public class UnitPlacer : MonoBehaviour
{
    [Header("References")]
    public GridPlacementSystem gridSystem;
    public Camera mainCamera;
    public Team playerTeam = Team.TeamA;
    
    [Header("Preview")]
    public Material previewMaterial;
    
    private UnitData selectedUnit;
    private GameObject previewObject;
    private List<PlacedUnit> placedUnits = new List<PlacedUnit>();
    
    private struct PlacedUnit
    {
        public GameObject unitObject;
        public Vector2Int gridPosition;
        public int cost;
    }
    
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (selectedUnit != null)
        {
            UpdatePreview();
            
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceUnit();
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            TryRemoveUnit();
        }
    }
    
    public void SelectUnit(UnitData unitData)
    {
        selectedUnit = unitData;
        
        if (previewObject != null)
            Destroy(previewObject);
        
        if (selectedUnit != null && selectedUnit.unitPrefab != null)
        {
            previewObject = Instantiate(selectedUnit.unitPrefab);
            
            Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                Material[] mats = r.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = previewMaterial;
                }
                r.materials = mats;
            }
            
            UnitAI ai = previewObject.GetComponent<UnitAI>();
            if (ai != null)
                ai.enabled = false;
            
            Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }
        }
    }
    
    void UpdatePreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector2Int gridPos = gridSystem.GetGridPosition(hitPoint);
            
            if (gridPos.x >= 0)
            {
                Vector3 worldPos = gridSystem.GetWorldPosition(gridPos.x, gridPos.y);
                
                if (previewObject != null)
                {
                    previewObject.SetActive(true);
                    previewObject.transform.position = worldPos;
                    
                    bool canPlace = gridSystem.IsCellAvailable(gridPos.x, gridPos.y) &&
                                    CurrencyManager.Instance.CanAfford(selectedUnit.cost);
                    
                    Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
                    Color previewColor = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
                    
                    foreach (Renderer r in renderers)
                    {
                        foreach (Material mat in r.materials)
                        {
                            mat.color = previewColor;
                        }
                    }
                }
                
                gridSystem.SetHoveredCell(gridPos.x, gridPos.y);
            }
            else
            {
                if (previewObject != null)
                    previewObject.SetActive(false);
                
                gridSystem.SetHoveredCell(-1, -1);
            }
        }
    }
    
    void TryPlaceUnit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector2Int gridPos = gridSystem.GetGridPosition(hitPoint);
            
            if (gridPos.x >= 0 && gridSystem.IsCellAvailable(gridPos.x, gridPos.y))
            {
                if (CurrencyManager.Instance.SpendCurrency(selectedUnit.cost))
                {
                    Vector3 worldPos = gridSystem.GetWorldPosition(gridPos.x, gridPos.y);
                    GameObject unit = Instantiate(selectedUnit.unitPrefab, worldPos, Quaternion.identity);
                    
                    UnitAI ai = unit.GetComponent<UnitAI>();
                    if (ai != null)
                    {
                        ai.team = playerTeam;
                    }
                    
                    gridSystem.PlaceUnit(gridPos.x, gridPos.y);
                    
                    placedUnits.Add(new PlacedUnit
                    {
                        unitObject = unit,
                        gridPosition = gridPos,
                        cost = selectedUnit.cost
                    });
                }
            }
        }
    }
    
    void TryRemoveUnit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector2Int gridPos = gridSystem.GetGridPosition(hitPoint);
            
            if (gridPos.x >= 0)
            {
                for (int i = placedUnits.Count - 1; i >= 0; i--)
                {
                    if (placedUnits[i].gridPosition == gridPos)
                    {
                        CurrencyManager.Instance.AddCurrency(placedUnits[i].cost);
                        Destroy(placedUnits[i].unitObject);
                        gridSystem.RemoveUnit(gridPos.x, gridPos.y);
                        placedUnits.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
    
    public void ClearAllUnits()
    {
        foreach (PlacedUnit unit in placedUnits)
        {
            Destroy(unit.unitObject);
            gridSystem.RemoveUnit(unit.gridPosition.x, unit.gridPosition.y);
        }
        
        placedUnits.Clear();
    }
    
    public void DeselectUnit()
    {
        selectedUnit = null;
        
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
        
        gridSystem.SetHoveredCell(-1, -1);
    }
}