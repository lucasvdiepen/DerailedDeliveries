using System.Collections;
using UnityEngine;

namespace DerailedDeliveries.Framework.StateMachine
{
    /// <summary>
    /// A class that represents a menu state in the state machine.
    /// </summary>
    public abstract class MenuState : State
    {
        /// <summary>
        /// The canvas group that is used to fade in and out.
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasGroup;

        /// <summary>
        /// The duration of the fade effect.
        /// </summary>
        [SerializeField]
        private float _fadeDuration = 0.5f;

        /// <summary>
        /// <inheritdoc/>
        /// Menu state also sets the alpha to 0.
        /// </summary>
        private protected override void Awake()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            base.Awake();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateEnter()
        {
            yield return base.OnStateEnter();

            yield return FadeIn();

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateExit()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
            yield return FadeOut();

            yield return base.OnStateExit();
        }

        /// <summary>
        /// Fades in the menu.
        /// </summary>
        private protected virtual IEnumerator FadeIn() => ExecuteFadeEffect(0, 1);

        /// <summary>
        /// Fades out the menu.
        /// </summary>
        private protected virtual IEnumerator FadeOut() => ExecuteFadeEffect(1, 0);

        /// <summary>
        /// Executes the fade effect.
        /// </summary>
        /// <param name="fromAlpha">From alpha value.</param>
        /// <param name="targetAlpha">Target alpha value.</param>
        private IEnumerator ExecuteFadeEffect(float fromAlpha, float targetAlpha)
        {
            float elapsedTime = 0;
            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(fromAlpha, targetAlpha, elapsedTime / _fadeDuration);
                yield return null;
            }
        }
    }
}