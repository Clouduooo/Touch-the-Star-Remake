using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip[] BGM;
    [SerializeField] AudioClip[] effect;
    [SerializeField] AudioClip star;
    AudioSource audioSource;
    [SerializeField] GameObject canvas;
    [SerializeField] Image image;
    private float t;
    [SerializeField] float fadeDuration;    //change in inspector
    private bool changeScene;
    private float alpha;

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
            t = 0f;
            canvas.SetActive(false);
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Screen.SetResolution(1920, 1080, true, 60);
        audioSource.clip = BGM[0];
        audioSource.loop = true;
        audioSource.Play();
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            audioSource.loop = false;
        }
        if(changeScene)
        {
            changeScene = false;
            canvas.SetActive(true);
            image.color = new Color(0, 0, 0, 0);
            StartCoroutine(FadeIn());
            StartCoroutine(Change());
        }
    }

    public void ChangeScene()
    {
        changeScene = true;
    }

    //public void PlayEffect()
    //{
    //    int i = Random.Range(0, effect.Length);
    //    audioSource.PlayOneShot(effect[i]);
    //}

    //public void PlayStar()
    //{
    //    audioSource.PlayOneShot(star, 0.001f);
    //}

    IEnumerator FadeIn()
    {
        while (image.color.a < 1)
        {
            alpha += Time.deltaTime / fadeDuration;
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        alpha = 1f;
    }
    IEnumerator Change()
    {
        yield return new WaitForSeconds(fadeDuration);
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        audioSource.clip = BGM[SceneManager.GetActiveScene().buildIndex];
        audioSource.Play();
        while (image.color.a > 0)
        {
            alpha -= Time.deltaTime / fadeDuration; // fadeTime是你希望渐出效果持续的时间
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        image.color = new Color(0, 0, 0, 0);
        alpha = 0f;
    }
}
