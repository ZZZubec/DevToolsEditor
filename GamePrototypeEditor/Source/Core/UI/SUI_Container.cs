using System;
using Urho3DNet;

namespace GPE.Core.UI 
{
    [ObjectFactory]
    public class SUI_Container : SUI_Element
    {
        public SUI_Container(Context context):base(context)
        {
            //
        }

        public override void Paint(SUI_Element parentElement)
        {
            base.Paint(parentElement);
            width = parentElement.imageElement.Width - parentElement.offset.X*2;
            height = parentElement.imageElement.Height - parentElement.offset.Y*2;
            imageElement.Resize(width, height);
        }
    }
}