using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using NBehave.VS2010.Plugin.Contracts;

namespace NBehave.VS2010.Plugin.Domain
{
    [Serializable]
    public class PluginConfiguration : IPluginConfiguration, IDisposable
    {
        private string configFile;
        private static readonly object SaveLock = new object();
        private readonly ISolutionEventsListener solutionEvents;
        private readonly IVisualStudioService visualStudioService;
        private bool disposed;
        private bool createHtmlReport;

        public bool CreateHtmlReport
        {
            get { return createHtmlReport; }
            set
            {
                createHtmlReport = value;
                OnPropertyChanged("CreateHtmlReport");
            }
        }

        private PluginConfiguration()
        { }

        public PluginConfiguration(IVisualStudioService visualStudioService, ISolutionEventsListener solutionEvents)
            : this()
        {
            this.visualStudioService = visualStudioService;
            this.solutionEvents = solutionEvents;
            this.solutionEvents.AfterSolutionLoaded += SolutionLoaded;
            this.solutionEvents.BeforeSolutionClosed += SolutionClosed;
            if (!visualStudioService.IsSolutionOpen)
                return;

            SetConfigFile();
            LoadConfig();
        }

        private void SolutionLoaded()
        {
            if (visualStudioService.IsSolutionOpen)
            {
                SetConfigFile();
                LoadConfig();
            }
        }

        private void SolutionClosed()
        {
            SaveConfig();
        }

        private void SetConfigFile()
        {
            var solutionFolder = visualStudioService.SolutionFolder;
            configFile = Path.Combine(solutionFolder, ".nbehave", "plugin.config");
        }

        private void LoadConfig()
        {
            if (!File.Exists(configFile))
            {
                CreateHtmlReport = true;
                SaveConfig();
                return;
            }
            var serializer = new XmlSerializer(typeof(PluginConfiguration));
            PluginConfiguration configuration;
            using (var stream = new StreamReader(configFile))
            {
                configuration = (PluginConfiguration)serializer.Deserialize(stream);
            }
            SetProperties(configuration);
        }

        private void SetProperties(PluginConfiguration configuration)
        {
            foreach (var property in configuration.GetType().GetProperties())
            {
                var value = property.GetValue(configuration, null);
                property.SetValue(this, value, null);
            }
        }

        private void SaveConfig()
        {
            lock (SaveLock)
            {
                CreateFolder();
                var serializer = new XmlSerializer(typeof(PluginConfiguration));
                using (var writer = new StreamWriter(configFile, false))
                {
                    serializer.Serialize(writer, this);
                }
            }
        }

        private void CreateFolder()
        {
            var path = Path.GetDirectoryName(configFile) ?? "";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if (solutionEvents != null)
                {
                    solutionEvents.AfterSolutionLoaded -= SolutionLoaded;
                    solutionEvents.BeforeSolutionClosed -= SolutionClosed;
                }
            }
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}