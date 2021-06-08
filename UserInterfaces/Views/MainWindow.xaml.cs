using System.Windows;
using System.Windows.Input;
using UserInterfaces.ViewModels;

namespace UserInterfaces.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            OnDataContextChanged(null,new DependencyPropertyChangedEventArgs());
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CommandManager.RemovePreviewExecutedHandler(MainRtb, ((MainWindowViewModel) DataContext).CommandExecuted);
            CommandManager.AddPreviewExecutedHandler(MainRtb, ((MainWindowViewModel) DataContext).CommandExecuted);
        }
    }
}