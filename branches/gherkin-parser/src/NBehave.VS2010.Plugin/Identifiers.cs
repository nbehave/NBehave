// Guids.cs
// MUST match guids.h
using System;

namespace NBehave.VS2010.Plugin
{
    internal static class Identifiers
    {
        public const string PackageGuidString = "39b466a1-7b58-4a9d-91d1-010d05512884";
        public const string CommandGroupGuidString = "14e96ffb-091b-4471-9a94-f5c9790b5f4b";

        public const uint RunCommandId = 0x100;
        public static readonly Guid CommandGroupGuid = new Guid(CommandGroupGuidString);
        public static uint DebugCommandId = 0x101;
    } ;
}