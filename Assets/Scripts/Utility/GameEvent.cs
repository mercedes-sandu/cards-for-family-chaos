using CCSS;

namespace Utility
{
    public static class GameEvent
    {
        // delegates

        /// <summary>
        /// Handles when a card to display has been selected by the system.
        /// </summary>
        public delegate void CardSelectedHandler(Card card, int weekNumber);
        
        /// <summary>
        /// Handles when a choice is being hovered over by the player.
        /// </summary>
        public delegate void HoverChoiceHandler(Choice choice, bool enteringHover, bool towardChoiceOne);
        
        /// <summary>
        /// Handles when a choice is made by the player.
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
        /// <param name="weekNumber"></param>
        public static void CardSelected(Card card, int weekNumber) => OnCardSelected?.Invoke(card, weekNumber);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="enteringHover"></param>
        /// <param name="towardChoiceOne"></param>
        public static void HoverChoice(Choice choice, bool enteringHover, bool towardChoiceOne) => OnChoiceHover?.Invoke(choice, enteringHover, towardChoiceOne);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        public static void MakeChoice(Choice choice) => OnChoiceMade?.Invoke(choice);
    }
}