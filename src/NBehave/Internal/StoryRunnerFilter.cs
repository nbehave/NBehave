using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NBehave.Internal
{
    [Serializable]
    public class StoryRunnerFilter : ISerializable
    {
        private readonly Regex namespaceFilter;
        private readonly Regex classNameFilter;
        private readonly Regex methodNameFilter;

        public StoryRunnerFilter()
            : this(".", ".", ".")
        {
        }

        public StoryRunnerFilter(string namespaceFilter, string classNameFilter, string methodNameFilter)
        {
            this.namespaceFilter = new Regex(AnchorValue(namespaceFilter));
            this.classNameFilter = new Regex(AnchorValue(classNameFilter));
            this.methodNameFilter = new Regex(AnchorValue(methodNameFilter));
        }

        public Regex NamespaceFilter
        {
            get { return this.namespaceFilter; }
        }

        public Regex ClassNameFilter
        {
            get { return this.classNameFilter; }
        }

        public Regex MethodNameFiler
        {
            get { return this.methodNameFilter; }
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

        private string AnchorValue(string value)
        {
            if (value == ".")
            {
                return value;
            }

            return "^" + value + "$";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("n", namespaceFilter);
            info.AddValue("c", classNameFilter);
            info.AddValue("m", methodNameFilter);
        }

        protected StoryRunnerFilter(SerializationInfo info, StreamingContext context)
        {
            namespaceFilter = (Regex) info.GetValue("n", typeof (Regex));
            classNameFilter = (Regex) info.GetValue("c", typeof (Regex));
            methodNameFilter = (Regex) info.GetValue("m", typeof (Regex));
        }
    }
}
