using System.Collections;
using CCSS;
using GameSetup;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class MainSceneUI : MonoBehaviour
    {
        [SerializeField] private RectTransform cardRectTransform;
        [SerializeField] private TextMeshProUGUI weekText;

        [SerializeField] private Image compatibilityBar;

        [SerializeField] private int minCompatibilityValue;
        [SerializeField] private int maxCompatibilityValue;
        [SerializeField] private float compatibilityBarMoveTime;

        [SerializeField] private Canvas pauseMenu;
        [SerializeField] private Canvas familiesMenu;

        private int _currentCompatibilityValue;

        // card rotation toward mouse
        private const float MaxCardRotation = 10f;
        private Coroutine _cardRotationCoroutine;
        private bool _cardRotationCoroutineRunning = false;
        private Coroutine _cardResetCoroutine;
        private bool _cardResetCoroutineRunning = false;

        private Choice _lastMadeChoice;

        private Animator _compatibilityBarAnimator;
        private static readonly int FadeIn = Animator.StringToHash("fadeIn");

        /// <summary>
        /// Subscribes to game events.
        /// </summary>
        private void Awake()
        {
            _compatibilityBarAnimator = compatibilityBar.transform.parent.parent.GetComponent<Animator>();
            pauseMenu.enabled = false;
            familiesMenu.enabled = false;

            GameEvent.OnCardSelected += UpdateWeek;
            GameEvent.OnChoiceMade += UpdateCompatibilityBar;
        }

        /// <summary>
        /// Sets the family compatibility.
        /// </summary>
        private void Start()
        {
            _currentCompatibilityValue = GameManager.CurrentCompatibility;
            compatibilityBar.fillAmount = (float)_currentCompatibilityValue / maxCompatibilityValue;
            
            SetupSceneUI.Instance.SetupAllButtons(SetupMaster.FamilyOneSurname, SetupMaster.FamilyTwoSurname);
        }

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
                towardChoiceOne ? GameManager.CurrentCard.ChoiceOne : GameManager.CurrentCard.ChoiceTwo, true,
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
                towardChoiceOne ? GameManager.CurrentCard.ChoiceOne : GameManager.CurrentCard.ChoiceTwo, false,
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
                towardChoiceOne ? GameManager.CurrentCard.ChoiceOne : GameManager.CurrentCard.ChoiceTwo, false,
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
            _lastMadeChoice = towardChoiceOne ? GameManager.CurrentCard.ChoiceOne : GameManager.CurrentCard.ChoiceTwo;
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
            _compatibilityBarAnimator.SetBool(FadeIn, true);
        }

        /// <summary>
        /// Fades out the compatibility bar's label when the player's mouse pointer exits the bar.
        /// </summary>
        public void CompatibilityBarPointerExit()
        {
            _compatibilityBarAnimator.SetBool(FadeIn, false);
        }

        /// <summary>
        /// Updates the compatibility bar's value based on the choice made by the player.
        /// </summary>
        /// <param name="choice">The choice made by the player.</param>
        private void UpdateCompatibilityBar(Choice choice)
        {
            int compatibilityModifier = choice.CompatibilityModifier;
            if (compatibilityModifier == 0) return;

            _currentCompatibilityValue = Mathf.Clamp(_currentCompatibilityValue + compatibilityModifier,
                minCompatibilityValue, maxCompatibilityValue);
            StartCoroutine(MoveBar());
        }

        /// <summary>
        /// Coroutine that moves the compatibility bar to the new value.
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveBar()
        {
            float time = 0f;
            float startValue = compatibilityBar.fillAmount;
            float endValue = (float)_currentCompatibilityValue / maxCompatibilityValue;

            while (time < compatibilityBarMoveTime)
            {
                time += Time.deltaTime;
                compatibilityBar.fillAmount = Mathf.Lerp(startValue, endValue, time / compatibilityBarMoveTime);
                yield return null;
            }
            
            // todo: check for game over conditions if compatibility hits max
        }

        /// <summary>
        /// Pauses the game and displays the pause menu.
        /// </summary>
        public void PauseButton()
        {
            Time.timeScale = 0;
            pauseMenu.enabled = true;
        }

        /// <summary>
        /// Pauses the game and displays the families menu.
        /// </summary>
        public void FamiliesButton()
        {
            Time.timeScale = 0;
            familiesMenu.enabled = true;
        }

        /// <summary>
        /// Called by the resume button in the pause menu.
        /// </summary>
        public void ResumeButton()
        {
            Time.timeScale = 1;
            pauseMenu.enabled = false;
        }

        /// <summary>
        /// Called by the quit button in the pause menu.
        /// </summary>
        public void QuitButton()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("StartMenu");
            
        }

        /// <summary>
        /// Unsubscribes from game events.
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnCardSelected -= UpdateWeek;
            GameEvent.OnChoiceMade -= UpdateCompatibilityBar;
        }
    }
}