using CCSS;
using UnityEngine;
using Utility;

namespace UI
{
    public class CardUI : MonoBehaviour
    {
        private void Awake()
        {
            GameEvent.OnCardSelected += DisplayCard;
        }
        
        private void DisplayCard(Card card)
        {
            // todo: play card exit animation
            // todo: update card ui
            // todo: play card entry animation
        }

        private void OnDestroy()
        {
            GameEvent.OnCardSelected -= DisplayCard;
        }
    }
}