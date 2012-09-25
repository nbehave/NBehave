using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MEFedMVVM.Common;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Expression.Interactivity.Core;
using NBehave.VS2010.Plugin.Domain;
using NBehave.VS2010.Plugin.Editor.Domain;
using NBehave.VS2010.Plugin.Editor.Glyphs.Views;

namespace NBehave.VS2010.Plugin.Editor.Glyphs.ViewModels
{
    [ExportViewModel("RunOrDebugViewModel")]
    public class RunOrDebugViewModel : NotifyPropertyChangedBase, IDesignTimeAware
    {
        private List<dynamic> buttons;

        [Import(AllowDefault = false)]
        public IScenarioRunner ScenarioRunner { get; set; }

        public RunOrDebugViewModel()
        {
            buttons = new List<dynamic>
            {
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Debug.png", UriKind.Absolute)), Text="Start With Debugger", Command = DebugClicked },
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Play.png", UriKind.Absolute)), Text="Start Without Debugger", Command = RunClicked }
            };
        }

        // must be public or  plugin wont work
        public List<dynamic> Buttons
        {
            get
            {
                return buttons;
            }
        }

        private ICommand RunClicked
        {
            get
            {
                return GetCommand(false);
            }
        }

        private ICommand DebugClicked
        {
            get
            {
                return GetCommand(true);
            }
        }

        private ICommand GetCommand(bool debug)
        {
            return new ActionCommand(() => RunScenario(debug));
        }

        private void RunScenario(bool debug)
        {
            string tempFileName = GherkinText.ToString().ToTempFile();
            ScenarioRunner.Run(tempFileName, debug);
        }

        private bool initialized;
        public void InitialiseProperties(Point position, FrameworkElement visualElement, IRunOrDebugView runOrDebugView, GherkinText gherkinText)
        {
            View = runOrDebugView;
            RelativeVisualElement = visualElement;
            Position = Point.Subtract(position, new Vector(3, 3));
            GherkinText = gherkinText;
            
            // Dont want multiple events to fire.
            if (initialized)
                return;

            OnPropertyChanged(() => Buttons);
            PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "SelectedItem" && SelectedItem != null)
                    {
                        SelectedItem.Command.Execute(null);
                        View.Deselect();
                        IsVisible = false;
                    }
                };
            initialized = true;
        }

        public GherkinText GherkinText { get; set; }

        protected IRunOrDebugView View { get; set; }

        private FrameworkElement relativeVisualElement;

        public FrameworkElement RelativeVisualElement
        {
            get { return relativeVisualElement; }
            set
            {
                relativeVisualElement = value;
                OnPropertyChanged(() => RelativeVisualElement);
            }
        }

        private Point position;

        public Point Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged(() => Position);
            }
        }

        public void Show()
        {
            View.Deselect();
            IsVisible = true;
        }

        private bool isVisible;
        private dynamic selectedItem;

        // must be public or  plugin wont work
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged(() => IsVisible);
            }
        }

        public void DesignTimeInitialization()
        {
            buttons = new List<dynamic>
            {
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Debug.png", UriKind.Absolute)), Text="Start With Debugger" },
                new { Icon = new BitmapImage(new Uri("pack://application:,,,/NBehave.VS2010.Plugin;component/Editor/Resources/Icons/Play.png", UriKind.Absolute)), Text="Start Without Debugger" }
            };
        }

        // must be public or  plugin wont work
        public dynamic SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }
    }
}
