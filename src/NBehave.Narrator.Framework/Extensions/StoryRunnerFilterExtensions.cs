using System;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.Narrator.Framework.Extensions
{
    public static class In
    {
        public static StoryRunnerFilter Context<TActionStepsClass>()
        {
            return new StoryRunnerFilter().And<TActionStepsClass>();
        }

        public static StoryRunnerFilter GlobalContext()
        {
            return new StoryRunnerFilter();
        }
    }

    public static class StoryRunnerFilterExtensions
    {
        public static StoryRunnerFilter Context<TActionStepsClass>(this StoryRunnerFilter filter)
        {
            Type type = typeof(TActionStepsClass);
            string classFilterValue = filter.ClassNameFilter.UpdateWith(type.Name);
            var namespaceFilter = filter.NamespaceFilter.UpdateWith(type.Namespace);
            return new StoryRunnerFilter(namespaceFilter, classFilterValue, filter.MethodNameFiler.ToString());
        }

        private static string UpdateWith(this Regex initialFilter, string valueToApply)
        {
            string filterValue = initialFilter.ToString().TrimStart('^').TrimEnd('$');
            if (filterValue == ".")
            {
                return valueToApply;
            }
            return filterValue + string.Format("|{0}", valueToApply);
        }

        public static StoryRunnerFilter And<TActionStepsClass>(this StoryRunnerFilter filter)
        {
            return filter.Context<TActionStepsClass>();
        }

    }
}