using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    private float volume;
    [Header("Audio Sources")]
    //Audio Sources
    [SerializeField] private AudioSource PlayerAudioSource;
    [SerializeField] private AudioSource BGMAudioSource;
    [SerializeField] private AudioSource EnemyAudioSource;
    [SerializeField] private AudioSource AmbrosiaAudioSource;
    [SerializeField] private AudioSource IchorAudioSource;

    [Header("Music")]
    //Back Ground Music (PLACEHOLDER)
    [SerializeField] private AudioClip BGMClip;

    [Header("Player Footsteps")]
    //Player Footsteps Sound
    [SerializeField] private AudioClip[] PlyrFootstepsClips;
    bool isWalking = false;
    private float footstepLength;

    [Header("Player Gun")]
    //Player Gun sounds
    [SerializeField] private AudioClip PlyrGunFire;
    [SerializeField] private AudioClip PlyrGunCock;
    [SerializeField] private AudioClip PlyrGunReload;
    [SerializeField] private AudioClip PlyrGunChargeFire;
    [SerializeField] private AudioClip PlyrGunChargeUp;

    [Header("Player Dash")]
    //Player Dash Sounds
    [SerializeField] private AudioClip[] PlyrDashClips;
    [SerializeField] private AudioClip PlyrDashEmpty;
    [SerializeField] private AudioClip PlyrDashReady;
    private bool isDashing = false;
    private float dashLength;

    [Header("Player Player Hit")]
    //Player Other Sounds
    [SerializeField] private AudioClip PlyrHit;

    [Header("Enemy Sounds")]
    //small enemy sounds
    [SerializeField] private AudioClip EnHit;
    [SerializeField] private AudioClip EnDead;
    [SerializeField] private AudioClip EnShoot;

    [Header("Ambrosia Sounds")]
    //Ambrosia Sounds
    [SerializeField] private AudioClip[] AmbFootstepsClips;
    [SerializeField] private AudioClip[] AmbDashReady;
    [SerializeField] private AudioClip[] AmbDashAttack;
    [SerializeField] private AudioClip AmbSpitPrep;
    [SerializeField] private AudioClip AmbSpitAttack;
    [SerializeField] private AudioClip AmbLickPrep;
    [SerializeField] private AudioClip AmbLickAttack;
    [SerializeField] private AudioClip AmbScreamAttack;
    [SerializeField] private AudioClip AmbHit;
    [SerializeField] private AudioClip AmbDown;
    [SerializeField] private AudioClip AmbConsume;
    private bool isWalkingAmb = false;
    private float footstepLengthAmb;

    [Header("Ichor Sounds")]
    //Ichor Sounds
    [SerializeField] private AudioClip IchorStndAttack;
    [SerializeField] private AudioClip IchorBlastAttack;
    [SerializeField] private AudioClip IchorScreenAttack;
    [SerializeField] private AudioClip IchorTumorSpawn;
    [SerializeField] private AudioClip IchorTumorDestroy;
    [SerializeField] private AudioClip IchorHit;
    [SerializeField] private AudioClip IchorImmune;
    [SerializeField] private AudioClip IchorKnocked;
    [SerializeField] private AudioClip IchorCrit;
    [SerializeField] private AudioClip IchorDown;

    public void Update()
    {
        //update volume
        volume = PlayerPrefs.GetFloat("volume", volume);
        PlayerAudioSource.volume = volume;

        //BGMAudioSource.volume = volume;
        //EnemyAudioSource.volume = volume;
        //IchorAudioSource.volume = volume;

        //Player footsteps
        footstepLength -= Time.deltaTime;
        PlayFootstep();
        if (footstepLength < 0f) footstepLength = 0f;
        //Player Dash
        dashLength -= Time.deltaTime;
        PlayPDash();
        if (dashLength < 0f) dashLength = 0f;

        if (AmbrosiaAudioSource != null)
        {
            AmbrosiaAudioSource.volume = volume;

            //Ambrosia Footsteps
            footstepLengthAmb -= Time.deltaTime;
            PlayAmbFootsteps();
            if (footstepLengthAmb < 0f) footstepLengthAmb = 0f;
        }
    }
    public void PlayBGM()
    {
        BGMAudioSource.clip = BGMClip;
        BGMAudioSource.loop = true;
        BGMAudioSource.Play();
    }

    public void PlayFootstep()
    {
        if (isWalking && footstepLength < 0f) 
        {         
        //set ptich
        PlayerAudioSource.pitch = Random.Range(0.9f, 1.2f);
        //pick random from array
        int index = Random.Range(0, PlyrFootstepsClips.Length);
        AudioClip footstepClip = PlyrFootstepsClips[index];
        footstepLength = footstepClip.length+0.3f;
        //play sound
        PlayerAudioSource.PlayOneShot(footstepClip);
        }
    }
    public void SetWalking(bool value)
    {
        isWalking = value;
    }
    public void PlayPGunFire()
    {
        //set the random pitch
        PlayerAudioSource.pitch = Random.Range(0.8f, 1.2f);
        //play sound
        PlayerAudioSource.PlayOneShot(PlyrGunFire);
    }
    public void PlayPGunCock()
    {
        //reset pitch
        PlayerAudioSource.pitch = 1f;
        //play sound
        PlayerAudioSource.PlayOneShot(PlyrGunCock);
    }
    public void PlayGunReload()
    {
        //reset pitch
        PlayerAudioSource.pitch = 1f;
        //play sound
        PlayerAudioSource.PlayOneShot(PlyrGunReload);
    }   
    public void PlayGunChargeUp()
    {
        //incease pitch
        PlayerAudioSource.pitch += 0.15f;
        //play sound
        PlayerAudioSource.PlayOneShot(PlyrGunChargeUp);
    }
    public void PlayGunChargeFire()
    {
        //Randomise pitch
        PlayerAudioSource.pitch = Random.Range(0.8f,1.2f);
        PlayerAudioSource.PlayOneShot(PlyrGunChargeFire);
    }
    public void PlayPDash()
    {
        if (isDashing && dashLength > 0)
        {
            //set ptich
            PlayerAudioSource.pitch = Random.Range(0.9f, 1.2f);
            //pick random from array
            int index = Random.Range(0, PlyrDashClips.Length);
            AudioClip dashClip = PlyrDashClips[index];
            dashLength = dashClip.length + 0.3f;
            //play sound
            PlayerAudioSource.PlayOneShot(dashClip);
        }
    }
    public void SetDashing(bool value)
    {
        isDashing = value;
    }
    public void PlayDashEmpty()
    {
        PlayerAudioSource.pitch = 1f;
        PlayerAudioSource.PlayOneShot(PlyrDashEmpty);
    }
    public void PlayPDashReady()
    {
        PlayerAudioSource.pitch = 1f;
        PlayerAudioSource.PlayOneShot(PlyrDashReady);
    }

    public void PlayEnemyShoot()
    {
        EnemyAudioSource.pitch = Random.Range(0.5f, 1.5f);
        EnemyAudioSource.PlayOneShot(EnShoot);
    }
    public void PlayEnemyHit()
    {
        //randomise pitch
        EnemyAudioSource.pitch = Random.Range(0.5f, 1.5f);
        EnemyAudioSource.PlayOneShot(EnHit);
    }
    public void PlayEnemyDead()
    {
        EnemyAudioSource.pitch = Random.Range(0.5f, 1.5f);
        EnemyAudioSource.PlayOneShot(EnDead);
    }

    public void PlayAmbFootsteps()
    {
        if (isWalkingAmb && footstepLengthAmb < 0f)
        {
            //set ptich
            AmbrosiaAudioSource.pitch = Random.Range(0.9f, 1.2f);
            //pick random from array
            int index = Random.Range(0, AmbFootstepsClips.Length);
            AudioClip footstepClip = AmbFootstepsClips[index];
            footstepLengthAmb = footstepClip.length + 0.2f;
            //play sound
            AmbrosiaAudioSource.PlayOneShot(footstepClip);
        }
    }
    public void SetWalkingAmb(bool value)
    {
        isWalkingAmb = value;
    }
    public void PlayAmbDashReady(int state)
    {
        //Randomise Pitch
        AmbrosiaAudioSource.pitch = Random.Range(0.8f, 1.2f);
        AmbrosiaAudioSource.PlayOneShot(AmbDashReady[state - 1]);
    }
    public void PlayAmbDashAttack(int state)
    {
        //Randomise Pitch
        AmbrosiaAudioSource.pitch = Random.Range(0.8f, 1.2f);
        AmbrosiaAudioSource.PlayOneShot(AmbDashAttack[state - 1]);
    }
    public void PlayAmbLickPrep()
    {
        //Randomise Pitch
        AmbrosiaAudioSource.pitch = Random.Range(0.8f, 1.2f);
        AmbrosiaAudioSource.PlayOneShot(AmbLickPrep);
    }
    public void PlayAmbLickAttack()
    {
        //Randomise Pitch
        AmbrosiaAudioSource.pitch = Random.Range(0.8f, 1.2f);
        AmbrosiaAudioSource.PlayOneShot(AmbLickAttack);
    }
    public void PlayAmbScreamAttack()
    {
        //Randomise Pitch
        AmbrosiaAudioSource.pitch = Random.Range(0.8f, 1.2f);
        AmbrosiaAudioSource.PlayOneShot(AmbScreamAttack);
    }
    public void PlayAmbHit()
    {
        //Randomise Pitch
        AmbrosiaAudioSource.pitch = Random.Range(0.6f, 1.4f);
        AmbrosiaAudioSource.PlayOneShot(AmbHit);
    }
    public void PlayAmbDown()
    {
        //Reset Pitch
        AmbrosiaAudioSource.pitch = 1f;
        AmbrosiaAudioSource.PlayOneShot(AmbDown);
    }
    public void PlayAmbConsume()
    {
        AmbrosiaAudioSource.PlayOneShot(AmbConsume);
    }

    public void PlayIchorStandardAttack()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.8f, 1.2f);
        IchorAudioSource.PlayOneShot(IchorStndAttack);
    }
    public void PlayIchorBlastAttack()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.9f, 1.1f);
        IchorAudioSource.PlayOneShot(IchorBlastAttack);
    }
    public void PlayIchorScreenAttack()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.8f, 1.1f);
        IchorAudioSource.PlayOneShot(IchorBlastAttack);
    }
    public void PlayIchorTumorSpawn()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.7f, 1.3f);
        IchorAudioSource.PlayOneShot(IchorTumorSpawn);
    }
    public void PlayIchorTumorDestroy()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.8f, 1.4f);
        IchorAudioSource.PlayOneShot(IchorTumorDestroy);
    }
    public void PlayIchorHit()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.6f, 1.4f);
        IchorAudioSource.PlayOneShot(IchorHit);
    }
    public void PlayIchorImmune()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.9f, 1.1f);
        IchorAudioSource.PlayOneShot(IchorImmune);
    }
    public void PlayIchorKnocked()
    {
        //Reset Pitch
        IchorAudioSource.pitch = 1f;
        IchorAudioSource.PlayOneShot(IchorKnocked);
    }
    public void PlayIchorCrit()
    {
        //Randomise Pitch
        IchorAudioSource.pitch = Random.Range(0.9f, 1.1f);
        IchorAudioSource.PlayOneShot(IchorCrit);
    }
    public void PlayIchorDown()
    {
        //Reset Pitch
        IchorAudioSource.pitch = 1f;
        IchorAudioSource.PlayOneShot(IchorDown);
    }
}
