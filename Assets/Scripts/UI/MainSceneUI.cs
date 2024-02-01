using System.Collections;
using UnityEngine;

namespace UI
{
    public class MainSceneUI : MonoBehaviour
    {
        [SerializeField] private RectTransform cardRectTransform;

        private const float MaxCardRotation = 10f;
        private bool _mouseOnRightSide = false;
        private bool _mouseInCardArea = false;
        private Coroutine _cardRotationCoroutine;
        private bool _cardRotationCoroutineRunning = false;
        private Coroutine _cardResetCoroutine;
        private bool _cardResetCoroutineRunning = false;

        private void Update()
        {
            if (!_mouseInCardArea) return;
            CheckSideFlip();
        }

        private void CheckSideFlip()
        {
            if ((!_mouseOnRightSide || !(Input.mousePosition.x < cardRectTransform.position.x)) &&
                (_mouseOnRightSide || !(Input.mousePosition.x > cardRectTransform.position.x))) return;
            _mouseOnRightSide = !_mouseOnRightSide;
            _cardRotationCoroutine = StartCoroutine(RotateCardTowardMouseCoroutine());
        }

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

        public void RotateCardTowardMouse()
        {
            StopExistingCoroutines();
            _mouseOnRightSide = Input.mousePosition.x > cardRectTransform.position.x;
            _mouseInCardArea = true;
        }

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
    }
}