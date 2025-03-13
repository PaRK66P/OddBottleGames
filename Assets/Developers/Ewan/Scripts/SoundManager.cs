using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Back Ground Music (PLACEHOLDER)
    private AudioSource BGM;
    private AudioClip BGMClip;
    //Player Footsteps Sound
    private AudioSource PFootsteps;
    private AudioClip[] SFootstepsClips;
    //Player Gun sounds
    private AudioSource PGunSFX;
    private AudioClip SGunFire;
    private AudioClip SGunCock;
    private AudioClip SGunReload;
    //Dash Sounds
    private AudioSource PDash;
    private AudioClip SDash;


    public void PlayBGM()
    {
        BGM.clip = BGMClip;
        BGM.loop = true;
        BGM.Play();
    }

    public void PlayPGunFire()
    {
        //set the random pitch
        PGunSFX.pitch = Random.Range(0.8f, 1.2f);
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
    public void PlayPDash()
    {
        //set ptich
        PDash.pitch = Random.Range(0.9f, 1.2f);
        PDash.PlayOneShot(SDash);
    }
}
