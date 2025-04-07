using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundData : MonoBehaviour
{
    [SerializeField] SoundFXData[] soundFXData;
    Dictionary<string, List<AudioClip>> soundDictionary;

    void Start()
    {
        DictionaryInit();
    }

    void DictionaryInit()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundFXData soundFXData in soundFXData)
        {
            soundDictionary[soundFXData.name] = soundFXData.clip;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> clips = soundDictionary[name];
            if (clips.Count > 0)
            {
                return clips[UnityEngine.Random.Range(0, clips.Count)];
            }
        }
        return null;
    }
}

[System.Serializable]
public struct SoundFXData
{
    public string name;
    public List<AudioClip> clip;
}