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
        /// 
        /// </summary>
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="familyOne"></param>
        public void SetupNode(Character character, bool familyOne)
        {
            _character = character;
            _familyOne = familyOne;
            _button.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnClick()
        {
            SetupSceneUI.Instance.FamilyMemberNodeSelected(_character, _familyOne, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelected(bool selected)
        {
            _button.image.sprite = selected ? nodeSelected : nodeNormal;
        }
    }
}