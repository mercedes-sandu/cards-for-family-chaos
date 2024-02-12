using System.Collections;
using CCSS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class MainSceneUI : MonoBehaviour
    {
        [SerializeField] private RectTransform cardRectTransform;
        [SerializeField] private TextMeshProUGUI weekText;
        [SerializeField] private Animator compatibilityBarAnimator;

        // card rotation toward mouse
        private static bool _canRotateCard = false;
        private const float MaxCardRotation = 10f;
        private Coroutine _cardRotationCoroutine;
        private bool _cardRotationCoroutineRunning = false;
        private Coroutine _cardResetCoroutine;
        private bool _cardResetCoroutineRunning = false;

        private Choice _lastMadeChoice;

        private static readonly int FadeIn = Animator.StringToHash("fadeIn");

        /// <summary>
        /// Subscribes to game events.
        /// </summary>
        private void Awake()
        {
            GameEvent.OnCardSelected += UpdateWeek;
        }

        /// <summary>
        /// Sets the variable that determines whether the card can be rotated toward the mouse.
        /// </summary>
        /// <param name="canRotateCard">True if the card can be rotated toward the player's mouse position, false
        /// otherwise.</param>
        public static void SetCanRotateCard(bool canRotateCard) => _canRotateCard = canRotateCard;

        /// <summary>
        /// Stops any coroutines that are currently running.
        /// </summary>
        private void StopExistingCoroutines()
        {
            if (_cardResetCoroutineRunning)
            {
                StopCoroutine(_cardResetCoroutine);
                _cardResetCoroutineRunning = false;
            }

            if (_cardRotationCoroutineRunning)
            {
                StopCoroutine(_cardRotationCoroutine);
                _cardRotationCoroutineRunning = false;
            }
        }

        /// <summary>
        /// Starts the coroutine that rotates the card toward the player's mouse position.
        /// </summary>
        /// <param name="towardChoiceOne">True if the card is rotating toward the first choice, false if the card is
        /// rotating toward the second choice.</param>
        public void RotateCardTowardMouse(bool towardChoiceOne)
        {
            StopExistingCoroutines();
            GameEvent.HoverChoice(
                towardChoiceOne ? GameManager.CurrentCard.Choices[0] : GameManager.CurrentCard.Choices[1], true,
                towardChoiceOne);
            _cardRotationCoroutine = StartCoroutine(RotateCardTowardMouseCoroutine(towardChoiceOne));
        }

        /// <summary>
        /// Starts the coroutine that resets the card's rotation, while the player has not yet made a choice.
        /// </summary>
        /// <param name="towardChoiceOne">True if the card is rotating toward the first choice, false if the card is
        /// rotating toward the second choice.</param>
        public void ResetCardPositionNoChoice(bool towardChoiceOne)
        {
            StopExistingCoroutines();
            GameEvent.HoverChoice(
                towardChoiceOne ? GameManager.CurrentCard.Choices[0] : GameManager.CurrentCard.Choices[1], false,
                towardChoiceOne);
            _cardResetCoroutine = StartCoroutine(ResetCardPositionCoroutine(false));
        }

        /// <summary>
        /// Starts the coroutine that resets the card's rotation, after the player has made a choice.
        /// </summary>
        /// <param name="towardChoiceOne">True if the card is rotating toward the first choice, false if the card is
        /// rotating toward the second choice.</param>
        private void ResetCardPositionChoice(bool towardChoiceOne)
        {
            StopExistingCoroutines();
            GameEvent.HoverChoice(
                towardChoiceOne ? GameManager.CurrentCard.Choices[0] : GameManager.CurrentCard.Choices[1], false,
                towardChoiceOne);
            _cardResetCoroutine = StartCoroutine(ResetCardPositionCoroutine(true));
        }

        /// <summary>
        /// Rotates the card toward the player's mouse position.
        /// </summary>
        /// <param name="towardChoiceOne">True if the card is rotating toward the first choice, false if the card is
        /// rotating toward the second choice.</param>
        /// <returns></returns>
        private IEnumerator RotateCardTowardMouseCoroutine(bool towardChoiceOne)
        {
            _cardRotationCoroutineRunning = true;
            Quaternion targetRotation =
                Quaternion.Euler(0f, 0f, towardChoiceOne ? MaxCardRotation : -MaxCardRotation);

            while (cardRectTransform.rotation != targetRotation)
            {
                cardRectTransform.rotation = Quaternion.Lerp(cardRectTransform.rotation, targetRotation, 0.1f);
                yield return null;
            }

            _cardRotationCoroutineRunning = false;
            cardRectTransform.rotation = targetRotation;
        }

        /// <summary>
        /// Resets the card's rotation. If a choice has been made by the player, the corresponding game event is called.
        /// </summary>
        /// <param name="choiceMade">True if a choice has been made by the player, false otherwise.</param>
        /// <returns></returns>
        private IEnumerator ResetCardPositionCoroutine(bool choiceMade)
        {
            _cardResetCoroutineRunning = true;
            Quaternion targetRotation = Quaternion.identity;

            while (cardRectTransform.rotation != targetRotation)
            {
                cardRectTransform.rotation = Quaternion.Lerp(cardRectTransform.rotation, targetRotation, 0.1f);
                yield return null;
            }

            _cardResetCoroutineRunning = false;
            cardRectTransform.rotation = targetRotation;

            if (choiceMade)
            {
                GameEvent.MakeChoice(_lastMadeChoice);
            }
        }

        /// <summary>
        /// Sets the last choice made by the player and resets the card's rotation.
        /// </summary>
        /// <param name="towardChoiceOne">True if the card is rotating toward the first choice, false if the card is
        /// rotating toward the second choice.</param>
        public void MakeChoice(bool towardChoiceOne)
        {
            _lastMadeChoice = GameManager.CurrentCard.Choices[towardChoiceOne ? 0 : 1];
            _canRotateCard = false;
            ResetCardPositionChoice(towardChoiceOne);
        }

        /// <summary>
        /// Updates the text that displays the current week number.
        /// </summary>
        /// <param name="card">The card that is to be displayed.</param>
        /// <param name="weekNumber">The current week number to be displayed.</param>
        private void UpdateWeek(Card card, int weekNumber)
        {
            weekText.text = $"Week {weekNumber}";
        }

        /// <summary>
        /// Fades in the compatibility bar's label when the player's mouse pointer hovers over the bar.
        /// </summary>
        public void CompatibilityBarPointerEnter()
        {
            compatibilityBarAnimator.SetBool(FadeIn, true);
        }

        /// <summary>
        /// Fades out the compatibility bar's label when the player's mouse pointer exits the bar.
        /// </summary>
        public void CompatibilityBarPointerExit()
        {
            compatibilityBarAnimator.SetBool(FadeIn, false);
        }

        /// <summary>
        /// Pauses the game and displays the pause menu.
        /// </summary>
        public void PauseButton()
        {
        }

        /// <summary>
        /// Pauses the game and displays the families menu.
        /// </summary>
        public void FamiliesButton()
        {
        }

        /// <summary>
        /// Unsubscribes from game events.
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnCardSelected -= UpdateWeek;
        }
    }
}