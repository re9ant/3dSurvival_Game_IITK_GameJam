using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] bool update = true;
    [SerializeField] GameObject TutorialMenu;

    [SerializeField] Slider mouseSlider;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject DeathScreen;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(mouseSlider != null)
        {
            mouseSlider.value = mouseLook.mouseSensitivity;
        }
    }

    private void Update()
    {
        if (!update)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !PlayerManager.instance.isDead)
        {
            ActivatePauseMenu();
        }
    }

    public void ActivateTutorial()
    {
        TutorialMenu.SetActive(true);
    }

    public void ActivatePauseMenu()
    {
        if (Time.timeScale == 0)
        {
            DeactivatePauseMenu();
            return;
        }
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
    }
    
    public void DeactivatePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
        PauseMenu.SetActive(false);
    }

    public void SetMouseSensitivity()
    {
        mouseLook.mouseSensitivity = mouseSlider.value;
    }

    public void ActivateDeathScreen()
    {
        DeathScreen.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    
    public void LoadGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
