using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip intro;
    public AudioClip loop;
    public static MusicPlayer mp;

    void Awake()
    {
        if (mp == null)
        {
            DontDestroyOnLoad(this);
            mp = this;
        }
        else if (mp != this)
            Destroy(this);
    }

    void Start()
    {
        StartCoroutine(PlayMusic());
    }

    IEnumerator PlayMusic()
    {
        source.clip = intro;
        source.loop = false; 
        source.Play();

        yield return new WaitForSeconds(intro.length);

        source.clip = loop;
        source.loop = true;
        source.Play();
    }
}
