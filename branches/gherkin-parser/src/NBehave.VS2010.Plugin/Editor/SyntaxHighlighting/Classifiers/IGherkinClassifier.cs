using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using NBehave.VS2010.Plugin.Editor.Domain;

namespace NBehave.VS2010.Plugin.Editor.SyntaxHighlighting.Classifiers
{
    public interface IGherkinClassifier
    {
        bool CanClassify(ParserEvent parserEvent);
        IList<ClassificationSpan> Classify(ParserEvent parserEvent);
    }
}