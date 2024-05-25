using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenScript : MonoBehaviour
{
    public void TitleStart()
    {
        SceneManager.LoadScene("IntrocutsceneScene");
    }
    public void TempSceneSwitcher()
    {
        SceneManager.LoadScene("TheMines");
    }
}
