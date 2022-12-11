
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource soundFxSource;
    
    public AudioClip ShootingClip;

    public AudioClip ReloadingClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void PlayReloaded()
    {
        soundFxSource.clip = ReloadingClip;
        soundFxSource.Play();
    }

    public void PlayShoot()
    {
        soundFxSource.clip = ShootingClip;
        soundFxSource.Play();
    }
}
