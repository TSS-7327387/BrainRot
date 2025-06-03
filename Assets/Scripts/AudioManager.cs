
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGM,sfx,sfx2,clickSource,powersSFX;
    public AudioClip popClip,loseClip,clipSFX,bgm1,bgm2;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySFX2(AudioClip audio = null)
    {
        sfx2.Stop();
        if(audio == null)
            sfx2.PlayOneShot(popClip);
        else
            sfx2.PlayOneShot(audio);
    }
    public void PlaySFX(AudioClip clip)
    {
        sfx.Stop();
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
        sfx2.volume = val ? 1 : 0;
        clickSource.volume = val ? 1 : 0;
        powersSFX.volume = val ? 1 : 0;
    }
    public void SetMusic(bool val)
    {
        BGM.volume = val ? 1 : 0.45f;
    }
    public void PlayClick()
    {
        clickSource.Play();
    }
    public void ChangeBGM(bool mainMenu)
    {
        BGM.Stop();
        BGM.clip = mainMenu ? bgm1 : bgm2;
        DOVirtual.DelayedCall(1, ()=>{BGM.Play(); });
    }
    public void ShutBGM()
    {
        BGM.Stop();
    }

}
