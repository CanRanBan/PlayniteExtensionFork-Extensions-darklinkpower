using Playnite.SDK.Models;
using Playnite.SDK.Plugins;

namespace JastUsaLibrary
{
    public class FakeInstallController : InstallController
    {
        private string installDir;

        public FakeInstallController(Game game, string installDir) : base(game)
        {
            this.installDir = installDir;
        }

        public override void Install(InstallActionArgs args)
        {
            var installInfo = new GameInstallationData()
            {
                InstallDirectory = installDir
            };

            InvokeOnInstalled(new GameInstalledEventArgs(installInfo));
            return;
        }
    }

    public class FakeUninstallController : UninstallController
    {
        public FakeUninstallController(Game game) : base(game)
        {
            Name = "Uninstall";
        }

        public override void Uninstall(UninstallActionArgs args)
        {
            InvokeOnUninstalled(new GameUninstalledEventArgs());
        }
    }

}
