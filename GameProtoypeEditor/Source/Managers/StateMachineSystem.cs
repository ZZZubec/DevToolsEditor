
using System;
using Urho3DNet;

namespace GPE.StateMachine
{
    public enum EnumEditorState
    {
        SHOW, BUILD
    }
    public class StateMachineSystem
    {
        private EnumEditorState _editorState;
        public EnumEditorState editorState
        {
            get { return _editorState; }
            set { OnChangeEditorState(value); }
        }

        public Action<EnumEditorState> onChangeEditorState;
        public Action<bool> onSpaceKeyChangeState;
        private bool _isSpaceDown;
        public bool isSpaceDown {
            get {return _isSpaceDown;}
            private set { _isSpaceDown = value; }
        }

        public StateMachineSystem()
        {
            onChangeEditorState = OnChangeEditorState;
            onSpaceKeyChangeState = OnChangeSpaceKeyState;
        }

        private void OnChangeEditorState(EnumEditorState value)
        {
            if (_editorState != value)
            {
                _editorState = value;
                onChangeEditorState?.Invoke(value);
            }
        }

        public void SetSpaceKeyState(bool value)
        {
            if (this.isSpaceDown != value)
                OnChangeSpaceKeyState(value);
        }

        private void OnChangeSpaceKeyState(bool value)
        {
            if (this.isSpaceDown != value)
            {
                this.isSpaceDown = value;
                onSpaceKeyChangeState.Invoke(value);
            }
        }
    }
}