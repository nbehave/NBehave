using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework.Hooks;

namespace NBehave.Narrator.Framework
{
    public class HooksCatalog
    {
        private readonly Dictionary<Type, List<HookMetaData>> hooks = new Dictionary<Type, List<HookMetaData>>();

        public void AddRange(IEnumerable<HookMetaData> items)
        {
            foreach (var item in items)
            {
                List<HookMetaData> lst;
                if (hooks.TryGetValue(item.HookAttrib.GetType(), out lst) == false)
                {
                    lst = new List<HookMetaData>();
                    hooks.Add(item.HookAttrib.GetType(), lst);
                }
                lst.Add(item);
            }
        }

        public IEnumerable<HookMetaData> OfType<T>()
        {
            List<HookMetaData> items;
            if (hooks.TryGetValue(typeof(T), out items))
                return items;
            return new List<HookMetaData>();
        }
    }

    public class HooksParser
    {
        private readonly HooksCatalog hooksCatalog;
        private readonly MethodWithAttributeFinder methodWithAttributeFinder;

        public HooksParser(HooksCatalog hooksCatalog)
        {
            this.hooksCatalog = hooksCatalog;
            methodWithAttributeFinder = new MethodWithAttributeFinder(new StoryRunnerFilter());
        }

        public void FindHooks(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (type.IsAbstract == false) //filter with StoryRunnerFilter?
                    FindHooks(type);
            }
        }

        public void FindHooks(Type type)
        {
            if (type.GetCustomAttributes(typeof(HooksAttribute), true).Length == 0)
                return;
            var methods = methodWithAttributeFinder.FindMethodsWithAttribute<HookMetaData, HookAttribute>(type, BuildHookType);
            hooksCatalog.AddRange(methods);
        }

        private HookMetaData BuildHookType(HookAttribute hookAttrib, MethodInfo method)
        {
            if (method.GetParameters().Any())
            {
                var message = string.Format("Method {0} on class {1} may not have any parameters.", method.Name, method.DeclaringType.Name);
                throw new ArgumentException(message, "method");
            }
            return new HookMetaData(method, hookAttrib);
        }
    }

    public class HookMetaData
    {
        private readonly object instance;
        private readonly MethodInfo method;
        public HookAttribute HookAttrib { get; private set; }

        public HookMetaData(MethodInfo method, HookAttribute hookAttrib)
        {
            this.method = method;
            instance = Activator.CreateInstance(method.DeclaringType);
            HookAttrib = hookAttrib;
        }

        public void Invoke()
        {
            method.Invoke(instance, null);
        }
    }
}