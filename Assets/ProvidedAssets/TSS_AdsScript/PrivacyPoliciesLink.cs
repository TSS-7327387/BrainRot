using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPoliciesLink : MonoBehaviour
{
    public InternetCheck internetCheck;

    public void PrivacyPolicyLink()
    {
        Application.OpenURL(GlobalConstant.PrivacyPoliciesLInk);
    }

    public void Accept()
    {
        internetCheck.PrivacyPolicy = 1;
        internetCheck.Init();
        CanvasScriptSplash.instance.ChangeCanvas(CanvasStats.Loading);

        DOVirtual.DelayedCall(1,
            () =>
            {
                CanvasScriptSplash.instance.LoadScene(1);
            });
    }
}