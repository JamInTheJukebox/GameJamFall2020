using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioMixer audioMixer;

    void Awake()
    {
        Slider volume = gameObject.GetComponent<Slider>();
        float val;
        audioMixer.GetFloat("volume", out val);
        volume.value = val;
    }

    public void setVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        AudioManager.audioMixer = audioMixer;
    }
}
