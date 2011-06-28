using System;
using JetBrains.Application.Components;
using JetBrains.Application.Configuration;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using Microsoft.Win32;

namespace NBehave.ReSharper.Plugin
{
    [SolutionComponent(ProgramConfigurations.VS_ADDIN)]
    public class TestProviderSettings : UnitTestProviderSettingsBase
    {
        private string _nbehaveInstallDir;

        public TestProviderSettings(SolutionSettingsComponent solutionSettings)
            : base(solutionSettings)
        { }

        protected override string SectionName
        {
            get
            {
                return "UnitTestRunner";
            }
        }

        [XmlExternalizable(null, SettingName = "NBehaveInstallDir")]
        public virtual string NBehaveInstallDir
        {
            get
            {
                if (string.IsNullOrEmpty(_nbehaveInstallDir))
                {
                    _nbehaveInstallDir = string.Empty;
                    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software");
                    if (registryKey != null)
                    {
                        RegistryKey registryKey2 = registryKey.OpenSubKey("NBehave");
                        if (registryKey2 != null)
                        {
                            string[] subKeyNames = registryKey2.GetSubKeyNames();
                            if (subKeyNames.Length > 0)
                            {
                                Array.Sort(subKeyNames);
                                string name = subKeyNames[subKeyNames.Length - 1];
                                RegistryKey registryKey4 = registryKey2.OpenSubKey(name);
                                if (registryKey4 != null)
                                {
                                    object value = registryKey4.GetValue("InstallDir");
                                    if (value != null)
                                    {
                                        _nbehaveInstallDir = value.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                return _nbehaveInstallDir;
            }
            set
            {
                _nbehaveInstallDir = value;
            }
        }
    }
}