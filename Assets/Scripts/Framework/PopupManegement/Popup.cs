using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DerailedDeliveries.Framework.PopupManagement
{
    public class Popup : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _popupCanvasGroup;

        public virtual IEnumerator ShowPopup()
        {
            yield return _popupCanvasGroup.DOFade(1, .5f).SetEase(Ease.OutCubic).WaitForCompletion();
        }

        public virtual IEnumerator ClosePopup()
        {
            yield return _popupCanvasGroup.DOFade(0, .5f).SetEase(Ease.OutCubic).WaitForCompletion();
        }
    }
}