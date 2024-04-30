using GameSetup;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class SetupSceneUI : MonoBehaviour
    {
        public static SetupSceneUI Instance = null;

        [SerializeField] private Button[] familyButtons;
        [SerializeField] private Button playAsCharacterButton;
        [SerializeField] private TextMeshProUGUI characterInformationText;

        [SerializeField] private Sprite activeTabNormal;
        [SerializeField] private Sprite activeTabHover;
        [SerializeField] private Sprite activeTabPressed;
        [SerializeField] private Sprite inactiveTabNormal;
        [SerializeField] private Sprite inactiveTabHover;
        [SerializeField] private Sprite inactiveTabPressed;

        [SerializeField] private Color32 activeTabTextColor;
        [SerializeField] private Color32 inactiveTabTextColor;

        private TextMeshProUGUI[] _familyNameTexts;
        private TextMeshProUGUI _playAsCharacterButtonText;

        private int _currentFamilyIndex = 1;

        private FamilyMemberNode _currentNode;
        private Character _currentPlayerCharacter;
        private bool _currentPlayerFamilyOne;

        private Canvas _canvas;

        /// <summary>
        /// Sets the instance and gets the button image components for the family tabs. 
        /// </summary>
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            _familyNameTexts = new TextMeshProUGUI[familyButtons.Length];
            for (var i = 0; i < familyButtons.Length; i++)
            {
                _familyNameTexts[i] = familyButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            }

            if (playAsCharacterButton)
            {
                playAsCharacterButton.image.enabled = false;
                _playAsCharacterButtonText = playAsCharacterButton.GetComponentInChildren<TextMeshProUGUI>();
                _playAsCharacterButtonText.enabled = false;
            }

            characterInformationText.text = "Click on a node to see more information about that character.";

            _canvas = GetComponent<Canvas>();
        }

        /// <summary>
        /// Sets the text of the family buttons to the names of the families.
        /// </summary>
        /// <param name="familyOneName">The first family's surname.</param>
        /// <param name="familyTwoName">The second family's surname.</param>
        public void SetupAllButtons(string familyOneName, string familyTwoName)
        {
            _familyNameTexts[0].text = $"{familyOneName} Family";
            _familyNameTexts[1].text = $"{familyTwoName} Family";
            _familyNameTexts[2].text = "Both Families";

            SetActiveButton(1);
        }

        /// <summary>
        /// Sets the current active tab. 
        /// </summary>
        /// <param name="index">The index of the active tab. Note that this index starts at 1, not 0.</param>
        public void SetActiveButton(int index)
        {
            familyButtons[_currentFamilyIndex - 1].image.sprite = inactiveTabNormal;
            familyButtons[_currentFamilyIndex - 1].spriteState = new SpriteState
            {
                highlightedSprite = inactiveTabHover,
                pressedSprite = inactiveTabPressed
            };
            _familyNameTexts[_currentFamilyIndex - 1].color = inactiveTabTextColor;

            _currentFamilyIndex = index;
            familyButtons[_currentFamilyIndex - 1].image.sprite = activeTabNormal;
            familyButtons[_currentFamilyIndex - 1].spriteState = new SpriteState
            {
                highlightedSprite = activeTabHover,
                pressedSprite = activeTabPressed
            };
            _familyNameTexts[_currentFamilyIndex - 1].color = activeTabTextColor;
        }

        /// <summary>
        /// Updates the character information text and enables the play as character button when a family member node is
        /// selected.
        /// </summary>
        /// <param name="characterSelected">The character that has been selected by the player.</param>
        /// <param name="familyOne">True if the selected character is in the first family, false if the selected
        /// character is in the second family.</param>
        /// <param name="nodeSelected">The node button pressed by the player.</param>
        public void FamilyMemberNodeSelected(Character characterSelected, bool familyOne, FamilyMemberNode nodeSelected)
        {
            if (playAsCharacterButton) _playAsCharacterButtonText.text = $"Play as {characterSelected.FirstName} {characterSelected.Surname}";
            characterInformationText.text =
                $"<b>{characterSelected.FirstName} {characterSelected.Surname}</b>\n \nAge: {characterSelected.Age}\nAlignment: {characterSelected.Alignment}\nPersonality Traits: {string.Join(", ", characterSelected.PersonalityTraits)}\nOccupation: {characterSelected.Occupation}";

            if (playAsCharacterButton && !playAsCharacterButton.image.enabled)
            {
                playAsCharacterButton.image.enabled = true;
                _playAsCharacterButtonText.enabled = true;
            }
            
            if (_currentNode) _currentNode.SetSelected(false);

            _currentNode = nodeSelected;
            _currentPlayerCharacter = characterSelected;
            _currentPlayerFamilyOne = familyOne;
            _currentNode.SetSelected(true);
        }

        /// <summary>
        /// Sets the player's character and family to the currently selected character and family, loads the main game.
        /// </summary>
        public void PlayAsButtonPressed()
        {
            Player.PlayerCharacter = _currentPlayerCharacter;
            Player.PlayerCharacterFamilyOne = _currentPlayerFamilyOne;
            SceneManager.LoadScene("MainScene");
        }

        /// <summary>
        /// Called by the close button in the families menu.
        /// </summary>
        public void CloseFamiliesMenu()
        {
            _canvas.enabled = false;
        }
    }
}