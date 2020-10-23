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

        public WindowsSystem(EditorApplication app)
        {
            this.app = app;
            this.stateSystem = app.stateSystem;
            _vec_icon_size = new Vector2(icon_size, icon_size);
            isToolBar = true;

            g_width = Context.Instance.Graphics.Width;
            g_height = Context.Instance.Graphics.Height;

            UI_node = app._camera.CreateChild("UI");
            float aspect = (float)g_width / (float)g_height;
            UI_node.SetScale2D(new Vector2(aspect-0.22f, 1f));
            materialUI = Context.Instance.Cache.GetResource<Material>("UI/Materials/diff.xml");

            var texture = new Texture2D(Context.Instance);
            texture.SetSize(g_width, g_height, 15, TextureUsage.TextureStatic, 1, false);

            var ww = 80;
            var wh = 36;

            simpleUI = UI_node.CreateComponent<SimpleUI>();
            simpleUI.SetApp(app);
            var window = simpleUI.AddUI(SUI.WINDOW, "window", new int[] {0, 200, 200, 120});
            var node = simpleUI.AddUI(SUI.NODE, "gameObject", new int[] {0, 0}, window.Node);
            simpleUI.AddUI(SUI.NODE, "treeTo_oxy", new int[] {0, 0}, node.Node);
            simpleUI.Render();
            //app.LogInfo(app.Dump(simpleUI.Node).ToString());
            //app.LogInfo(window.GetTypeHash().ToString());

            //window.GetImage().SavePNG("sc_image.png");
            texture.SetData(simpleUI.GetImage());
            materialUI.SetTexture(0, texture);
            
            CreateWindow("test", 200, 200, ww, wh);
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

            /*
            float[] vd = {
                //front
                0.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 1,  //0
                0.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 0,  //1
                1.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 0,  //2
                    
                1.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 0,  //2
                1.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 1,  //3
                0.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 1,  //0
            };

            var projView = app._viewport.Camera.GetInverseViewProj();
            for (var i = 0; i < vd.Length; i += 8)
            {
                Vector3 vec = new Vector3(vd[i+0], vd[i+1], vd[i+2]);
                vec = projView * vec + app._camera.Direction * 0.001f;
                app.LogInfo($"vec:{vec.X},{vec.Y},{vec.Z}");
                vd[i+0] = vec.X;
                vd[i+1] = vec.Y;
                vd[i+2] = vec.Z;
            }

            app._debugRenderer.AddPolygon(
                new Vector3(vd[0], vd[1], vd[2]), 
                new Vector3(vd[8], vd[9], vd[10]),
                new Vector3(vd[16], vd[17], vd[18]),
                new Vector3(vd[32], vd[33], vd[34]),
                Color.Red, true
                );
            */
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

/*
        private void CreateWindow(string window_name, int x, int y, int width, int height)
        {
            const float wi = 1305;
            const float hi = 768;
            var node = UI_node.CreateChild(window_name);
            float xx = x / wi;
            float yy = y / hi;
            float w = width / wi;
            float h = height / hi;

            float[] vd = {
                //front
                0.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 1,  //0
                0.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 0,  //1
                1.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 0,  //2
                    
                1.0f, 1.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 0,  //2
                1.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   1, 1,  //3
                0.0f, 0.0f, -0.0f,     0.0f,  0.0f, 1.0f,   0, 1,  //0
            };

            for (var i = 0; i < vd.Length; i += 8)
            {
                if (vd[i + 0] == 0.0f)
                    vd[i + 0] = xx;
                else
                    vd[i + 0] = xx + w;

                if (vd[i + 1] == 0.0f)
                    vd[i + 1] = yy - h;
                else
                    vd[i + 1] = yy;
            }
            var model = app.CreateModel(vd, id, new BoundingBox(-1, 1));
            var staticModel = app.CreateStaticModel(UI_node, model, materialUI);
        }
*/
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

            var projView = app._viewport.Camera.GetViewProj();
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