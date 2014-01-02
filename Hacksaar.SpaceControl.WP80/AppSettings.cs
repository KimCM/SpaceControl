using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hacksaar.SpaceControl.WP80.Space;
using Windows.Storage;

namespace Hacksaar.SpaceControl.WP80
{
    public class AppSettings
    {
        protected const string KeyHostname = "HostnameKey";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "KeyPort")]
        protected const string KeyPort = "PortKey";
        protected const string KeyUsername = "UsernameKey";
        protected const string KeyPassword = "PasswordKey";
        protected const string KeyPrivateKeyFilePath = "PrivateKeyFilePathKey";
        protected const string KeyPrivateKeyFile = "PrivateKeyFileKey";
        protected const string KeyPrivateKeyFilePassphrase = "PrivateKeyFilePassphraseKey";
        protected const string KeySpaceApiUrl = "SpaceApiUriKey";
        protected const string KeyLogSeverity = "LogSeverityKey";
        protected const string KeyTimeoutInSeconds = "TimeoutInSecondsKey";

        private readonly IsolatedStorageSettings settings;

        public ConnectInfo ConnectionInfo
        {
            get
            {
                return new ConnectInfo
                    {
                        Hostname = Hostname,
                        Port = Port,
                        Username = Username,
                        Password = Password,
                        PrivateKeyFile = PrivateKeyFile,
                        PrivateKeyFilePassPhrase = PrivateKeyPassphrase,
                        TimeoutInSeconds = TimeoutInSeconds
                    };
            }
        }

        public AppSettings()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string key, Object value)
        {
            bool valueChanged = false;

            if (settings.Contains(key))
            {
                if (!Equals(settings[key], value))
                {
                    settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                settings.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (settings.Contains(key))
                return (T)settings[key];

            return defaultValue;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }

        /// <summary>
        /// Property to get and set the Url to the SpaceApi
        /// </summary>
        public string SpaceApiUrl
        {
            get { return GetValueOrDefault(KeySpaceApiUrl, @"http://spaceapi.net/cache/Hacksaar"); }
            set { if (AddOrUpdateValue(KeySpaceApiUrl, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the TyShell Hostname
        /// </summary>
        public string Hostname
        {
            get { return GetValueOrDefault(KeyHostname, "192.168.178.222"); }
            set { if (AddOrUpdateValue(KeyHostname, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the TyShell Port
        /// </summary>
        public int Port
        {
            get { return GetValueOrDefault(KeyPort, 22); }
            set { if (AddOrUpdateValue(KeyPort, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the TyShell Username
        /// </summary>
        public string Username
        {
            get { return GetValueOrDefault(KeyUsername, string.Empty); }
            set { if (AddOrUpdateValue(KeyUsername, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the TyShell Password
        /// </summary>
        public string Password
        {
            get { return GetValueOrDefault(KeyPassword, string.Empty); }
            set { if (AddOrUpdateValue(KeyPassword, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the TyShell PrivateKeyFilePath
        /// </summary>
        public string PrivateKeyFilePath
        {
            get { return GetValueOrDefault(KeyPrivateKeyFilePath, string.Empty); }
            set { if (AddOrUpdateValue(KeyPrivateKeyFilePath, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the TyShell PrivateKeyFile
        /// </summary>
        public byte[] PrivateKeyFile
        {
            get { return GetValueOrDefault(KeyPrivateKeyFile, new byte[] { }); }
            set { if (AddOrUpdateValue(KeyPrivateKeyFile, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the TyShell PrivateKeyPassphrase
        /// </summary>
        public string PrivateKeyPassphrase
        {
            get { return GetValueOrDefault(KeyPrivateKeyFilePassphrase, string.Empty); }
            set { if (AddOrUpdateValue(KeyPrivateKeyFilePassphrase, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the Log Level / Log Severity
        /// </summary>
        public SpaceControlLogSeverity LogSeverity
        {
            get { return GetValueOrDefault(KeyLogSeverity, SpaceControlLogSeverity.Info); }
            set { if (AddOrUpdateValue(KeyLogSeverity, value)) Save(); }
        }

        /// <summary>
        /// Property to get and set the Timeout for SSH commands
        /// </summary>
        public uint TimeoutInSeconds
        {
            get { return GetValueOrDefault(KeyTimeoutInSeconds, 10u); }
            set { if (AddOrUpdateValue(KeyTimeoutInSeconds, value)) Save(); }
        }
    }
}
