using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HTTPMan.Views
{
    public class HTTPsDebuggerView : UserControl
    {
        public HTTPsDebuggerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
        }
    }
}