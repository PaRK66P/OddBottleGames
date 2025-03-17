using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Back Ground Music (PLACEHOLDER)
    private AudioSource BGM;
    private AudioClip BGMClip;

    //Player Footsteps Sound
    private AudioSource PFootsteps; //NOTE: FIND HOW TO COMPLIE ARRAY
    private AudioClip SFootstepsClips;

    //Player Gun sounds
    private AudioSource PGunSFX;
    private AudioClip SGunFire;
    private AudioClip SGunCock;
    private AudioClip SGunReload;
    private AudioClip SGunChargeFire;
    private AudioClip SGunChargeUp;

    //Player Dash Sounds
    private AudioSource PDash; //NOTE; FIND HOW TO COMPLIE ARRAY
    private AudioSource PDashStatus;
    private AudioClip SDash;
    private AudioClip SDashEmpty;
    private AudioClip SDashReady;

    //Player Other Sounds
    private AudioSource PHit;
    private AudioClip SHit;

    public void InitialiseSFX()
    {
        //SGunCock.Load<AudioClip>("");
    }
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
    public void PlayGunChargeUp()
    {
        //incease pitch
        PGunSFX.pitch += 0.15f;
        //play sound
        PGunSFX.PlayOneShot(SGunChargeUp);
    }
    public void PlayGunChargeFire()
    {
        //Randomise pitch
        PGunSFX.pitch = Random.Range(0.8f,1.2f);
        PGunSFX.PlayOneShot(SGunChargeFire);
    }
    public void PlayPDash()
    {
        //set ptich
        PDash.pitch = Random.Range(0.9f, 1.2f);
        PDash.PlayOneShot(SDash);
    }
    public void PlayDashEmpty()
    {
        PDash.pitch = 1f;
        PDash.PlayOneShot(SDashEmpty);
    }
}
