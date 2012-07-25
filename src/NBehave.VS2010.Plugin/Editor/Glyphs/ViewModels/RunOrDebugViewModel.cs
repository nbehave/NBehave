using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Concurrency;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MEFedMVVM.Common;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Expression.Interactivity.Core;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Editor.Glyphs.Views;

namespace NBehave.VS2010.Plugin.Editor.Glyphs.ViewModels
{
    [ExportViewModel("RunOrDebugViewModel")]
    public class RunOrDebugViewModel : NotifyPropertyChangedBase, IDesignTimeAware
    {
        private List<dynamic> _buttons;

        [Import(AllowDefault = false)]
        public IScenarioRunner ScenarioRunner { get; set; }

        public List<dynamic> Buttons
        {
            get
            {
                return _buttons;
            }
        }

        public ICommand RunClicked
        {
            get
            {
                return GetCommand(false);
            }
        }

        public ICommand DebugClicked
        {
            get 
            {
                return GetCommand(true);
            }
        }

        private ICommand GetCommand(bool debug)
        {
            return new ActionCommand(() =>
                                         {
                                             string tempFileName = ScenarioText.ToTempFile();
                                             ScenarioRunner.Run(tempFileName, debug);
                                         });
        }

        public void InitialiseProperties(Point position, FrameworkElement visualElement, IRunOrDebugView runOrDebugView, string scenarioText)
        {
            Position = Point.Subtract(position, new Vector(3, 3));
            RelativeVisualElement = visualElement;
            View = runOrDebugView;
            ScenarioText = scenarioText;

            _buttons = new List<dynamic>
            {
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Debug.png", UriKind.Absolute)), Text="Start With Debugger", Command = DebugClicked },
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Play.png", UriKind.Absolute)), Text="Start Without Debugger", Command = RunClicked }
            };
            OnPropertyChanged(() => Buttons);

            Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => ((sender, args) => handler(sender, args)),
                    eventHandler => PropertyChanged += eventHandler,
                    changedEventHandler => PropertyChanged += changedEventHandler)
                .Where(@event => @event.EventArgs.PropertyName == "SelectedItem" && SelectedItem != null)
                .Throttle(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(event1 =>
                               {
                                   SelectedItem.Command.Execute(null);
                                   View.Deselect();
                               });
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
            View.Deselect();
            IsVisible = true;
            
            Observable
                .Timer(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250), Scheduler.Dispatcher)
                .Select(time => View.IsMouseOverPopup)
                .TakeWhile(isMouseOver => isMouseOver).Subscribe(isMouseOver => { }, () => IsVisible = false);
        }

        private bool _isVisible;
        private dynamic _selectedItem;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged(() => IsVisible);
            }
        }

        public void DesignTimeInitialization()
        {
            _buttons = new List<dynamic>
            {
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Debug.png", UriKind.Absolute)), Text="Start With Debugger" },
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Play.png", UriKind.Absolute)), Text="Start Without Debugger" }
            };
        }

        public dynamic SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }
    }
}
