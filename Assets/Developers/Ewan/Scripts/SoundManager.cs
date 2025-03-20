using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private AudioSource pAudioSource;
    //Back Ground Music (PLACEHOLDER)
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioClip BGMClip;

    //Player Footsteps Sound
    [SerializeField] private AudioClip[] SFootsteps;
    bool isWalking = false;
    private float footstepLength;

    //Player Gun sounds
    [SerializeField] private AudioClip SGunFire;
    [SerializeField] private AudioClip SGunCock;
    [SerializeField] private AudioClip SGunReload;
    [SerializeField] private AudioClip SGunChargeFire;
    [SerializeField] private AudioClip SGunChargeUp;

    //Player Dash Sounds
    [SerializeField] private AudioClip[] SDashes;
    [SerializeField] private AudioClip SDashEmpty;
    [SerializeField] private AudioClip SDashReady;

    //Player Other Sounds
    [SerializeField] private AudioClip SHit;

    //small enemy sounds
    [SerializeField] private AudioSource eAudioSource;
    [SerializeField] private AudioClip SEnHit;

    public void Update()
    {
        footstepLength -= Time.deltaTime;
    }
    public void PlayBGM()
    {
        BGM.clip = BGMClip;
        BGM.loop = true;
        BGM.Play();
    }

    public void PlayFootstep()
    {
        if (isWalking && footstepLength > 0f) 
        {         
        //set ptich
        pAudioSource.pitch = Random.Range(0.9f, 1.2f);
        //pick random from array
        int index = Random.Range(0, SFootsteps.Length);
        AudioClip footstepClip = SFootsteps[index];
        footstepLength = footstepClip.length;
        //play sound
        pAudioSource.PlayOneShot(footstepClip);
        }
    }
    public void SetWalking(bool value)
    {
        isWalking = value;
    }
    public void PlayPGunFire()
    {
        //set the random pitch
        pAudioSource.pitch = Random.Range(0.8f, 1.2f);
        //play sound
        pAudioSource.PlayOneShot(SGunFire);
    }
    public void PlayPGunCock()
    {
        //reset pitch
        pAudioSource.pitch = 1f;
        //play sound
        pAudioSource.PlayOneShot(SGunCock);
    }
    public void PlayGunReload()
    {
        //reset pitch
        pAudioSource.pitch = 1f;
        //play sound
        pAudioSource.PlayOneShot(SGunReload);
    }   
    public void PlayGunChargeUp()
    {
        //incease pitch
        pAudioSource.pitch += 0.15f;
        //play sound
        pAudioSource.PlayOneShot(SGunChargeUp);
    }
    public void PlayGunChargeFire()
    {
        //Randomise pitch
        pAudioSource.pitch = Random.Range(0.8f,1.2f);
        pAudioSource.PlayOneShot(SGunChargeFire);
    }
    public void PlayPDash()
    {
        //set ptich
        pAudioSource.pitch = Random.Range(0.9f, 1.2f);
        //pick random from array
        int index = Random.Range(0, SDashes.Length);
        AudioClip dashClip = SDashes[index];
        //play sound
        pAudioSource.PlayOneShot(dashClip);
    }
    public void PlayDashEmpty()
    {
        pAudioSource.pitch = 1f;
        pAudioSource.PlayOneShot(SDashEmpty);
    }
    public void PlayEnemyHit()
    {
        //randomise pitch
        eAudioSource.pitch = Random.Range(0.5f, 1.5f);
        eAudioSource.PlayOneShot(SEnHit);
    }
}
