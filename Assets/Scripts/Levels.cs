using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
    public void OpenLevel(int levelNum)
    {
        string levelName = "Level" + levelNum;
        SceneManager.LoadScene(levelName);
    }
}
