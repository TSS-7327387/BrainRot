using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class PowerManager : MonoBehaviour
{
    public GameObject hammer;
    public Transform hammerParent;
    public float hammerRadius=0.5f;
    public GameObject watchAdForPower;
    public TextMeshProUGUI hammerNo, starNo;
    public GameObject hammerEffect, starEffect;
    Vector3 hammerParentStartPos;
    public Button hammerBtn, starBtn;
    public AudioClip hammerSound, starSound;

    private void Start()
    {
        hammerParentStartPos = hammerParent.position;
        hammerBtn.onClick.AddListener(ShowHammer);
        starBtn.onClick.AddListener(UseStarPower);
        PlayerPrefs.SetInt("Hammer",3);
        PlayerPrefs.SetInt("Star", 2);
        UpdateText();
    }
    bool usingHammer;
    bool usingStar;
    #region HammerWorking
    private void Update()
    {
        if (usingHammer && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) || touch.position.y > Screen.height - (Screen.height / 4))
                return;
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchWorldPos.z = 0;
            if (touch.phase == TouchPhase.Ended)
            {
                UseHammerPower(touchWorldPos);
            }
        }
    }
    void UpdateText()
    {
        hammerNo.text = PlayerPrefs.GetInt("Hammer").ToString();
        starNo.text = PlayerPrefs.GetInt("Star").ToString();
    }
    public void ShowHammer()
    {
        GameManager.Instance.oSpawner.hDrag.gameOver = true;
        if (PlayerPrefs.GetInt("Hammer") > 0)
        {
            hammer.SetActive(true);
            PlayerPrefs.SetInt("Hammer", PlayerPrefs.GetInt("Hammer")-1);
            UpdateText();
            hammerBtn.interactable = false;
            starBtn.interactable = false;
            DOVirtual.DelayedCall(0.1f,()=> { usingHammer = true; });
            TSS_AnalyticalManager.instance.CustomBtnEvent("using_HammerPower");
        }
        else
            ShowNotEnoughPower(true);
    }
    public void UseHammerPower(Vector3 pos)
    {
        usingHammer = false;
        hammerParent.DOMove(pos, 0.5f).OnComplete(() =>
        {
            hammer.transform.DORotate(new Vector3(0, 0, -51), 0.33f).SetEase(Ease.OutBounce).SetLoops(2, LoopType.Yoyo).OnComplete(
                () =>
                {
                    AudioManager.instance.PlayPowerSFX(hammerSound);
                    /*Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
                    Vector2 touchPos2D = new Vector2(touchWorldPos.x, touchWorldPos.y);*/
                    HammerEffect(pos);
                    // Option 1: Raycast2D to detect objects with colliders

                });
        });

    }
    void HammerEffect(Vector3 pos)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos, hammerRadius);

        if (hitColliders.Length > 0)
        {
            foreach (Collider2D col in hitColliders)
            {
                Debug.Log("Touched object: " + col.gameObject.name);

                // Optional: Filter by tag
                if (col.CompareTag("Mergable"))
                {
                    Debug.Log("Touched Enemy: " + col.name);
                    Destroy(col.gameObject);
                    Instantiate(hammerEffect,pos,Quaternion.identity, transform);
                }

                // Optional: Access components
                // var yourScript = col.GetComponent<YourScript>();
                // if (yourScript != null) { yourScript.OnTouched(); }
            }
        }
        hammerParent.DOMove(hammerParentStartPos, 0.5f).OnComplete(() => { 
            hammer.SetActive(false);
            hammerBtn.interactable = true;
            starBtn.interactable = true;
            GameManager.Instance.oSpawner.hDrag.gameOver = false;
        });
    }
    #endregion
    #region StarPower
    public void UseStarPower()
    {
        GameManager.Instance.oSpawner.hDrag.gameOver = true;
        if (PlayerPrefs.GetInt("Star") > 0)
        {
            usingStar = true;
            PlayerPrefs.SetInt("Star", PlayerPrefs.GetInt("Star")-1);
            UpdateText();
            starEffect.gameObject.SetActive(true);
            hammerBtn.interactable = false;
            starBtn.interactable = false;
            StartCoroutine(MoveItemsCoroutine(UnityEngine.Random.Range(3,6)));
            TSS_AnalyticalManager.instance.CustomBtnEvent("using_StarPower");
        }
        else
            ShowNotEnoughPower(false);
    }
    public List<Transform> containerItems; 
    public Transform containerCenter;
    public float moveDistance;
    
    private IEnumerator MoveItemsCoroutine(int moveCount)
    {
        int moved = 0;
        while (moved < moveCount)
        {
            // Refresh the list after potential explosions
            containerItems.RemoveAll(item => item == null);
            if (containerItems.Count == 0) break;

            // Select a random item
            Transform item = containerItems[UnityEngine.Random.Range(0, containerItems.Count-1)];
            if (item != null)
            {
                AudioManager.instance.PlayPowerSFX(starSound);
                var rb = item.GetComponent<Rigidbody2D>();
                rb.simulated = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                waitBeforeMove = true;
                Vector2 directionToCenter = (containerCenter.position - item.position).normalized;
                Vector3 targetPos = item.position + (Vector3)(directionToCenter * moveDistance);
                CameraShake.Instance.StartShake();
                yield return StartCoroutine(MoveItemSmoothly(item, targetPos, 0.2f));
                CameraShake.Instance.StopShake();
                rb.simulated = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                OnExplosionStart();
                moved++;

                // ⚠️ Wait for potential physics/explosions to resolve
                yield return new WaitUntil(()=>!waitBeforeMove); // Adjust as per physics settle time

            }
        }
        GameManager.Instance.oSpawner.hDrag.gameOver = false;
        usingStar = false;
        waitBeforeMove = false;
        starEffect.gameObject.SetActive(false);
        hammerBtn.interactable = true;
        starBtn.interactable = true;
    }
    private IEnumerator MoveItemSmoothly(Transform item, Vector3 targetPos, float duration)
    {
        Vector3 startPos = item.position;
        float time = 0;
        while (time < duration)
        {
            item.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        item.position = targetPos;
    }
    Action callback;
    void ShowNotEnoughPower(bool isHammerPower)
    {
        watchAdForPower.gameObject.SetActive(true);
        AudioManager.instance.PlayClick();
        callback = isHammerPower ? () => {
            PlayerPrefs.SetInt("Hammer", PlayerPrefs.GetInt("Hammer")+2);
            UpdateText();
        } : () => {
            PlayerPrefs.SetInt("Star", PlayerPrefs.GetInt("Star")+2);
            UpdateText();
        };
        TSS_AnalyticalManager.instance.CustomBtnEvent(isHammerPower?"not enough hammer":"not enough stars");
    }
    public void ShutWatchAd()
    {
        watchAdForPower.gameObject.SetActive(false);
        AudioManager.instance.PlayClick();
        GameManager.Instance.oSpawner.hDrag.gameOver = false;
    }
    public void OnClickAd()
    {
        TssAdsManager._Instance.ShowRewardedAd(callback,"GettingPowersFromAd");
    }
    private CancellationTokenSource cts;
    private float settleDelay = 0.5f;

    bool waitBeforeMove;

    // Call this on explosion start
    public void OnExplosionStart()
    {
        if (!usingStar)
        {
            return;
        }
        cts?.Cancel();  // Cancel previous settle wait if any
        cts = new CancellationTokenSource();

        // Start a delayed task to cancel token after 0.5s if no new explosion comes
        _ = HandleDelayedSettleAsync(cts);
    }

    private async Task HandleDelayedSettleAsync(CancellationTokenSource myCts)
    {
        try
        {
            await Task.Delay((int)(settleDelay * 1000), myCts.Token);
            if (!myCts.IsCancellationRequested)
            {
                waitBeforeMove = false;
                myCts.Cancel();  // No explosion for 0.5s, settle explosions
            }
        }
        catch (TaskCanceledException)
        {
            // Expected if new explosion starts during delay
        }
    }
    #endregion
}
