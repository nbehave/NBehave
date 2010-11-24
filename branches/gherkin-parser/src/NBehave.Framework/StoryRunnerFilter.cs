using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class StoryRunnerFilter
    {
        private readonly Regex _namespaceFilter;
        private readonly Regex _classNameFilter;
        private readonly Regex _methodNameFilter;

        public StoryRunnerFilter()
            : this(".", ".", ".")
        { }

        public StoryRunnerFilter(string namespaceFilter, string classNameFilter, string methodNameFilter)
        {
            _namespaceFilter = new Regex(AnchorValue(namespaceFilter));
            _classNameFilter = new Regex(AnchorValue(classNameFilter));
            _methodNameFilter = new Regex(AnchorValue(methodNameFilter));
        }


        public static StoryRunnerFilter GetFilter(MemberInfo member)
        {
            var nsFilter = ".";
            var clsFilter = ".";
            var memberNameFilter = ".";
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
                    default:
                        break;
                }
            }
            return new StoryRunnerFilter(nsFilter, clsFilter, memberNameFilter);
        }


        public Regex NamespaceFilter
        {
            get { return _namespaceFilter; }
        }

        public Regex ClassNameFilter
        {
            get { return _classNameFilter; }
        }

        public Regex MethodNameFiler
        {
            get { return _methodNameFilter; }
        }

        private string AnchorValue(string value)
        {
            if (value == ".")
                return value;
            return "^" + value + "$";
        }

    }
}
