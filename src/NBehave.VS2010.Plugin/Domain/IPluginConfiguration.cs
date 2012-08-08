using System.ComponentModel;

namespace NBehave.VS2010.Plugin.Domain
{
    public interface IPluginConfiguration : INotifyPropertyChanged
    {
        bool CreateHtmlReport { get; set; }
    }
}