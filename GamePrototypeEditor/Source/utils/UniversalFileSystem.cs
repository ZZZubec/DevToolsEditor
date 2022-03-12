using GPE;
using Salamandr.utils.Network;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Urho3DNet;

namespace GPE.utils
{
    public class UniversalFileSystem
    {
        public string external_path = "";
        private Platforms platform;
        //private Application app;
        public EditorApplication universalApp;

        public UniversalFileSystem(Platforms platform, EditorApplication universalApp)
        {
            this.platform = platform;
            //this.app = app;
            this.universalApp = universalApp;

#if UNITY_STANDALONE || UNITY_EDITOR
            external_path = "./Builds/";
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        global_path = GetAndroidInternalFilesDir("/games/Sector5661/");
#endif
            if (platform.currentPlatform != Platforms.Android && platform.currentPlatform != Platforms.iOS)
            {
                external_path = "h:/develop/salamandr.dev/Unity/2019/MapEditorSector/Builds/";
            }
            universalApp.LogInfo("global_path:" + external_path);
        }

        public void SaveFile(string filename, GameBuffer buffer)
        {
            //
        }

        public GameBuffer ReadFile(string filename)
        {
            var path_filename = System.IO.Path.Combine(external_path, filename);

            universalApp.LogInfo(string.Format("UFS->ReadFile:{0}", path_filename));

            GameBuffer buffer = null;
            if (universalApp.GetCache().Exists(path_filename))
            {
                File file = universalApp.GetCache().GetFile(path_filename, true);
                universalApp.LogInfo(string.Format("UFS->ReadFile()->file_size:{0}", file.GetSize()));
                int f_size = (int)file.GetSize();
                ByteVector uch = new ByteVector(f_size);
                buffer = new GameBuffer(f_size);

                file.ReadBinary(uch);
                file.Close();
                buffer.bytes = uch.ToArray();

                universalApp.LogInfo(string.Format("UFS->ReadFile()->buffer_length:{0}", buffer.bytes.Length));
            }
            else
                universalApp.LogError(string.Format("filename does not exists:{0}",universalApp));
            return buffer;
        }

        public string[] ReadTextFile(string filename)
        {
            return ConvertBinToStringLine(ReadFile(filename));
        }

        public string[] ConvertBinToStringLine(GameBuffer buffer)
        {
            var lines = new List<string>();
            if (buffer != null)
            {
                while (buffer.getLength() < buffer.bytes.Length)
                {
                    lines.Add(buffer.readUnformatedString());
                }
            }
            return lines.ToArray();
        }
    }
}
