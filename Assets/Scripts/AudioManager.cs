
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGM,sfx,clickSource,powersSFX;
    public AudioClip popClip,loseClip,clipSFX,bgm1,bgm2;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySFX()
    {
        sfx.PlayOneShot(popClip);
    }
    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }
    public void PlayPowerSFX(AudioClip clip)
    {
        powersSFX.PlayOneShot(clip);
    }
    public void PlayLoseSFX()
    {
        sfx.PlayOneShot(loseClip);
    }
    public void SetSounds(bool val)
    {
        sfx.volume = val ? 1 : 0;
        clickSource.volume = val ? 1 : 0;
        powersSFX.volume = val ? 1 : 0;
    }
    public void SetMusic(bool val)
    {
        BGM.volume = val ? 1 : 0;
    }
    public void PlayClick()
    {
        clickSource.Play();
    }
    public void ChangeBGM(bool mainMenu)
    {
        BGM.clip = mainMenu ? bgm1 : bgm2;
        DOVirtual.DelayedCall(0.5f, ()=>{BGM.Play(); });
    }

}
