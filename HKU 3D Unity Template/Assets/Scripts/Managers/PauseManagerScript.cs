using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManagerScript : MonoBehaviour
{
    public static PauseManagerScript Instance { get; private set; }

    [SerializeField] private AudioEventSO openPauseSFX;

    [Header("References")]
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void Pause()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;

        InputManager.Instance.PauseGame();

        AudioManagerScript.Instance.PlayOneShot(openPauseSFX, transform.position);

        pauseMenu.SetActive(true);
    }

    private void Resume()
    {
        if (!isPaused)
        {
            return;
        }

        isPaused = false;

        InputManager.Instance.ResumeGame();

        pauseMenu.SetActive(false);
    }
}
