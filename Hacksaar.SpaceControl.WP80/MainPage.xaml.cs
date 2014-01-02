using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Hacksaar.SpaceControl.WP80.Space;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Hacksaar.SpaceControl.WP80.Resources;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Data;

namespace Hacksaar.SpaceControl.WP80
{
    public partial class MainPage : PhoneApplicationPage
    {
        public const string Url = "/MainPage.xaml";

        private const string DoorOpenImage = @"/Assets/SpaceIsOpen.jpg";
        private const string DoorClosedImage = @"/Assets/SpaceIsClosed.jpg";

        public SpaceControlLogger Logger { get; set; }

        private Sphinx Sphinx { get; set; }

        private AppSettings AppSettings { get; set; }

        private BackgroundWorker buzzWorker = new BackgroundWorker();
        private BackgroundWorker unlockWorker = new BackgroundWorker();
        private BackgroundWorker doorStateWorker = new BackgroundWorker();

        public MainPage()
        {
            Logger = new SpaceControlLogger(Dispatcher);

            InitializeComponent();
            AppSettings = new AppSettings();
            LogDisplay.DataContext = Logger;
            Logger.PropertyChanged += LoggerOnPropertyChanged;

            Sphinx = new Sphinx(AppSettings.ConnectionInfo, Logger);

            buzzWorker.DoWork += (s, e) => Sphinx.Buzz(AppSettings.ConnectionInfo);
            unlockWorker.DoWork += (s, e) => Sphinx.Unlock(AppSettings.ConnectionInfo);
            doorStateWorker.DoWork += (s, e) => RefreshPublicDoorStateAsync();

            BuildLocalizedApplicationBar();

            doorStateWorker.RunWorkerAsync();
        }

        private void LoggerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (Equals("Logged", propertyChangedEventArgs.PropertyName))
                Dispatcher.BeginInvoke(() => { LogDisplay.ScrollIntoView(Logger.Logged.LastOrDefault()); });
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar { IsMenuEnabled = true };

            var appBarClearLogButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/delete.png", UriKind.Relative));
            appBarClearLogButton.Text = AppResources.AppBarButtonClearLogText;
            appBarClearLogButton.Click += (s, e) => Logger.ClearLog();
            ApplicationBar.Buttons.Add(appBarClearLogButton);

            var appBarSettingsButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.settings.png", UriKind.Relative));
            appBarSettingsButton.Text = AppResources.AppBarButtonSettingsText;
            appBarSettingsButton.Click += (s, e) => NavigationService.Navigate(new Uri(Settings.Url, UriKind.RelativeOrAbsolute));
            ApplicationBar.Buttons.Add(appBarSettingsButton);
        }

        private void Buzz_OnClick(object sender, RoutedEventArgs e)
        {
            if (buzzWorker.IsBusy)
                return;

            buzzWorker.RunWorkerAsync();
        }

        private void Unlock_OnClick(object sender, RoutedEventArgs e)
        {
            if (unlockWorker.IsBusy)
                return;

            unlockWorker.RunWorkerAsync();
        }

        private void CheckDoorState_Click(object sender, RoutedEventArgs e)
        {
            if (doorStateWorker.IsBusy)
                return;

            doorStateWorker.RunWorkerAsync();
        }

        private async void RefreshPublicDoorStateAsync()
        {
            try
            {
                Logger.LogDebug(AppResources.Log_PublicDoorState_StartRequest);
                string spaceApiUrl = AppSettings.SpaceApiUrl;
                var task = GetJson(spaceApiUrl);

                AcivateDoorStateProgressBar();

                string rawJson = await task;
                Logger.LogDebug(AppResources.Log_PublicDoorState_SpaceApiResponse, spaceApiUrl, rawJson);

                dynamic hacksaarApiContent = JsonConvert.DeserializeObject(rawJson);
                bool isDoorOpen = hacksaarApiContent.state.open;
                Logger.LogInfo(AppResources.Log_PublicDoorState_State, isDoorOpen);

                var imageUrl = isDoorOpen ? DoorOpenImage : DoorClosedImage;
                Logger.LogDebug(AppResources.Log_PublicDoorState_ImageUrl, imageUrl);

                Dispatcher.BeginInvoke(() => PublicDoorStateIndicator.Source = new BitmapImage(new Uri(imageUrl, UriKind.Relative)));
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                Dispatcher.BeginInvoke(() => PublicDoorStateIndicator.Source = null);
            }
            finally
            {
                DeactivateDoorStateProgressBar();
                Logger.LogDebug(AppResources.Log_PublicDoorState_EndRequest);
            }
        }

        private void DeactivateDoorStateProgressBar()
        {
            Dispatcher.BeginInvoke(() =>
           {
               PublicDoorStateProgressBar.IsIndeterminate = false;
               PublicDoorStateProgressBar.Visibility = Visibility.Collapsed;
               PublicDoorStateIndicator.Visibility = Visibility.Visible;
           });
        }

        private void AcivateDoorStateProgressBar()
        {
            Dispatcher.BeginInvoke(() =>
            {
                PublicDoorStateProgressBar.IsIndeterminate = true;
                PublicDoorStateProgressBar.Visibility = Visibility.Visible;
                PublicDoorStateIndicator.Visibility = Visibility.Collapsed;
            });
        }

        public async Task<string> GetJson(string urlToCall)
        {
            var request = (HttpWebRequest)WebRequest.Create(urlToCall);
            request.Method = HttpMethod.Get;
            var response = await request.GetResponseAsync();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }
    }
}