using System;
using System.IO;

namespace NBehave.Narrator.Framework.EventListeners
{
    public class FileOutputEventListener : IEventListener, IDisposable
    {
        private readonly string _path;
        private bool _disposed;
        private bool _insideNamedTheme;
        private StreamWriter _writer;

        public FileOutputEventListener(string path)
        {
            _path = path;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion

        void IEventListener.FeatureCreated(string feature)
        {
            _writer.WriteLine();
            if (_insideNamedTheme)
                _writer.Write('\t');
            _writer.WriteLine("Feature: " + feature);
        }

        void IEventListener.FeatureNarrative(string message)
        {
            if (_insideNamedTheme)
                _writer.Write('\t');

            _writer.Write('\t');
            _writer.WriteLine(message);
        }

        void IEventListener.ScenarioCreated(string scenarioTitle)
        {
            _writer.WriteLine();
            if (_insideNamedTheme)
                _writer.Write('\t');
            _writer.Write('\t');
            _writer.WriteLine("Scenario: " + scenarioTitle);
        }

        void IEventListener.ScenarioMessageAdded(string message)
        {
            if (_insideNamedTheme)
                _writer.Write('\t');
            _writer.Write("\t\t");

            _writer.WriteLine(message);
        }

        void IEventListener.RunStarted()
        {
            _writer = File.CreateText(_path);
        }

        void IEventListener.RunFinished()
        {
            Dispose(true);
        }

        void IEventListener.ThemeStarted(string name)
        {
            if (! string.IsNullOrEmpty(name))
            {
                _writer.WriteLine("Theme: {0}", name);
                _insideNamedTheme = true;
            }
        }

        void IEventListener.ThemeFinished()
        {
            _insideNamedTheme = false;
            _writer.WriteLine();
        }

        public void ScenarioResult(ScenarioResult result)
        { }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_writer != null)
                    _writer.Close();
            }

            _disposed = true;
        }
    }
}
