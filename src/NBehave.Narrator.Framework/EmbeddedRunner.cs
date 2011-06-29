using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NBehave.Narrator.Framework.EventListeners;

namespace NBehave.Narrator.Framework
{
    public static class EmbeddedRunner
    {
        public static void Execute(this string featureFile, params IEventListener[] eventListeners)
        {
            var stackTrace = new StackTrace();
            Type type = stackTrace.GetFrame(1).GetMethod().DeclaringType;
            featureFile.Execute(type.Assembly, eventListeners);
        }

        public static void Execute(this string featureFile, Assembly assembly, params IEventListener[] eventListenersArg)
        {
            var multiEventListener = AddEventListeners(eventListenersArg);
            var assemblyPath = new Uri(assembly.CodeBase).LocalPath;
            var absolutePathToFeature = AbsolutePathToFeature(featureFile, assemblyPath);
            var config = NBehaveConfiguration
                .New
                .SetAssemblies(new[] { assemblyPath })
                .SetEventListener(multiEventListener)
                .SetScenarioFiles(new[] { absolutePathToFeature });
            var runner = config.Build();
            runner.Run();
        }

        private static MultiOutputEventListener AddEventListeners(IEnumerable<IEventListener> eventListenersArg)
        {
            var eventListeners = new List<IEventListener>();
            eventListeners.AddRange(eventListenersArg);

            eventListeners.Add(new ColorfulConsoleOutputEventListener());
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