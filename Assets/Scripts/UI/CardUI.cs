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

        private Choice _lastChoiceMade;

        /// <summary>
        /// Grabs the animator and disables it. Hides the choices text. Also subscribes to game events.
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.enabled = false;

            choiceOneText.alpha = 0;
            choiceTwoText.alpha = 0;

            GameEvent.OnCardSelected += DisplayCard;
            GameEvent.OnChoiceHover += FadeChoiceText;
            GameEvent.OnChoiceMade += SlideCardOut;
        }

        /// <summary>
        /// Disables mouse interaction with the cardTemplate and displays the cardTemplate's text and choices.
        /// </summary>
        /// <param name="cardTemplate">The cardTemplate that is to be displayed.</param>
        /// <param name="weekNumber">The current week number to be displayed.</param>
        private void DisplayCard(CardTemplate cardTemplate, int weekNumber)
        {
            MainSceneUI.SetCanRotateCard(false);
            cardText.text = cardTemplate.Scenario;
            choiceOneText.text = cardTemplate.Choices[0].ChoiceText;
            choiceTwoText.text = cardTemplate.Choices[1].ChoiceText;
            _animator.enabled = true;
            _animator.SetBool(SlidingIn, true);
        }

        /// <summary>
        /// Event that plays when the cardTemplate is done sliding in. Called by the animator.
        /// </summary>
        public void DisableAnimator()
        {
            _animator.enabled = false;
            MainSceneUI.SetCanRotateCard(true);
        }

        /// <summary>
        /// Starts the coroutine to fade the choice text in or out.
        /// </summary>
        /// <param name="choice">The choice which is being hovered over by the player.</param>
        /// <param name="enteringHover">True if the player's pointer is entering the hover area, false if it is exiting.
        /// </param>
        /// <param name="towardChoiceOne">True if the player's pointer is hovering over the first choice, false if the
        /// player's pointer is hovering over the second choice.</param>
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

        /// <summary>
        /// The coroutine that fades the choice text in or out.
        /// </summary>
        /// <param name="enteringHover">True if the player's pointer is entering the hover area, false if it is exiting.
        /// </param>
        /// <param name="choiceText">The choice text object.</param>
        /// <param name="towardChoiceOne">True if the player's pointer is hovering over the first choice, false if the
        /// player's pointer is hovering over the second choice.</param>
        /// <returns></returns>
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
        /// Resets cardTemplate state and slides the cardTemplate out of view when the player makes a choice.
        /// </summary>
        /// <param name="choice">The choice that the player made.</param>
        private void SlideCardOut(Choice choice)
        {
            _lastChoiceMade = choice;
            choiceOneText.alpha = 0;
            choiceTwoText.alpha = 0;
            _animator.enabled = true;
            _animator.SetBool(SlidingIn, false);
        }

        /// <summary>
        /// Calls the game manager to select a new cardTemplate to display. Called by the animator at the end of the slide out
        /// animation.
        /// </summary>
        public void SelectNewCard()
        {
            GameManager.SelectNewCard(_lastChoiceMade);
        }

        /// <summary>
        /// Unsubscribes from game events.
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnCardSelected -= DisplayCard;
            GameEvent.OnChoiceHover -= FadeChoiceText;
            GameEvent.OnChoiceMade -= SlideCardOut;
        }
    }
}