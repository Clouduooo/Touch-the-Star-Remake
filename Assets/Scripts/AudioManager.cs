using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip[] BGM;
    [SerializeField] AudioClip[] effect;
    [SerializeField] AudioClip star;
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
        audioSource.clip = BGM[0];
        audioSource.loop = true;
        audioSource.Play();
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 5)
        {
            audioSource.loop = false;
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        audioSource.clip = BGM[SceneManager.GetActiveScene().buildIndex + 1];
        audioSource.Play();
    }

    public void PlayEffect()
    {
        //int i = Random.Range(0, effect.Length);
        //audioSource.PlayOneShot(effect[i]);
    }

    public void PlayStar()
    {
        audioSource.PlayOneShot(star);
    }
}
