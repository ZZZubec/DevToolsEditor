using Urho3DNet;

namespace GPE.Core.UI
{
    [ObjectFactory]
    public class SUI_Node : SUI_Element
    {
        public bool isExpand = false;
        bool isSelected = false;
        bool isHover = false;
        private Color selectedColor;
        private Color backgroundColor;
        //private ISUI_Controler controler;

        public SUI_Node(Context context) : base(context)
        {
            this.spriteName = "SUI_Node";
            var UI = Icons.UI.GetImage();
            selectedColor = UI.GetPixel(7, 18);
            backgroundColor = UI.GetPixel(2, 18);
            minHeight = Icons.font2_glyph_height + 2;
            offset = new IntVector2(12 + Icons.font2_glyph_width, 0);
        }

        public override void Paint(SUI_Element parentElement)
        {
            width = Node.Name.Length * (2+Icons.font2_glyph_width) + 13;
            if (Node.GetNumChildren(false) > 0 && isExpand)
                width += 10;

            var UI = Icons.UI.GetImage();
            var x = 0;
            imageElement.SetSize(width, height, 4);
            imageElement.Clear(Color.Transparent);

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
                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(10, 27, 9, 11), new IntVector2(0, 0));
                else
                    simpleUI.CopyRectFromImage(UI, imageElement, new IntRect(10, 16, 9, 11), new IntVector2(0, 0));
                x += 11 + 2;
            }
            x += simpleUI.DrawText(imageElement, x, 1, Node.Name);

            imageElement.SavePNG("cr_window.png");
        }

    }
}