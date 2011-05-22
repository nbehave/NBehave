// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryRunnerFilter.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the StoryRunnerFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace NBehave.Narrator.Framework
{
    using System.Reflection;
    using System.Text.RegularExpressions;

    [Serializable]
    public class StoryRunnerFilter : ISerializable
    {
        private readonly Regex _namespaceFilter;
        private readonly Regex _classNameFilter;
        private readonly Regex _methodNameFilter;

        public StoryRunnerFilter()
            : this(".", ".", ".")
        {
        }

        public StoryRunnerFilter(string namespaceFilter, string classNameFilter, string methodNameFilter)
        {
            _namespaceFilter = new Regex(AnchorValue(namespaceFilter));
            _classNameFilter = new Regex(AnchorValue(classNameFilter));
            _methodNameFilter = new Regex(AnchorValue(methodNameFilter));
        }

        public Regex NamespaceFilter
        {
            get { return this._namespaceFilter; }
        }

        public Regex ClassNameFilter
        {
            get { return this._classNameFilter; }
        }

        public Regex MethodNameFiler
        {
            get { return this._methodNameFilter; }
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
            info.AddValue("n", _namespaceFilter);
            info.AddValue("c", _classNameFilter);
            info.AddValue("m", _methodNameFilter);
        }

        protected StoryRunnerFilter(SerializationInfo info, StreamingContext context)
        {
            _namespaceFilter = (Regex) info.GetValue("n", typeof (Regex));
            _classNameFilter = (Regex) info.GetValue("c", typeof (Regex));
            _methodNameFilter = (Regex) info.GetValue("m", typeof (Regex));
        }
    }
}
