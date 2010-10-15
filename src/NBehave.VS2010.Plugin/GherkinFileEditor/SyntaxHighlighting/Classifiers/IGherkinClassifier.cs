using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace NBehave.VS2010.Plugin.GherkinFileEditor.SyntaxHighlighting.Classifiers
{
    public interface IGherkinClassifier
    {
        bool CanClassify(ParserEvent parserEvent);
        IList<ClassificationSpan> Classify(ParserEvent event3);
    }
}