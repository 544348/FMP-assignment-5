using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    //variables
    private GameObject menu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField]private GameObject controlsMenu;
    void Start()
    {
        menu = GameObject.Find("Menu");
        menu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }
    public void hideMenu()
    {
        menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void QuitGame()
    {
        Debug.Log("ExitingGame");
        Application.Quit();
    }
    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        menu.SetActive(false);
    }
    public void SettingsBack()
    {
        settingsMenu.SetActive(false);
        menu.SetActive(true);
    }
    public void OpenControls()
    {
        controlsMenu.SetActive(true);
        menu.SetActive(false);
    }
    public void ControlsBack()
    {
        controlsMenu.SetActive(false);
        menu.SetActive(true);
    }
    public void TempSceneSwitcher()
    {
        SceneManager.LoadScene("TheMines");
    }
    public void TitleStart()
    {
        SceneManager.LoadScene("IntrocutsceneScene");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf|| settingsMenu.activeSelf|| controlsMenu.activeSelf)
            {
                menu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                settingsMenu.SetActive(false);
                controlsMenu.SetActive(false);
            }
            else
            {
                menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}