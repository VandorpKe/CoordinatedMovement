using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace RTS.InputManager
{
    public static class DragSelect
    {
        private static Texture2D selectionTexture;

        public static Texture2D SelectionTexture
        {
            get 
            { 
                if(selectionTexture == null)
                {
                    selectionTexture = new Texture2D(1, 1);
                    selectionTexture.SetPixel(0, 0, Color.white);
                    selectionTexture.Apply();
                }

                return selectionTexture;
            }
        }

        public static void DrawSelectionRect( Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, SelectionTexture);
        }

        public static void DrawSelectionRectBorder(Rect rect, float thickness, Color color)
        {
            // Top border
            DrawSelectionRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Bottom border
            DrawSelectionRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
            // Left border
            DrawSelectionRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right border
            DrawSelectionRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        }

        public static Rect GetScreenRect(Vector3 screenPos1, Vector3 screenPos2)
        {
            // Bottom right to top left
            screenPos1.y = Screen.height - screenPos1.y;
            screenPos2.y = Screen.height - screenPos2.y;

            // Corners
            Vector3 botRight = Vector3.Max(screenPos1, screenPos2);
            Vector3 topLeft = Vector3.Min(screenPos1, screenPos2);

            // Create rectangle
            return Rect.MinMaxRect(topLeft.x, topLeft.y, botRight.x, botRight.y);
        }

        // Use to check if object is inside these bounds
        public static Bounds GetViewPortBounds(Camera camera, Vector3 screenPos1, Vector3 screenPos2)
        {
            Vector3 pos1 = camera.ScreenToViewportPoint(screenPos1);
            Vector3 pos2 = camera.ScreenToViewportPoint(screenPos2);

            Vector3 min = Vector3.Min(pos1, pos2);
            Vector3 max = Vector3.Max(pos1, pos2);

            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            return bounds;
        }
    }
}
