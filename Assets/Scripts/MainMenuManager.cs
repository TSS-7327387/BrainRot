
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        TssAdsManager._Instance.ShowBanner("MainMenu");
    }

    public void OnClickPlay()
    {
        AudioManager.instance.PlayClick();
        CanvasScriptSplash.instance.LoadScene(2);
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
    public void OnClickSettings()
    {
        AudioManager.instance.PlayClick();
    }
}
