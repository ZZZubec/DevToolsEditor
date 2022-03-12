using System;
using Urho3DNet;

namespace GPE.Core.UI
{
    public enum SUI
    {
        UI,
        WINDOW,
        NODE
    }

    [ObjectFactory]
    public class SimpleUI : LogicComponent
    {
        private bool needUdate;
        private Image image;
        public EditorApplication app;

        public bool font1;
        private int font_width = 42;
        private int font_glyph_width = 7;
        private int font_glyph_height = 11;
        private Image font_image;

        public SimpleUI(Context context) : base(context)
        {
            image = new Image(context);
            image.SetSize(context.Graphics.Width, context.Graphics.Height, 4);
            UpdateEventMask = UpdateEvent.UseUpdate;
            needUdate = false;
            font1 = true;
            UpdateFont();
        }

        public void Resize(int width, int height)
        {
            image.SetSize(width, height, 4);
        }

        private void UpdateFont()
        {
            font_width = font1 ? Icons.font1_symbols_width : Icons.font2_symbols_width;
            font_glyph_width = font1 ? Icons.font1_glyph_width : Icons.font2_glyph_width;
            font_glyph_height = font1 ? Icons.font1_glyph_height : Icons.font2_glyph_height;
            font_image = font1 ? Icons.font1.GetImage() : Icons.font2.GetImage();
        }

        public override void Update(float timeStep)
        {

        }

        public void SetApp(EditorApplication app)
        {
            this.app = app;
        }

        public void Resize()
        {
            image.SetSize(Context.Instance.Graphics.Width, Context.Instance.Graphics.Height, 4);
            image.Clear(Color.Transparent);
            Render();
        }

        public void Render()
        {
            Render(Node, new IntVector2(0,0), null);
        }

        public void Render(Node node, IntVector2 offsetPosition, ISUI_Element elementParent)
        {
            IntRect rect = new IntRect(0, 0, image.Width, image.Height);
            NodeList nodeList = node.GetChildren(false);
            //Node.GetChildren(dest: nodeList, recursive: true);
            var interfaceType = typeof(SUI_Element);
            foreach (Node n in nodeList)
            {
                var comps = n.GetComponents();
                foreach (Component c in comps)
                {
                    if (interfaceType.IsInstanceOfType(c))
                    {
                        var element = (SUI_Element)c;
                        if (element != null)
                        {
                            app.LogInfo($"{elementParent} -> {element.spriteName}_{n.Name} ({offsetPosition.X}, {offsetPosition.Y})");
                            Image img = element.Render(IntVector2.Zero, null);
                            offsetPosition.X += elementParent != null ? elementParent.GetOffsetElement().X : 0;
                            offsetPosition.Y += elementParent != null ? elementParent.GetOffsetElement().X : 0;
                            
                            if (typeof(SUI_Window).IsInstanceOfType(element))
                            {
                                offsetPosition += ((SUI_Window)element).position;
                            }
                            
                            CopyRectFromImage(img, image, new IntRect(0,0, img.Width, img.Height), offsetPosition);
                        }
                    }
                }
            }
        }

        public SUI_Element AddUI(SUI element, string name, int[] args = null, SUI_Element nn = null)
        {
            needUdate = true;
            SUI_Element result = null;
            Node n = null;
            if (nn == null)
                n = Node.CreateChild(name);
            else
            {
                n = nn.Node.CreateChild(name);
                //nn.GetComponent<SUI_Element>().needUpdate = true;
            }

            switch (element)
            {
                default:
                    result = n.CreateComponent<SUI_Element>();
                    break;
                case SUI.WINDOW:
                    result = n.CreateComponent<SUI_Window>();
                    if (nn != null) ;
                        result.SetParent(nn);
                    if (args != null)
                        ((SUI_Window)result).SetArgs(args);
                    break;
                case SUI.NODE:
                    result = n.CreateComponent<SUI_Node>();
                    break;
            }

            result.simpleUI = this;
            return result;
        }

        public void SetImage(Image img)
        {
            this.image = img;
            Render();
        }

        public Image GetImage()
        {
            return image;
        }

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

        public int DrawText(Image imageElement, int x, int y, string text, bool withShadow = false)
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
                int xx = v % font_width;
                int yy = v / font_width;

                var pos = new IntVector2(x + oxx, y);
                var rec = new IntRect(
                    xx * font_glyph_width,
                    yy * font_glyph_height,
                    font_glyph_width,
                    font_glyph_height);

                if (withShadow)
                {
                    CopyRectFromImage(font_image, imageElement, rec, pos, c);
                    pos.X++;
                    //pos.Y--;
                }
                CopyRectFromImage(font_image, imageElement, rec, pos);
                oxx += font_glyph_width + offsetX;
            }
            return x + oxx;
        }
    }
}