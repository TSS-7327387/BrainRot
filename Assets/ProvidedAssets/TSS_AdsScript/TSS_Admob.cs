using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using ToastPlugin;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TSS_Admob : MonoBehaviour
{
    [HideInInspector] private string outputMessage = "";

    public string
        LowBannerID,
        bannerIDMed,
        bannerIDHigh,
        interstitialID,
        InterMediumFloorID,
        InterHighFloorID,
        rewardedIDHigh,
        rewardedIDMed,
        rewardedIDLow,
        appOpenIDHigh,
        appOpenIDMed,
        appOpenIDLow,
        appId,
        rectbannerID;

    public static bool isInterstialAdPresent = false;

    [HideInInspector]
    public enum RequestFloorType
    {
        High,
        Meduim,
        Simple,
        Failed
    }

    [HideInInspector] public RequestFloorType FloorType;
    [HideInInspector] public BannerSize RectbannerSize;

    private AdPosition adPosition;
    private AdPosition adPositionRect;
    private AdSize adSize;

    private AdSize adSizeRect;
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;
    private AppOpenAd appOpenAd;
    private BannerView bannerView;
    private BannerView TopbannerView;
    private BannerView RectbannerView;

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    private float deltaTime;
    [HideInInspector] public UnityEvent OnAdLoadedEvent;
    [HideInInspector] public UnityEvent OnAdFailedToLoadEvent;
    [HideInInspector] public UnityEvent OnAdOpeningEvent;
    [HideInInspector] public UnityEvent OnAdFailedToShowEvent;
    [HideInInspector] public UnityEvent OnUserEarnedRewardEvent;
    [HideInInspector] public UnityEvent OnAdClosedEvent;
    public bool isAdmobInitialized;


    public BannerPosition bannerPosition;
    [HideInInspector] public BannerSize bannerSize;
    [HideInInspector] public BannerPosition rectbannerPosition;
    [HideInInspector] public BannerSize rectbannerSize;

    public void Initialize()
    {
        DontDestroyOnLoad(this.gameObject);

        PostInit();
    }

    public void PrintStatus(string message)
    {
        print(message);
    }

    private AdRequest CreateAdRequest()
    {
        AdRequest ad = new AdRequest();
        return ad;
    }

    public void PostInit()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);

        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

        BannerSpecs();
        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration();

        //.SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    public void BannerSpecs()
    {
        switch (bannerPosition)
        {
            case BannerPosition.Bottom:
                adPosition = AdPosition.Bottom;
                break;
            case BannerPosition.Top:
                adPosition = AdPosition.Top;
                break;
            case BannerPosition.TopLeft:
                adPosition = AdPosition.TopLeft;
                break;
            case BannerPosition.TopRight:
                adPosition = AdPosition.TopRight;
                break;
            case BannerPosition.BottomLeft:
                adPosition = AdPosition.BottomLeft;
                break;
            case BannerPosition.BottomRight:
                adPosition = AdPosition.BottomRight;
                break;
            case BannerPosition.Center:
                adPosition = AdPosition.Center;
                break;
        }

        switch (rectbannerPosition)
        {
            case BannerPosition.Bottom:
                adPositionRect = AdPosition.Bottom;
                break;
            case BannerPosition.Top:
                adPositionRect = AdPosition.Top;
                break;
            case BannerPosition.TopLeft:
                adPositionRect = AdPosition.TopLeft;
                break;
            case BannerPosition.TopRight:
                adPositionRect = AdPosition.TopRight;
                break;
            case BannerPosition.BottomLeft:
                adPositionRect = AdPosition.BottomLeft;
                break;
            case BannerPosition.BottomRight:
                adPositionRect = AdPosition.BottomRight;
                break;
            case BannerPosition.Center:
                adPositionRect = AdPosition.Center;
                break;
        }

        switch (RectbannerSize)
        {
            case BannerSize.Banner:
                adSizeRect = AdSize.Banner;
                break;
            case BannerSize.SmartBanner:
                adSizeRect = AdSize.SmartBanner;
                break;
            case BannerSize.MediumRectangle:
                adSizeRect = AdSize.MediumRectangle;
                break;
            case BannerSize.IABBanner:
                adSizeRect = AdSize.IABBanner;
                break;
            case BannerSize.Leaderboard:
                adSizeRect = AdSize.Leaderboard;
                break;
            case BannerSize.Adaptive:

                //float widthInPixels = Screen.safeArea.width > 0 ? Screen.safeArea.width : Screen.width;
                //int width = (int)(widthInPixels / MobileAds.Utils.GetDeviceScale());
                //MonoBehaviour.print("requesting width: " + width.ToString());
                adSizeRect = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

                break;
        }

        switch (bannerSize)
        {
            case BannerSize.Banner:
                adSize = AdSize.Banner;
                break;
            case BannerSize.SmartBanner:
                adSize = AdSize.SmartBanner;
                break;
            case BannerSize.MediumRectangle:
                adSize = AdSize.MediumRectangle;
                break;
            case BannerSize.IABBanner:
                adSize = AdSize.IABBanner;
                break;
            case BannerSize.Leaderboard:
                adSize = AdSize.Leaderboard;
                break;
            case BannerSize.Adaptive:

                //float widthInPixels = Screen.safeArea.width > 0 ? Screen.safeArea.width : Screen.width;
                //int width = (int)(widthInPixels / MobileAds.Utils.GetDeviceScale());
                //MonoBehaviour.print("requesting width: " + width.ToString());
                adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

                break;
        }
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        Debug.Log("Initialization complete.");
        RequestAndLoadAppOpenAd();
        RequestAndLoadInterstitialAd();
        RequestBannerAd();
        RequestAndLoadRewardedAd();
        LeftRequestBannerAd();
        //RequestRecBannerAd();
        //RequestAndLoadInterstitialAd();
        //TopRequestBannerAd();

        /* MobileAdsEventExecutor.ExecuteInUpdate(() =>
         {
             print("I am executing in the background");
         });*/

        isAdmobInitialized = true;
        /*MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            RequestAndLoadAppOpenAd();
            RequestAndLoadInterstitialAd();
            RequestBannerAd();
            RequestAndLoadRewardedAd();
            RequestAndLoadInterstitialAd();
            TopRequestBannerAd();


            isAdmobInitialized = true;
        });*/
        DOVirtual.DelayedCall(2, () => Application.targetFrameRate = -1);
    }

    #region AD INSPECTOR

    public void OpenAdInspector()
    {
        PrintStatus("Opening Ad inspector.");

        MobileAds.OpenAdInspector((error) =>
        {
            if (error != null)
            {
                PrintStatus("Ad inspector failed to open with error: " + error);
            }
            else
            {
                PrintStatus("Ad inspector opened successfully.");
            }
        });
    }

    #endregion

    #region APPOPEN ADS

    public void OnApplicationPause(bool paused)
    {
        // Display the app open ad when the app is foregrounded
        if (!paused)
        {
            if (isInterstialAdPresent)
            {
                isInterstialAdPresent = false;
                return;
            }

            if (GlobalConstant.isLogger)
                print("InsideAppopen");

            ShowAppOpenAd();
        }
    }

    public bool IsAppOpenAdAvailable
    {
        get
        {
            return (appOpenAd != null
                    && appOpenAd.CanShowAd());
        }
    }

    public void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        /*UnityEngine.Debug.Log("App State is " + state);

        // OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (state == AppState.Foreground)
            {
                //
                // ShowAppOpenAd();
            }
        });*/
    }

    public void DestroyAppOpenAd()
    {
        if (this.appOpenAd != null)
        {
            this.appOpenAd.Destroy();
            this.appOpenAd = null;
        }
    }

    public RequestFloorType appOpenAdRequestFloorType;

    public void RequestAndLoadAppOpenAd()
    {
        if (GlobalConstant.isLogger)
            print(GlobalConstant.AdsON + "GlobalConstant.IsAppOpens");
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        var adUnitId = appOpenIDHigh;
        if (GlobalConstant.UseAdBidding)
        {
            PrintStatus("Requesting App Open ad.");

            switch (appOpenAdRequestFloorType)
            {
                case RequestFloorType.High:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = appOpenIDHigh;
#elif UNITY_IPHONE
            adUnitId = appOpenIDHigh;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;

                case RequestFloorType.Meduim:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = appOpenIDMed;
#elif UNITY_IPHONE
            adUnitId = appOpenIDMed;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;

                case RequestFloorType.Simple:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = appOpenIDLow;
#elif UNITY_IPHONE
            adUnitId = appOpenIDLow;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;

                case RequestFloorType.Failed:
                    appOpenAdRequestFloorType = RequestFloorType.High;
                    return; // Removed unnecessary `break;`
            }
        }
        else
        {
#if UNITY_EDITOR
            adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = appOpenIDLow;
#elif UNITY_IPHONE
            adUnitId = appOpenIDLow;
#else
            adUnitId = "unexpected_platform";
#endif
        }

        // Destroy old instance if it exists
        if (appOpenAd != null)
        {
            DestroyAppOpenAd();
        }

        // Create a new app open ad instance.
        AppOpenAd.Load(adUnitId, CreateAdRequest(),
            (AppOpenAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                    PrintStatus("App open ad failed to load with error: " + loadError.GetMessage());
                    if (GlobalConstant.UseAdBidding)
                    {
                        appOpenAdRequestFloorType = (RequestFloorType)Math.Min((int)appOpenAdRequestFloorType + 1,
                            (int)RequestFloorType.Failed);

                        RequestAndLoadAppOpenAd();
                    }

                    return;
                }
                else if (ad == null)
                {
                    PrintStatus("App open ad failed to load.");
                    return;
                }

                PrintStatus("App Open ad loaded. Please background the app and return.");
                this.appOpenAd = ad;
                this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;
                ad.OnAdFullScreenContentOpened += () =>
                {
                    PrintStatus("App open ad opened.");
                    OnAdOpeningEvent.Invoke();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    PrintStatus("App open ad closed.");

                    if (TssAdsManager._Instance && TssAdsManager._Instance._isBannerShowing)
                    {
                        TssAdsManager._Instance.ShowBanner("appOpenedClosed");
                        TssAdsManager._Instance.admobInstance.TopShowBanner();
                    }

                    /*  if (TssAdsManager._Instance && TssAdsManager._Instance._isRecShowing)
                      {
                          TssAdsManager._Instance.RecShowBanner("appOpenedClosed");
                      }*/

                    OnAdClosedEvent.Invoke();
                };
                ad.OnAdImpressionRecorded += () => { PrintStatus("App open ad recorded an impression."); };
                ad.OnAdClicked += () => { PrintStatus("App open ad recorded a click."); };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    PrintStatus("App open ad failed to show with error: " + error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2})",
                        "App open ad received a paid event.",
                        adValue.CurrencyCode,
                        adValue.Value);
                    if (TSS_AnalyticalManager.instance)
                        TSS_AnalyticalManager.instance.Revenue_ReportAdmob(adValue, "AppOpen");
                    PrintStatus(msg);
                };
            });
    }

    public void ShowAppOpenAd()
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (GlobalConstant.isLogger)
        {
            print(GlobalConstant.AdsON);
        }

        if (GlobalConstant.isLogger)
            print(IsAppOpenAdAvailable);
        if (!IsAppOpenAdAvailable)
        {
            RequestAndLoadAppOpenAd();
            return;
        }

        if (TssAdsManager._Instance)
        {
            TssAdsManager._Instance.HideRecBannerAppOpen();
            TssAdsManager._Instance.HideBanner();
            TssAdsManager._Instance.admobInstance.TopHideBanner();
        }

        HideBanner();
        appOpenAd.Show();
    }

    #endregion

    #region Bottom BANNER ADS

    public RequestFloorType bannerAdRequestFloorType;

    public void RequestBannerAd()
    {
        Debug.Log("Requesting Banner ad.");
        string adUnitId = bannerIDMed;

#if UNITY_EDITOR
        adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = bannerIDMed;
#elif UNITY_IPHONE
            adUnitId = bannerIDMed;
#else
            adUnitId = "unexpected_platform";
#endif

        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // ✅ Ad Position is now configurable
        // AdPosition adPosition = AdPosition.TopRight; // Change as needed

        bannerView = new BannerView(adUnitId, AdSize.Banner, adPosition);

        bannerView.OnBannerAdLoaded += () =>
        {
            PrintStatus("Banner ad loaded.");
            OnAdLoadedEvent.Invoke();
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            PrintStatus("Banner ad failed to load with error: " + error.GetMessage());
            OnAdFailedToLoadEvent.Invoke();

          /*  if (GlobalConstant.UseAdBidding)
            {
                // ✅ Prevents infinite recursion by clamping the request floor type
                bannerAdRequestFloorType =
                    (RequestFloorType)Math.Min((int)bannerAdRequestFloorType + 1, (int)RequestFloorType.Failed);

                DOVirtual.DelayedCall(1, RequestBannerAd);
            }*/
        };

        bannerView.OnAdImpressionRecorded += () => { PrintStatus("Banner ad recorded an impression."); };
        bannerView.OnAdClicked += () => { PrintStatus("Banner ad recorded a click."); };
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            PrintStatus("Banner ad opening.");
            OnAdOpeningEvent.Invoke();
        };

        bannerView.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("Banner ad closed.");
            OnAdClosedEvent.Invoke();
            if (GlobalConstant.UseAdBidding)
                bannerAdRequestFloorType = RequestFloorType.High;
            DOVirtual.DelayedCall(1, RequestBannerAd);
        };

        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}.");
            TSS_AnalyticalManager.instance?.Revenue_ReportAdmob(adValue, "Banner");
        };

        bannerView.LoadAd(CreateAdRequest());
        HideBanner();
    }


    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }

    public void ShowBanner()
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (bannerView != null)
        {
            bannerView.Show();
        }
        else
        {
            RequestBannerAd();
        }
    }

    #endregion

    #region Top BANNER ADS

    public RequestFloorType TopbannerAdRequestFloorType;

    public void LeftRequestBannerAd()
    {
        Debug.Log("Requesting Left Banner ad.");
        string adUnitId = bannerIDHigh;
     
#if UNITY_EDITOR
            adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = bannerIDHigh;
#elif UNITY_IPHONE
            adUnitId = bannerIDHigh;
#else
            adUnitId = "unexpected_platform";
#endif


            if (TopbannerView != null)
            {
                TopbannerView.Destroy();
            }

            // ✅ Ad Position is now configurable
            AdPosition _adPosition = AdPosition.Bottom; // Change as needed

            TopbannerView = new BannerView(adUnitId, AdSize.Banner, _adPosition);

            TopbannerView.OnBannerAdLoaded += () =>
            {
                PrintStatus("Banner ad loaded.");
                OnAdLoadedEvent.Invoke();
            };

            TopbannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                PrintStatus("Banner ad failed to load with error: " + error.GetMessage());
                OnAdFailedToLoadEvent.Invoke();

              /*  // ✅ Prevents infinite recursion by clamping the request floor type
                bannerAdRequestFloorType =
                    (RequestFloorType)Math.Min((int)bannerAdRequestFloorType + 1, (int)RequestFloorType.Failed);

                DOVirtual.DelayedCall(1, LeftRequestBannerAd);*/
            };

            TopbannerView.OnAdImpressionRecorded += () => { PrintStatus("Banner ad recorded an impression."); };
            TopbannerView.OnAdClicked += () => { PrintStatus("Banner ad recorded a click."); };
            TopbannerView.OnAdFullScreenContentOpened += () =>
            {
                PrintStatus("Banner ad opening.");
                OnAdOpeningEvent.Invoke();
            };

            TopbannerView.OnAdFullScreenContentClosed += () =>
            {
                PrintStatus("Banner ad closed.");
                OnAdClosedEvent.Invoke();

                DOVirtual.DelayedCall(1, LeftRequestBannerAd);
            };

            TopbannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}.");
                TSS_AnalyticalManager.instance?.Revenue_ReportAdmob(adValue, "Banner");
            };

            TopbannerView.LoadAd(CreateAdRequest());
            TopHideBanner();
        
    }


    public void TopHideBanner()
    {
        if (TopbannerView != null)
        {
            TopbannerView.Hide();
        }
    }

    public void TopShowBanner()
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (TopbannerView != null)
        {
            TopbannerView.Show();
        }
        else
        {
            LeftRequestBannerAd();
        }
    }

    #endregion


    #region REC BANNER ADS

    public RequestFloorType RECbannerAdRequestFloorType;

    public void RequestRecBannerAd()
    {
        Debug.Log("Requesting Banner ad.");
        string adUnitId = LowBannerID;

#if UNITY_EDITOR
        adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = LowBannerID;
#elif UNITY_IPHONE
            adUnitId = LowBannerID;
#else
            adUnitId = "unexpected_platform";
#endif

        if (RectbannerView != null)
        {
            RectbannerView.Destroy();
            TssAdsManager._Instance._isRecBannerReady = false;
        }

        RectbannerView = new BannerView(adUnitId, AdSize.MediumRectangle, AdPosition.TopLeft);

        RectbannerView.OnBannerAdLoaded += () =>
        {
            PrintStatus("Banner ad loaded.");
            OnAdLoadedEvent.Invoke();
            TssAdsManager._Instance._isRecBannerReady = true;
        };

        RectbannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            PrintStatus("Banner ad failed to load with error: " + error.GetMessage());
            OnAdFailedToLoadEvent.Invoke();

            RequestRecBannerAd();
            TssAdsManager._Instance._isRecBannerReady = false;
        };

        RectbannerView.OnAdImpressionRecorded += () => { PrintStatus("Banner ad recorded an impression."); };
        RectbannerView.OnAdClicked += () => { PrintStatus("Banner ad recorded a click."); };
        RectbannerView.OnAdFullScreenContentOpened += () =>
        {
            PrintStatus("Banner ad opening.");
            OnAdOpeningEvent.Invoke();
        };

        RectbannerView.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("Banner ad closed.");

            OnAdClosedEvent.Invoke();

            DOVirtual.DelayedCall(.1f, RequestRecBannerAd);
        };

        RectbannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            TSS_AnalyticalManager.instance?.Revenue_ReportAdmob(adValue, "Banner");
        };

        RectbannerView.LoadAd(CreateAdRequest());
    }


    public void HideRecBanner()
    {
        if (RectbannerView != null)
        {
            RectbannerView.Hide();
            TssAdsManager._Instance._isRecShowing = false;
        }
    }

    public void ShowRecBanner()
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (RectbannerView != null)
        {
            RectbannerView.Show();
        }
        else
        {
            RequestRecBannerAd();
        }
    }

    #endregion

    #region INTERSTITIAL ADS

    [HideInInspector] public bool Once;

    public RequestFloorType interAdRequestFloorType;

    public void RequestAndLoadInterstitialAd()
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        PrintStatus("Requesting Interstitial ad.");
        string adUnitId = InterHighFloorID;
        if (GlobalConstant.UseAdBidding)
        {
            switch (interAdRequestFloorType)

            {
                case RequestFloorType.High:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = InterHighFloorID;
#elif UNITY_IPHONE
            adUnitId = InterHighFloorID;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;
                case RequestFloorType.Meduim:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = InterMediumFloorID;
#elif UNITY_IPHONE
            adUnitId = InterMediumFloorID;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;
                case RequestFloorType.Simple:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = interstitialID;
#elif UNITY_IPHONE
            adUnitId = interstitialID;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;
                case RequestFloorType.Failed:
                    interAdRequestFloorType = RequestFloorType.High; // ✅ Fixed
                    return;
            }
        }
        else
        {
#if UNITY_EDITOR
            adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = interstitialID;
#elif UNITY_IPHONE
            adUnitId = interstitialID;
#else
            adUnitId = "unexpected_platform";
#endif
        }

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        InterstitialAd.Load(adUnitId, CreateAdRequest(), (InterstitialAd ad, LoadAdError loadError) =>
        {
            if (loadError != null)
            {
                PrintStatus("Interstitial ad failed to load with error: " + loadError.GetMessage());
                if (GlobalConstant.UseAdBidding)
                {
                    interAdRequestFloorType =
                        (RequestFloorType)Math.Min((int)interAdRequestFloorType + 1,
                            (int)RequestFloorType.Failed); // ✅ Prevents infinite recursion
                    RequestAndLoadInterstitialAd();
                }

                return;
            }

            if (ad == null)
            {
                PrintStatus("Interstitial ad failed to load.");
                if (GlobalConstant.UseAdBidding)
                {
                    interAdRequestFloorType =
                        (RequestFloorType)Math.Min((int)interAdRequestFloorType + 1,
                            (int)RequestFloorType.Failed); // ✅ Prevents infinite recursion
                    RequestAndLoadInterstitialAd();
                }

                return;
            }

            PrintStatus("Interstitial ad loaded.");
            interstitialAd = ad;

            ad.OnAdFullScreenContentClosed += () =>
            {
                PrintStatus("Interstitial ad closed.");
                if (GlobalConstant.UseAdBidding)
                {
                    interAdRequestFloorType = RequestFloorType.High;
                }

                RequestAndLoadInterstitialAd();
                OnAdClosedEvent.Invoke();
            };

            ad.OnAdPaid += (AdValue adValue) =>
            {
                TSS_AnalyticalManager.instance?.Revenue_ReportAdmob(adValue, "Interstitial");
            };
        });
    }

    public void ShowInterstitial(bool checkMax = true)
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        try
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                isInterstialAdPresent = true;
                interstitialAd.Show();
                if (TSS_AnalyticalManager.instance)
                    TSS_AnalyticalManager.instance.CustomOtherEvent("Admob_Inter_Shown");
            }
            else if (checkMax)
            {
                RequestAndLoadInterstitialAd();
                if (GlobalConstant.ISMAXON)
                    AppLovinMax.Instance?.ShowInterstitial(false);
                if (TSS_AnalyticalManager.instance)
                    TSS_AnalyticalManager.instance.CustomOtherEvent("Admob_Inter_Failed");
            }
        }
        catch
        {
            // ignored
        }
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }

    #endregion

    #region REWARDED ADS

    public RequestFloorType rewardedFlooringType;

    public void RequestAndLoadRewardedAd()
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        PrintStatus("Requesting Rewarded ad.");

        string adUnitId = rewardedIDHigh;
        if (GlobalConstant.UseAdBidding)
        {
            switch (rewardedFlooringType)
            {
                case RequestFloorType.High:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = rewardedIDHigh;
#elif UNITY_IPHONE
            adUnitId = rewardedIDHigh;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;
                case RequestFloorType.Meduim:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = rewardedIDMed;
#elif UNITY_IPHONE
            adUnitId = rewardedIDMed;
#else
            adUnitId = "unexpected_platform";
#endif
                    break;
                case RequestFloorType.Simple:
#if UNITY_EDITOR
                    adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = rewardedIDLow;  // ✅ Fixed
#elif UNITY_IPHONE
            adUnitId = rewardedIDLow;  // ✅ Fixed
#else
            adUnitId = "unexpected_platform";
#endif
                    break;
                case RequestFloorType.Failed:
                    rewardedFlooringType = RequestFloorType.High; // ✅ Fixed
                    return;
            }
        }
        else
        {
#if UNITY_EDITOR
            adUnitId = "unused";
#elif UNITY_ANDROID
            adUnitId = rewardedIDLow;  // ✅ Fixed
#elif UNITY_IPHONE
            adUnitId = rewardedIDLow;  // ✅ Fixed
#else
            adUnitId = "unexpected_platform";
#endif
        }

        RewardedAd.Load(adUnitId, CreateAdRequest(), (RewardedAd ad, LoadAdError loadError) =>
        {
            if (loadError != null)
            {
                PrintStatus("Rewarded ad failed to load with error: " + loadError.GetMessage());
                if (GlobalConstant.UseAdBidding)
                {
                    rewardedFlooringType =
                        (RequestFloorType)Math.Min((int)rewardedFlooringType + 1,
                            (int)RequestFloorType.Failed); // ✅ Prevents infinite recursion
                    RequestAndLoadRewardedAd();
                }

                return;
            }

            if (ad == null)
            {
                PrintStatus("Rewarded ad failed to load.");
                if (GlobalConstant.UseAdBidding)
                {
                    rewardedFlooringType =
                        (RequestFloorType)Math.Min((int)rewardedFlooringType + 1,
                            (int)RequestFloorType.Failed); // ✅ Prevents infinite recursion
                    RequestAndLoadRewardedAd();
                }

                return;
            }

            PrintStatus("Rewarded ad loaded.");
            rewardedAd = ad;

            ad.OnAdFullScreenContentOpened += () =>
            {
                PrintStatus("Rewarded ad opening.");
                OnAdOpeningEvent.Invoke();
            };
         /*   ad.OnUserEarnedReward += (reward) => {
                PrintStatus("User earned reward.");
                
            };*/

            ad.OnAdFullScreenContentClosed += () =>
            {
                GlobalConstant.RewardedAdsWatched(TssAdsManager._Instance.action);

                if (GlobalConstant.UseAdBidding)
                    rewardedFlooringType = RequestFloorType.High;

                RequestAndLoadRewardedAd();
                PrintStatus("Rewarded ad closed.");
                OnAdClosedEvent.Invoke();
            };

            ad.OnAdPaid += (AdValue adValue) =>
            {
                TSS_AnalyticalManager.instance?.Revenue_ReportAdmob(adValue, "Rewarded");
            };
        });
    }

    public void ShowRewardedAdmob(Action ac)
    {
        if (!GlobalConstant.AdsON)
        {
            return;
        }

        if (rewardedAd != null)
        {
            isInterstialAdPresent = true;

            rewardedAd.Show((Reward reward) => { PrintStatus("Rewarded ad granted a reward: " + reward.Amount); });
        }
        else
        {
            if (GlobalConstant.ISMAXON)
                AppLovinMax.Instance?.ShowRewardedAd(ac);
            RequestAndLoadRewardedAd();
        }
    }

    #endregion
}