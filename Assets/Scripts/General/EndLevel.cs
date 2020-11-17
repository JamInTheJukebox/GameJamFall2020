using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndLevel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI coins;
    [SerializeField] Animation anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TimerUI.stopTimer = true;
            time.text = TimerUI.timeText;
            if (ItemTracker.count.ContainsKey(ItemType.COIN))
            {
                string countText = ItemTracker.count[ItemType.COIN].ToString();
                coins.text = ItemTracker.count[ItemType.COIN] < 10 ? "0" + countText : countText;
            }
            anim.Play("EndScreenAppear");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {

    }
}
