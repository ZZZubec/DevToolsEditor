using System;
using Urho3DNet;

namespace GPE.Core.UI
{
    [ObjectFactory]
    public class SimpleUI : LogicComponent
    {
        public EditorApplication app;

        public SimpleUI(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate;
        }

        public void OnResize(int width, int height)
        {
            //
        }

        public override void Update(float deltaTime)
        {

        }

        public void OnResize()
        {
            Render();
        }

        public void Render()
        {
            //
        }

        public void SetApp(EditorApplication app)
        {
            this.app = app;
        }
    }
}