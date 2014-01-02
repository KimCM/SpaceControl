using Hacksaar.SpaceControl.WP80.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Hacksaar.SpaceControl.WP80
{
    public class SpaceControlLogger : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dispatcher UiDispatcher { get; set; }

        public SpaceControlLogger(Dispatcher dispatcher)
        {
            UiDispatcher = dispatcher;
            Logged = new ObservableCollection<SpaceControlLogEntry>();
        }

        public ObservableCollection<SpaceControlLogEntry> Logged { get; private set; }

        public void ClearLog()
        {
            Logged.Clear();
            LogDebug(AppResources.Log_Logger_LogCleared);
        }

        private void Log(SpaceControlLogSeverity severity, string message, params object[] args)
        {
            var logMessage = string.Format(CultureInfo.CurrentCulture, message, args);
            var logEntry = new SpaceControlLogEntry { CreatedAt = DateTime.Now, Severity = severity, Text = logMessage };
            UiDispatcher.BeginInvoke(delegate { Logged.Add(logEntry); });
            NotifyPropertyChanged("Logged");
        }

        public void LogDebug(string message, params object[] args)
        {
            Log(SpaceControlLogSeverity.Debug, message, args);
        }

        public void LogInfo(string message, params object[] args)
        {
            Log(SpaceControlLogSeverity.Info, message, args);
        }

        public void LogWarning(Exception exception)
        {
            Log(SpaceControlLogSeverity.Warning, exception.ToString());
        }

        public void LogWarning(string message, params object[] args)
        {
            Log(SpaceControlLogSeverity.Warning, message, args);
        }

        public void LogError(Exception exception)
        {
            Log(SpaceControlLogSeverity.Error, exception.ToString());
        }

        public void LogError(string message, params object[] args)
        {
            Log(SpaceControlLogSeverity.Error, message, args);
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
