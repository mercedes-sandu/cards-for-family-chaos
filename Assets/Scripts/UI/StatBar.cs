using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private bool randomizeStartValue = false;
        [SerializeField] private int minRandomStartValue;
        [SerializeField] private int maxRandomStartValue;

        private int _currentValue;
        private bool _dotCoroutineRunning = false;
        private Coroutine _dotCoroutine;

        private Animator _animator;

        private static readonly Dictionary<Player.Stat, int> StatToAnimBool = new()
        {
            {Player.Stat.Reputation, Animator.StringToHash("reputationFadeIn")},
            {Player.Stat.Money, Animator.StringToHash("moneyFadeIn")},
            {Player.Stat.Health, Animator.StringToHash("healthFadeIn")}
        };

        /// <summary>
        /// Sets the current value of the stat bar to the max value. Subscribes to game events.
        /// </summary>
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            
            _currentValue = randomizeStartValue ? Random.Range(minRandomStartValue, maxRandomStartValue) : maxValue;
            
            dot.color = new Color(dot.color.r, dot.color.g, dot.color.b, 0);

            GameEvent.OnChoiceHover += UpdateDot;
            GameEvent.OnChoiceMade += UpdateBar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetValue() => _currentValue;

        /// <summary>
        /// Fades the dot above the stat bar in or out depending on whether the stat corresponding to this stat bar is
        /// being modified by the choice the player is hovering over. If the stat is modified by the choice, then the
        /// dot will fade in. Otherwise, it will fade out.
        /// </summary>
        /// <param name="choice">The choice the player is hovering over.</param>
        /// <param name="enteringHover">True if the player's mouse pointer is entering the hover area, false otherwise.
        /// </param>
        /// <param name="towardChoiceOne">True if the player's mouse pointer is hovering over the left choice, false
        /// otherwise.</param>
        private void UpdateDot(Choice choice, bool enteringHover, bool towardChoiceOne)
        {
            int statModifier = choice.StatModifiers
                .Select(statModifier => statModifier.Stat == stat ? statModifier.Value : 0).Sum();

            if (_dotCoroutineRunning)
            {
                StopCoroutine(_dotCoroutine);
                _dotCoroutineRunning = false;
            }

            _dotCoroutine = StartCoroutine(FadeDot(enteringHover && statModifier != 0));
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
            float endValue = (float)_currentValue / maxValue;

            while (time < barMoveTime)
            {
                time += Time.deltaTime;
                bar.fillAmount = Mathf.Lerp(startValue, endValue, time / barMoveTime);
                yield return null;
            }
        }

        /// <summary>
        /// Fades in the stat bar's label when the player's mouse pointer hovers over the bar.
        /// </summary>
        public void PointerEnter()
        {
            _animator.SetBool(StatToAnimBool[stat], true);
        }

        /// <summary>
        /// Fades out the stat bar's label when the player's mouse pointer exits the bar.
        /// </summary>
        public void PointerExit()
        {
            _animator.SetBool(StatToAnimBool[stat], false);
        }

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