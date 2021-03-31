using UnityEngine;
using UnityEngine.UI;

namespace VisualFeedback
{
    public class VisualFeedback : MonoBehaviour
    {
        public GameObject imageObject;
        Image image;
        Texture2D texture;

        public void Start()
        {
            texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
            image = imageObject.GetComponent<Image>();
        }

        public void DrawTarget(Point target)
        {
            Color drawColor = Color.red;
            Draw(target, drawColor);
        }

        public void DrawPath(Point target)
        {
            Color drawColor = Color.black;
            Draw(target, drawColor);
        }

        public void FinishDrawing()
        {
            texture.Apply();
            image.sprite = Sprite.Create(texture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f));
        }

        public void ResetTexture()
        {
            texture = new Texture2D(Screen.width, Screen.height);
        }

        //Not sure if neccessary
        public void ClearDrawing()
        {
            for(int x = 0; x < Screen.width; x++)
            {
                for(int y = 0; y < Screen.height; y++)
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
            texture.Apply();
            image.sprite = Sprite.Create(texture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0.5f, 0.5f));
        }

        void Draw(Point target, Color color)
        {
            int x = target.x;
            int y = target.y;
            texture.SetPixel(x, y, color);
        }
    }
}
