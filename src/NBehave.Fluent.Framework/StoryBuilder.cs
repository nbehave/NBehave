using System;

namespace NBehave.Fluent.Framework
{
    public class StoryBuilder
    {
        private readonly Feature _feature;
        private string Role { get; set; }
        private string Goal { get; set; }
        private string Reason { get; set; }

        public StoryBuilder(Feature feature)
        {
            _feature = feature;
        }

        private Feature ConstructNarrative()
        {
            _feature.Narrative = String.Format("As a {0}, I want {1}", Role, Goal)
                                 + (String.IsNullOrEmpty(Reason) ? "" : String.Format(" so that {0}", Reason));
            return _feature;
        }

        public static implicit operator Feature(StoryBuilder builder)
        {
            return builder.ConstructNarrative();
        }

        internal class AsAFragment : IAsAFragment
        {
            private readonly StoryBuilder _builder;

            public AsAFragment(Feature feature)
            {
                _builder = new StoryBuilder(feature);
            }

            IIWantFragment IAsAFragment.AsA(string role)
            {
                _builder.Role = role;
                return new IWantFragment(_builder);
            }
        }

        internal class IWantFragment : IIWantFragment
        {
            private readonly StoryBuilder _builder;

            public IWantFragment(StoryBuilder builder)
            {
                _builder = builder;
            }

            ISoThatFragment IIWantFragment.IWant(string goal)
            {
                _builder.Goal = goal;
                return new SoThatFragment(_builder);
            }
        }

        internal class SoThatFragment : ISoThatFragment
        {
            private readonly StoryBuilder _builder;

            public SoThatFragment(StoryBuilder builder)
            {
                _builder = builder;
            }

            StoryBuilder ISoThatFragment.SoThat(string reason)
            {
                _builder.Reason = reason;
                return _builder;
            }
        }
    }
}
