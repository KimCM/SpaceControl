using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Common;
using Hacksaar.SpaceControl.WP80.Resources;

namespace Hacksaar.SpaceControl.WP80.Space
{
    /// <summary>
    /// TyShell repräsentiert die Python-Shell zur Türsteuerung. Tür-Python-Shell => TyShell.
    /// </summary>
    public class TyShell : IShell
    {
        public SpaceControlLogger Logger { get; set; }

        public ConnectInfo RemoteConfig { get; set; }

        private const string TyShellPrompt = "$";
        private const string TyShellWelcomeMessage = "Welcome to tyshell. Use help to see what you can do.\r\n$ ";

        public TyShell(ConnectInfo remoteConfig, SpaceControlLogger logger)
        {
            Logger = logger;
            RemoteConfig = remoteConfig;
        }

        public string ExecuteCommand(string command)
        {
            var result = string.Empty;
            try
            {
                using (var ssh = GetOrCreateSshClient(RemoteConfig))
                {
                    Connect(ssh);

                    using (var stream = ssh.CreateShellStream("terminal " + command, 80, 24, 800, 600, 1024))
                    {
                        var timeout = new TimeSpan(0, 0, Convert.ToInt32(RemoteConfig.TimeoutInSeconds));

                        var line = stream.ReadLine();
                        stream.Expect(new[] { new ExpectAction(TyShellPrompt, response => Logger.LogDebug(response)) });

                        stream.Expect(timeout, new[] { new ExpectAction(TyShellWelcomeMessage, response => Logger.LogInfo(response)) });
                        stream.Flush();

                        Logger.LogDebug(AppResources.Log_TyShell_SendingCommand, command);
                        stream.WriteLine(command);
                        stream.Expect(timeout, new[] { new ExpectAction("", response => Logger.LogDebug(AppResources.Log_TyShell_Response, response)) });

                        result = stream.ReadLine();
                        Logger.LogDebug(AppResources.Log_TyShell_Result, result);
                        stream.Close();
                    }

                    Disconnect(ssh);
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(e);
            }
            return result;
        }

        private SshClient GetOrCreateSshClient(ConnectInfo remote)
        {
            if (remote.PrivateKeyFile == null || remote.PrivateKeyFile.Count() <= 0)
                return new SshClient(remote.Hostname, remote.Port, remote.Username, remote.Password);

            using (var stream = new MemoryStream(remote.PrivateKeyFile))
            {
                var privateKeyFile = string.IsNullOrEmpty(remote.PrivateKeyFilePassPhrase)
                                         ? new PrivateKeyFile(stream)
                                         : new PrivateKeyFile(stream, remote.PrivateKeyFilePassPhrase);

                return new SshClient(remote.Hostname, remote.Port, remote.Username, new[] { privateKeyFile });
            }
        }

        private void Connect(SshClient ssh)
        {
            if (ssh.IsConnected)
                return;

            Logger.LogDebug(AppResources.Log_TyShell_TryingToConnect, RemoteConfig.Username, RemoteConfig.Hostname, RemoteConfig.Port);
            ssh.Connect();
            Logger.LogInfo(AppResources.Log_TyShell_ConnectedSuccessfully, RemoteConfig.Username, RemoteConfig.Hostname, RemoteConfig.Port);
        }

        private void Disconnect(SshClient ssh)
        {
            Logger.LogDebug(AppResources.Log_TyShell_Disconnecting);
            ssh.Disconnect();
            Logger.LogDebug(AppResources.Log_TyShell_DisconnectedSuccessfully);
        }
    }
}
