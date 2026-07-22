using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectGenesis.Core
{
    public static class GameplayInputGate
    {
        public static bool IsTextEntryFocused
        {
            get
            {
                GameObject selected = EventSystem.current != null
                    ? EventSystem.current.currentSelectedGameObject
                    : null;
                InputField input = selected != null
                    ? selected.GetComponentInParent<InputField>()
                    : null;
                return input != null && input.isFocused;
            }
        }

        public static bool IsPointerOverUi =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
