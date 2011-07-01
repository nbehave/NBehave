using System.Collections.Generic;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace NBehave.ReSharper.Plugin.UnitTestRunner
{
    public static class TaskExecutionNodeExtensions
    {

        public static IEnumerable<RemoteTask> AllTasks(this IEnumerable<TaskExecutionNode> nodes)
        {
            var tasks = new List<RemoteTask>();
            foreach (var node in nodes)
            {
                tasks.Add(node.RemoteTask);
                tasks.AddRange(AllTasks(node.Children));
            }
            return tasks;
        }
    }
}