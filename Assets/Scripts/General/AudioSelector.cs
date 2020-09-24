using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSelector : MonoBehaviour
{
    public AudioClip AwakeClip;

    private void Start()
    {
        AudioManager.Instance.PlayMusicwithFade(AwakeClip);
    }
}
