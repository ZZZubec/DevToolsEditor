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

        public SimpleUI(Context context) : base(context)
        {
            image = new Image(context);
            image.SetSize(context.Graphics.Width, context.Graphics.Height, 4);
            UpdateEventMask = UpdateEvent.UseUpdate;
            needUdate = false;
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
            IntRect rect = new IntRect(0, 0, image.Width, image.Height);
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
                            app.LogInfo(element.GetSpriteName());
                            element.Render(image, rect, null);
                        }
                    }
                }
            }
        }

        public SUI_Element AddUI(SUI element, string name, int[] args = null, Node nn = null)
        {
            needUdate = true;
            SUI_Element result = null;
            Node n = null;
            if (nn == null)
                n = Node.CreateChild(name);
            else
            {
                n = nn.CreateChild(name);
                //nn.GetComponent<SUI_Element>().needUpdate = true;
            }

            switch (element)
            {
                default:
                    result = n.CreateComponent<SUI_Element>();
                    break;
                case SUI.WINDOW:
                    result = n.CreateComponent<SUI_Window>();
                    break;
                case SUI.NODE:
                    result = n.CreateComponent<SUI_Node>();
                    break;
            }
            if (args != null)
                result.SetArgs(args);
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
    }
}