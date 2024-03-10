using Playnite.SDK;
using PluginsCommon;
using System.IO;
using System.Reflection;

namespace JastUsaLibrary
{
    public class JastUsaLibraryClient : LibraryClient
    {
        public override bool IsInstalled => true;
        public override string Icon => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources", @"JastUsaLibraryIcon.png");

        public override void Open()
        {
            ProcessStarter.StartUrl(@"https://jastusa.com/");
        }
    }
}