using CommandLine;
using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Urho3DNet;

namespace GPE.Core.UI
{

    public interface ISUI_Controler
    {
        //
    }

    public interface ISUI_Element
    {
        string spriteName { get; }

        IntVector2 offset { get; }

        Image Render(IntVector2 rectOffset, SUI_Element parentElement);
        IntRect GetClipRect();
        IntVector2 GetSize();

        IntVector2 GetOffsetElement();
    }

    [ObjectFactory]
    public class SUI_Element : Component, ISUI_Element
    {
        public bool needUpdate;
        public SimpleUI simpleUI;
        public int width;
        public int height;

        private string _spriteName;
        public string spriteName {
            get { return _spriteName; }
            set { _spriteName = value; }
        }
        public Image imageElement;

        public string name;

        public int minWidth = 16 + 5;
        public int minHeight = 12 + 5;
        private IntVector2 _offset;
        public IntVector2 offset {
            get { return _offset; }
            set { _offset = value; }
        }

        private SUI_Element parent;

        public SUI_Element(Context context) : base(context)
        {
            imageElement = new Image(context);
            imageElement.SetSize(100, 100, 4);
            needUpdate = true;
            spriteName = "SUI_Element";
            offset = new IntVector2(0,0);
            width = minWidth;
            height = minHeight;
        }

        public void SetParent(SUI_Element parent)
        {
            this.parent = parent;
        }

        public virtual Image Render(IntVector2 offsetPosition, SUI_Element parentElement)
        {
            //offset = new IntVector2(offsetPosition.X + position.X, offsetPosition.Y + position.Y);
            if (needUpdate)
            {
                if (imageElement.Width != width || imageElement.Height != height)
                {
                    imageElement.Resize(
                        width < minWidth ? minWidth : width,
                        height < minHeight ? minHeight : height);
                    imageElement.Clear(Color.Transparent);
                }
                Paint(parentElement);
                needUpdate = false;
            }

            return imageElement;
        }

        public virtual void Paint(SUI_Element parentElement) { }

        public Image GetImage()
        {
            return imageElement;
        }

        public void UpdateSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public virtual IntVector2 GetOffsetElement()
        {
            var offsetElement = IntVector2.Zero;
            if (this.parent != null)
                offsetElement += this.parent.GetOffsetElement();
            offsetElement += offset;
            return offsetElement;
        }

        public IntRect GetClipRect()
        {
            return new IntRect(0, 0, imageElement.Width, imageElement.Height);
        }

        public IntVector2 GetSize()
        {
            return new IntVector2(width, height);
        }
    }
}