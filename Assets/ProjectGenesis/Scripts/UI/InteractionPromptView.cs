using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text promptText;

        private void Awake()
        {
            Hide();
        }

        public void Initialize(GameObject promptRoot, Text text)
        {
            root = promptRoot;
            promptText = text;
            Hide();
        }

        public void Show(string targetName)
        {
            if (promptText != null)
            {
                promptText.text = $"Кликни по NPC: {targetName}";
            }

            if (root != null)
            {
                root.SetActive(true);
            }
        }

        public void ShowMessage(string message)
        {
            if (promptText != null)
            {
                promptText.text = message;
            }

            if (root != null)
            {
                root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }
    }
}
