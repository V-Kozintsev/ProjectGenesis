using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectGenesis.UI
{
    [DisallowMultipleComponent]
    public sealed class InventoryTrashDropTarget : MonoBehaviour, IDropHandler
    {
        [SerializeField] private InventoryView inventoryView;

        public InventoryView InventoryView => inventoryView;

        public void Initialize(InventoryView view)
        {
            inventoryView = view;
        }

        public void OnDrop(PointerEventData eventData)
        {
            InventorySlotDragHandler source = eventData.pointerDrag != null
                ? eventData.pointerDrag.GetComponent<InventorySlotDragHandler>()
                : null;
            if (source == null || inventoryView == null ||
                source.InventoryView != inventoryView)
            {
                return;
            }

            inventoryView.RequestDestroySlot(source.SlotIndex);
        }
    }
}
