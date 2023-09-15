using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Star : MonoBehaviour
{
    [SerializeField] GameObject star;
    AudioManager audioManager;
    bool isPlayed;

    private void Awake()
    {
        isPlayed = false;
        star.SetActive(false);
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CircleLoop") && !isPlayed)
        {
            audioManager.PlayStar();
            star.SetActive(true);
            isPlayed = true;
        }

        if (star.activeSelf && collision.CompareTag("Player"))
        {
            audioManager.ChangeScene();
        }
    }
}
