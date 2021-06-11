using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectMode : MonoBehaviour
{
    public void StartArcade()
    {
        SceneManager.LoadScene("Arcade");
    }

    public void StartTA()
    {
        SceneManager.LoadScene("TimeAttack");
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}
