using CCSS;

namespace Utility
{
    public static class GameEvent
    {
        // delegates

        /// <summary>
        /// Handles when a cardTemplate to display has been selected by the system.
        /// </summary>
        public delegate void CardSelectedHandler(CardTemplate cardTemplate, int weekNumber);
        
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
        /// Listens for when a cardTemplate to display has been selected by the system.
        /// </summary>
        public static event CardSelectedHandler OnCardSelected;
        
        /// <summary>
        /// Listens for when a choice is being hovered over by the player.
        /// </summary>
        public static event HoverChoiceHandler OnChoiceHover;
        
        /// <summary>
        /// Listens for when a choice is made by the player.
        /// </summary>
        public static event MakeChoiceHandler OnChoiceMade;

        // static methods
        
        /// <summary>
        /// Invoker for when a cardTemplate to display has been selected by the system.
        /// </summary>
        /// <param name="cardTemplate">The cardTemplate to be displayed by the system.</param>
        /// <param name="weekNumber">The number of the current week.</param>
        public static void CardSelected(CardTemplate cardTemplate, int weekNumber) => OnCardSelected?.Invoke(cardTemplate, weekNumber);
        
        /// <summary>
        /// Invoker for when a choice is being hovered over by the player.
        /// </summary>
        /// <param name="choice">The choice that is being hovered on by the player.</param>
        /// <param name="enteringHover">True if the player's pointer is entering the hover area, false if the player's
        /// pointer is exiting the hover area.</param>
        /// <param name="towardChoiceOne">True if the cardTemplate is rotating toward the first choice, false if the cardTemplate is
        /// rotating toward the second choice.</param>
        public static void HoverChoice(Choice choice, bool enteringHover, bool towardChoiceOne) => OnChoiceHover?.Invoke(choice, enteringHover, towardChoiceOne);
        
        /// <summary>
        /// Invoker for when a choice is made by the player.
        /// </summary>
        /// <param name="choice">The choice made by the player.</param>
        public static void MakeChoice(Choice choice) => OnChoiceMade?.Invoke(choice);
    }
}