using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace DerailedDeliveries.Framework.PopupManagement
{
    /// <summary>
    /// A class that is responsible for showing and hiding a popup.
    /// </summary>
    public class Popup : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _popupCanvasGroup;

        [SerializeField]
        private float _fadeDuration = 0.5f;

        [SerializeField]
        private float _hoverHeight = 1;

        [SerializeField]
        private float _hoverDuration = 1.5f;

        /// <summary>
        /// Gets whether the popup is showing or not.
        /// </summary>
        public bool IsShowing { get; private set; }
        
        /// <summary>
        /// Invoked when this popup is show.
        /// </summary>
        public Action OnShowPopup;

        /// <summary>
        /// Invoked when this popup is closed.
        /// </summary>
        public Action OnClosePopup;

        private Tween _hoverAnimation;
        private Coroutine _popupCoroutine;
        private float _initialYPosition;
        private float _endYPosition;


        private protected virtual void Awake()
        {
            _initialYPosition = _popupCanvasGroup.transform.localPosition.y;
            _endYPosition = _initialYPosition + _hoverHeight;

            _popupCanvasGroup.alpha = 0;
            _popupCanvasGroup.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if(UnityEngine.Camera.main == null)
                return;

            _popupCanvasGroup.transform.rotation = Quaternion.LookRotation(
                _popupCanvasGroup.transform.position - UnityEngine.Camera.main.transform.position);
        }

        /// <summary>
        /// Shows the popup.
        /// </summary>
        public void Show()
        {
            if(IsShowing)
                return;

            IsShowing = true;

            StopPopupCoroutine();
            _popupCoroutine = StartCoroutine(ShowPopup());
        } 

        /// <summary>
        /// Closes the popup.
        /// </summary>
        public void Close()
        {
            if(!IsShowing)
                return;

            IsShowing = false;

            StopPopupCoroutine();
            _popupCoroutine = StartCoroutine(ClosePopup());
        }

        private void StopPopupCoroutine()
        {
            if (_popupCoroutine == null)
                return;

            StopCoroutine(_popupCoroutine);
        }

        private protected virtual IEnumerator ShowPopup()
        {
            _popupCanvasGroup.gameObject.SetActive(true);
            OnShowPopup?.Invoke();

            if (_hoverAnimation == null || !_hoverAnimation.active)
            {
                _hoverAnimation = DOTween.To
                (
                    () => _initialYPosition,
                    y =>
                    {
                        _popupCanvasGroup.transform.localPosition = new Vector3
                        (
                            _popupCanvasGroup.transform.localPosition.x,
                            y,
                            _popupCanvasGroup.transform.localPosition.z
                        );
                    },
                    _endYPosition,
                    _hoverDuration
                )
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            }

            yield return _popupCanvasGroup.DOFade(1, _fadeDuration).SetEase(Ease.OutCubic).WaitForCompletion();
        }

        private protected virtual IEnumerator ClosePopup()
        {
            yield return _popupCanvasGroup.DOFade(0, _fadeDuration)
                .OnStart(() => OnClosePopup?.Invoke())
                .SetEase(Ease.OutCubic)
                .WaitForCompletion();

            _popupCanvasGroup.gameObject.SetActive(false);
        }
    }
}