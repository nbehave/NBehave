using NBehave.Narrator.Framework;

namespace NBehave.VS2010.Plugin.Editor.Domain
{
    public class GherkinText
    {
        private readonly Scenario scenario;
        private readonly string scenarioAsString;

        public GherkinText(Scenario scenario)
        {
            this.scenario = scenario;
            scenarioAsString = scenario.ToString();
        }

        public override string ToString()
        {
            return scenarioAsString;
        }

        protected bool Equals(GherkinText other)
        {
            if (other == null) return false;
            return scenarioAsString.Equals(other.scenario.ToString());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as GherkinText);
        }

        public override int GetHashCode()
        {
            return scenarioAsString.GetHashCode();
        }

        public static bool operator ==(GherkinText left, GherkinText right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GherkinText left, GherkinText right)
        {
            return !Equals(left, right);
        }
    }
}