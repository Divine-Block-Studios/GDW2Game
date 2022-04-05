using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class GameSettings : MonoBehaviour
    {
        [Header("Settings UI")] [SerializeField]
        private TMP_InputField nameInputField;

        public string GetPlayerName => nameInputField.text;


        // Start is called before the first frame update
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            nameInputField.characterLimit = 12;
            nameInputField.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
            nameInputField.onValueChanged.AddListener((text) => nameInputField.text = text.ToUpper());
        }

        public void SaveSettings()
        {
            //Save everything
        }
    }
}
