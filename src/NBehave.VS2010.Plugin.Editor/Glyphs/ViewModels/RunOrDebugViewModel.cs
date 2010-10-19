using System;
using System.ComponentModel.Composition;
using System.Concurrency;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MEFedMVVM.Common;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Expression.Interactivity.Core;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Editor.Glyphs.Views;

namespace NBehave.VS2010.Plugin.Editor.Glyphs.ViewModels
{
    [ExportViewModel("RunOrDebugViewModel")]
    public class RunOrDebugViewModel : NotifyPropertyChangedBase
    {

        [Import]
        public ScenarioRunner ScenarioRunner { get; set; }

        public ICommand RunClicked
        {
            get
            {
                return new ActionCommand(() =>
                {
                    string tempFileName = GetTempScenarioFile();

                    ScenarioRunner.Run(tempFileName, false);
                });
            }
        }

        public ICommand DebugClicked
        {
            get
            {
                return new ActionCommand(() =>
                {
                    string tempFileName = GetTempScenarioFile();

                    ScenarioRunner.Run(tempFileName, true);
                });
            }
        }

        private string GetTempScenarioFile()
        {
            var tempFileName = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempFileName))
            {
                writer.Write(ScenarioText);
            }
            return tempFileName;
        }

        public void InitialiseProperties(Point position, FrameworkElement visualElement, IRunOrDebugView runOrDebugView, string scenarioText)
        {
            Position = Point.Subtract(position, new Vector(3, 3));
            RelativeVisualElement = visualElement;
            this.View = runOrDebugView;
            this.ScenarioText = scenarioText;
        }

        protected string ScenarioText { get; set; }

        protected IRunOrDebugView View { get; set; }

        private FrameworkElement _relativeVisualElement;
        public FrameworkElement RelativeVisualElement
        {
            get { return _relativeVisualElement; }
            set
            {
                _relativeVisualElement = value;
                OnPropertyChanged(() => RelativeVisualElement);
            }
        }

        private Point _position;
        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged(() => Position);
            }
        }

        public void Show()
        {
            IsVisible = true;
            
            Observable
                .Timer(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250), Scheduler.Dispatcher)
                .Select(time => View.IsMouseOverPopup)
                .TakeWhile(isMouseOver => isMouseOver).Subscribe(isMouseOver => { }, () => IsVisible = false);
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged(() => IsVisible);
            }
        }
    }
}
