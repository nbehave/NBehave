using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework.Hooks;
using NBehave.Narrator.Framework.Internal;

namespace NBehave.Narrator.Framework
{
    public class HooksCatalog
    {
        private readonly Dictionary<Type, List<HookMetaData>> hooks = new Dictionary<Type, List<HookMetaData>>();

        public void Add(HookMetaData item)
        {
            List<HookMetaData> lst;
            if (hooks.TryGetValue(item.HookAttrib.GetType(), out lst) == false)
            {
                lst = new List<HookMetaData>();
                hooks.Add(item.HookAttrib.GetType(), lst);
            }
            lst.Add(item);
        }

        public void AddRange(IEnumerable<HookMetaData> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public IEnumerable<HookMetaData> OfType<T>()
        {
            return OfType(typeof(T));
        }

        public IEnumerable<HookMetaData> OfType(Type type)
        {
            List<HookMetaData> items;
            if (hooks.TryGetValue(type, out items))
                return items;
            return new List<HookMetaData>();
        }
    }

    public class HooksParser
    {
        private readonly HooksCatalog hooksCatalog;
        private readonly MethodWithAttributeFinder methodWithAttributeFinder;
        private readonly DelegatesWithAttributeFinder delegatesWithAttributeFinder;

        public HooksParser(HooksCatalog hooksCatalog)
        {
            this.hooksCatalog = hooksCatalog;
            methodWithAttributeFinder = new MethodWithAttributeFinder(new StoryRunnerFilter());
            delegatesWithAttributeFinder = new DelegatesWithAttributeFinder(new StoryRunnerFilter());
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
            FindMethods(type);
            FindDelegates(type);
        }

        private void FindMethods(Type type)
        {
            var methods = methodWithAttributeFinder.FindMethodsWithAttribute<HookMetaData, HookAttribute>(type, BuildMethodHookType);
            hooksCatalog.AddRange(methods);
        }

        private void FindDelegates(Type type)
        {
            var delegates = delegatesWithAttributeFinder.FindDelegatesWithAttribute<HookMetaData, HookAttribute>(type, BuildDelegateHookType);
            hooksCatalog.AddRange(delegates);

        }

        private HookMetaData BuildDelegateHookType(HookAttribute hookAttrib, FieldInfo @delegate)
        {
            if (@delegate.FieldType != typeof(Action))
            {
                var message = string.Format("Field {0} with attribute {1} must be of type SystemAction.", @delegate.Name, hookAttrib.GetType().Name);
                throw new ArgumentException(message, "delegate");
            }
            var obj = GetInstance(@delegate);
            var action = (Action)@delegate.GetValue(obj);
            return new DelegateHookMetaData(action, hookAttrib);
        }

        private readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        private object GetInstance(FieldInfo @delegate)
        {
            object obj;
            Type type = @delegate.DeclaringType;
            if (instances.TryGetValue(type, out obj) == false)
            {
                obj = Activator.CreateInstance(type);
                instances.Add(type, obj);
            }
            return obj;
        }

        private HookMetaData BuildMethodHookType(HookAttribute hookAttrib, MethodInfo method)
        {
            if (method.GetParameters().Any())
            {
                var message = string.Format("Method {0} on class {1} may not have any parameters.", method.Name, method.DeclaringType.Name);
                throw new ArgumentException(message, "method");
            }
            return new MethodHookMetaData(method, hookAttrib);
        }
    }

    public class DelegateHookMetaData : HookMetaData
    {
        private readonly Action action;

        public DelegateHookMetaData(Action action, HookAttribute hookAttrib)
            : base(hookAttrib)
        {
            this.action = action;
        }

        public override void Invoke()
        {
            action();
        }
    }

    public class MethodHookMetaData : HookMetaData
    {
        private readonly object instance;
        private readonly MethodInfo method;

        public MethodHookMetaData(MethodInfo method, HookAttribute hookAttrib)
            : base(hookAttrib)
        {
            this.method = method;
            instance = Activator.CreateInstance(method.DeclaringType);

        }

        public override void Invoke()
        {
            method.Invoke(instance, null);
        }
    }

    public abstract class HookMetaData
    {
        public HookAttribute HookAttrib { get; private set; }

        protected HookMetaData(HookAttribute hookAttrib)
        {
            HookAttrib = hookAttrib;
        }

        public abstract void Invoke();
    }
}