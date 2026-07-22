using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class DeathRespawnView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text experienceLossText;
        [SerializeField] private Button villageRespawnButton;

        private UnityAction currentRespawnAction;

        public bool IsVisible => root != null && root.activeSelf;

        public void Initialize(
            GameObject windowRoot,
            Text lossLabel,
            Button respawnButton)
        {
            root = windowRoot;
            experienceLossText = lossLabel;
            villageRespawnButton = respawnButton;
            Hide();
        }

        private void OnDestroy()
        {
            ClearButtonAction();
        }

        public void Show(int experienceLoss, Action respawnAction)
        {
            if (root != null)
            {
                root.SetActive(true);
            }

            if (experienceLossText != null)
            {
                experienceLossText.text = experienceLoss > 0
                    ? $"Потеря опыта: {experienceLoss}"
                    : "Потери опыта нет";
            }

            ClearButtonAction();
            if (villageRespawnButton != null && respawnAction != null)
            {
                currentRespawnAction = () => respawnAction();
                villageRespawnButton.onClick.AddListener(currentRespawnAction);
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }

            ClearButtonAction();
        }

        private void ClearButtonAction()
        {
            if (villageRespawnButton != null && currentRespawnAction != null)
            {
                villageRespawnButton.onClick.RemoveListener(currentRespawnAction);
            }

            currentRespawnAction = null;
        }
    }
}
