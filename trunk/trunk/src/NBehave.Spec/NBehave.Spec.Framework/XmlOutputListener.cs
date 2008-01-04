using System;
using System.Text;
using System.Xml;


namespace NBehave.Spec.Framework
{
    public class XmlOutputListener
    {
        private string _assemblyName;
        private string _currentContext = null;
        private string _currentSpecification = null;
        private int _failuresForCurrentSpec = 0;
        private string _fileName;
        private string _product;
        private SpecRunner _runner;
        private XmlWriter _writer;

        public XmlOutputListener(string assemblyName, string product, string fileName)
        {
            _assemblyName = assemblyName;
            _product = product;
            _fileName = fileName;
        }

        public void Add(SpecRunner runner)
        {
            _runner = runner;
            InitialiseXmlWriter();
            AddHandlers();
        }

        public void Remove()
        {
            RemoveHandlers();
            _runner = null;
            CloseXmlWriter();
        }

        private void AddHandlers()
        {
            if (null != _runner)
            {
                _runner.ContextChanged += new ExecutionChangedHandler(OnRunner_ContextChanged);
                _runner.SpecificationChanged += new ExecutionChangedHandler(OnRunner_SpecificationChanged);
                _runner.SpecificationFailed += new SpecificationFailedHandler(OnRrunner_SpecificationFailed);
            }
        }

        private void RemoveHandlers()
        {
            _runner.ContextChanged -= new ExecutionChangedHandler(OnRunner_ContextChanged);
            _runner.SpecificationChanged -= new ExecutionChangedHandler(OnRunner_SpecificationChanged);
        }

        private void OnRunner_ContextChanged(string details)
        {
            ContextChanged(details);
        }

        private void ContextChanged(string details)
        {
            CloseCurrentSpec();

            _currentSpecification = null;

            if (_currentContext != null)
            {
                _writer.WriteEndElement();
            }

            _writer.WriteStartElement(details);
            _currentContext = details;
        }

        private void OnRunner_SpecificationChanged(string details)
        {
            SpecificationChanged(details);
        }

        private void SpecificationChanged(string details)
        {
            CloseCurrentSpec();

            _writer.WriteStartElement(details);
            _currentSpecification = details;
        }

        private void OnRrunner_SpecificationFailed(Failure failure)
        {
            SpecificationFailed();
        }

        private void SpecificationFailed()
        {
            _failuresForCurrentSpec++;
        }

        private void CloseCurrentSpec()
        {
            if (_currentSpecification != null)
            {
                _writer.WriteElementString("Result", _failuresForCurrentSpec == 0 ? "Passed" : "Failed");
                _writer.WriteEndElement();
                _failuresForCurrentSpec = 0;
            }
        }

        private void InitialiseXmlWriter()
        {
            _writer = new XmlTextWriter(_fileName, Encoding.UTF8);
            _writer.WriteStartDocument();
            _writer.WriteStartElement("NSpec");
            _writer.WriteStartElement("Info");
            _writer.WriteElementString("NSpec", _product);
            _writer.WriteElementString("Runtime", Environment.Version.ToString());
            _writer.WriteElementString("OS", Environment.OSVersion.ToString());
            _writer.WriteStartElement("Assembly");
            _writer.WriteAttributeString("Name", _assemblyName);
        }

        public void CloseXmlWriter()
        {
            CloseCurrentSpec();
            _writer.WriteEndDocument();
            _writer.Close();
        }
    }
}