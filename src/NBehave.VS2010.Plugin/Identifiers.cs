using System;

namespace NBehave.VS2010.Plugin
{
    internal static class Identifiers
    {
        // These guids must match those in VSPackage2.vsct
        public const string NBehavePackageGuidString = "39b466a1-7b58-4a9d-91d1-010d05512884";
        
        public static readonly Guid CommandGroupSet = new Guid("14e96ffb-091b-4471-9a94-f5c9790b5f4b");
        public static readonly Guid TopLevelMenuCmdSet = new Guid("c55f2134-fce5-4d0d-b42c-e4fb78bdfb3e");

        public const uint RunCommandId = 0x100;
        public const uint DebugCommandId = 0x101;

        public const int MenuCommandHtmlReportToggleId = 0x201;


    }
}