using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridDepth = 10;
    public float cellSize = 2f;
    public Vector3 gridOrigin = Vector3.zero;
    
    [Header("Visual")]
    public Material gridMaterial;
    public Color availableColor = new Color(0, 1, 0, 0.3f);
    public Color occupiedColor = new Color(1, 0, 0, 0.3f);
    public Color hoverColor = new Color(0, 0.5f, 1, 0.5f);
    
    private bool[,] occupiedCells;
    private GameObject[,] gridVisuals;
    private Vector2Int hoveredCell = new Vector2Int(-1, -1);
    
    void Start()
    {
        occupiedCells = new bool[gridWidth, gridDepth];
        gridVisuals = new GameObject[gridWidth, gridDepth];
        CreateGridVisuals();
    }
    
    void CreateGridVisuals()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.transform.parent = transform;
                cell.transform.position = GetWorldPosition(x, z);
                cell.transform.rotation = Quaternion.Euler(90, 0, 0);
                cell.transform.localScale = Vector3.one * cellSize * 0.95f;
                
                Renderer renderer = cell.GetComponent<Renderer>();
                renderer.material = new Material(gridMaterial);
                renderer.material.color = availableColor;
                
                Destroy(cell.GetComponent<Collider>());
                
                gridVisuals[x, z] = cell;
            }
        }
    }
    
    public Vector3 GetWorldPosition(int x, int z)
    {
        return gridOrigin + new Vector3(x * cellSize, -1, z * cellSize);
    }
    
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - gridOrigin;
        int x = Mathf.RoundToInt(localPos.x / cellSize);
        int z = Mathf.RoundToInt(localPos.z / cellSize);
        
        if (x >= 0 && x < gridWidth && z >= 0 && z < gridDepth)
            return new Vector2Int(x, z);
        
        return new Vector2Int(-1, -1);
    }
    
    public bool IsCellAvailable(int x, int z)
    {
        if (x < 0 || x >= gridWidth || z < 0 || z >= gridDepth)
            return false;
        
        return !occupiedCells[x, z];
    }
    
    public bool PlaceUnit(int x, int z)
    {
        if (!IsCellAvailable(x, z))
            return false;
        
        occupiedCells[x, z] = true;
        UpdateCellVisual(x, z);
        return true;
    }
    
    public void RemoveUnit(int x, int z)
    {
        if (x < 0 || x >= gridWidth || z < 0 || z >= gridDepth)
            return;
        
        occupiedCells[x, z] = false;
        UpdateCellVisual(x, z);
    }
    
    public void SetHoveredCell(int x, int z)
    {
        if (hoveredCell.x >= 0 && hoveredCell.x < gridWidth && 
            hoveredCell.y >= 0 && hoveredCell.y < gridDepth)
        {
            UpdateCellVisual(hoveredCell.x, hoveredCell.y);
        }
        
        hoveredCell = new Vector2Int(x, z);
        
        if (x >= 0 && x < gridWidth && z >= 0 && z < gridDepth)
        {
            Renderer renderer = gridVisuals[x, z].GetComponent<Renderer>();
            renderer.material.color = hoverColor;
        }
    }
    
    void UpdateCellVisual(int x, int z)
    {
        if (x < 0 || x >= gridWidth || z < 0 || z >= gridDepth)
            return;
        
        Renderer renderer = gridVisuals[x, z].GetComponent<Renderer>();
        renderer.material.color = occupiedCells[x, z] ? occupiedColor : availableColor;
    }
    
    public void ShowGrid()
    {
        foreach (GameObject cell in gridVisuals)
        {
            cell.SetActive(true);
        }
    }
    
    public void HideGrid()
    {
        foreach (GameObject cell in gridVisuals)
        {
            cell.SetActive(false);
        }
    }
}