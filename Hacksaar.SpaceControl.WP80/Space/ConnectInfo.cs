using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacksaar.SpaceControl.WP80.Space
{
    public class ConnectInfo
    {
        public string Hostname { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        /// <summary>
        /// Authenticate with a password OR with the private key file
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Authenticate with a private key file. By setting the private key file the Password-Property will be ignored.
        /// </summary>
        public byte[] PrivateKeyFile { get; set; }

        /// <summary>
        /// Timeout for the SSH command
        /// </summary>
        public uint TimeoutInSeconds { get; set; }

        /// <summary>
        /// Optional: The passphrase to access the private key file
        /// </summary>
        public string PrivateKeyFilePassPhrase { get; set; }

        protected bool Equals(ConnectInfo other)
        {
            return string.Equals(Hostname, other.Hostname)
                && Port == other.Port
                && TimeoutInSeconds == other.TimeoutInSeconds
                && string.Equals(Username, other.Username)
                && string.Equals(Password, other.Password)
                && Equals(PrivateKeyFile, other.PrivateKeyFile)
                && string.Equals(PrivateKeyFilePassPhrase, other.PrivateKeyFilePassPhrase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConnectInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Hostname != null ? Hostname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Port;
                hashCode = (hashCode * 397) ^ (int)TimeoutInSeconds;
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PrivateKeyFile != null ? PrivateKeyFile.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PrivateKeyFilePassPhrase != null ? PrivateKeyFilePassPhrase.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
