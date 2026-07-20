using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectGenesis.UI
{
    [DisallowMultipleComponent]
    public sealed class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform targetWindow;
        [SerializeField, Min(24f)] private float minimumVisibleWidth = 140f;
        [SerializeField, Min(24f)] private float minimumVisibleHeaderHeight = 56f;

        private Canvas rootCanvas;

        public RectTransform TargetWindow => targetWindow;

        public void Initialize(RectTransform window)
        {
            targetWindow = window;
        }

        private void Awake()
        {
            rootCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (targetWindow != null)
            {
                targetWindow.SetAsLastSibling();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (targetWindow == null)
            {
                return;
            }

            float scaleFactor = rootCanvas != null ? rootCanvas.scaleFactor : 1f;
            targetWindow.anchoredPosition += eventData.delta / Mathf.Max(0.01f, scaleFactor);
            KeepWindowReachable();
        }

        private void KeepWindowReachable()
        {
            RectTransform bounds = targetWindow.parent as RectTransform;
            if (bounds == null)
            {
                return;
            }

            Vector3[] windowCorners = new Vector3[4];
            Vector3[] boundsCorners = new Vector3[4];
            targetWindow.GetWorldCorners(windowCorners);
            bounds.GetWorldCorners(boundsCorners);

            float horizontalCorrection = 0f;
            float verticalCorrection = 0f;
            float windowWidth = windowCorners[2].x - windowCorners[0].x;
            float boundsWidth = boundsCorners[2].x - boundsCorners[0].x;

            if (windowWidth <= boundsWidth)
            {
                if (windowCorners[0].x < boundsCorners[0].x)
                {
                    horizontalCorrection = boundsCorners[0].x - windowCorners[0].x;
                }
                else if (windowCorners[2].x > boundsCorners[2].x)
                {
                    horizontalCorrection = boundsCorners[2].x - windowCorners[2].x;
                }
            }
            else if (windowCorners[2].x < boundsCorners[0].x + minimumVisibleWidth)
            {
                horizontalCorrection =
                    boundsCorners[0].x + minimumVisibleWidth - windowCorners[2].x;
            }
            else if (windowCorners[0].x > boundsCorners[2].x - minimumVisibleWidth)
            {
                horizontalCorrection =
                    boundsCorners[2].x - minimumVisibleWidth - windowCorners[0].x;
            }

            float windowHeight = windowCorners[1].y - windowCorners[0].y;
            float boundsHeight = boundsCorners[1].y - boundsCorners[0].y;
            if (windowHeight <= boundsHeight)
            {
                if (windowCorners[0].y < boundsCorners[0].y)
                {
                    verticalCorrection = boundsCorners[0].y - windowCorners[0].y;
                }
                else if (windowCorners[1].y > boundsCorners[1].y)
                {
                    verticalCorrection = boundsCorners[1].y - windowCorners[1].y;
                }
            }
            else if (windowCorners[1].y < boundsCorners[0].y + minimumVisibleHeaderHeight)
            {
                verticalCorrection =
                    boundsCorners[0].y + minimumVisibleHeaderHeight - windowCorners[1].y;
            }
            else if (windowCorners[1].y > boundsCorners[1].y)
            {
                verticalCorrection = boundsCorners[1].y - windowCorners[1].y;
            }

            Vector3 localCorrection = bounds.InverseTransformVector(
                new Vector3(horizontalCorrection, verticalCorrection, 0f));
            targetWindow.anchoredPosition += new Vector2(localCorrection.x, localCorrection.y);
        }
    }
}
