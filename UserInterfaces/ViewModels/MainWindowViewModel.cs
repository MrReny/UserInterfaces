using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Prism.Commands;
using ReactiveUI;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace UserInterfaces.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {
        public readonly char[] Separators = new[] {' ', '.', ',', '!', '?', ':', ';', '\n'};
        public string MainDocumentXaml
        {
            get => _mainDocumentXaml;
            set => this.RaiseAndSetIfChanged(ref _mainDocumentXaml, value);
        }

        private string _elapsed;

        public string Elapsed
        {
            get => _elapsed;
            set => this.RaiseAndSetIfChanged(ref _elapsed, value);
        }

        private string _estimated;

        public string Estimated
        {
            get => _estimated;
            set => this.RaiseAndSetIfChanged(ref _estimated, value);
        }

        public Stopwatch Stopwatch { get; set; }

        public TimeSpan EstimatedTimeSpan { get; set; }

        public DelegateCommand OpenFile { get; set; }
        public DelegateCommand SaveFile { get; set; }
        public DelegateCommand SaveAsFile { get; set; }

        public DelegateCommand KeyDown { get; set; }

        private readonly DispatcherTimer _dispatcherTimer;
        private string _mainDocumentXaml;

        public string FilePath { get; set; }

        public MainWindowViewModel()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherTimerTick;
            _dispatcherTimer.Interval = new TimeSpan(0,0,0,0,50);
            _dispatcherTimer.Start();
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            OpenFile = new DelegateCommand(OpenFileDialog);
            SaveFile = new DelegateCommand(SaveCurrentFile);
            SaveAsFile = new DelegateCommand(SaveFileDialog);
            KeyDown = new DelegateCommand(OnKeyDown);

            EstimatedTimeSpan = TimeSpan.Zero;

        }

        private void OpenFileDialog()
        {
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                FilePath = fd.FileName;

                var fileStream = fd.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    MainDocumentXaml = reader.ReadToEnd();
                }
            }

        }

        private void SaveCurrentFile()
        {
            if(string.IsNullOrEmpty(FilePath)) SaveFileDialog();
            try
            {
                var fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                var wr = new StreamWriter(fs);
                wr.Write(MainDocumentXaml);
                wr.Flush();
                wr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        private void SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = FilePath;
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                SaveCurrentFile();
            }
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            Elapsed = Stopwatch.Elapsed.ToString();
        }

        public void OnKeyDown()
        {
            var trim = MainDocumentXaml.TrimEnd();
            if (trim.Length != 0)
            {
                if (Separators.Contains(trim.Last()))
                    EstimatedTimeSpan = EstimatedTimeSpan.Add(new TimeSpan(0, 0, 0, 1, 350));
            }

            EstimatedTimeSpan = EstimatedTimeSpan.Add(new TimeSpan(0, 0, 0, 0, 200));
            Estimated = EstimatedTimeSpan.ToString();
        }

        public void CommandExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            EstimatedTimeSpan = EstimatedTimeSpan.Add(new TimeSpan(0, 0, 0,1,750));
            Estimated = EstimatedTimeSpan.ToString();
        }

    }
}