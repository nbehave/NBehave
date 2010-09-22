using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionStep
    {
        public IEnumerable<string> FeatureSteps;
        public IEnumerable<string> ScenarioTitles;
        public IEnumerable<string> ScenarioSteps;
        public IEnumerable<string> Examples;
        public IEnumerable<string> AllWords;

        private readonly IEnumerable<Language> _languages;
        public const string DefaultLanguage = "en";

        const string FeatureKey = "feature";
        const string ScenarioKey = "scenario";
        const string ExamplesKey = "examples";
        const string TitleKey = "title";
        private readonly IEnumerable<string> _stepKeys = new[] { "given", "when", "then", "and", "but" };

        public ActionStep(IEnumerable<Language> languages)
            : this(languages, DefaultLanguage)
        {
        }

        public ActionStep(IEnumerable<Language> languages, string language)
        {
            _languages = languages;
            Language languageToUse = _languages.First(_ => _.Lang == language);
            Setup(languageToUse);
        }

        private void Setup(Language language)
        {
            SetupFeatureSteps(language);
            SetupScenarioTitles(language);
            SetupExamples(language);
            SetupScenarioSteps(language);
            SetupAllWords();
        }

        private void SetupFeatureSteps(Language language)
        {
            var featureSteps = new List<string>();
            featureSteps.AddRange(language[FeatureKey]);
            FeatureSteps = featureSteps;
        }

        private void SetupScenarioTitles(Language language)
        {
            var scenarioTitles = new List<string>();
            scenarioTitles.AddRange(language[ScenarioKey]);
            ScenarioTitles = scenarioTitles;
        }

        private void SetupExamples(Language language)
        {
            var ex = new List<string>();
            ex.AddRange(language[ExamplesKey]);
            Examples = ex;
        }

        private void SetupScenarioSteps(Language language)
        {
            var scenarioSteps = new List<string>();
            AddScenarioSteps(scenarioSteps, language);
            ScenarioSteps = scenarioSteps;
        }

        private void SetupAllWords()
        {
            var words = new List<string>();

            words.AddRange(FeatureSteps);
            words.AddRange(ScenarioTitles);
            words.AddRange(Examples);
            words.AddRange(ScenarioSteps);
            AllWords = words;
        }

        private readonly Regex _title = new Regex(@"\s*\w+:?\s+(?<title>.*)", RegexOptions.Compiled);
        public string GetTitle(string actionStep)
        {
            Match m = _title.Match(actionStep);
            string t = m.Groups[TitleKey].Value;
            return t;
        }

        public bool IsFeatureTitle(string actionStep)
        {
            return (IsStepWithMoreThanOneKeyWord(actionStep, FeatureSteps));
        }

        private bool IsStepWithMoreThanOneKeyWord(string actionStep, IEnumerable<string> keyWords)
        {
            foreach (var action in keyWords)
            {
                var isNarrative = new Regex(string.Format(@"\s*{0}:?\s+", action));
                if (isNarrative.IsMatch(actionStep))
                    return true;
            }
            return false;
        }

        public bool IsScenarioTitle(string text)
        {
            return MatchList(text, ScenarioTitles, @"\s*{0}:?\s+");
        }

        public bool IsScenarioStep(string text)
        {
            if (MatchList(text, ScenarioSteps, @"\s*{0}\s+"))
                return true;

            return IsScenarioTitle(text);
        }

        public bool IsExample(string text)
        {
            return MatchList(text, Examples, @"\s*{0}:?\s+");
        }

        private bool MatchList(string text, IEnumerable<string> listToMatch, string regex)
        {
            return listToMatch.Select(step => new Regex(string.Format(regex, step))).Any(_ => _.IsMatch(text));
        }

        private void AddScenarioSteps(List<string> words, Language language)
        {
            foreach (var stepKey in _stepKeys)
                words.AddRange(language[stepKey]);
        }
    }
}
