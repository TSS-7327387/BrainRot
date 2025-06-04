
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settings;
    public Image soundImg, musicImg;
    public Sprite soundSpriteOn,soundSpriteOff, musicSpriteOn, musicSpriteOff;
    bool musicEnabled = false, soundEnabled = false;
    public AudioClip playClip,springClip,prankClip;
    // Start is called before the first frame update
    void OnEnable()
    {
        DOVirtual.DelayedCall(1, () =>
        {
            TssAdsManager._Instance.ShowBanner("MainMenu");
            TssAdsManager._Instance.admobInstance.TopShowBanner();
        });
        musicEnabled = PlayerPrefs.GetInt("Music", 1) == 1;
        soundEnabled = PlayerPrefs.GetInt("Sounds", 1) == 1;
        SetImagesSprite();
        AudioManager.instance.ChangeBGM(true);
    }
    void SetImagesSprite()
    {
        soundImg.sprite = soundEnabled? soundSpriteOn:soundSpriteOff;
        musicImg.sprite = musicEnabled? musicSpriteOn:musicSpriteOff;
    }
    public void PlaySpringSound()
    {
        AudioManager.instance.PlaySFX(springClip);
    }
    public void OnClickSounds()
    {
        soundEnabled =! soundEnabled;
        PlayerPrefs.SetInt("Sounds", soundEnabled? 1: 0);
        AudioManager.instance.SetSounds(soundEnabled);
        SetImagesSprite();
    }
    public void OnClickMusic()
    {
        musicEnabled =! musicEnabled;
        PlayerPrefs.SetInt("Music", musicEnabled ? 1: 0);
        AudioManager.instance.SetMusic(musicEnabled);
        SetImagesSprite();
    }
    public void OnClickPP()
    {
        AudioManager.instance.PlayClick();
        Application.OpenURL(GlobalConstant.PrivacyPoliciesLInk);
    }
    public void OnClickPlay()
    {
        AudioManager.instance.PlaySFX(playClip);
        CanvasScriptSplash.instance.LoadScene(2);
        AudioManager.instance.ChangeBGM(false);
        DOVirtual.DelayedCall(1, () => { TssAdsManager._Instance.ShowInterstitial("FromMainToGameplay"); });
    }
    public void OnClickPlayPranks()
    {
        AudioManager.instance.PlaySFX(prankClip);
        CanvasScriptSplash.instance.LoadScene(3);
        AudioManager.instance.ShutBGM();
        DOVirtual.DelayedCall(1.5f, () => { TssAdsManager._Instance.ShowInterstitial("FromMainToPranks"); });
    }
    public void OnClickRateUs()
    {
        AudioManager.instance.PlayClick();
        Application.OpenURL(GlobalConstant.RateUsLink);
    }
    public void OnClickMoreGames()
    {
        AudioManager.instance.PlayClick();
        Application.OpenURL(GlobalConstant.MoreGamesLink);
    }
    public void OnClickSettings(bool value)
    {
        settings.SetActive(value);
        AudioManager.instance.PlayClick();
        if(!value)
        {
            TssAdsManager._Instance.ShowInterstitial("From main menu settings");
        }
    }
}
