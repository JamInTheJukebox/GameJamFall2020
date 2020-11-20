using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSelector : MonoBehaviour
{
    public AudioClip AwakeClip;
    public AudioClip Clip2;

    public float loopTime = 2f;
    int iterator = 0;

    private void Start()
    {
        AudioManager.Instance.PlayMusicwithFade(AwakeClip);
        StartCoroutine(LoopMusic());
    }

    private IEnumerator LoopMusic()
    {
        while (true)
        {
            AudioClip nextClip = (iterator == 0) ? (Clip2) : AwakeClip;
            iterator = (iterator == 0) ? 1 : 0;
            yield return new WaitForSeconds(loopTime);
            AudioManager.Instance.PlayMusicwithCrossFade(nextClip,1);
        }
    }
}
