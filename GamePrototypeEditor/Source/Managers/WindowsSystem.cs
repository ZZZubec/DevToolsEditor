using System;
using GPE.Core.UI;
using GPE.StateMachine;
using ImGuiNet;
using Urho3DNet;

namespace GPE.Windows
{
    public class WindowsSystem
    {
        private bool isToolBar;

        private EditorApplication app;
        private StateMachineSystem stateSystem;

        private int icon_size = 24;

        private Vector2 _vec_icon_size;

        private Node UI_node;
        private Material materialUI;

        private SimpleUI simpleUI;

        private int[] id = {
                0, 1, 2,
                3, 4, 5
            };

        private int g_width, g_height;

        private Texture2D texture;

        public WindowsSystem(EditorApplication app)
        {
            this.app = app;
            this.stateSystem = app.stateSystem;
            _vec_icon_size = new Vector2(icon_size, icon_size);
            isToolBar = true;

            g_width = Context.Instance.Graphics.Width;
            g_height = Context.Instance.Graphics.Height;

            UI_node = app._camera3DNode.CreateChild("UI");
            float aspect = (float)g_width / (float)g_height;
            UI_node.SetScale2D(new Vector2(aspect-0.22f, 1f));
            UI_node.Position += new Vector3(0.01f, -0.01f, 0.99f);
            materialUI = app.GetCache().GetResource<Material>("UI/Materials/diff2.xml");

            texture = new Texture2D(Context.Instance);
            texture.SetSize(g_width, g_height, 15, TextureUsage.TextureStatic, 1, false);

            var ww = 80;
            var wh = 36;

            simpleUI = UI_node.CreateComponent<SimpleUI>();
            simpleUI.SetApp(app);

            ReInit();
            
            app.SubscribeToEvent(E.Resized, OnResized);
        }

        private void ReInit()
        {
            texture = new Texture2D(Context.Instance);
            texture.SetSize(g_width, g_height, 15, TextureUsage.TextureStatic, 1, false);
            simpleUI.OnResize(g_width, g_height);
            simpleUI.Render();
            materialUI.SetTexture(0, texture);
        }

        private void OnResized(VariantMap eventData)
        {
            if (g_width != Context.Instance.Graphics.Width || g_height != Context.Instance.Graphics.Height)
            {
                g_width = Context.Instance.Graphics.Width;
                g_height = Context.Instance.Graphics.Height;
                ReInit();
            }
        }

        private void ShowToolBar()
        {
            if (isToolBar)
            {
                if (ImGui.Begin("ToolBar"))
                {
                    if (ImGui.BeginTabBar("ToolBar"))
                    {
                        if (ImGui.BeginTabItem("Modes"))
                        {
                            if (ImGui.ImageButton(Icons.select.ShaderResourceView, _vec_icon_size, Vector2.ZERO, new Vector2(1, 1), 3, Color.Transparent, new Color(0.2f, 0.6f, 0.2f, 1f)))
                            {
                                //
                            }
                            if (ImGui.ImageButton(Icons.terrain.ShaderResourceView, _vec_icon_size, Vector2.ZERO, new Vector2(1, 1), 3, Color.Transparent, new Color(0.2f, 0.6f, 0.2f, 1f)))
                            {
                                //
                            }
                            if (ImGui.ImageButton(Icons.paint.ShaderResourceView, _vec_icon_size, Vector2.ZERO, new Vector2(1, 1), 3, Color.Transparent, new Color(0.2f, 0.6f, 0.2f, 1f)))
                            {
                                //
                            }
                        }
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
                ImGui.End();
            }
        }

        public void InputUpdate(float timestep)
        {
            ShowToolBar();

            if (Context.Instance.Input.GetKeyDown(Key.KeySpace))
            {
                if (!stateSystem.isSpaceDown)
                    stateSystem.SetSpaceKeyState(true);
            }
            else
            {
                if (stateSystem.isSpaceDown)
                    stateSystem.SetSpaceKeyState(false);
            }
        }

        private void CreateWindow(string window_name, int x, int y, int width, int height)
        {
            const float wi = 1305;
            const float hi = 768;
            var node = UI_node.CreateChild(window_name);
            //node.Position = new Vector3(x,y,0);

            float[] vd = {
                //front
                -1.0f, -1.0f, 1.1f,     1.0f,  1.0f, 1.0f,   0, 1,  //0
                -1.0f,  1.0f, 1.1f,     1.0f,  1.0f, 1.0f,   0, 0,  //1
                 1.0f,  1.0f, 1.1f,     1.0f,  1.0f, 1.0f,   1, 0,  //2
                    
                 1.0f,  1.0f, 1.1f,     1.0f,  1.0f, 1.0f,   1, 0,  //2
                 1.0f, -1.0f, 1.1f,     1.0f,  1.0f, 1.0f,   1, 1,  //3
                -1.0f, -1.0f, 1.1f,     1.0f,  1.0f, 1.0f,   0, 1,  //0
            };

            for (var i = 0; i < vd.Length; i += 8)
            {
                vd[i+0] *= 1;
                vd[i+1] *= 0.88f;
                //vd[i+2] = vec.Z;
            }

            var model = app.CreateModel(vd, id, new BoundingBox(-1,1));
            var staticModel = app.CreateStaticModel(node, model, materialUI);
        }

        private void CreateWindowHeader(Node node, int x, int y, int width, int height )
        {
            var window_UI = new Vector4(0f, 0f, 16/256f, 8/256f);
            CreatePolygon(node, window_UI, window_UI.Z, window_UI.W);

            var window_UI3 = new Vector4(72/256, 0f, 8/256f, 8/256f);
            var w = width - window_UI.Z - window_UI3.Z;
            var window_UI2 = new Vector4(window_UI.Z, 0f, window_UI3.X-window_UI.Z, 8/256f);
        }

        public void CreatePolygon(Node node, Vector4 window_UI, float wf, float hf)
        {
            float[] vd = {
                //front
                0.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 1,  //0
                0.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 0,  //1
                1.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 0,  //2
                    
                1.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 0,  //2
                1.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 1,  //3
                0.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 1,  //0
            };

            var projView = app._viewport3D.Camera.ViewProj;
            for (var i = 0; i < vd.Length; i += 8)
            {
                Vector3 vec = new Vector3(vd[i+0], vd[i+1], vd[i+2]);
                vec = projView * vec;
                vd[i+0] = vec.X;
                vd[i+1] = vec.Y;
                vd[i+2] = vec.Z;
                if (vd[i + 6] == 0.0f)
                    vd[i + 6] = window_UI.X;
                else
                    vd[i + 6] = window_UI.X + window_UI.Z;

                if (vd[i + 7] == 0.0f)
                    vd[i + 7] = window_UI.Y;
                else
                    vd[i + 7] = window_UI.Y + window_UI.W;
            }
            var model = app.CreateModel(vd, id, new BoundingBox(0, 1));
            var staticModel = app.CreateStaticModel(UI_node, model, materialUI);
        }

        public void SetCamera(Node _camera, Camera camera)
        {
        }
    }
}