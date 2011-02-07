using System.Collections.Generic;
using System.IO;

namespace NBehave.Narrator.Framework.Text
{
    public interface IRunner
    {
        void Load(IEnumerable<string> scenarioFiles);
        bool IsDryRun { get; set; }
        StoryRunnerFilter StoryRunnerFilter { get; set; }
        void LoadAssembly(string path);
        FeatureResults Run();
        FeatureResults Run(PlainTextOutput output);
        void Load(Stream stream);
    }
}