using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    //Back Ground Music (PLACEHOLDER)
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioClip BGMClip;

    //Player Footsteps Sound
    [SerializeField] private AudioSource PFootsteps; //NOTE: FIND HOW TO COMPLIE ARRAY
    [SerializeField] private AudioClip SFootstepsClips;

    //Player Gun sounds
    [SerializeField] private AudioClip SGunFire;
    [SerializeField] private AudioClip SGunCock;
    [SerializeField] private AudioClip SGunReload;
    [SerializeField] private AudioClip SGunChargeFire;
    [SerializeField] private AudioClip SGunChargeUp;

    //Player Dash Sounds
    [SerializeField] private AudioSource PDash; //NOTE; FIND HOW TO COMPLIE ARRAY
    [SerializeField] private AudioClip SDash;
    [SerializeField] private AudioClip SDashEmpty;
    [SerializeField] private AudioClip SDashReady;

    //Player Other Sounds

    [SerializeField] private AudioClip SHit;

    public void PlayBGM()
    {
        BGM.clip = BGMClip;
        BGM.loop = true;
        BGM.Play();
    }

    public void PlayPGunFire()
    {
        //set the random pitch
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        //play sound
        audioSource.PlayOneShot(SGunFire);
    }
    public void PlayPGunCock()
    {
        //reset pitch
        audioSource.pitch = 1f;
        //play sound
        audioSource.PlayOneShot(SGunCock);
    }
    public void PlayGunReload()
    {
        //reset pitch
        audioSource.pitch = 1f;
        //play sound
        audioSource.PlayOneShot(SGunReload);
    }
    public void PlayGunChargeUp()
    {
        //incease pitch
        audioSource.pitch += 0.15f;
        //play sound
        audioSource.PlayOneShot(SGunChargeUp);
    }
    public void PlayGunChargeFire()
    {
        //Randomise pitch
        audioSource.pitch = Random.Range(0.8f,1.2f);
        audioSource.PlayOneShot(SGunChargeFire);
    }
    public void PlayPDash()
    {
        //set ptich
        audioSource.pitch = Random.Range(0.9f, 1.2f);
        audioSource.PlayOneShot(SDash);
    }
    public void PlayDashEmpty()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(SDashEmpty);
    }
}
