using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] Transform entryContainer;
    [SerializeField] Transform entryTemplate;
    [SerializeField] Transform scoreTable;
    [SerializeField] Transform inputName;
    [SerializeField] GameObject inputWarning;

    public bool newLevel = true;
    int indexSaveTo = 0;
    List<HighscoreEntry> highscoreEntries;

    private void Awake()
    {
        entryTemplate.gameObject.SetActive(false);
        scoreTable.gameObject.SetActive(true);
        inputName.gameObject.SetActive(false);

        //highscoreEntries = new List<HighscoreEntry>()
        //{
        //    new HighscoreEntry{time=6000f, name="AAA"},
        //    new HighscoreEntry{time=2000f, name="AAA"},
        //    new HighscoreEntry{time=3000f, name="AAA"},
        //    new HighscoreEntry{time=5000f, name="AAA"},
        //    new HighscoreEntry{time=7000f, name="AAA"},
        //    new HighscoreEntry{time=1000f, name="AAA"},
        //    new HighscoreEntry{time=4000f, name="AAA"},
        //    new HighscoreEntry{time=8000f, name="AAA"},
        //    new HighscoreEntry{time=9000f, name="AAA"},
        //    new HighscoreEntry{time=10000f, name="AAA"}
        //};
    }

    public void TallyScores()
    {
        string prevScores = PlayerPrefs.GetString("localLeaderboard", "{\"entries\": []}");
        highscoreEntries = JsonUtility.FromJson<Highscores>(prevScores).entries;
        if (newLevel)
        {
            highscoreEntries.Add(new HighscoreEntry { time = TimerUI.timeFloat, name = "" });
            newLevel = false;
        }
        highscoreEntries = highscoreEntries.OrderBy(o => o.time).ToList();
        if (highscoreEntries.Count > 10)
        {
            int count = highscoreEntries.Count - 10;
            highscoreEntries.RemoveRange(10, count);
        }

        for (int i = 0; i < highscoreEntries.Count; i++)
        {
            if (highscoreEntries[i].name == "")
            {
                indexSaveTo = i;
                scoreTable.gameObject.SetActive(false);
                inputName.gameObject.SetActive(true);
                return;
            }
        }

        CreateHighscoreEntries(highscoreEntries);
    }

    public void SaveNewEntryName()
    {
        string name = inputName.Find("Name").GetComponent<TMP_InputField>().text;
        if (name == "")
        {
            inputWarning.SetActive(true);
            return;
        }

        highscoreEntries[indexSaveTo].name = name;
        inputWarning.SetActive(false);
        inputName.gameObject.SetActive(false);
        scoreTable.gameObject.SetActive(true);
        CreateHighscoreEntries(highscoreEntries);
    }

    void CreateHighscoreEntries(List<HighscoreEntry> entries)
    {
        float templateHeight = 25f;
        for (int i = 0; i < (entries.Count < 10 ? entries.Count : 10); i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);

            int rank = i + 1;
            entryTransform.Find("Position").GetComponent<TextMeshProUGUI>().text = rank.ToString();
            entryTransform.Find("Time").GetComponent<TextMeshProUGUI>().text = FormatTime(entries[i].time);
            entryTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = entries[i].name;
        }

        Highscores hs = new Highscores { entries = entries };
        string json = JsonUtility.ToJson(hs);
        PlayerPrefs.SetString("localLeaderboard", json);
        Debug.Log(json);
        PlayerPrefs.Save();
    }

    public string FormatTime(float time)
    {
        string minutes, seconds, milliseconds;
        minutes = Mathf.Floor(time / 60).ToString("0");
        seconds = Mathf.Floor(time % 60).ToString("00");
        milliseconds = (100 * time % 100).ToString("00");
        return minutes + ":" + seconds + "." + milliseconds;
    }

    private class Highscores
    {
        public List<HighscoreEntry> entries;
    }

    [System.Serializable]
    private class HighscoreEntry
    {
        public float time;
        public string name;
    }
}
