using ImGuiNet;
using GPE.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Urho3DNet;
using GPE.Windows;
using GPE;
using GPE.Core;

namespace GPE
{
    public class EditorApplication : Application
    {

        public EditorApplication(Context context, ApplicationOptions options) : base(context)
        {
            _options = options;
        }

        public Vector3 StartPositionCamera = new Vector3(0, 2, 0);

        private readonly ApplicationOptions _options;
        private Scene _scene;
        public Viewport _viewport3D;

        public Node _camera3DNode;

        //private KinematicCharacterController _characterController;
        public Node _cameraRoot;
        private Node _light = null;
        private Material _material = null;

        private UniversalFileSystem _ufs;

        //private Node _nodeWorld;

        //private float _scale = 1f;

        private float angleX;
        private float angleY;

        private bool isShowing = true;
        private IntVector2 mouse_position;
        private UI ui;

        private float[] vertexData = new float[]
        {
            // Position             Normal
            //front
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0, 1, //0
            -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0, 0, //1
            +0.5f, 0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 1, 0, //2

            +0.5f, +0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 1, 0, //2
            +0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 1, 1, //3
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0, 1, //0

            //back
            +0.5f, -0.5f, +0.5f, 0.0f, 0.0f, -1.0f, 0, 1, //4
            +0.5f, +0.5f, +0.5f, 0.0f, 0.0f, -1.0f, 0, 0, //5
            -0.5f, +0.5f, +0.5f, 0.0f, 0.0f, -1.0f, 1, 0, //6

            -0.5f, +0.5f, +0.5f, 0.0f, 0.0f, -1.0f, 1, 0, //6
            -0.5f, -0.5f, +0.5f, 0.0f, 0.0f, -1.0f, 1, 1, //7
            +0.5f, -0.5f, +0.5f, 0.0f, 0.0f, -1.0f, 0, 1, //4

            //top
            -0.5f, +0.5f, -0.5f, 0.0f, +1.0f, 0.0f, 0, 1, //8
            -0.5f, +0.5f, +0.5f, 0.0f, +1.0f, 0.0f, 0, 0, //9
            +0.5f, +0.5f, +0.5f, 0.0f, +1.0f, 0.0f, 1, 0, //10

            +0.5f, +0.5f, +0.5f, 0.0f, +1.0f, 0.0f, 1, 0, //10
            +0.5f, +0.5f, -0.5f, 0.0f, +1.0f, 0.0f, 1, 1, //11
            -0.5f, +0.5f, -0.5f, 0.0f, +1.0f, 0.0f, 0, 1, //8

            //left
            -0.5f, -0.5f, +0.5f, +1.0f, +0.0f, +0.0f, 0, 1,
            -0.5f, +0.5f, +0.5f, +1.0f, +0.0f, +0.0f, 0, 0,
            -0.5f, +0.5f, -0.5f, +1.0f, +0.0f, +0.0f, 1, 0,

            -0.5f, +0.5f, -0.5f, +1.0f, +0.0f, +0.0f, 1, 0,
            -0.5f, -0.5f, -0.5f, +1.0f, +0.0f, +0.0f, 1, 1,
            -0.5f, -0.5f, +0.5f, +1.0f, +0.0f, +0.0f, 0, 1,

            //right
            +0.5f, -0.5f, -0.5f, -1.0f, +0.0f, +0.0f, 0, 1,
            +0.5f, +0.5f, -0.5f, -1.0f, +0.0f, +0.0f, 0, 0,
            +0.5f, +0.5f, +0.5f, -1.0f, +0.0f, +0.0f, 1, 0,

            +0.5f, +0.5f, +0.5f, +1.0f, +0.0f, +0.0f, 1, 0,
            +0.5f, -0.5f, +0.5f, +1.0f, +0.0f, +0.0f, 1, 1,
            +0.5f, -0.5f, -0.5f, +1.0f, +0.0f, +0.0f, 0, 1,
        };

        private int[] indexData = new int[]
        {
            0, 1, 2,
            3, 4, 5,

            6, 7, 8,
            9, 10, 11,

            12, 13, 14,
            15, 16, 17,

            18, 19, 20,
            21, 22, 23,

            24, 25, 26,
            27, 28, 29
        };

        private DebugHud _debugHud;

        //private RigidBody _body;
        //private CollisionShape _shape;
        public DebugRenderer _debugRenderer;
        
        public WindowsSystem ws;


        protected override void Dispose(bool disposing)
        {
            Context.Renderer.SetViewport(0, null); // Enable disposal of viewport by making it unreferenced by engine.
            _viewport3D.Dispose();
            _scene.Dispose();
            _camera3DNode.Dispose();
            _light.Dispose();
            base.Dispose(disposing);
        }

        public override void Setup()
        {
            var currentDir = Directory.GetCurrentDirectory();
            _options.Windowed = true;
            _options.Width = 1305;
            _options.Height = 768;
            EngineParameters[Urho3D.EpFullScreen] = !_options.Windowed;
            if (_options.Windowed)
            {
                EngineParameters[Urho3D.EpWindowResizable] = true;
            }

            if (_options.Width.HasValue)
            {
                EngineParameters[Urho3D.EpWindowWidth] = _options.Width.Value;
            }

            if (_options.Height.HasValue)
            {
                EngineParameters[Urho3D.EpWindowHeight] = _options.Height.Value;
            }

            EngineParameters[Urho3D.EpWindowTitle] = "Game Prototype Editor";
            EngineParameters[Urho3D.EpResourcePrefixPaths] = $"{currentDir};{currentDir}/../../../../../Content;{currentDir}/../Content;";
            EngineParameters[Urho3D.EpHighDpi] = _options.HighDpi;
            EngineParameters[Urho3D.EpRenderPath] = _options.RenderPath;
        }

        public override void Start()
        {
            InitScene();
            SubscribeEvents();
        }

        private void InitScene()
        {
            ui = Context.GetSubsystem<UI>();
            var uiStyle = GetCache().GetResource<XMLFile>("UI/DefaultStyle.xml");
            ui.Root.SetDefaultStyle(uiStyle);

            ui.Cursor = new Cursor(Context);
            ui.Cursor.SetDefaultStyle(uiStyle);

            var text = new Text(Context);
            text.SetText("test");
            ui.Root.AddChild(text);

            // Viewport
            _scene = new Scene(Context);
            _scene.Name = "_scene";
            //_scene.LoadFile("Test/Scenes/ThreeScene.xml");
            _scene.CreateComponent<PhysicsWorld>();
            _scene.CreateComponent<Octree>();
            var zone = _scene.CreateComponent<Zone>();
            zone.SetBoundingBox(new BoundingBox(-30, 3200));
            zone.FogStart = 200f;
            zone.FogEnd = 300f;
            var _c = new Color(43 / 255f, 43 / 255f, 43 / 255f, 0.7f);
            //zone.AmbientColor = _c;
            zone.FogColor = _c;

            _light = _scene.CreateChild("DirectionalLight");
            _light.WorldPosition = new Vector3(0, 10, -10);
            _light.WorldDirection = new Vector3(-2, -2, 1);
            var light = _light.CreateComponent<Light>();
            light.Color = Color.White;
            light.CastShadows = true;
            light.ShadowCascade = new CascadeParameters(50, 0, 0, 0, 0.9f);
            //_light.Brightness = 1;
            light.LightMode = LightMode.LmRealtime;
            light.LightType = LightType.LightDirectional;

            _cameraRoot = _scene.CreateChild("Camera Pivot", CreateMode.Replicated);
            _cameraRoot.Position = new Vector3(0, 0, 0);
            _camera3DNode = _cameraRoot.CreateChild("Main Camera", CreateMode.Replicated);
            _camera3DNode.Position = StartPositionCamera;

            _camera3DNode.LookAt(Vector3.Zero, Vector3.Up);
            //_camera.LookAt(new Vector3(6, 0, 6), Vector3.Up);
            _viewport3D = new Viewport(Context);
            _viewport3D.Scene = _scene;
            _viewport3D.Camera = (Camera) _camera3DNode.GetOrCreateComponent((StringHash) typeof(Camera).Name);
            _viewport3D.Camera.FarClip = 10f;
            _viewport3D.Camera.Fov = 45f;
            _viewport3D.Camera.NearClip = 0.1f;

            Context.Renderer.SetViewport(0, _viewport3D);

    #region WindowsSystem
            ws = new WindowsSystem(this, _scene, _cameraRoot);
    #endregion

    #region Mouse
            Context.Input.SetMouseVisible(true);
            Context.Input.SetMouseGrabbed(false);
            Context.Input.SetMouseMode(MouseMode.MmFree);
            Context.Input.MousePosition = new IntVector2(Context.Graphics.Width / 2, Context.Graphics.Height / 2);
            //Context.Input.SetMouseMode(MouseMode.MmRelative, false);
    #endregion

            _debugHud = Context.Engine.CreateDebugHud();
            _debugHud.Mode = DebugHudMode.DebughudShowAll;

            if (ws != null)
            {
                ws.SetCamera(_camera3DNode, _viewport3D);
            }


            #region InitMap

            LogInfo("Dirs:");
            foreach (string d in Context.ResourceCache.ResourceDirs)
            {
                LogInfo(string.Format("Dir: {0}", d));
            }

            var _platforms = new Platforms(EnumPlatform.PC);
            _ufs = new UniversalFileSystem(_platforms, this);

            //_material = GetCache().GetResource<Material>("n_sector/materials/m_blocks.xml", true);
            //_material.Name = "blocks";

            int blockID = 7;
            LogInfo($"blockID:{blockID}, value:{blockID % 16}");
            //CreateStaticModelBlock(_model, vertexData, indexData, _material, blockID, new BoundingBox(-0.5f, 0.5f));
            //var sb = Dump(_scene); LogInfo(sb.ToString());

            #endregion

            _debugRenderer = _scene.CreateComponent<DebugRenderer>();
        }
        
        private void SubscribeEvents()
        {
            SubscribeToEvent(E.KeyDown, KeyDown);
            SubscribeToEvent(E.MouseButtonDown, OnMouseDown);
            SubscribeToEvent(E.MouseMove, OnMouseMove);
            SubscribeToEvent(E.MouseButtonUp, OnMouseUp);
            SubscribeToEvent(E.Resized, ScreenResize);
            SubscribeToEvent(E.Update, OnUpdate);
        }

        private void ScreenResize(VariantMap map)
        {
            if (ws != null)
            {
                ws.OnResized(map);
            }
        }

        private void KeyDown(VariantMap map)
        {
            var key = map[E.KeyDown.Key].Int;

            if (key == (int)Key.KeyEscape)
            {
                Context.Instance.Engine.Exit();
            }
        }

        private void OnMouseMove(VariantMap map)
        {
            mouse_position = new IntVector2(map[E.MouseMove.X].Int, map[E.MouseMove.Y].Int);
        }

        private void OnMouseDown(VariantMap map)
        {
            var button = map[E.MouseButtonDown.Button].Int;

            //
        }

        private void OnMouseUp(VariantMap map)
        {
            var button = map[E.MouseButtonDown.Button].Int;

            //
        }

        private void OnUpdate(VariantMap map)
        {
            float deltaTime = map[E.Update.TimeStep].Float;

            Debug.Assert(this != null);

            int xx = (int) _camera3DNode.Position.X / 16;
            int yy = (int) _camera3DNode.Position.Z / 16;

            if (Context.Input.GetKeyDown(Key.KeyEscape))
            {
                Context.Engine.Exit();
            }

            ShowGrid();
            ShowMenu();

            if (isShowing)
            {
                /*
                if (ImGui.Begin("Urho3D.NET"))
                {

                    ImGui.TextColored(Color.White,
                        $"chunk:{xx}_{yy}\n" +
                        $"pos:{_camera3DNode.Position.X},{_camera3DNode.Position.Y},{_camera3DNode.Position.Z}\n" +
                        $"rotXYZ:{_camera3DNode.Rotation.EulerAngles.X},{_camera3DNode.Rotation.EulerAngles.Y},{_camera3DNode.Rotation.EulerAngles.Z}\n" +
                        $"rotXY:{angleX},{angleY}\n" +
                        $"pos:{ui.CursorPosition.X},{ui.CursorPosition.Y}\n" +
                        $"Frame time: {deltaTime}, \n");
                    if (ImGui.Button("Exit"))
                    {
                        Context.Engine.Exit();
                    }
                }

                ImGui.End();
                */

            }

            var isMoving = false;
            if (Context.Input.GetMouseButtonDown(MouseButton.MousebRight))
            {
                if (!isMoving)
                {
                    Context.Input.SetMouseVisible(false);
                    Context.Input.SetMouseGrabbed(true);
                    Context.Input.SetMouseMode(MouseMode.MmRelative, true);
                    angleX = _cameraRoot.Rotation.EulerAngles.X;
                    angleY = _cameraRoot.Rotation.EulerAngles.Y;
                }

                //isMoving = true;
            }

            if (isMoving)
            {
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

                if (Context.Input.GetKeyDown(Key.KeyW))
                {
                    moveY = 1;
                }

                if (Context.Input.GetKeyDown(Key.KeyS))
                {
                    moveY = -1;
                }

                if (Context.Input.GetKeyDown(Key.KeyA))
                {
                    moveX = -1;
                }

                if (Context.Input.GetKeyDown(Key.KeyD))
                {
                    moveX = 1;
                }

                moveDirection =
                    moveX * scaleSpeed * _cameraRoot.Right * deltaTime +
                    moveY * scaleSpeed * _cameraRoot.Direction * deltaTime;
                _cameraRoot.Position += moveDirection;

                IntVector2 vec = Context.Input.MouseMove;
                if (deltaTime > 0.01f)
                    deltaTime = 0.01f;
                angleY += vec.X * mouseSensivity * deltaTime;
                angleX += vec.Y * mouseSensivity * deltaTime;
                _cameraRoot.Rotation = new Quaternion(new Vector3(angleX, angleY, 0));
                /**/
            }
            else
            {
                if (Context.Input.IsMouseGrabbed())
                {
                    Context.Input.SetMouseGrabbed(false);
                    Context.Input.SetMouseVisible(true);
                    Context.Input.SetMouseMode(MouseMode.MmFree);
                }
            }

            if (ws != null)
            {
                ws.OnUpdate(deltaTime);
            }
        }

        private void OnChangeSpaceKeyState(bool value)
        {
            if (value)
                isShowing = !isShowing;
        }

        private void ShowMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open", "Ctrl+O"))
                    {
                        /* Do stuff */
                    }

                    if (ImGui.MenuItem("Save", "Ctrl+S"))
                    {
                        /* Do stuff */
                    }

                    if (ImGui.MenuItem("Close", "Ctrl+W"))
                    {
                        isShowing = false;
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Create"))
                {
                    if (ImGui.MenuItem("Terrain", "Ctrl+T"))
                    {
                        //
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }
        }

        private void ShowGrid()
        {
            var depthTest = true;
            var _cvec = new Vector4(0.4f, 0.4f, 0.4f, 0.8f);
            var colorGrid = new Color(_cvec.X, _cvec.Y, _cvec.Z, _cvec.W);
            var colorGrid2 = new Color(_cvec.X / 2, _cvec.Y / 2, _cvec.Z / 2, _cvec.W / 2);
            var gridSize = 20;
            var i = 10;
            for (float x = 0; x <= gridSize + 0.1f; x += 0.1f)
            {
                var color = colorGrid2;
                //gridSize = 20;
                if (i == 10)
                {
                    color = colorGrid;
                    i = 0;
                    //gridSize *= 10;
                }

                //Z
                _debugRenderer.AddLine(new Vector3(x, 0, 0), new Vector3(x, 0, gridSize), color, depthTest);
                //X
                _debugRenderer.AddLine(new Vector3(0, 0, x), new Vector3(gridSize, 0, x), color, depthTest);
                i++;
            }
        }

        public StringBuilder Dump(Node node, StringBuilder sb = null, string fullPath = "_root")
        {
            fullPath = $"{fullPath}->{node.Name}";
            if (sb == null)
                sb = new StringBuilder();
            GetComponents(node, sb, fullPath);

            var nodeList = node.GetChildren();
            foreach (Node n in nodeList)
            {
                sb.AppendLine($"{fullPath}->{n.Name}");
                //Dump(n, sb, $"{fullPath}->{node.Name}");
                var nl = node.GetChildren();
                if (nl.Count > 0)
                    Dump(n, sb, fullPath);
                else
                    GetComponents(n, sb, $"{fullPath}->{n.Name}");
            }

            return sb;
        }

        public void GetComponents(Node node, StringBuilder sb, string fullPath)
        {
            var comps = node.GetComponents();
            foreach (Component comp in comps)
            {
                sb.AppendLine($"{fullPath}:<{comp.GetTypeName()}>");
            }
        }


        /// <summary>
        /// Create Model
        /// </summary>
        /// <param name="f_verts">vertices (normal, uv)</param>
        /// <param name="f_facesIndex">int faces index</param>
        /// <param name="boundingBox">bounding box</param>
        /// <returns>Model</returns>
        public Model CreateModel(float[] f_verts, int[] f_facesIndex, BoundingBox boundingBox)
        {
            uint numVertices = (uint) f_verts.Length;
            uint numFaces = (uint) f_facesIndex.Length;

            Model model = new Model(Context);
            VertexBuffer vertexBuffer = new VertexBuffer(Context);
            IndexBuffer indexBuffer = new IndexBuffer(Context);
            Geometry geometry = new Geometry(Context);

            VertexElementList vertexElements = new VertexElementList();
            vertexElements.Add(new VertexElement(VertexElementType.TypeVector3, VertexElementSemantic.SemPosition));
            vertexElements.Add(new VertexElement(VertexElementType.TypeVector3, VertexElementSemantic.SemNormal));
            vertexElements.Add(new VertexElement(VertexElementType.TypeVector2, VertexElementSemantic.SemTexcoord));

            int sizeVB = Marshal.SizeOf(f_verts[0]) * f_verts.Length;
            int sizeIB = Marshal.SizeOf(f_facesIndex[0]) * f_facesIndex.Length;
            IntPtr pnt_vb = Marshal.AllocHGlobal(sizeVB);
            IntPtr pnt_ib = Marshal.AllocHGlobal(sizeIB);
            try
            {

                Marshal.Copy(f_verts, 0, pnt_vb, f_verts.Length);
                vertexBuffer.IsShadowed = true;
                vertexBuffer.SetSize(numFaces, vertexElements);
                vertexBuffer.SetData(pnt_vb);

                Marshal.Copy(f_facesIndex, 0, pnt_ib, f_facesIndex.Length);
                //IndexBuffer.PackIndexData(indexData, pnt_ib, false, 0, (uint)indexData.Length);
                indexBuffer.IsShadowed = true;
                indexBuffer.SetSize(numFaces, true);
                indexBuffer.SetData(pnt_ib);

                geometry.VertexBuffers.Add(vertexBuffer);
                geometry.IndexBuffer = indexBuffer;
                geometry.SetDrawRange(PrimitiveType.TriangleList, 0, numFaces);
                model.NumGeometries = 1;
                model.SetNumGeometryLodLevels(0, 1);
                model.SetGeometry(0, 0, geometry);
                model.BoundingBox = boundingBox;

                VertexBufferRefList vbRL = new VertexBufferRefList();
                IndexBufferRefList ibRL = new IndexBufferRefList();
                vbRL.Add(vertexBuffer);
                ibRL.Add(indexBuffer);
                UIntArray morphStart = new UIntArray();
                UIntArray morphCount = new UIntArray();
                morphStart.Add(0);
                morphCount.Add(0);
                model.SetVertexBuffers(vbRL, morphStart, morphCount);
                model.IndexBuffers = ibRL;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt_vb);
                Marshal.FreeHGlobal(pnt_ib);
            }

            return model;
        }

        public StaticModel CreateStaticModel(Node node, Model model, Material material)
        {
            StaticModel staticModel = node.CreateComponent<StaticModel>();
            staticModel.SetModel(model);
            staticModel.SetMaterial(material);
            staticModel.CastShadows = false;


            return staticModel;
        }

        public StaticModel CreateStaticModelBlock(Node node, float[] f_verts, int[] f_facesIndex, Material material,
            int blockID, BoundingBox boundingBox)
        {
            if (blockID > 0)
            {
                int bID = blockID - 1;
                int xx = bID % 16;
                int yy = bID / 16;

                LogInfo($"blockID:{blockID}, xx:{xx}, yy:{yy}");
                float u0 = xx * 0.0625f;
                float v0 = yy * 0.0625f;
                float u1 = u0 + 0.0625f;
                float v1 = v0 + 0.0625f;

                for (int i = 0; i < f_verts.Length; i += 8)
                {
                    if (f_verts[i + 6] == 0)
                        f_verts[i + 6] = u0;
                    else
                        f_verts[i + 6] = u1;

                    if (f_verts[i + 7] == 0)
                        f_verts[i + 7] = v0;
                    else
                        f_verts[i + 7] = v1;
                }
            }

            Model model = CreateModel(f_verts, f_facesIndex, boundingBox);
            return CreateStaticModel(node, model, material);
        }

        public ResourceCache GetCache()
        {
            return Context.ResourceCache;
        }

        public void LogError(string v)
        {
            Debug.WriteLine(v);
            System.Console.Error.WriteLine(v);
        }

        public void LogInfo(string v)
        {
            Debug.WriteLine(v);
            System.Console.WriteLine(v);
        }

        public Context GetContext()
        {
            return Context;
        }

        public UniversalFileSystem GetUFS()
        {
            return _ufs;
        }

        public string ExtractFilename(string filename)
        {
            string[] cols = filename.Replace("\\", "/").Split('/');
            return cols[cols.Length - 1];
        }
    }

}
