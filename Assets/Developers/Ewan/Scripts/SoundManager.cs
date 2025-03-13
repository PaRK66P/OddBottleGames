using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource BGM;
    private AudioClip BGMClip;

    private AudioSource PFootsteps;
    private AudioClip[] SFootstepsClips;

    private AudioSource PGunSFX;
    private AudioClip SGunFire;
    private AudioClip SGunCock;
    private AudioClip SGunReload;

    private AudioClip PDash;
    private AudioClip SDashClip;


    public void PlayPGunFire()
    {
        //set the random pitch
        PGunSFX.pitch = Random.Range(0.7f, 1.3f);
        //play sound
        PGunSFX.PlayOneShot(SGunFire);
    }
    public void PlayPGunCock()
    {
        //reset pitch
        PGunSFX.pitch = 1f;
        //play sound
        PGunSFX.PlayOneShot(SGunCock);
    }
    public void PlayGunReload()
    {
        //reset pitch
        PGunSFX.pitch = 1f;
        //play sound
        PGunSFX.PlayOneShot(SGunReload);
    }
}
