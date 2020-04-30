using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundMgr : MonoBehaviour
{
    public static soundMgr instance;
    AudioSource audioSource;
    AudioSource nowPlaying;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play(string sound) {
        transform.Find(sound).GetComponent<AudioSource>().Play();
    }
    public void PlayOneShot(string sound)
    {
        audioSource.PlayOneShot(transform.Find(sound).GetComponent<AudioSource>().clip);
    }
    public void PlayLoop(string sound)
    {
       if (nowPlaying != null) {
            nowPlaying.Stop();
        }
        nowPlaying =  transform.Find(sound).GetComponent<AudioSource>();
        nowPlaying.Play();
    }
    public void LoopStop() {
        if (nowPlaying != null)
        {
            nowPlaying.Stop();
        }
    }
}
