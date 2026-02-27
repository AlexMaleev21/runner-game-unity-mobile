using UnityEngine;
using Zenject;

public abstract class BaseWindow : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _canvasGroup;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    public virtual void Hide()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }
}