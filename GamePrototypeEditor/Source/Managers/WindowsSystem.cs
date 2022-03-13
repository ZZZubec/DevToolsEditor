using System;
using GPE.Core.UI;
using ImGuiNet;
using Urho3DNet;

namespace GPE.Windows
{
    public class WindowsSystem
    {
        private bool isToolBar;

        private EditorApplication app;

        private int icon_size = 24;

        private Vector2 _vec_icon_size;

        private Node UI_node;
        private Material materialUI;

        private SimpleUI simpleUI;

        private int[] id =
        {
            0, 1, 2,
            3, 4, 5
        };

        private float[] vd =
        {
            //front
            -1.0f, -1.0f, 1.1f, 0.0f, 1.0f, 0.0f, 0, 1, //0
            -1.0f, +1.0f, 1.1f, 0.0f, 1.0f, 0.0f, 0, 0, //1
            +1.0f, +1.0f, 1.1f, 0.0f, 1.0f, 0.0f, 1, 0, //2

            +1.0f, +1.0f, 1.1f, 0.0f, 1.0f, 0.0f, 1, 0, //2
            +1.0f, -1.0f, 1.1f, 0.0f, 1.0f, 0.0f, 1, 1, //3
            -1.0f, -1.0f, 1.1f, 0.0f, 1.0f, 0.0f, 0, 1, //0
        };

        private int g_width, g_height;

        private Texture2D _renderTexture;

        private Viewport _viewportRTT;

        private Sprite ui_sprite;

        private UI ui;

        private bool bIsMainCamera = true;

        private Node cameraRoot, cam_ui;

        private Scene scene;

        private Vector3 scale;

        public WindowsSystem(EditorApplication app, Scene scene, Node cameraRoot)
        {
            this.app = app;
            this.scene = scene;
            this.cameraRoot = cameraRoot;
            
            _vec_icon_size = new Vector2(icon_size, icon_size);
            isToolBar = true;

            g_width = Context.Instance.Graphics.Width;
            g_height = Context.Instance.Graphics.Height;

            UI_node = scene.CreateChild("UI");
            float aspect = (float)g_width / (float)g_height;
            //UI_node.SetScale2D(new Vector2(aspect-0.22f, 1f));
            //UI_node.Position += new Vector3(0.01f, -0.01f, 0.99f);


            simpleUI = UI_node.CreateComponent<SimpleUI>();
            simpleUI.SetApp(app);
            
            cam_ui = scene.CreateChild("cam_UI");
            cam_ui.Position = new Vector3(0, 2, 0);
            var cam = cam_ui.CreateComponent<Camera>();
            cam.FarClip = 10.0f;
            cam.NearClip = 0.1f;

            ui = Context.Instance.GetSubsystem<UI>();
            
            ui_sprite = new Sprite(Context.Instance);
            ui_sprite.SetColor(Color.Gray);
            ui_sprite.Position = new Vector2(g_width/2 - 100, g_height/2 - 100);
            ui_sprite.SetFixedSize(new IntVector2(200, 200));
            
            ui.Root.AddChild(ui_sprite);

            ReInit();
            
            materialUI = app.GetCache().GetResource<Material>("UI/Materials/diff2.xml");
            materialUI.SetTechnique(0, Context.Instance.ResourceCache.GetResource<Technique>("Techniques/DiffAlpha.xml"));
            materialUI.SetTexture(TextureUnit.TuDiffuse, _renderTexture);
            materialUI.DepthBias = new BiasParameters(-0.001f, 0.0f);

            _viewportRTT = new Viewport(Context.Instance, scene, cam_ui.GetComponent<Camera>());
            _renderTexture.RenderSurface.SetViewport(0, _viewportRTT);
            
            UI_node = cameraRoot.CreateChild("UI_node");

            var model = Context.Instance.ResourceCache.GetResource<Model>("Models/Plane.mdl");
            var n = UI_node.CreateChild("Plane");
            var st = n.CreateComponent<StaticModel>();
            st.SetModel(model);
            st.SetMaterial(materialUI);
            
            ui_sprite.Texture = _renderTexture;
            //CreateWindow("Plane");

            //Context.Instance.Renderer.SetViewport(0, _viewportRTT);
        }

        private void ReInit()
        {
            if (_renderTexture == null)
            {
                var resulution = Context.Instance.Graphics.GetDesktopResolution(0);
                _renderTexture = new Texture2D(Context.Instance);
                _renderTexture.SetSize(resulution.X, resulution.Y, Graphics.GetRGBAFormat(),
                    TextureUsage.TextureRendertarget, 0, false);
                _renderTexture.FilterMode = TextureFilterMode.FilterBilinear;
            }

            simpleUI.OnResize(g_width, g_height);
            simpleUI.Render();

            if (materialUI != null)
            {
                materialUI.SetTexture(0, _renderTexture);
                _renderTexture.RenderSurface.SetViewport(0, _viewportRTT);
            }

        }

        public void OnResized(VariantMap eventData)
        {
            if (g_width != Context.Instance.Graphics.Width || g_height != Context.Instance.Graphics.Height)
            {
                g_width = Context.Instance.Graphics.Width;
                g_height = Context.Instance.Graphics.Height;
                this.ReInit();
            }
        }

        private void ShowToolBar()
        {
            if (ImGui.Begin("ToolBar", ref isToolBar, ImGuiWindowFlags.ImGuiWindowFlagsNoDecoration ))
            {
                if (ImGui.BeginTabBar("ToolBar"))
                {
                    if (ImGui.BeginTabItem("Modes"))
                    {
                        ImGui.ImageButton(_renderTexture.RenderSurface.ReadOnlyView, new Vector2(200, 200));
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

        public void OnUpdate(float deltaTime)
        {
            if (Context.Instance.Input.GetKeyPress(Key.KeySpace))
            {
                if(bIsMainCamera)
                    Context.Instance.Renderer.SetViewport(0, _viewportRTT);
                else
                    Context.Instance.Renderer.SetViewport(0, app._viewport3D);
                bIsMainCamera = !bIsMainCamera;
            }
            //ShowToolBar();
            if (cameraRoot != null && UI_node != null)
            {
                if (Context.Instance.Input.GetKeyDown(Key.KeyLeftbracket))
                {
                    UI_node.WorldScale -= new Vector3(1, 0, 0) * deltaTime;
                }
                if (Context.Instance.Input.GetKeyDown(Key.KeyRightbracket))
                {
                    UI_node.WorldScale += new Vector3(1, 0, 0) * deltaTime;
                }
                
                if (Context.Instance.Input.GetKeyDown(Key.KeyPageup))
                {
                    UI_node.WorldScale += new Vector3(0, 0, 1) * deltaTime;
                }
                if (Context.Instance.Input.GetKeyDown(Key.KeyPagedown))
                {
                    UI_node.WorldScale -= new Vector3(0, 0, 1) * deltaTime;
                }

                if (Context.Instance.Input.GetKeyDown(Key.KeyHome))
                {
                    UI_node.Position += new Vector3(0, 1, 0) * deltaTime;
                }
                if (Context.Instance.Input.GetKeyDown(Key.KeyEnd))
                {
                    UI_node.Position -= new Vector3(0, 1, 0) * deltaTime;
                }
                
                //UI_node.Position -= new Vector3(0, 1, 0) * deltaTime;
                if (ImGui.Begin("Urho3D.NET"))
                {
                    ImGui.TextColored(Color.White,
                        $"pos:{cameraRoot.Position.X},{cameraRoot.Position.Y},{cameraRoot.Position.Z}\n" +
                        $"posUI:{UI_node.Position.X},{UI_node.Position.Y},{UI_node.Position.Z}\n" +
                        $"scale:{UI_node.WorldScale.X},{UI_node.WorldScale.Y},{UI_node.WorldScale.Z}\n" +
                        $"Frame time: {deltaTime}, \n");
                }

                ImGui.End();
                
                var scaleSpeed = 2f;
                var mouseSensivity = 20f;

                /*
                while (_camera3DNode.Rotation.EulerAngles.X >= 89)
                {
                    angleX -= 0.1f;
                    angleY = 0.1f;
                    _camera3DNode.Rotation = new Quaternion(new Vector3(angleX, angleY, 0));
                }

                while (_camera3DNode.Rotation.EulerAngles.X <= -89)
                {
                    angleX += 0.1f;
                    angleY = 0.1f;
                    _camera3DNode.Rotation = new Quaternion(new Vector3(angleX, angleY, 0));
                }
                */
                Vector3 moveDirection = Vector3.Zero;
                float moveX = 0;
                float moveY = 0;

                if (Context.Instance.Input.GetKeyDown(Key.KeyW))
                {
                    moveY = 1;
                }

                if (Context.Instance.Input.GetKeyDown(Key.KeyS))
                {
                    moveY = -1;
                }

                if (Context.Instance.Input.GetKeyDown(Key.KeyA))
                {
                    moveX = -1;
                }

                if (Context.Instance.Input.GetKeyDown(Key.KeyD))
                {
                    moveX = 1;
                }

                moveDirection =
                    moveX * scaleSpeed * cam_ui.Right * deltaTime +
                    moveY * scaleSpeed * cam_ui.Direction * deltaTime;
                cam_ui.Position += moveDirection;
                /**/
            }
        }

        private void CreateWindow(string window_name)
        {
            float aspect = (float)g_height / g_width;
            var node = UI_node.CreateChild(window_name);
            //node.Position = new Vector3(x,y,0);

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

        public void SetCamera(Node _camera, Viewport viewport)
        {
        }
    }
}