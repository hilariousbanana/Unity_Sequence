using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip clip;
}

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBGM;

    public string[] PlaySoundName;

    public Sound[] Effects;
    public Sound[] BGM;

    public void PlaySound(string _name)
    {
        for (int i = 0; i < Effects.Length; i++)
        {
            if(_name == Effects[i].Name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if(!audioSourceEffects[j].isPlaying)
                    {
                        PlaySoundName[j] = Effects[i].Name;
                        audioSourceEffects[j].clip = Effects[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }

                Debug.Log("All AudioSources Being Used.");
                return;
            }
        }
        Debug.Log(_name + " Can't Be Found.");
    }

    public void StopAllSound()
    {
        for(int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if(PlaySoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
            Debug.Log("Can't Find Audio Clip Named " + _name + ".");
        }
    }

}
