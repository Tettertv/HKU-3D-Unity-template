using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    //private Player playerInput;

    private bool wasTalking = false;

    private int gameplayBlockers = 0;

    public bool GameplayBlocked => gameplayBlockers > 0;

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

        //playerInput = new Player();

        SubscribeInputs();

        //playerInput.DefaultMovement.Enable();
        //playerInput.System.Enable();
    }

    private void SubscribeInputs()
    {
        //Subribe to sum shit here okey?
    }
    public void BlockGameplay()
    {
        gameplayBlockers++;

        if (gameplayBlockers == 1)
        {
            DisableGameplay();
        }
    }

    public void UnblockGameplay()
    {
        gameplayBlockers = Mathf.Max(0, gameplayBlockers - 1);

        if (gameplayBlockers == 0)
        {
            EnableGameplay();
        }
    }

    private void EnableGameplay()
    {
        //playerInput.DefaultMovement.Enable();
    }

    private void DisableGameplay()
    {
        //playerInput.DefaultMovement.Disable();
    }

    public void PauseGame()
    {
        BlockGameplay();

        /*if (playerInput.Dialogue.enabled)
        {
            //playerInput.Dialogue.Disable();
            wasTalking = true;
        }*/
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (wasTalking)
        {
            wasTalking = false;
            //playerInput.Dialogue.Enable();
        }

        Time.timeScale = 1f;
        UnblockGameplay();
    }
}
