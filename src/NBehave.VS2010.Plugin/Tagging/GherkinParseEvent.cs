using System.Collections.Generic;
using System.Linq;
using NBehave.Gherkin;

namespace NBehave.VS2010.Plugin.Tagging
{
    public class GherkinParseEvent
    {
        public GherkinParseEvent(GherkinTokenType gherkinTokenType, params Token[] tokens)
        {
            GherkinTokenType = gherkinTokenType;
            Tokens = tokens.ToList();
        }

        public readonly GherkinTokenType GherkinTokenType;
        public readonly List<Token> Tokens;

        public bool Equals(GherkinParseEvent obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return obj.GherkinTokenType == GherkinTokenType
                &&  TokensAreEqual(obj);
        }

        private bool TokensAreEqual(GherkinParseEvent other)
        {
            if (other.Tokens.Count != Tokens.Count)
                return false;
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i] != other.Tokens[i])
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GherkinParseEvent);
        }

        public override int GetHashCode()
        {
            return GherkinTokenType.GetHashCode();
        }

        public static bool operator ==(GherkinParseEvent left, GherkinParseEvent right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GherkinParseEvent left, GherkinParseEvent right)
        {
            return !Equals(left, right);
        }
    }
}