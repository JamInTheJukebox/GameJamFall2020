using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    private Animator Pause_Anim;
    private bool CanPauseAgain = true;
    private int Paused = 1;

    private void Awake()
    {
        Pause_Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && CanPauseAgain)
        {
            CanPauseAgain = false;
            Paused = (Paused == 1) ? 0 : 1;
            PauseGame(Paused);

        }
    }

    public void EnablePausing()
    {
        CanPauseAgain = true;
    }

    public void PauseGame(int newState)         // 0 means you are paused, 1 means you are not paused.
    {
        Time.timeScale = newState;
        Pause_Anim.SetInteger("Paused", newState);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
