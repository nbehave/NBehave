using System;
using System.Windows;
using System.Windows.Input;
using MEFedMVVM.Common;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Expression.Interactivity.Core;

namespace NBehave.VS2010.Plugin.Editor.Glyphs.ViewModels
{
    [ExportViewModel("RunOrDebugViewModel")]
    public class RunOrDebugViewModel : NotifyPropertyChangedBase
    {

        public ICommand RunClicked
        {
            get
            {
//                Observable
//                .Timer(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250), Scheduler.Dispatcher)
//                .Select(time => _popup.IsMouseOver)
//                .TakeWhile(isMouseOver => isMouseOver).Subscribe(isMouseOver => { }, () => _popup.IsOpen = false);

                //            tempFileName = Path.GetTempFileName();
                //            using (var writer = new StreamWriter(tempFileName))
                //            {
                //                writer.Write(_snapshotSpan.GetText());
                //            }
                //
                //            _scenarioRunner.Run(tempFileName, false);

                return new ActionCommand(() =>
                                             {

                                             });
            }
        }

        public void InitialiseProperties(Point position, FrameworkElement visualElement)
        {
            this.Position = Point.Subtract(position, new Vector(3, 3));
            this.RelativeVisualElement = visualElement;
            OnPropertyChanged(() => Position);
            OnPropertyChanged(() => RelativeVisualElement);
        }

        protected FrameworkElement RelativeVisualElement { get; set; }

        protected Point Position { get; set; }

        public void Show()
        {
            IsVisible = true;
            OnPropertyChanged(() => IsVisible);
        }

        protected bool IsVisible { get; set; }
    }
}
