using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NBehave.TestDriven.Plugin
{
    public static class AssemblyHelper
    {
        public static IEnumerable<string> DeduceRootNamespaceParts(Assembly assembly)
        {
            var splitNamespaces = assembly.GetTypes()
                .Where(type => type.IsPublic)
                .Select(type => type.Namespace)
                .Distinct()
                .Where(_ => _ != "JetBrains.Annotations")
                .Select(ns => ns.Split('.'))
                .ToArray();

            var largestCommonality = 0;

            do
            {
                if (largestCommonality >= splitNamespaces[0].Length)
                    break;

                var valueToTest = splitNamespaces[0][largestCommonality];
                var commonality = largestCommonality;
                var areAllSame = splitNamespaces.All(strings => strings.Length > commonality && strings[commonality] == valueToTest);
                if (!areAllSame)
                    break;
                largestCommonality++;
            } while (true);

            return splitNamespaces[0].Take(largestCommonality);
        }
    }
}