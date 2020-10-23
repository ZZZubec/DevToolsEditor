using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Urho3DNet;

namespace GPE.Core.UI
{
    public interface ISUI_Element
    {
        void Render(Image image, IntRect rectOffset, SUI_Element parentElement);
        string GetSpriteName();
    }

    [ObjectFactory]
    public class SUI_Element : Component, ISUI_Element
    {
        public bool needUpdate;
        public SimpleUI simpleUI;
        public IntRect positionRect;
        public string spriteName;
        public Image imageElement;

        public string name;

        public IntRect clipRect;

        public int minWidth = 16 + 5;
        public int minHeight = 12 + 5;
        public IntRect offset;

        public SUI_Element(Context context) : base(context)
        {
            imageElement = new Image(context);
            imageElement.SetSize(100, 100, 4);
            needUpdate = true;
        }

        public void SetArgs(int[] args)
        {
            if (args.Length >= 4)
                positionRect = new IntRect(args[0], args[1], args[2], args[3]);
            else if (args.Length >= 2)
                positionRect = new IntRect(args[0], args[1], 0, 0);
        }

        public virtual void Render(Image image, IntRect rectOffset, SUI_Element parentElement)
        {
            var offsetVec = new IntVector2(rectOffset.Left + positionRect.Left, rectOffset.Top + positionRect.Top);
            offset = new IntRect(offsetVec.X, offsetVec.Y, rectOffset.Right, rectOffset.Bottom);
            if (needUpdate)
            {
                if (imageElement.Width != positionRect.Bottom || imageElement.Height != positionRect.Right)
                {
                    imageElement.Resize(
                        positionRect.Right < minWidth ? minWidth : positionRect.Right,
                        positionRect.Bottom < minHeight ? minHeight : positionRect.Bottom);
                    imageElement.Clear(Color.Transparent);
                }
                Paint(parentElement);
                needUpdate = false;
            }

            if (!needUpdate)
                CopyRectFromImage(imageElement, image, new IntRect(0, 0, imageElement.Width, imageElement.Height), offsetVec);

            if (Node.GetNumChildren(false) > 0)
                RenderChild(image);
        }

        public virtual void Paint(SUI_Element parentElement) { }

        public void CopyRectFromImage(Image imageSource, Image imageDest, IntRect source, IntVector2 dest, uint replaceColor = 128)
        {
            var xx = 0;
            for (float x = 0; x < source.Right; x++)
            {
                var yy = 0;
                for (float y = 0; y < source.Bottom; y++)
                {
                    var color = imageSource.GetPixel((int)(source.Left + x), (int)(source.Top + y));
                    if (color.ToVector4().W != 0)
                    {
                        if (replaceColor != 128)
                            imageDest.SetPixelInt((int)(dest.X + xx), (int)(dest.Y + yy), replaceColor);
                        else
                            imageDest.SetPixel((int)(dest.X + xx), (int)(dest.Y + yy), color);
                    }
                    yy++;
                }
                xx++;
            }
        }

        public string GetSpriteName()
        {
            return spriteName;
        }

        public Image GetImage()
        {
            return imageElement;
        }

        public virtual void RenderChild(Image image)
        {
            var nodeList = Node.GetChildren();
            var interfaceType = typeof(SUI_Element);
            foreach (Node n in nodeList)
            {
                var comps = n.GetComponents();
                foreach (Component c in comps)
                {
                    if (interfaceType.IsInstanceOfType(c))
                    {
                        var element = (ISUI_Element)c;
                        if (element != null)
                        {
                            element.Render(image, GetOffsetElement(), this);
                        }
                    }
                }
            }
        }

        public void UpdateSize(int width, int height)
        {
            positionRect.Right = width;
            positionRect.Bottom = height;
        }

        public virtual IntRect GetOffsetElement()
        {
            return offset;
        }
    }

    [ObjectFactory]
    public class SUI_Window : SUI_Element
    {
        public int border = 2;

        public SUI_Window(Context context) : base(context)
        {
            this.spriteName = "window";
        }

        public override void Paint(SUI_Element parentElement)
        {
            var UI = Icons.UI.GetImage();

            var x = 0;
            var y = 0;

            var rc = 5;
            var w = positionRect.Right - rc;
            var h = positionRect.Bottom - rc;
            //top
            {
                CopyRectFromImage(UI, imageElement, new IntRect(0, 0, 16, 8), new IntVector2(x, y));
                x += 16;

                var el_w = 50;
                int count = (int)Math.Floor(w / (double)el_w);
                for (int i = 0; i < count; i++)
                {
                    CopyRectFromImage(UI, imageElement, new IntRect(16, 0, el_w, 8), new IntVector2(x, y));
                    x += el_w;
                }
                if (x < w)
                {
                    var ww = w - x;
                    CopyRectFromImage(UI, imageElement, new IntRect(16, 0, ww, 8), new IntVector2(x, y));
                    x += ww;
                }

                CopyRectFromImage(UI, imageElement, new IntRect(75, 0, rc, 8), new IntVector2(x, y));
            }

            //height
            {
                y += 8;

                var el_w = 50;
                int count = (int)Math.Floor(w / (double)50d);
                int count2 = (int)Math.Floor(h / 4d);
                for (int j = 0; j < count2; j++)
                {
                    x = 0;
                    CopyRectFromImage(UI, imageElement, new IntRect(0, 8, 4, 4), new IntVector2(x, y));

                    x += 4;
                    for (int i = 0; i < count; i++)
                    {
                        CopyRectFromImage(UI, imageElement, new IntRect(4, 8, el_w, 4), new IntVector2(x, y));
                        x += el_w;
                    }
                    if (x < w)
                    {
                        var ww = w - x;
                        CopyRectFromImage(UI, imageElement, new IntRect(4, 8, ww, 4), new IntVector2(x, y));
                        x += ww;
                    }

                    CopyRectFromImage(UI, imageElement, new IntRect(75, 8, rc, 4), new IntVector2(x, y));
                    y += 4;
                }
                if (y < h)
                {
                    var wh = h - y;
                    //
                }
            }

            //bottom
            {
                x = 0;
                y = h;
                CopyRectFromImage(UI, imageElement, new IntRect(0, 11, 6, 5), new IntVector2(x, y));

                x = 6;
                int count = (int)Math.Floor(w / 20d);
                for (int i = 0; i < count; i++)
                {
                    CopyRectFromImage(UI, imageElement, new IntRect(6, 11, 20, 5), new IntVector2(x, y));
                    x += 20;
                }
                if (x < w)
                {
                    var ww = w - x;
                    CopyRectFromImage(UI, imageElement, new IntRect(6, 11, ww, 5), new IntVector2(x, y));
                    x += ww;
                }

                CopyRectFromImage(UI, imageElement, new IntRect(75, 11, rc, 5), new IntVector2(x, y));
            }

            //imageElement.SavePNG("cr_window.png");
        }

        public override IntRect GetOffsetElement()
        {
            return new IntRect(offset.Left + 5 + border, offset.Top + 8 + border, offset.Right - 5 * 2 - border * 2, offset.Bottom - 8 * 2 - border * 2);
        }
    }

    [ObjectFactory]
    public class SUI_Node : SUI_Window
    {
        public bool isExpand = true;
        bool isSelected = false;
        bool isHover = false;
        private Color selectedColor;
        private Color backgroundColor;

        public SUI_Node(Context context) : base(context)
        {
            this.spriteName = "node";
            var UI = Icons.UI.GetImage();
            selectedColor = UI.GetPixel(7, 18);
            backgroundColor = UI.GetPixel(2, 18);
        }

        public override void Paint(SUI_Element parentElement)
        {
            var UI = Icons.UI.GetImage();
            var x = 0;
            positionRect.Right = parentElement.positionRect.Right - positionRect.Left;
            positionRect.Bottom = Icons.font2_glyph_height < minHeight ? minHeight : Icons.font2_glyph_height;
            imageElement.SetSize(parentElement.positionRect.Right, positionRect.Bottom, 4);
            clipRect = new IntRect(0, 0, imageElement.Width, imageElement.Height);

            simpleUI.app.LogInfo($"node:{Node.Name}, wh:{imageElement.Width},{imageElement.Height}");

            if (isSelected || isHover)
            {
                imageElement.Clear(selectedColor);
            }
            else
                imageElement.Clear(Color.Transparent);

            if (Node.GetNumChildren(false) > 0)
            {
                if (isExpand)
                    CopyRectFromImage(UI, imageElement, new IntRect(10, 27, 9, 11), new IntVector2(0, 4));
                else
                    CopyRectFromImage(UI, imageElement, new IntRect(10, 16, 9, 11), new IntVector2(0, 4));
                x += 11 + 2;
            }
            x += DrawText(x, (imageElement.Height - Icons.font2_glyph_height) / 2, Node.Name, false);

            imageElement.SavePNG("cr_window.png");
        }

        public int DrawText(int x, int y, string text, bool withShadow = false)
        {
            var step = 1;
            var c = new Color(0.3f, 0.3f, 0.3f, 1f).ToUInt();
            var offsetX = 1;
            if (withShadow)
                 offsetX += 1;
            var oxx = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var v = Icons.font_symbols.IndexOf(text[i]);
                int xx = v % Icons.font2_symbols_width;
                int yy = v / Icons.font2_symbols_width;

                var pos = new IntVector2(x + oxx, y);
                var rec = new IntRect(
                    xx * Icons.font2_glyph_width,
                    yy * Icons.font2_glyph_height,
                    Icons.font2_glyph_width,
                    Icons.font2_glyph_height);

                if (withShadow)
                {
                    CopyRectFromImage(Icons.font2.GetImage(), imageElement, rec, pos, c);
                    pos.X++;
                    //pos.Y--;
                }
                CopyRectFromImage(Icons.font2.GetImage(), imageElement, rec, pos);
                oxx += Icons.font2_glyph_width + offsetX;
            }
            return x + oxx;
        }

        public override void RenderChild(Image image)
        {
            if (isExpand)
            {
                base.RenderChild(image);
            }
        }

        public override IntRect GetOffsetElement()
        {
            var b_of = base.GetOffsetElement();
            return new IntRect(b_of.Left + 12, b_of.Top + 6, b_of.Right, b_of.Bottom);
        }
    }
}