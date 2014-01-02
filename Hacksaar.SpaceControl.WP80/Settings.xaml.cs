using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Storage;
using Windows.Storage;
using Hacksaar.SpaceControl.WP80.Resources;

namespace Hacksaar.SpaceControl.WP80
{
    public partial class Settings : PhoneApplicationPage
    {
        public const string Url = "/Settings.xaml";

        public Settings()
        {
            InitializeComponent();
        }

        private async void FilePickerPrivateKeyFile_Click(object sender, RoutedEventArgs e)
        {
            await ProcessSdKeyFile(string.Empty); // read from sd card root
        }

        // Assign the path or token value, depending on how the page was launched.
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Route is from a file association.
            if (NavigationContext.QueryString.ContainsKey("fileToken"))
            {
                var fileToken = NavigationContext.QueryString["fileToken"];
                await ProcessExternalKeyFile(fileToken);
            }
            // Route is from the SD card.
            else if (NavigationContext.QueryString.ContainsKey("sdFilePath"))
            {
                var sdFilePath = NavigationContext.QueryString["sdFilePath"];
                await ProcessSdKeyFile(sdFilePath);
            }
        }

        public async Task ProcessExternalKeyFile(string fileToken)
        {
            string incomingIdFilename = Windows.Phone.Storage.SharedAccess.SharedStorageAccessManager.GetSharedFileName(fileToken);

            string messageBoxText = string.Format(CultureInfo.CurrentCulture, AppResources.Settings_Message_ConfirmPrivateKeyFileReplacement, incomingIdFilename);
            var messageBoxCaption = AppResources.Settings_Caption_ConfirmPrivateKeyFileReplacement;
            var mboxResult = MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OKCancel);
            if (mboxResult != MessageBoxResult.OK)
                return;

            var tempFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("tmp", CreationCollisionOption.OpenIfExists);

            try
            {
                //// Copy the id_rsa (.key file) to the local temp folder.
                IStorageFile idFile = await Windows.Phone.Storage.SharedAccess.SharedStorageAccessManager.CopySharedFileAsync(
                        tempFolder, incomingIdFilename, NameCollisionOption.GenerateUniqueName, fileToken);

                var stream = await idFile.OpenReadAsync();
                PrivateKeyFilePath.Text = incomingIdFilename;
                //TODO KimCM: new AppSettings() is evil! Inject the existing AppSettings instead of creating a new one.
                new AppSettings().PrivateKeyFile = stream.AsStreamForRead().ReadFully();
            }
            finally
            {
                //TODO KimCM: await this operation?
                tempFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public async Task ProcessSdKeyFile(string path)
        {
            ExternalStorageDevice sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

            if (sdCard == null)
            {
                MessageBox.Show(AppResources.Settings_Message_MissingSdCard);
                return;
            }

            try
            {
                var folder = string.IsNullOrEmpty(path) ? sdCard.RootFolder : await sdCard.GetFolderAsync(path);
                var keyFiles = await folder.GetFilesAsync();

                ExternalStorageFile esfToImport = null;
                foreach (var esf in keyFiles)
                {
                    if (esf.Path.EndsWith(".key"))
                        esfToImport = esf;
                }

                if (esfToImport != null)
                {
                    PrivateKeyFilePath.Text = esfToImport.Path;
                    var stream = await esfToImport.OpenForReadAsync();
                    //TODO KimCM: new AppSettings() is evil!
                    new AppSettings().PrivateKeyFile = stream.ReadFully();
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(AppResources.Settings_Message_MissingKeyFile);
            }
        }
    }
}