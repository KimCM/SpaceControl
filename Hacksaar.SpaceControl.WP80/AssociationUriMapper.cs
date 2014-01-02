using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Windows.Phone.Storage.SharedAccess;

namespace Hacksaar.SpaceControl.WP80
{
    class AssociationUriMapper : UriMapperBase
    {
        private string tempUri;

        public override Uri MapUri(Uri uri)
        {
            tempUri = uri.ToString();

            if (tempUri.Contains("/FileTypeAssociation"))
            {
                int fileIDIndex = tempUri.IndexOf("fileToken=", StringComparison.InvariantCulture) + 10;
                string fileID = tempUri.Substring(fileIDIndex);
                string incomingFileName = SharedStorageAccessManager.GetSharedFileName(fileID);
                string incomingFileType = Path.GetExtension(incomingFileName);

                // Map the .key file to the settings page.
                switch (incomingFileType)
                {
                    case ".key":
                        return new Uri("/Settings.xaml?fileToken=" + fileID, UriKind.Relative);
                    default:
                        return new Uri("/MainPage.xaml", UriKind.Relative);
                }
            }
            // Otherwise perform normal launch.
            return uri;
        }
    }
}
