using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip bgm0, bgm1, bgm2, bgm3, bgm4, bgm5;
    AudioSource audioSource;
    public static AudioManager Instance
    {
        get; private set;
    }
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            //if instance already exists, which means this script is another new AudioManager Script, delete it
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.clip = bgm0;
        audioSource.Play();
    }

    private void Update()
    {
        
    }
}
