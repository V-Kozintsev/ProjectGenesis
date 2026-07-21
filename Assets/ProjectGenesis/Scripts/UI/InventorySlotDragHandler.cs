using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectGenesis.UI
{
    [DisallowMultipleComponent]
    public sealed class InventorySlotDragHandler : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler
    {
        [SerializeField] private InventoryView inventoryView;
        [SerializeField, Min(0)] private int slotIndex;

        private CanvasGroup canvasGroup;
        private bool isDragging;

        public InventoryView InventoryView => inventoryView;
        public int SlotIndex => slotIndex;

        public void Initialize(InventoryView view, int inventorySlotIndex)
        {
            inventoryView = view;
            slotIndex = Mathf.Max(0, inventorySlotIndex);
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = inventoryView != null && inventoryView.CanDragSlot(slotIndex);
            if (!isDragging)
            {
                return;
            }

            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.58f;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 1f;
            }

            isDragging = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            InventorySlotDragHandler source = eventData.pointerDrag != null
                ? eventData.pointerDrag.GetComponent<InventorySlotDragHandler>()
                : null;
            if (source == null || !source.isDragging || source.inventoryView != inventoryView)
            {
                return;
            }

            inventoryView.TryMoveOrSwapSlots(source.slotIndex, slotIndex);
        }
    }
}
