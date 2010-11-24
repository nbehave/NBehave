namespace NBehave.Narrator.Framework.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NBehave.Narrator.Framework.Messages;
    using NBehave.Narrator.Framework.Processors;
    using NBehave.Narrator.Framework.Tiny;

    public class LoadScenarioFiles : IMessageProcessor
    {
        private readonly NBehaveConfiguration _configuration;
        private readonly ITinyMessengerHub _hub;

        public LoadScenarioFiles(NBehaveConfiguration configuration, ITinyMessengerHub hub)
        {
            this._configuration = configuration;
            this._hub = hub;

            this._hub.Subscribe<RunStarted>(started => this.Initialise());
        }

        public void Initialise()
        {
            IEnumerable<string> files = this._configuration
                .ScenarioFiles
                .Select(this.GetFiles)
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