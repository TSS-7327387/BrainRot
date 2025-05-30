using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SoundBtnPrefab : MonoBehaviour
{
    public Image pic,background;
    public int ID;
    public Button btn;
    public AudioManagerUI manager;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(PlaySound);
        transform.DOScale(1,1).SetDelay((float)(ID+1)/2).SetEase(Ease.OutSine);
    }

    // Update is called once per frame
    void PlaySound()
    {
        manager.PlayButtonSound(ID);
    }
    public void SetSelected(bool isOn)
    {
        background.color = isOn ? Color.blue : Color.white; // Example toggle
    }
}
