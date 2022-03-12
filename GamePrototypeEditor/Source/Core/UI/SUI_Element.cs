using CommandLine;
using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Urho3DNet;

namespace GPE.Core.UI
{
    public class SUI_Element
    {
        public string name;
        public IntVector2 position;
        public IntVector2 size;

        private string _spriteName;
        public string spriteName {
            get { return _spriteName; }
            set { _spriteName = value; }
        }

        private SUI_Element parent;

        public SUI_Element(string name, IntVector2 position, int width, int height)
        {
            this.name = name;
            this.position = position;
            this.size = new IntVector2(width, height);
        }

        public void SetParent(SUI_Element parent)
        {
            this.parent = parent;
        }

        public virtual void Render()
        {
        }

        public virtual void Update(float deltaTime) { }

        public IntVector2 GetSize()
        {
            return size;
        }
    }
}