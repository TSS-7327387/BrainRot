using System;
using GoogleMobileAds.Api;
using UnityEngine;


public class TssAdsManager : MonoBehaviour
{
    public static TssAdsManager _Instance;

    public enum AdPriority
    {
        Admob,
        Max
    }

    public AdPriority adPriority;

    public static bool isAdsRemove
    {
        get => PlayerPrefs.GetInt("RemoveAds", 0) == 1;
        set => PlayerPrefs.SetInt("RemoveAds", value ? 1 : 0);
    }

    public void OnRemoveAds()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        admobInstance.HideBanner();
    }

    [Header("Admob IDS")] public bool _isBannerShowing, _isBannerReady;
    [Header("Admob IDS")] public bool _isRecShowing, _isRecBannerReady;
    private bool _isFlooringBannerReady, _isFlooringBannerShowing;
    private bool isMRecShowing;

    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;

    [Header("Max IDS")] public string MaxSdkKey = "";


    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void Init()
    {
    
        Debug.Log("Application.version " + Application.version);
 
        PostInit();
    }

    public TSS_Admob admobInstance;
    public AppLovinMax appLovinMax;

    void PostInit()
    {
        adPriority = GlobalConstant.adPriority;
        admobInstance.bannerIDMed = GlobalConstant.TSS_Admob_Banner_MID;
        admobInstance.InterMediumFloorID = GlobalConstant.TSS_Admob_Inter_IdMid;
        admobInstance.rewardedIDMed = GlobalConstant.TSS_Admob_Rewarded_Id_Mid;
        admobInstance.appOpenIDMed = GlobalConstant.TSS_Admob_AppOpen_Id_Mid;

        admobInstance.bannerIDHigh = GlobalConstant.TSS_Admob_Banner_HIGH;
        admobInstance.InterHighFloorID = GlobalConstant.TSS_Admob_Inter_IdHigh;
        admobInstance.rewardedIDHigh = GlobalConstant.TSS_Admob_Rewarded_Id_High;
        admobInstance.appOpenIDHigh = GlobalConstant.TSS_Admob_AppOpen_IdHigh;

        admobInstance.LowBannerID = GlobalConstant.TSS_Admob_Banner_Simple;
        admobInstance.interstitialID = GlobalConstant.TSS_Admob_Inter_IdLow;
        admobInstance.rewardedIDLow = GlobalConstant.TSS_Admob_Rewarded_Id_Simple;
        admobInstance.appOpenIDLow = GlobalConstant.TSS_Admob_AppOpen_Id_Low;


        appLovinMax.MaxSdkKey = MaxSdkKey;
        appLovinMax.InterstitialAdUnitId = GlobalConstant.InterstitialAdUnitId;
        appLovinMax.RewardedAdUnitId = GlobalConstant.RewardedAdUnitId;


        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        admobInstance.Initialize();
        appLovinMax.Init();
    }

    public void ShowInterstitial(string placement)
    {
        TSS_Admob.isInterstialAdPresent = true;
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        TSS_AnalyticalManager.instance.InterstitialEvent(placement);

        if (adPriority == AdPriority.Max)
        {
            if (appLovinMax)
                appLovinMax.ShowInterstitial();
            
        }
        else
        {
            appLovinMax.InitializeInterstitialAds();
            admobInstance.ShowInterstitial();
        }
    }

    #region Rewarded Ad Methods

    public Action action;

    public void ShowRewardedAd(Action ac, string placement)
    {
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        action = ac;
        if (adPriority == AdPriority.Max)
        {
            appLovinMax.ShowRewardedAd(ac);
            
        }
        else
            admobInstance.ShowRewardedAdmob(ac);

        if (TSS_AnalyticalManager.instance)
        {
            TSS_AnalyticalManager.instance.VideoEvent(placement);
        }
    }

    public void ShowRewardedAd(string placement)
    {
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (GlobalConstant.isLogger)
            print("TSS_Admob.Instance");
        if (adPriority == AdPriority.Max)
        {
            appLovinMax.ShowRewardedAd(action);
        }
        else
            admobInstance.ShowRewardedAdmob(action);

        if (TSS_AnalyticalManager.instance)
        {
            TSS_AnalyticalManager.instance.VideoEvent(placement);
        }
    }

    #endregion

    #region Banner Ad Methods

    public void ShowBanner(string placement)
    {
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        _isBannerShowing = true;

        admobInstance.ShowBanner();
        if (TSS_AnalyticalManager.instance)
        {
            TSS_AnalyticalManager.instance.CustomScreenEvent(placement);
        }
    }

    public void RecShowBanner(string placement)
    {
        if (isAdsRemove)
        {
            return;
        }

        if (!GlobalConstant.AdsON)
        {
            return;
        }

        _isBannerShowing = true;

        admobInstance.ShowRecBanner();
        if (TSS_AnalyticalManager.instance)
        {
            TSS_AnalyticalManager.instance.CustomScreenEvent(placement);
        }
    }


    public void HideBanner()
    {
        _isBannerReady = false;
        _isBannerShowing = false;
        admobInstance.HideBanner();
    }
    public void HideRecBanner()
    {
        _isBannerReady = false;
        _isBannerShowing = false;
        admobInstance.HideRecBanner();
    } 
    public void HideBannerAppOpen()
    {
        admobInstance.HideBanner();
    }
    public void HideRecBannerAppOpen()
    {
        admobInstance.HideRecBanner();
    } 
    public bool IsBannerAdAvailable()
    {
        return _isBannerReady;
    }

    #endregion
}