using CommandLine;
using GPE;
using System;
using System.Reflection;
using Urho3DNet;

namespace GameProtoypeEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            //Urho3D.ParseArguments(Assembly.GetExecutingAssembly(), args);
            //Launcher.Run(_ => new EditorApplication(_));
            Parser.Default.ParseArguments<ApplicationOptions>(args)
                .WithParsed<ApplicationOptions>(o =>
                {
                    o.Windowed = true;
                    using (var context = new Context())
                    {
                        using (var application = new EditorApplication(context, o))
                        {
                            application.Run();
                        }
                    }
                });
        }
    }
}
