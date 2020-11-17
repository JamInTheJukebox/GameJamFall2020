﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI coins;
    [SerializeField] GameObject endScreen;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}