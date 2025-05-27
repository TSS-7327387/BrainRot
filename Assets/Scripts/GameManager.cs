
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ObjectSpawner oSpawner;
    public GameObject failedPanel,settingPanel;
    public TextMeshProUGUI scoreT,failedFinalScore;
    public ParticleEffectPool particlePool;
    public static event Action OnGameOver,OnGameContinue;
    public PowerManager powerManager;
    private void Awake()
    {
        if(Instance != null)
            Destroy(Instance);
        Instance = this;
    }
    private void OnEnable()
    {
        scoreT.text = "00";
       // OnGameOver += GameOver;
    }
    int playerScore;
    public void ShowEffect(Vector3 pos,int points,GameObject nextTierPrefab)
    {
        particlePool.ShowEffect(pos);
        UpdateScore(points);
        AudioManager.instance.PlaySFX();
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
    }
    public void ShowSettings(bool val)
    {
        settingPanel.SetActive(val);
        AudioManager.instance.PlayClick();
        DOVirtual.DelayedCall(0.1f, () => { oSpawner.gameOver = val; }) ;
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
    public void GameOver()
    {
        OnGameOver?.Invoke();
        DOVirtual.DelayedCall(1, () => { failedPanel.SetActive(true);
        });
    }
    public void GameContinue()
    {
        failedPanel.SetActive(false);
        OnGameContinue?.Invoke();
    }

    public void GiveReward()
    {
        failedPanel.SetActive(false);
    }
}
