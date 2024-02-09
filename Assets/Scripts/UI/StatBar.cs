using System.Collections;
using System.Linq;
using CCSS;
using GameSetup;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField] private Image bar;
        [SerializeField] private Image dot;

        [SerializeField] private int minValue;
        [SerializeField] private int maxValue;

        [SerializeField] private Player.Stat stat;

        [SerializeField] private float barMoveTime;
        [SerializeField] private float dotFadeTime;

        private int _currentValue;

        /// <summary>
        /// Sets the current value of the stat bar to the max value. Subscribes to game events.
        /// </summary>
        private void Awake()
        {
            _currentValue = maxValue;

            GameEvent.OnChoiceHover += UpdateDot;
            GameEvent.OnChoiceMade += UpdateBar;
        }

        /// <summary>
        /// Fades the dot above the stat bar in or out depending on whether the stat corresponding to this stat bar is
        /// being modified by the choice the player is hovering over. If the stat is modified by the choice, then the
        /// dot will fade in. Otherwise, it will fade out.
        /// </summary>
        /// <param name="choice">The choice the player is hovering over.</param>
        private void UpdateDot(Choice choice)
        {
            int statModifier = choice.StatModifiers
                .Select(statModifier => statModifier.Stat == stat ? statModifier.Value : 0).Sum();

            StartCoroutine(FadeDot(statModifier != 0));
        }

        /// <summary>
        /// Updates the value of the stat bar based on the stat modifiers of the choice the player made. 
        /// </summary>
        /// <param name="choice">The choice the player made.</param>
        private void UpdateBar(Choice choice)
        {
            int statModifier = choice.StatModifiers
                .Select(statModifier => statModifier.Stat == stat ? statModifier.Value : 0).Sum();
            
            if (statModifier == 0) return;
            
            _currentValue = Mathf.Clamp(_currentValue + statModifier, minValue, maxValue);
            StartCoroutine(MoveBar());
        }

        /// <summary>
        /// Coroutine that fades the dot above the stat bar in or out.
        /// </summary>
        /// <param name="fadeIn">True if the dot is fading in, false otherwise.</param>
        /// <returns></returns>
        private IEnumerator FadeDot(bool fadeIn)
        {
            float time = 0f;
            Color startColor = dot.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, fadeIn ? 1 : 0);
            
            while (time < dotFadeTime)
            {
                time += Time.deltaTime;
                dot.color = Color.Lerp(startColor, endColor, time / dotFadeTime);
                yield return null;
            }
        }
        
        /// <summary>
        /// Coroutine that moves the stat bar to the new value.
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveBar()
        {
            float time = 0f;
            float startValue = bar.fillAmount;
            float endValue = (float) _currentValue / maxValue;

            while (time < barMoveTime)
            {
                time += Time.deltaTime;
                bar.fillAmount = Mathf.Lerp(startValue, endValue, time / barMoveTime);
                yield return null;
            }
        }
        
        // todo: add hover function for stat bar icon to display stat name

        /// <summary>
        /// Unsubscribes from game events.
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnChoiceHover -= UpdateDot;
            GameEvent.OnChoiceMade -= UpdateBar;
        }
    }
}