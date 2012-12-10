using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NBehave.Configuration;
using NBehave.EventListeners;
using NBehave.Extensions;

namespace NBehave
{
    public static class EmbeddedRunner
    {
        public static void ExecuteText(this string featureText, params IEventListener[] eventListeners)
        {
            var stackTrace = new StackTrace();
            Type type = stackTrace.GetFrame(1).GetMethod().DeclaringType;
            featureText.ExecuteText(type.Assembly, eventListeners);
        }

        public static void ExecuteText(this string featureText, Assembly assembly, params IEventListener[] eventListeners)
        {
            var file = Path.GetTempFileName();
            File.WriteAllText(file, featureText);
            file.ExecuteFile(assembly, eventListeners);
        }

        public static void ExecuteFile(this string featureFile, params IEventListener[] eventListeners)
        {
            var stackTrace = new StackTrace();
            Type type = stackTrace.GetFrame(1).GetMethod().DeclaringType;
            featureFile.ExecuteFile(type.Assembly, eventListeners);
        }

        public static void ExecuteFile(this string featureFile, Assembly assembly, params IEventListener[] eventListenersArg)
        {
            var multiEventListener = AddEventListeners(eventListenersArg);
            var assemblyPath = new Uri(assembly.CodeBase).LocalPath;
            var absolutePathToFeature = AbsolutePathToFeature(featureFile, assemblyPath);
            var config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { assemblyPath })
                .SetEventListener(multiEventListener)
                .SetScenarioFiles(new[] { absolutePathToFeature })
                .DontIsolateInAppDomain();
            var runner = config.Build();
            runner.Run();
        }

        private static MultiOutputEventListener AddEventListeners(IEnumerable<IEventListener> eventListenersArg)
        {
            var eventListeners = new List<IEventListener>();
            eventListeners.AddRange(eventListenersArg);

            if (eventListeners.Any() == false)
                eventListeners.Add(EventListeners.EventListeners.ColorfulConsoleOutputEventListener());
            eventListeners.Add(new FailSpecResultEventListener());

            var multiEventListener = new MultiOutputEventListener(eventListeners.ToArray());
            return multiEventListener;
        }

        private static string AbsolutePathToFeature(string featureFile, string assemblyPath)
        {
            string absolutePathToFeature = (Path.IsPathRooted(featureFile)) ? featureFile : Path.Combine(Path.GetDirectoryName(assemblyPath), featureFile);
            return absolutePathToFeature;
        }
    }
}