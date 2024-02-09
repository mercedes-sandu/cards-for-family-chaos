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

        private Animator _animator;
        private static readonly int SlidingIn = Animator.StringToHash("slidingIn");

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.enabled = false;
            
            GameEvent.OnCardSelected += DisplayCard;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        private void DisplayCard(Card card)
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
        
        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnCardSelected -= DisplayCard;
        }
    }
}