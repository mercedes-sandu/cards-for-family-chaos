using System.Collections;
using CCSS;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class MainSceneUI : MonoBehaviour
    {
        [SerializeField] private RectTransform cardRectTransform;
        [SerializeField] private TextMeshProUGUI weekText;

        // card rotation toward mouse
        private static bool _canRotateCard = false;
        private const float MaxCardRotation = 10f;
        private Coroutine _cardRotationCoroutine;
        private bool _cardRotationCoroutineRunning = false;
        private Coroutine _cardResetCoroutine;
        private bool _cardResetCoroutineRunning = false;

        private Choice _lastMadeChoice;

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            GameEvent.OnCardSelected += UpdateWeek;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canRotateCard"></param>
        public static void SetCanRotateCard(bool canRotateCard) => _canRotateCard = canRotateCard;

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public void RotateCardTowardMouse(bool towardChoiceOne)
        {
            StopExistingCoroutines();
            GameEvent.HoverChoice(
                towardChoiceOne ? GameManager.CurrentCard.Choices[0] : GameManager.CurrentCard.Choices[1], true,
                towardChoiceOne);
            _cardRotationCoroutine = StartCoroutine(RotateCardTowardMouseCoroutine(towardChoiceOne));
        }

        public void ResetCardPositionNoChoice(bool towardChoiceOne)
        {
            StopExistingCoroutines();
            GameEvent.HoverChoice(
                towardChoiceOne ? GameManager.CurrentCard.Choices[0] : GameManager.CurrentCard.Choices[1], false,
                towardChoiceOne);
            _cardResetCoroutine = StartCoroutine(ResetCardPositionCoroutine(false));
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetCardPositionChoice(bool towardChoiceOne)
        {
            StopExistingCoroutines();
            GameEvent.HoverChoice(
                towardChoiceOne ? GameManager.CurrentCard.Choices[0] : GameManager.CurrentCard.Choices[1], false,
                towardChoiceOne);
            _cardResetCoroutine = StartCoroutine(ResetCardPositionCoroutine(true));
        }

        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        /// <param name="choiceMade"></param>
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
        /// 
        /// </summary>
        /// <param name="towardChoiceOne"></param>
        public void MakeChoice(bool towardChoiceOne)
        {
            _lastMadeChoice = GameManager.CurrentCard.Choices[towardChoiceOne ? 0 : 1];
            _canRotateCard = false;
            ResetCardPositionChoice(towardChoiceOne);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card">The card that is to be displayed.</param>
        /// <param name="weekNumber">The current week number to be displayed.</param>
        private void UpdateWeek(Card card, int weekNumber)
        {
            weekText.text = $"Week {weekNumber}";
        }

        // todo: implement pause button

        // todo: implement families button

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnCardSelected -= UpdateWeek;
        }
    }
}