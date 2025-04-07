using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    static AudioSource audioSource;
    static SoundData soundData;
    float volume = 0.2f;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
        soundData = GetComponent<SoundData>();
    }

    public static void Play(string clipname)
    {
        AudioClip clip = soundData.GetRandomClip(clipname);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
