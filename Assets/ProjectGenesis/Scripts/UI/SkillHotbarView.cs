using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.UI
{
    public sealed class SkillHotbarView : MonoBehaviour
    {
        [SerializeField] private Button[] slotButtons;
        [SerializeField] private Text[] slotLabels;
        [SerializeField] private Text feedbackText;
        [SerializeField] private PlayerSkillController skillController;

        private float feedbackHideTime;

        public void Initialize(
            Button[] buttons,
            Text[] labels,
            Text feedbackLabel,
            PlayerSkillController playerSkills)
        {
            slotButtons = buttons;
            slotLabels = labels;
            feedbackText = feedbackLabel;
            skillController = playerSkills;
        }

        private void Awake()
        {
            if (slotButtons != null)
            {
                for (int index = 0; index < slotButtons.Length; index++)
                {
                    int slotIndex = index;
                    if (slotButtons[index] != null)
                    {
                        slotButtons[index].onClick.AddListener(() => UseSlot(slotIndex));
                    }
                }
            }

            if (skillController != null)
            {
                skillController.Feedback += ShowFeedback;
            }

            RefreshSlots();
            HideFeedback();
        }

        private void Update()
        {
            RefreshSlots();

            if (feedbackText != null && feedbackText.gameObject.activeSelf && Time.time >= feedbackHideTime)
            {
                HideFeedback();
            }
        }

        private void OnDestroy()
        {
            if (slotButtons != null)
            {
                for (int index = 0; index < slotButtons.Length; index++)
                {
                    if (slotButtons[index] != null)
                    {
                        slotButtons[index].onClick.RemoveAllListeners();
                    }
                }
            }

            if (skillController != null)
            {
                skillController.Feedback -= ShowFeedback;
            }
        }

        private void UseSlot(int slotIndex)
        {
            skillController?.TryUseQuickSlot(slotIndex);
        }

        private void RefreshSlots()
        {
            if (slotButtons == null || skillController == null)
            {
                return;
            }

            for (int index = 0; index < slotButtons.Length; index++)
            {
                SkillDefinition skill = skillController.GetSkill(index);
                float remainingCooldown = skillController.GetRemainingCooldown(index);
                bool hasSkill = skill != null;
                bool isReady = hasSkill && remainingCooldown <= 0f;

                if (slotButtons[index] != null)
                {
                    slotButtons[index].interactable = isReady;
                    Image image = slotButtons[index].GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = isReady
                            ? new Color(0.18f, 0.27f, 0.34f, 1f)
                            : new Color(0.09f, 0.1f, 0.11f, 0.92f);
                    }
                }

                if (slotLabels != null && index < slotLabels.Length && slotLabels[index] != null)
                {
                    if (!hasSkill)
                    {
                        slotLabels[index].text = "-";
                    }
                    else if (remainingCooldown > 0f)
                    {
                        slotLabels[index].text = $"{Mathf.CeilToInt(remainingCooldown)}";
                    }
                    else
                    {
                        slotLabels[index].text = skill.DisplayName;
                    }
                }
            }
        }

        private void ShowFeedback(string message)
        {
            if (feedbackText == null)
            {
                return;
            }

            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
            feedbackHideTime = Time.time + 2.2f;
        }

        private void HideFeedback()
        {
            if (feedbackText != null)
            {
                feedbackText.gameObject.SetActive(false);
            }
        }
    }
}
