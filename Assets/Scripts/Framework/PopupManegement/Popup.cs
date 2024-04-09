using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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

        private TweenerCore<float, float, FloatOptions> _hoverAnimation;
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
            _popupCanvasGroup.transform.rotation = Quaternion.LookRotation(
                _popupCanvasGroup.transform.position - Camera.main.transform.position);
        }

        /// <summary>
        /// Shows the popup.
        /// </summary>
        public void Show()
        {
            StopPopupCoroutine();

            _popupCoroutine = StartCoroutine(ShowPopup());
        } 

        /// <summary>
        /// Closes the popup.
        /// </summary>
        public void Close()
        {
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

            if(_hoverAnimation == null || !_hoverAnimation.active)
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
            yield return _popupCanvasGroup.DOFade(0, _fadeDuration).SetEase(Ease.OutCubic).WaitForCompletion();

            _popupCanvasGroup.gameObject.SetActive(false);
        }
    }
}