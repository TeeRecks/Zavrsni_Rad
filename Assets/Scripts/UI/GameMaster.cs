using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public AudioSource audioSource;

    private AudioClip spiderDeath;
    private AudioClip knightDeath;
    private AudioClip golemDeath;
    private AudioClip demonDeath;
    private AudioClip spiderHit;
    private AudioClip knightHit;
    private AudioClip genericHit;
    private AudioClip pickupSound;
    private AudioClip walkingSound;
    private AudioClip teleport;

    private void Start()
    {
        spiderDeath = Resources.Load<AudioClip>("Sounds/spider_death");
        knightDeath = Resources.Load<AudioClip>("Sounds/knight_death");
        golemDeath = Resources.Load<AudioClip>("Sounds/golem_death");
        demonDeath = Resources.Load<AudioClip>("Sounds/demon_death");
        spiderHit = Resources.Load<AudioClip>("Sounds/spider_hit");
        knightHit = Resources.Load<AudioClip>("Sounds/knight_hit");
        genericHit = Resources.Load<AudioClip>("Sounds/generic_hit");
        pickupSound = Resources.Load<AudioClip>("Sounds/pickup");
        walkingSound = Resources.Load<AudioClip>("Sounds/walking");
        teleport = Resources.Load<AudioClip>("Sounds/teleport");
    }

    public void NextMap()
    {
        System.Random nextMapRND = new System.Random(GameValues.mapSeed);
        GameValues.mapSeed = nextMapRND.Next(0, 99999999);
        GameValues.PrepareValuesForNextLevel();
        TeleportSound();

        Invoke("LoadScene", 2.0f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void SpiderDeath()
    {
        audioSource.PlayOneShot(spiderDeath);
    }
    public void KnightDeath()
    {
        audioSource.PlayOneShot(knightDeath);
    }
    public void GolemDeath()
    {
        audioSource.PlayOneShot(golemDeath);
    }
    public void DemonDeath()
    {
        audioSource.PlayOneShot(demonDeath);
    }
    public void SpiderHit()
    {
        audioSource.PlayOneShot(spiderHit);
    }
    public void KnightHit()
    {
        audioSource.PlayOneShot(knightHit);
    }
    public void GenericHit()
    {
        audioSource.PlayOneShot(genericHit);
    }
    public void PickupSound()
    {
        audioSource.PlayOneShot(pickupSound);
    }
    public void WalkingSound()
    {
        audioSource.PlayOneShot(walkingSound);
    }
    public void TeleportSound()
    {
        audioSource.PlayOneShot(teleport);
    }
}
