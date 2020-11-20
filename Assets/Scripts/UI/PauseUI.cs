using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public Respawn playerRespawn;
    private Animator Pause_Anim;
    private bool CanPauseAgain = true;
    private int Paused = 1;
    public static bool disablePausing = false;

    private void Awake()
    {
        disablePausing = false;
        Pause_Anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (disablePausing) return;

        if (Movement.PlayerInput.PauseTriggered() && CanPauseAgain)
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
        //Invoke("EnablePausing",0.2f);
       // CanPauseAgain = newState == 1;
    }

    public void SetUnpaused(int newState)
    {
        Paused = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        ItemTracker.count.Clear();
        TimerUI.stopTimer = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartCheckpoint()
    {
        PauseGame(1);
        playerRespawn.respawnPlayer();
    }
}
