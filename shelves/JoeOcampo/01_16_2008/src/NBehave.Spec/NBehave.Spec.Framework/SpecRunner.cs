using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NBehave.Spec.Framework
{
    public delegate void SpecficationPassedHandler();

    public delegate void SpecificationFailedHandler(Failure failure);

    public delegate void SpecAssemblyFoundHandler(Assembly assembly);

    public delegate void SpecClassFoundHandler(Type type);

    public delegate void SpecMethodFoundHandler(MethodInfo methodInfo);

    public delegate void ExecutionChangedHandler(string info);

    public delegate void InvalidAssemblyHandler(string assembly);

    public class SpecRunner
    {
        private string _currentlyExecutingContext;
        private string _currentlyExecutingSpecification;
        private ArrayList _specClasses = new ArrayList();
        private List<Specify> _specs = new List<Specify>();

        private bool listenForNewSpecs = false;

        public SpecRunner()
            : this(true)
        {
        }

        public SpecRunner(bool listenForNewSpecs)
        {
            this.listenForNewSpecs = listenForNewSpecs;
            StartListening();
        }

        public event SpecficationPassedHandler SpecificationPassed;
        public event SpecificationFailedHandler SpecificationFailed;
        public event SpecAssemblyFoundHandler SpecAssemblyFound;
        public event SpecClassFoundHandler SpecClassFound;
        public event SpecMethodFoundHandler SpecMethodFound;
        public event ExecutionChangedHandler ContextChanged;
        public event ExecutionChangedHandler SpecificationChanged;
        public event InvalidAssemblyHandler InvalidAssembly;

        public void StartListening()
        {
            if (listenForNewSpecs)
            {
                Specify.NewSpecification += new NewSpecificationHandler(AddNewSpecification);
            }
        }

        public void StopListening()
        {
            if (listenForNewSpecs)
            {
                Specify.NewSpecification -= new NewSpecificationHandler(AddNewSpecification);
            }
        }

        public void LoadAssembly(string assemblyName)
        {
            if (File.Exists(assemblyName))
            {
                Assembly asm = Assembly.LoadFrom(assemblyName);

                if (SpecAssemblyFound != null)
                    SpecAssemblyFound(asm);

                foreach (Type t in asm.GetExportedTypes())
                {
                    if (t.GetCustomAttributes(typeof (ContextAttribute), false).Length > 0)
                    {
                        _specClasses.Add(Activator.CreateInstance(t));

                        if (SpecClassFound != null)
                            SpecClassFound(t);
                    }
                }
            }
            else
            {
                if (InvalidAssembly != null)
                {
                    InvalidAssembly(assemblyName);
                }
            }
        }

        public void Run()
        {
            foreach (object o in _specClasses)
            {
                _currentlyExecutingContext = o.GetType().Name;

                if (ContextChanged != null)
                    ContextChanged(_currentlyExecutingContext);

                Type specAttributeType = typeof (SpecificationAttribute);
                Type setUpAttributeType = typeof (SetUpAttribute);
                Type tearDownAttributeType = typeof (TearDownAttribute);
                Type contextSetUpAttributeType = typeof (ContextSetUpAttribute);
                Type contextTearDownAttributeType = typeof (ContextTearDownAttribute);

                List<MethodInfo> setUpMethods = GetMethods(o, setUpAttributeType);
                List<MethodInfo> tearDownMethods = GetMethods(o, tearDownAttributeType);
                List<MethodInfo> specMethods = GetMethods(o, specAttributeType);
                List<MethodInfo> contextSetUpMethods = GetMethods(o, contextSetUpAttributeType);
                List<MethodInfo> contextTearDownMethods = GetMethods(o, contextTearDownAttributeType);

                foreach (MethodInfo contextSetUpMethod in contextSetUpMethods)
                {
                    RunMethod(o, contextSetUpMethod);
                }

                foreach (MethodInfo specMethod in specMethods)
                {
                    _currentlyExecutingSpecification = specMethod.Name;

                    if (SpecificationChanged != null)
                        SpecificationChanged(_currentlyExecutingSpecification);

                    if (SpecMethodFound != null)
                        SpecMethodFound(specMethod);

                    foreach (MethodInfo setUpMethod in setUpMethods)
                    {
                        RunMethod(o, setUpMethod);
                    }

                    RunMethod(o, specMethod);

                    foreach (MethodInfo tearDownMethod in tearDownMethods)
                    {
                        RunMethod(o, tearDownMethod);
                    }
                }

                foreach (MethodInfo contextTearDownMethod in contextTearDownMethods)
                {
                    RunMethod(o, contextTearDownMethod);
                }
            }
        }

        private List<MethodInfo> GetMethods(object o, Type attributeType)
        {
            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (MethodInfo method in o.GetType().GetMethods())
            {
                if (method.GetCustomAttributes(attributeType, false).Length > 0)
                {
                    methods.Add(method);
                }
            }

            return methods;
        }

        private void RunMethod(object o, MethodInfo method)
        {
            try
            {
                method.Invoke(o, null);
                RunComparisons();
            }
            catch (TargetInvocationException e)
            {
                Failure f = new Failure();
                f.ContextName = _currentlyExecutingContext;
                f.SpecificationName = _currentlyExecutingSpecification;
                f.Exception = e.InnerException;

                NotifyFailure(f);
                RunComparisons();
            }
        }

        private void RunMethods(object o, Type attributeType)
        {
            foreach (MethodInfo method in o.GetType().GetMethods())
            {
                if (method.GetCustomAttributes(attributeType, false).Length > 0)
                {
                }
            }
        }

        private void AddNewSpecification(Specify spec)
        {
            _specs.Add(spec);
        }

        internal void AddLastSpecification()
        {
            AddNewSpecification(Specify.LastSpecification);
        }

        public void RunComparisons()
        {
            foreach (Specify spec in _specs)
            {
                if (spec.ComparesOK())
                    NotifySuccess();
                else
                {
                    Failure f = spec.LastFailure;
                    f.ContextName = _currentlyExecutingContext;
                    f.SpecificationName = _currentlyExecutingSpecification;
                    NotifyFailure(f);
                }
            }

            _specs.Clear();
        }

        private void NotifyFailure(Failure failure)
        {
            if (SpecificationFailed != null)
                SpecificationFailed(failure);
        }

        private void NotifySuccess()
        {
            if (SpecificationPassed != null)
                SpecificationPassed();
        }
    }
}