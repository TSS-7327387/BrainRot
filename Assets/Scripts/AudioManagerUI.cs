using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using System.Collections;
using DG.Tweening;

public class AudioManagerUI : MonoBehaviour
{
    [Header("Audio Components")]
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    public Sprite[] soundIcons;

    [Header("UI Controls")]
    public Button playPauseButton;
    public Button nextButton;
    public Button previousButton;
    public Slider progressSlider;
    public Sprite playIcon;
    public Sprite pauseIcon;

    [Header("Button Prefab")]
    public SoundBtnPrefab soundPrefab;
    public Transform soundBtnParent;

    private int currentClipIndex = 0;
    private bool isDragging = false;
    int adLimit;
    void OnEnable()
    {

        if (audioClips.Count > 0)
        {
            audioSource.clip = audioClips[currentClipIndex];
            UpdateSlider();
        }
        SetUpButtons();
        adLimit = 1;
        // Add listeners
        playPauseButton.onClick.AddListener(PlayPause);
        nextButton.onClick.AddListener(NextClip);
        previousButton.onClick.AddListener(PreviousClip);
        progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    List<SoundBtnPrefab> soundBtnPrefabs = new List<SoundBtnPrefab>();
    void SetUpButtons()
    {
        int i = 0;
        foreach (AudioClip clip in audioClips)
        {
            if (i < soundIcons.Length)
            {
                var btn = Instantiate(soundPrefab, soundBtnParent);
                soundBtnPrefabs.Add(btn);
                btn.pic.sprite = soundIcons[i];
                btn.ID = i;
                btn.manager = this;
                i++;
            }
        }
    }
    void Update()
    {
        if (audioSource.isPlaying && !isDragging)
        {
            UpdateSlider();
        }
        if (!audioSource.isPlaying && vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);
            vibrationCoroutine = null;
        }
    }

    public void PlayPause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            playPauseButton.image.sprite = playIcon;

            if (vibrationCoroutine != null)
            {
                StopCoroutine(vibrationCoroutine);
            }
        }
        else
        {
            audioSource.Play();
            playPauseButton.image.sprite = pauseIcon;

            if (vibrationCoroutine != null)
            {
                StopCoroutine(vibrationCoroutine);
            }
            vibrationCoroutine = StartCoroutine(VibrateWhilePlaying());
        }
    }

    public void NextClip()
    {
        if (currentClipIndex < audioClips.Count - 1)
        {
            currentClipIndex++;
        }
        PlayClip();
    }

    public void PreviousClip()
    {
        if (currentClipIndex > 0)
        {
            currentClipIndex--;
        }
        PlayClip();
    }
    void PlayClip()
    {
        audioSource.Stop();
        audioSource.clip = audioClips[currentClipIndex];
        audioSource.time = 0f;
        audioSource.Play();
        playPauseButton.image.sprite = pauseIcon;

        if (vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);
        }
        vibrationCoroutine = StartCoroutine(VibrateWhilePlaying());

        UpdateSlider();
        UpdateButtonStates();
    }
    void UpdateButtonStates()
    {
        foreach (SoundBtnPrefab sbp in soundBtnPrefabs)
        {

            sbp.SetSelected(sbp.ID == currentClipIndex);
        }
        
    }
    void UpdateSlider()
    {
        if (audioSource.clip != null)
        {
            progressSlider.maxValue = audioSource.clip.length;
            progressSlider.value = audioSource.time;
        }
    }

    public void OnSliderValueChanged(float value)
    {
        if (audioSource.clip != null && isDragging)
        {
            audioSource.time = value;
        }
    }

    // These two methods can be called by Slider events: OnPointerDown and OnPointerUp
    public void OnSliderPointerDown()
    {
        isDragging = true;
    }

    public void OnSliderPointerUp()
    {
        isDragging = false;
        audioSource.time = progressSlider.value;

        if (audioSource.isPlaying)
        {
            if (vibrationCoroutine != null)
            {
                StopCoroutine(vibrationCoroutine);
            }
            vibrationCoroutine = StartCoroutine(VibrateWhilePlaying());
        }
    }
    private Coroutine vibrationCoroutine;

    IEnumerator VibrateWhilePlaying()
    {
        while (audioSource.isPlaying)
        {
            switch (currentClipIndex)
            {
                case 0:
                    MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
                    break;
                case 1:
                    MMVibrationManager.Haptic(HapticTypes.Failure, false, true, this);
                    break;;
                case 5:
                    MMVibrationManager.Haptic(HapticTypes.RigidImpact, false, true, this);
                    break;;
                case 6:
                    MMVibrationManager.Haptic(HapticTypes.HeavyImpact, false, true, this);
                    yield return new WaitForSeconds(0.25f);
                    break;
                case 13:
                    MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, this);
                    break;;

            }
            yield return new WaitForSeconds(0.15f); // Customize the frequency
        }
    }
    // For other buttons (each with a different sound)
    public void PlayButtonSound(int id)
    {
        if (id < 0 || id >= audioClips.Count)
            return;

        if (id == 2)
        {
            MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
        }
        else if (id == 3)
        {
            MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
        }
        else if (id == 8)
        {
            DOVirtual.DelayedCall(0.11f,()=>{ MMVibrationManager.Haptic(HapticTypes.Success, false, true, this); });
        }
        else if (id == 1)
        {
            MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);
        }
        else
        {
            if (vibrationCoroutine != null)
            {
                StopCoroutine(vibrationCoroutine);
            }
        }
        vibrationCoroutine = StartCoroutine(VibrateWhilePlaying());
        currentClipIndex = id;
        audioSource.Stop();
        audioSource.clip = audioClips[currentClipIndex];
        audioSource.time = 0f;
        audioSource.Play();
        playPauseButton.image.sprite = pauseIcon;

        UpdateSlider();
        isDragging = false;
        UpdateButtonStates();

        adLimit++;
        if (adLimit % 5 == 0)
        {
            TssAdsManager._Instance.ShowInterstitial("InterAd at sound btn:" + adLimit);
        }
    }

    public void GoBack()
    {
        audioSource.Stop();
        AudioManager.instance.PlayClick();
        TssAdsManager._Instance.ShowInterstitial("Going home from Pranks");
        CanvasScriptSplash.instance.LoadScene(1);
        AudioManager.instance.ChangeBGM(true);
    }
}
