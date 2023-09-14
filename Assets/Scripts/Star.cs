using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Star : MonoBehaviour
{
    [SerializeField] GameObject star;
    AudioManager audioManager;

    private void Awake()
    {
        star.SetActive(false);
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CircleLoop"))
        {
            star.SetActive(true);
        }

        if (star.activeSelf && collision.CompareTag("Player"))
        {
            audioManager.ChangeScene();
        }
    }
}
