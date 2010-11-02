using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;

namespace NBehave.VS2010.Plugin.Editor.Glyphs.Controls
{
    public class ContextMenuButton : Button
    {
        public ImageSource Source
        {
            get { return base.GetValue(SourceProperty) as ImageSource; }
            set { base.SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
          DependencyProperty.Register("Source", typeof(ImageSource), typeof(ContextMenuButton)); 
    }
}
