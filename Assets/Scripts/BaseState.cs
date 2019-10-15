using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnDraw();
    void OnExit();
}

public class BaseState : MonoBehaviour, IState
{
    public enum EStateEffect
    {
        None,
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop,
        Zoom,
        ZoomFromChild,
        TweenAlpha
    }

    protected readonly float EffectTime = 0.4f;

    [SerializeField]
    protected EStateEffect StateEffect = EStateEffect.TweenAlpha;

    protected Canvas Canvas;
    protected RectTransform CanvasRectTransform;
    protected CanvasGroup CanvasGroup;
    protected bool IsAnimating;
    protected RectTransform RectTransform;

    protected bool IsAppearing;

    protected virtual void Awake()
    {
        Canvas = GetComponentInParent<Canvas>();

        if (Canvas)
            CanvasRectTransform = Canvas.GetComponent<RectTransform>();

        CanvasGroup = GetComponent<CanvasGroup>();
        RectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
        ActiveCanvas(false);
    }

    public virtual void OnEnter()
    {
        IsAppearing = true;
        PlayEffect();
    }

    public virtual void OnExit()
    {
        IsAppearing = false;
        PlayEffect();
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnDraw()
    {
    }

    private void PlayEffect()
    {
        if (RectTransform == null)
        {
            gameObject.SetActive(IsAppearing);
            ActiveCanvas(IsAppearing);
            return;
        }

        IsAnimating = true;
        gameObject.SetActive(true);
        ActiveCanvas(false);

        if (IsAppearing)
        {
            if (CanvasGroup)
                CanvasGroup.alpha = 0;
        }

        LeanTween.cancel(gameObject);

        switch (StateEffect)
        {
            case EStateEffect.None:
                PlayNoneEffect();
                break;

            case EStateEffect.LeftToRight:
                PlayLeftToRightEffect();
                PlayTweenAlphaEffect();
                break;

            case EStateEffect.RightToLeft:
                PlayRightToLeftEffect();
                PlayTweenAlphaEffect();
                break;

            case EStateEffect.TopToBottom:
                PlayTopToBottomEffect();
                PlayTweenAlphaEffect();
                break;

            case EStateEffect.BottomToTop:
                PlayBottomToTopEffect();
                PlayTweenAlphaEffect();
                break;

            case EStateEffect.Zoom:
                PlayZoomEffect();
                break;

            case EStateEffect.ZoomFromChild:
                PlayZoomFromChildEffect();
                break;

            case EStateEffect.TweenAlpha:
                PlayTweenAlphaEffect();
                break;
        }
    }

    private void ActiveCanvas(bool isActive)
    {
        if (CanvasGroup)
        {
            CanvasGroup.blocksRaycasts = isActive;
            CanvasGroup.interactable = isActive;
        }
    }

    private void PlayNoneEffect()
    {
        if (CanvasGroup)
            CanvasGroup.alpha = 1;

        Invoke("OnEffectFinish", Time.deltaTime);
    }

    private void PlayLeftToRightEffect()
    {
        StartCoroutine(PlayLeftToRightEffectCore());
    }

    private IEnumerator PlayLeftToRightEffectCore()
    {
        yield return null;

        if (CanvasGroup)
            CanvasGroup.alpha = 1;

        Vector2 fromPosition;
        Vector2 toPosition;

        if (IsAppearing)
        {
            fromPosition = new Vector3(-RectTransform.rect.width, 0);
            toPosition = Vector2.zero;
        }
        else
        {
            fromPosition = Vector2.zero;
            toPosition = new Vector3(-RectTransform.rect.width, 0);
        }

        RectTransform.anchoredPosition = fromPosition;
        LeanTween.moveLocal(gameObject, toPosition, EffectTime).setOnComplete(OnEffectFinish);
    }

    private void PlayRightToLeftEffect()
    {
        StartCoroutine(PlayRightToLeftEffectCore());
    }

    private IEnumerator PlayRightToLeftEffectCore()
    {
        yield return null;

        if (CanvasGroup)
            CanvasGroup.alpha = 1;

        Vector2 fromPosition;
        Vector2 toPosition;

        if (IsAppearing)
        {
            fromPosition = new Vector3(RectTransform.rect.width, 0);
            toPosition = Vector2.zero;
        }
        else
        {
            fromPosition = Vector2.zero;
            toPosition = new Vector3(RectTransform.rect.width, 0);
        }

        RectTransform.anchoredPosition = fromPosition;
        LeanTween.moveLocal(gameObject, toPosition, EffectTime).setOnComplete(OnEffectFinish);
    }

    private void PlayTopToBottomEffect()
    {
        StartCoroutine(PlayTopToBottomEffectCore());
    }

    private IEnumerator PlayTopToBottomEffectCore()
    {
        yield return null;

        if (CanvasGroup)
            CanvasGroup.alpha = 1;

        Vector2 fromPosition;
        Vector2 toPosition;

        if (IsAppearing)
        {
            fromPosition = new Vector3(0, RectTransform.rect.height);
            toPosition = Vector2.zero;
        }
        else
        {
            fromPosition = Vector2.zero;
            toPosition = new Vector3(0, RectTransform.rect.height);
        }

        RectTransform.anchoredPosition = fromPosition;
        LeanTween.moveLocal(gameObject, toPosition, EffectTime).setOnComplete(OnEffectFinish);
    }

    private void PlayBottomToTopEffect()
    {
        StartCoroutine(PlayBottomToTopEffectCore());
    }

    private IEnumerator PlayBottomToTopEffectCore()
    {
        yield return null;

        if (CanvasGroup)
            CanvasGroup.alpha = 1;

        Vector2 fromPosition;
        Vector2 toPosition;

        if (IsAppearing)
        {
            fromPosition = new Vector3(0, -RectTransform.rect.height);
            toPosition = Vector2.zero;
        }
        else
        {
            fromPosition = Vector2.zero;
            toPosition = new Vector3(0, -RectTransform.rect.height);
        }

        RectTransform.anchoredPosition = fromPosition;
        LeanTween.moveLocal(gameObject, toPosition, EffectTime).setOnComplete(OnEffectFinish);
    }

    private void PlayZoomEffect()
    {
        StartCoroutine(PlayZoomEffectCore());
    }

    private IEnumerator PlayZoomEffectCore()
    {
        yield return null;

        if (CanvasGroup)
            CanvasGroup.alpha = 1;

        Vector3 fromScale;
        Vector3 toScale;

        if (IsAppearing)
        {
            fromScale = Vector3.zero;
            toScale = Vector3.one;
        }
        else
        {
            fromScale = Vector3.one;
            toScale = Vector3.zero;
        }

        RectTransform.localScale = fromScale;
        LeanTween.scale(gameObject, toScale, EffectTime).setOnComplete(OnEffectFinish);
    }

    private void PlayZoomFromChildEffect()
    {
        if (CanvasRectTransform == null)
        {
            PlayNoneEffect();
            return;
        }

        StartCoroutine(PlayZoomFromChildEffectCore());
    }

    private IEnumerator PlayZoomFromChildEffectCore()
    {
        yield break;
    }

    private void PlayTweenAlphaEffect()
    {
        if (CanvasGroup == null)
        {
            PlayNoneEffect();
            return;
        }

        StartCoroutine(PlayTweenAlphaEffectCore());
    }

    private IEnumerator PlayTweenAlphaEffectCore()
    {
        Debug.Log("PlayTweenAlphaEffectCore");

        yield return null;

        float fromAlpha;
        float toAlpha;

        if (IsAppearing)
        {
            fromAlpha = 0;
            toAlpha = 1;
        }
        else
        {
            fromAlpha = 1;
            toAlpha = 0;
        }

        if (CanvasGroup)
        {
            LeanTween.cancel(gameObject);
            CanvasGroup.alpha = fromAlpha;
            LeanTween.alphaCanvas(CanvasGroup, toAlpha, EffectTime).setOnComplete(OnEffectFinish);
        }
    }

    protected void OnEffectFinish()
    {
        if (RectTransform)
            RectTransform.ForceUpdateRectTransforms();

        gameObject.SetActive(IsAppearing);
        ActiveCanvas(IsAppearing);
        IsAnimating = false;
    }

    public virtual void SetStageEffect(EStateEffect stateEffect)
    {
        StateEffect = stateEffect;
    }

}