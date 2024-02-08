using CCSS;

namespace Utility
{
    public static class GameEvent
    {
        // delegates
        
        /// <summary>
        /// 
        /// </summary>
        public delegate void SelectCardHandler(); // todo: figure out how to pass current player/graph state to here
        
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
        public static event SelectCardHandler OnSelectCard;
        
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
        public static void SelectCard() => OnSelectCard?.Invoke();
        
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