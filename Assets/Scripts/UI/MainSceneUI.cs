using System.Collections;
using CCSS;
using UnityEngine;
using Utility;

namespace UI
{
    public class MainSceneUI : MonoBehaviour
    {
        [SerializeField] private RectTransform cardRectTransform;

        // card rotation toward mouse
        private static bool _canRotateCard = false;
        private const float MaxCardRotation = 10f;
        private bool _mouseOnRightSide = false;
        private bool _mouseInCardArea = false;
        private Coroutine _cardRotationCoroutine;
        private bool _cardRotationCoroutineRunning = false;
        private Coroutine _cardResetCoroutine;
        private bool _cardResetCoroutineRunning = false;
        
        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (!_canRotateCard) return;
            if (!_mouseInCardArea) return;
            CheckSideFlip();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canRotateCard"></param>
        public static void SetCanRotateCard(bool canRotateCard) => _canRotateCard = canRotateCard;

        /// <summary>
        /// 
        /// </summary>
        private void CheckSideFlip()
        {
            if ((!_mouseOnRightSide || !(Input.mousePosition.x < cardRectTransform.position.x)) &&
                (_mouseOnRightSide || !(Input.mousePosition.x > cardRectTransform.position.x))) return;
            _mouseOnRightSide = !_mouseOnRightSide;
            _cardRotationCoroutine = StartCoroutine(RotateCardTowardMouseCoroutine());
        }

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

        // todo: fix weird rotation stuff
        /// <summary>
        /// 
        /// </summary>
        public void RotateCardTowardMouse()
        {
            StopExistingCoroutines();
            _mouseOnRightSide = Input.mousePosition.x > cardRectTransform.position.x;
            _mouseInCardArea = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetCardPosition()
        {
            _mouseInCardArea = false;

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

            _cardResetCoroutine = StartCoroutine(ResetCardPositionCoroutine());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateCardTowardMouseCoroutine()
        {
            _cardRotationCoroutineRunning = true;

            while (cardRectTransform.rotation !=
                   Quaternion.Euler(0f, 0f, _mouseOnRightSide ? MaxCardRotation : -MaxCardRotation))
            {
                cardRectTransform.rotation = Quaternion.Lerp(cardRectTransform.rotation,
                    Quaternion.Euler(0f, 0f, _mouseOnRightSide ? MaxCardRotation : -MaxCardRotation), 0.1f);
                yield return null;
            }

            _cardRotationCoroutineRunning = false;
            cardRectTransform.rotation = 
                Quaternion.Euler(0f, 0f, _mouseOnRightSide ? MaxCardRotation : -MaxCardRotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResetCardPositionCoroutine()
        {
            _cardResetCoroutineRunning = true;

            while (cardRectTransform.rotation != Quaternion.identity)
            {
                cardRectTransform.rotation = Quaternion.Lerp(cardRectTransform.rotation, Quaternion.identity, 0.1f);
                yield return null;
            }

            _cardResetCoroutineRunning = false;
            cardRectTransform.rotation = Quaternion.identity;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void MakeChoice()
        {
            // todo: turn into on click event
            if (!_mouseInCardArea) return;
            Choice[] choices = GameManager.CurrentCard.Choices;
            GameEvent.MakeChoice(choices[_mouseOnRightSide ? 1 : 0]);
            GameManager.SelectNewCard();
        }
    }
}