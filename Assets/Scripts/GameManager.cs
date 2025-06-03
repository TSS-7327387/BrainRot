
using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ObjectSpawner oSpawner;
    public GameObject failedPanel,settingPanel,infoPanel,fiveUp;
    public TextMeshProUGUI scoreT,failedFinalScore;
    public ParticleEffectPool particlePool;
    public static event Action OnGameOver,OnGameContinue;
    public PowerManager powerManager;

    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }
    private void OnEnable()
    {
        scoreT.text = "00";
        // OnGameOver += GameOver;musicEnabled = PlayerPrefs.GetInt("Music", 1) == 1;
        soundEnabled = PlayerPrefs.GetInt("Sounds", 1) == 1;
        SetImagesSprite();
        playerScore = 0;
        TssAdsManager._Instance.ShowBanner("Gameplay");
        TssAdsManager._Instance.admobInstance.TopShowBanner();
    }
    int playerScore;
    public void ShowEffect(Vector3 pos,int points,GameObject nextTierPrefab)
    {
        particlePool.ShowEffect(pos);
        UpdateScore(points);
        //AudioManager.instance.PlaySFX();
        oSpawner.NewObject(nextTierPrefab,pos);
        powerManager.OnExplosionStart();
    }
    public void UpdateScore(int score)
    {
        playerScore += score;
        scoreT.transform.DOKill();
        scoreT.transform.DOScale(new Vector3(1.5f, 1.5f, 0), 0.1f).OnComplete(() =>
        {
            scoreT.text = playerScore.ToString();
            scoreT.transform.localScale = Vector3.one;
        });
        if (playerScore % 500 == 0)
        {
            fiveUp.gameObject.SetActive(true);
            DOVirtual.DelayedCall(1.1f,()=> {
                fiveUp.gameObject.SetActive(false); 
                TssAdsManager._Instance.ShowInterstitial("Player reached:" + playerScore); });
        }
    }
    public void ShowSettings(bool val)
    {
        settingPanel.SetActive(val);
        AudioManager.instance.PlayClick();
        CheckSpwaner(val);
    }
    void CheckSpwaner(bool val)
    {
        DOVirtual.DelayedCall(0.1f, () => {
            oSpawner.hDrag.gameOver = val;
            oSpawner.gameOver = val;
            if (!val && !oSpawner.hangingPiece) oSpawner.SpawnRandomObject();
        });
    }
    public void ShowInfo(bool val)
    {
        infoPanel.SetActive(val);
        AudioManager.instance.PlayClick();
        CheckSpwaner(val);
        if (!val) TssAdsManager._Instance.ShowInterstitial("From info panel");
    }
    public void Home()
    {
        AudioManager.instance.PlayClick();
        TssAdsManager._Instance.ShowInterstitial("Going home from Settings");
        CanvasScriptSplash.instance.LoadScene(1);
    }
    public void Restart(bool showAd)
    {
        AudioManager.instance.PlayClick();
        if(showAd)TssAdsManager._Instance.ShowInterstitial("Restart from Settings");
        CanvasScriptSplash.instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnConitueClick()
    {
        TssAdsManager._Instance.ShowRewardedAd(GameContinue, "continue at:"+failedFinalScore);
    }
    public Image soundImg, musicImg;
    public Sprite soundSpriteOn, soundSpriteOff, musicSpriteOn, musicSpriteOff;
    bool musicEnabled = false, soundEnabled = false;
    void SetImagesSprite()
    {
        soundImg.sprite = soundEnabled ? soundSpriteOn : soundSpriteOff;
        musicImg.sprite = musicEnabled ? musicSpriteOn : musicSpriteOff;
    }
    public void OnClickSounds()
    {
        soundEnabled = !soundEnabled;
        PlayerPrefs.SetInt("Sounds", soundEnabled ? 1 : 0);
        AudioManager.instance.SetSounds(soundEnabled);
        SetImagesSprite();
    }
    
    public void OnClickMusic()
    {
        musicEnabled = !musicEnabled;
        PlayerPrefs.SetInt("Music", musicEnabled ? 1 : 0);
        AudioManager.instance.SetMusic(musicEnabled);
        SetImagesSprite();
    }
    public void GameOver()
    {
        AudioManager.instance.PlayLoseSFX();
        foreach (Delegate del in OnGameOver.GetInvocationList())
        {
            try
            {
                del.DynamicInvoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking method: {ex.Message}");
            }
        }
        failedFinalScore.text = playerScore.ToString();


               DOVirtual.DelayedCall(0.75f, () => {failedPanel.SetActive(true);
                });
    }
    public void GameContinue()
    {
        failedPanel.SetActive(false);
        foreach (Delegate del in OnGameContinue.GetInvocationList())
        {
            try
            {
                del.DynamicInvoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking method: {ex.Message}");
            }
        }
    }

    public void GiveReward()
    {
        failedPanel.SetActive(false);
    }
}
