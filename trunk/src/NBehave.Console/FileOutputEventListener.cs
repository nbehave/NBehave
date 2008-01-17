using System;
using System.IO;
using NBehave.Narrator.Framework;

namespace NBehave.Console
{
    [Obsolete("Use NBehave.Narrator.Framework.EventListeners.FileOuputEventListener instead")]
    public class FileOutputEventListener : IEventListener, IDisposable
    {
        private readonly string _path;
        private bool _disposed = false;
        private bool _insideNamedTheme = false;
        private StreamWriter _writer;

        public FileOutputEventListener(string path)
        {
            _path = path;
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            RunFinished();
        }

        #endregion

        #region IEventListener Members

        public void StoryCreated()
        {
            _writer.WriteLine();
        }

        public void StoryMessageAdded(string message)
        {
            if (_insideNamedTheme)
                _writer.Write('\t');

            _writer.WriteLine(message);
        }

        public void RunStarted()
        {
            _writer = File.CreateText(_path);
        }

        public void RunFinished()
        {
            Dispose(true);
        }

        public void ThemeStarted(string name)
        {
            if (! string.IsNullOrEmpty(name))
            {
                _writer.WriteLine("Theme: {0}", name);
                _insideNamedTheme = true;
            }
        }

        public void ThemeFinished()
        {
            _insideNamedTheme = false;
            _writer.WriteLine();
        }

        #endregion

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
