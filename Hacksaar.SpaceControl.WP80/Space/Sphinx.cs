using Hacksaar.SpaceControl.WP80.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacksaar.SpaceControl.WP80.Space
{
    public class Sphinx
    {
        protected const string BuzzCommand = "buzz"; // Kommando auf der TyShell zum Betätigen des Haustür-Buzzers
        protected const string UnlockCommand = "unlock"; // Kommando auf der TyShell zum Entriegeln der Wohnungstür

        private SpaceControlLogger Logger { get; set; }

        private IShell Shell { get; set; }

        public Sphinx(ConnectInfo remoteShellConnectionInfo, SpaceControlLogger logger)
        {
            Logger = logger;
            Shell = new TyShell(remoteShellConnectionInfo, Logger);
        }

        /// <summary>
        /// Löst den Haustür-Buzzer aus
        /// </summary>
        public void Buzz(ConnectInfo remoteConfig)
        {
            try
            {
                Logger.LogDebug(AppResources.Log_Sphinx_StartDoorBuzz);

                Shell.RemoteConfig = remoteConfig;
                Shell.ExecuteCommand(BuzzCommand);

                Logger.LogDebug(AppResources.Log_Sphinx_EndDoorBuzz);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Öffnet die Verriegelung der Wohnungstür
        /// </summary>
        public void Unlock(ConnectInfo remoteConfig)
        {
            try
            {
                Logger.LogDebug(AppResources.Log_Sphinx_StartDoorUnlock);

                Shell.RemoteConfig = remoteConfig;
                Shell.ExecuteCommand(UnlockCommand);

                Logger.LogDebug(AppResources.Log_Sphinx_EndDoorUnlock);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
