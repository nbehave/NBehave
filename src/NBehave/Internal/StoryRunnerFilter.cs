using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Internal
{
    [Serializable]
    public class StoryRunnerFilter : ISerializable
    {
        public const string MatchAnything = ".*";

        public StoryRunnerFilter()
            : this(MatchAnything, MatchAnything, MatchAnything)
        {
        }

        public StoryRunnerFilter(string namespaceFilter, string classNameFilter, string methodNameFilter)
        {
            NamespaceFilter = new Regex(AnchorValue(namespaceFilter));
            ClassNameFilter = new Regex(AnchorValue(classNameFilter));
            MethodNameFiler = new Regex(AnchorValue(methodNameFilter));
        }

        public Regex NamespaceFilter { get; private set; }
        public Regex ClassNameFilter { get; private set; }
        public Regex MethodNameFiler { get; private set; }

        public static StoryRunnerFilter GetFilter(MemberInfo member)
        {
            var nsFilter = MatchAnything;
            var clsFilter = MatchAnything;
            var memberNameFilter = MatchAnything;
            if (member != null)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Method:
                        memberNameFilter = member.Name;
                        clsFilter = member.DeclaringType.Name;
                        nsFilter = member.DeclaringType.Namespace;
                        break;
                    case MemberTypes.TypeInfo:
                        clsFilter = member.Name;
                        break;
                    case MemberTypes.All:
                        break;
                    case MemberTypes.Constructor:
                        break;
                    case MemberTypes.Custom:
                        break;
                    case MemberTypes.Event:
                        break;
                    case MemberTypes.Field:
                        break;
                    case MemberTypes.NestedType:
                        break;
                    case MemberTypes.Property:
                        break;
                }
            }

            return new StoryRunnerFilter(nsFilter, clsFilter, memberNameFilter);
        }

        private string AnchorValue(string value)
        {
            if (value == MatchAnything)
                return value;
            return "^" + value + "$";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("n", NamespaceFilter);
            info.AddValue("c", ClassNameFilter);
            info.AddValue("m", MethodNameFiler);
        }

        protected StoryRunnerFilter(SerializationInfo info, StreamingContext context)
        {
            NamespaceFilter = (Regex)info.GetValue("n", typeof(Regex));
            ClassNameFilter = (Regex)info.GetValue("c", typeof(Regex));
            MethodNameFiler = (Regex)info.GetValue("m", typeof(Regex));
        }
    }
}
