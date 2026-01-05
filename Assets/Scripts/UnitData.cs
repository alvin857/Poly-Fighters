using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "TABS/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    public string unitName = "Archer";
    public Sprite unitIcon;
    public int cost = 50;
    
    [Header("Prefab")]
    public GameObject unitPrefab;
    
    [Header("Stats Display")]
    [TextArea(2, 4)]
    public string description = "A ranged unit that attacks from a distance.";
}