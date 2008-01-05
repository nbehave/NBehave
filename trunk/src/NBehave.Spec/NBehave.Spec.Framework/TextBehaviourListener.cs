using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NBehave.Spec.Framework
{
    public class TextBehaviourListener
    {
        private string assemblyName;
        private string currentContext = null;
        private string currentSpecification = null;
        private Regex expandPattern = new Regex("(([^ ])([A-Z])|([^0-9 ])([0-9]+))");
        private string fileName;
        private string product;
        private SpecRunner runner;
        private StreamWriter writer;

        public TextBehaviourListener(string assemblyName, string product, string fileName)
        {
            this.assemblyName = assemblyName;
            this.product = product;
            this.fileName = fileName;
        }

        public void Add(SpecRunner runner)
        {
            this.runner = runner;
            InitialiseTextWriter();
            AddHandlers();
        }

        public void Remove()
        {
            RemoveHandlers();
            runner = null;
            CloseTextWriter();
        }

        private void AddHandlers()
        {
            if (runner != null)
            {
                runner.ContextChanged += new ExecutionChangedHandler(OnRunner_ContextChanged);
                runner.SpecificationChanged += new ExecutionChangedHandler(OnRunner_SpecificationChanged);
            }
        }

        private void RemoveHandlers()
        {
            runner.ContextChanged -= new ExecutionChangedHandler(OnRunner_ContextChanged);
            runner.SpecificationChanged -= new ExecutionChangedHandler(OnRunner_SpecificationChanged);
        }

        private void OnRunner_ContextChanged(string details)
        {
            ContextChanged(details);
        }

        private void ContextChanged(string details)
        {
            currentSpecification = null;

            if (currentContext != null)
            {
                writer.WriteLine("");
            }

            writer.WriteLine(ExpandSpec(details));
            currentContext = details;
        }

        private string ExpandSpec(string details)
        {
            return expandPattern.Replace(details, "$2$4 $3$5");
        }

        private void OnRunner_SpecificationChanged(string details)
        {
            SpecificationChanged(details);
        }

        private void SpecificationChanged(string details)
        {
            writer.WriteLine(" * " + ExpandSpec(details));
            currentSpecification = details;
        }

        private void InitialiseTextWriter()
        {
            writer = new StreamWriter(fileName);
            writer.WriteLine("NSpec:   " + product);
            writer.WriteLine("Runtime: " + Environment.Version.ToString());
            writer.WriteLine("OS:      " + Environment.OSVersion.ToString());
            writer.WriteLine("Assembly:" + assemblyName);
            writer.WriteLine("");
        }

        public void CloseTextWriter()
        {
            writer.Flush();
            writer.Close();
        }
    }
}