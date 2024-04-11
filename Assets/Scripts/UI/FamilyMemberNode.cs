using GameSetup;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FamilyMemberNode : MonoBehaviour
    {
        [SerializeField] private Sprite nodeNormal;
        [SerializeField] private Sprite nodeSelected;
        
        private Button _button;
        private Character _character;
        private bool _familyOne;
        
        /// <summary>
        /// Gets the button component.
        /// </summary>
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        /// <summary>
        /// Sets the node button's information and creates an onClick listener.
        /// </summary>
        /// <param name="character">The character corresponding to this node.</param>
        /// <param name="familyOne">True if the character is from the first family, false if the player is from the
        /// second family.</param>
        public void SetupNode(Character character, bool familyOne)
        {
            _character = character;
            _familyOne = familyOne;
            _button.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// Updates the current player-selected character.
        /// </summary>
        private void OnClick()
        {
            SetupSceneUI.Instance.FamilyMemberNodeSelected(_character, _familyOne, this);
        }

        /// <summary>
        /// Changes the node's appearance to reflect whether it is selected or not.
        /// </summary>
        /// <param name="selected">True if the node has been selected by the player, false otherwise.</param>
        public void SetSelected(bool selected)
        {
            _button.image.sprite = selected ? nodeSelected : nodeNormal;
        }
    }
}