
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGM,sfx;
    public AudioClip popClip,loseClip,clipSFX;
    public static AudioManager instance;
    
    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
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
    public void PlayLoseSFX()
    {
        sfx.PlayOneShot(loseClip);
    }
    public void SetSounds()
    {

    }
    public void SetMusic()
    {

    }
    public void PlayClick()
    {

    }

}
