using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bottom : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
    public void Easy()
    {
        SceneManager.LoadScene("Practic_Scene");
    }
    public void Hard()
    {
        SceneManager.LoadScene("Track1_1");
    }
}
