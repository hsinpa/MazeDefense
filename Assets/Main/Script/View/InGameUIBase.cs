using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class InGameUIBase : MonoBehaviour
{
    protected CanvasGroup _canvasGroup;

    protected void Init()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show(bool isOn)
    {
        _canvasGroup.alpha = (isOn) ? 1 : 0;
        _canvasGroup.blocksRaycasts = isOn;
        _canvasGroup.interactable = isOn;
    }



}
