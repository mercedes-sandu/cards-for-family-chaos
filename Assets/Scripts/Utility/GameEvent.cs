using CCSS;

namespace Utility
{
    public static class GameEvent
    {
        // delegates

        /// <summary>
        /// 
        /// </summary>
        public delegate void CardSelectedHandler(Card card);
        
        /// <summary>
        /// 
        /// </summary>
        public delegate void HoverChoiceHandler(Choice choice);
        
        /// <summary>
        /// 
        /// </summary>
        public delegate void MakeChoiceHandler(Choice choice);
        
        // events
        
        /// <summary>
        /// 
        /// </summary>
        public static event CardSelectedHandler OnCardSelected;
        
        /// <summary>
        /// 
        /// </summary>
        public static event HoverChoiceHandler OnChoiceHover;
        
        /// <summary>
        /// 
        /// </summary>
        public static event MakeChoiceHandler OnChoiceMade;

        // static methods
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        public static void CardSelected(Card card) => OnCardSelected?.Invoke(card);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        public static void HoverChoice(Choice choice) => OnChoiceHover?.Invoke(choice);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        public static void MakeChoice(Choice choice) => OnChoiceMade?.Invoke(choice);
    }
}