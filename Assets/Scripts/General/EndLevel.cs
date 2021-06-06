using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI coins;
    [SerializeField] GameObject endScreen;
    [SerializeField] TextMeshProUGUI leaderboardTime;
    [SerializeField] GameObject leaderboard;
    Animation anim;

    private void Awake()
    {
        anim = endScreen.GetComponent<Animation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TimerUI.stopTimer = true;
            time.text = TimerUI.timeText;
            leaderboardTime.text = TimerUI.timeText;
            if (ItemTracker.count.ContainsKey(ItemType.COIN))
            {
                string countText = ItemTracker.count[ItemType.COIN].ToString();
                coins.text = ItemTracker.count[ItemType.COIN] < 10 ? "0" + countText : countText;
            }
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Movement>().enabled = false;
            PauseUI.disablePausing = true;
            Slot_Machine_Controller.disableSlot = true;
            anim.Play("EndScreenAppear");
        }
    }

    public void ShowLeaderboard()
    {
        leaderboard.SetActive(true);
        leaderboard.GetComponent<Leaderboard>().TallyScores();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        ItemTracker.count.Clear();
        leaderboard.GetComponent<Leaderboard>().newLevel = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        AudioManager audio = FindObjectOfType<AudioManager>();
        audio.StopMusic();
    }
}
