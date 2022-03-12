using System;
using Urho3DNet;

namespace GPE.Core.UI
{
    [ObjectFactory]
    public class SUI_Window : SUI_Element, ISUI_Controler
    {
        public int border = 2;
        public IntVector2 position;
        private SUI_Container container;

        public SUI_Window(Context context) : base(context)
        {
            this.spriteName = "SUI_Window";
            offset = new IntVector2(5 + border, 8 + border);
        }

        public override void Paint(SUI_Element parentElement)
        {
            var UI = Icons.UI.GetImage();

            var x = 0;
            var y = 0;

            var rc = 5;
            var w = width - rc;
            var h = height - rc;
            //top
            {
                simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(0, 0, 16, 8), new IntVector2(x, y));
                x += 16;

                var el_w = 50;
                int count = (int)Math.Floor(w / (double)el_w);
                for (int i = 0; i < count; i++)
                {
                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(16, 0, el_w, 8), new IntVector2(x, y));
                    x += el_w;
                }
                if (x < w)
                {
                    var ww = w - x;
                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(16, 0, ww, 8), new IntVector2(x, y));
                    x += ww;
                }

                simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(75, 0, rc, 8), new IntVector2(x, y));
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
                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(0, 8, 4, 4), new IntVector2(x, y));

                    x += 4;
                    for (int i = 0; i < count; i++)
                    {
                        simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(4, 8, el_w, 4), new IntVector2(x, y));
                        x += el_w;
                    }
                    if (x < w)
                    {
                        var ww = w - x;
                        simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(4, 8, ww, 4), new IntVector2(x, y));
                        x += ww;
                    }

                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(75, 8, rc, 4), new IntVector2(x, y));
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
                simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(0, 11, 6, 5), new IntVector2(x, y));

                x = 6;
                int count = (int)Math.Floor(w / 20d);
                for (int i = 0; i < count; i++)
                {
                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(6, 11, 20, 5), new IntVector2(x, y));
                    x += 20;
                }
                if (x < w)
                {
                    var ww = w - x;
                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(6, 11, ww, 5), new IntVector2(x, y));
                    x += ww;
                }

                simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(75, 11, rc, 5), new IntVector2(x, y));
            }

            if (container == null)
                container = Node.CreateComponent<SUI_Container>();
            container.Render(new IntVector2(0, 0), this);
            if (Node.GetNumChildren(false) > 0)
            {
                simpleUI.app.LogInfo("window render:");
                Render(Node, offset);
                simpleUI.app.LogInfo("window end.");
            }
            simpleUI.CopyRectFromImage(container.imageElement, imageElement, new IntRect(0, 0, container.width, container.height), new IntVector2(0, 0));
            //imageElement.SetSubimage(container.imageElement, new IntRect(offset.X, offset.Y, offset.X + container.imageElement.Width, offset.Y + container.imageElement.Height));
        }

        public IntVector2 Render(Node node, IntVector2 offsetPosition, SUI_Element elementParent = null)
        {
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
                            simpleUI.app.LogInfo($"{elementParent} -> {element.spriteName}_{n.Name} ({offsetPosition.X}, {offsetPosition.Y})");
                            Image img = element.Render(IntVector2.Zero, null);
                            var leftOffset = offsetPosition.X;
                            leftOffset += elementParent != null ? elementParent.offset.X : 0;
                            var topOffset = offsetPosition.Y;
                            topOffset += elementParent != null ? elementParent.offset.Y : 0;
                            //image.SetSubimage(img, new IntRect(leftOffset, topOffset, leftOffset + img.Width, topOffset + img.Height));
                            var offsetPos = new IntVector2(leftOffset, topOffset);
                            simpleUI.CopyRectFromImage(img, container.imageElement, new IntRect(0, 0, img.Width, img.Height), offsetPos);

                            offsetPosition += new IntVector2(0, img.Height);
                            if (n.GetNumChildren() > 0)
                            {
                                if (typeof(SUI_Node).IsInstanceOfType(element))
                                {
                                    var el = (SUI_Node)element;
                                    if (el.isExpand)
                                        offsetPosition = Render(n, offsetPosition, element);
                                }
                                else
                                    offsetPosition = Render(n, offsetPosition, element);
                            }

                        }
                    }
                }
            }
            return offsetPosition;
        }

        public void SetArgs(int[] args)
        {
            if (args.Length >= 2)
            {
                position = new IntVector2(args[0], args[1]);
                width = minWidth;
                height = minHeight;
                if (args.Length >= 4)
                {
                    width = args[2];
                    height = args[3];
                }
            }
        }

        public IntVector2 GetPosition()
        {
            return position;
        }
    }
}