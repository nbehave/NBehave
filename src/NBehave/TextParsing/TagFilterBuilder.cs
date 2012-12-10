using System.Collections.Generic;
using System.Linq;
using NBehave.TextParsing.TagFilter;

namespace NBehave.TextParsing
{
    public static class TagFilterBuilder
    {
        public static TagFilter.TagFilter Build(IEnumerable<string[]> tagsFilter)
        {
            if (tagsFilter.Any())
            {
                IEnumerable<TagFilter.TagFilter> filters = tagsFilter.Select(_ => new OrFilter(_) as TagFilter.TagFilter);
                return new AndFilter(filters.ToArray());
            }
            return new NoFilter();
        }
    }
}