using System.Collections;
using CCSS;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cardText;
        [SerializeField] private TextMeshProUGUI choiceOneText;
        [SerializeField] private TextMeshProUGUI choiceTwoText;

        [SerializeField] private float choiceTextFadeTime;

        private Animator _animator;
        private static readonly int SlidingIn = Animator.StringToHash("slidingIn");

        private bool _choiceOneCoroutineRunning = false;
        private Coroutine _choiceOneCoroutine;
        private bool _choiceTwoCoroutineRunning = false;
        private Coroutine _choiceTwoCoroutine;

        /// <summary>
        /// Grabs the animator and disables it. Hides the choices text. Also subscribes to the OnCardSelected event.
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.enabled = false;

            choiceOneText.alpha = 0;
            choiceTwoText.alpha = 0;

            GameEvent.OnCardSelected += DisplayCard;
            GameEvent.OnChoiceHover += FadeChoiceText;
        }

        /// <summary>
        /// Disables mouse interaction with the card and displays the card's text and choices.
        /// </summary>
        /// <param name="card">The card that is to be displayed.</param>
        /// <param name="weekNumber">The current week number to be displayed.</param>
        private void DisplayCard(Card card, int weekNumber)
        {
            MainSceneUI.SetCanRotateCard(false);
            cardText.text = card.Scenario;
            choiceOneText.text = card.Choices[0].ChoiceText;
            choiceTwoText.text = card.Choices[1].ChoiceText;
            _animator.enabled = true;
            _animator.SetBool(SlidingIn, true);
        }

        /// <summary>
        /// Event that plays when the card is done sliding in. Called by the animator.
        /// </summary>
        public void DisableAnimator()
        {
            _animator.enabled = false;
            MainSceneUI.SetCanRotateCard(true);
        }

        private void FadeChoiceText(Choice choice, bool enteringHover, bool towardChoiceOne)
        {
            if (towardChoiceOne)
            {
                if (_choiceOneCoroutineRunning)
                {
                    StopCoroutine(_choiceOneCoroutine);
                    _choiceOneCoroutineRunning = false;
                }

                _choiceOneCoroutine =
                    StartCoroutine(FadeChoiceTextCoroutine(enteringHover, choiceOneText, towardChoiceOne));
            }
            else
            {
                if (_choiceTwoCoroutineRunning)
                {
                    StopCoroutine(_choiceTwoCoroutine);
                    _choiceTwoCoroutineRunning = false;
                }

                _choiceTwoCoroutine =
                    StartCoroutine(FadeChoiceTextCoroutine(enteringHover, choiceTwoText, towardChoiceOne));
            }
        }

        private IEnumerator FadeChoiceTextCoroutine(bool enteringHover, TextMeshProUGUI choiceText,
            bool towardChoiceOne)
        {
            if (towardChoiceOne) _choiceOneCoroutineRunning = true;
            else _choiceTwoCoroutineRunning = true;

            float targetAlpha = enteringHover ? 1 : 0;
            float currentAlpha = choiceText.alpha;
            float timeElapsed = 0;

            while (timeElapsed < choiceTextFadeTime)
            {
                choiceText.alpha = Mathf.Lerp(currentAlpha, targetAlpha, timeElapsed / choiceTextFadeTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            choiceText.alpha = targetAlpha;

            if (towardChoiceOne) _choiceOneCoroutineRunning = false;
            else _choiceTwoCoroutineRunning = false;
        }

        /// <summary>
        /// Unsubscribes from the OnCardSelected event.
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnCardSelected -= DisplayCard;
            GameEvent.OnChoiceHover -= FadeChoiceText;
        }
    }
}