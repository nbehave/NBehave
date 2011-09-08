using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NBehave.Narrator.Framework.Messages;
using NBehave.Narrator.Framework.Processors;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework.Contracts
{
    public class LoadScenarioFiles : IMessageProcessor
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly ITinyMessengerHub _hub;

        public LoadScenarioFiles(NBehaveConfiguration configuration, ITinyMessengerHub hub)
        {
            _configuration = configuration;
            _hub = hub;

            _hub.Subscribe<RunStartedEvent>(started => Initialise(), true);
        }

        public void Initialise()
        {
            IEnumerable<string> files = _configuration
                .ScenarioFiles
                .Select(loc => GetFiles(loc))
                .SelectMany(enumerable => enumerable);

            _hub.Publish(new ScenarioFilesLoaded(this, files));
        }

        private IEnumerable<string> GetFiles(string location)
        {
            string[] files;
            if (Path.IsPathRooted(location))
            {
                files = Directory.GetFiles(Path.GetDirectoryName(location), Path.GetFileName(location));
            }
            else
            {
                var absoluteLocation = GetAbsolutePath(location);
                var path = Path.GetFileName(absoluteLocation);
                var pattern = Path.GetDirectoryName(absoluteLocation);
                files = Directory.GetFiles(pattern, path);
            }

            return files;
        }

        private string GetAbsolutePath(string location)
        {
            var directory = Path.GetDirectoryName(location);
            var fileName = Path.GetFileName(location);
            var fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, directory));
            var fullLocation = Path.Combine(fullPath, fileName);
            return fullLocation;
        }
    }
}