using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject victoryUI;

    private bool triggered;

    void Update()
    {
        if (triggered) return;
        UnitAI[] allUnits = FindObjectsOfType<UnitAI>();
        int teamBCount = 0;
        foreach (UnitAI unit in allUnits)
        {
            if (unit.team == Team.TeamB)
            {
                teamBCount++;
            }
        }

        if (teamBCount == 0)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        triggered = true;

        if (victoryUI != null)
            victoryUI.SetActive(true);

        Time.timeScale = 0f;
    }
}