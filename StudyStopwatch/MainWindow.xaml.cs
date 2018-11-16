using System;
using System.Windows;
using System.Windows.Threading;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Controls;

namespace StudyStopwatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private bool isActive;
        private List<Stopwatch> timerLists;
        private Stopwatch currentTimer;
        private string lastDirectory;
        public MainWindow()
        {
            InitializeComponent();
            timerLists = new List<Stopwatch>();
            isActive = false;
            LoadAppConfigLastPath();
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            currentTimer = new Stopwatch();
            RefreshLabels();

            timer.Tick += Timer_Tick;
            button_activate.Click += StartTimer;
            button_save.Click += SaveStatistics;
            button_new.Click += NewTimer;
            button_add.Click += AddToList;
            button_load.Click += LoadJsonFromDialog;
            cbox_timers.SelectionChanged += ItemSelected;
            Closing += OnWindowExit;
        }

        private void LoadAppConfigLastPath()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                lastDirectory = appSettings["path"] ?? "C:\\Users";
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Oops cant load");
            }
        }
        private void OnWindowExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings["path"] == null)
                {
                    settings.Add("path", lastDirectory);
                }
                else
                {
                    settings["path"].Value = lastDirectory;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Oops cant save");
            }
        }

        private void ItemSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox cbox = (ComboBox)sender;
            currentTimer = (Stopwatch)cbox.SelectedItem;
            RefreshLabels();
        }

        private void LoadJsonFromDialog(object sender, RoutedEventArgs e)
        {
            if (isActive)
            {
                StartTimer(sender, e);
            }
            LoadStatistics();
        }

        private void AddToList(object sender, RoutedEventArgs e)
        {
            if (!timerLists.Contains(currentTimer))
            {
                timerLists.Add(currentTimer);
                PopulateCombobox();
                if (isActive)
                {
                    StartTimer(sender, e);
                }
            }
        }

        private void NewTimer(object sender, RoutedEventArgs e)
        {
            timerLists.Add(currentTimer);
            currentTimer = new Stopwatch();
            if (isActive)
            {
                StartTimer(sender, e);
            }
            RefreshLabels();
        }

        private void LoadStatistics()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Json files", "*.json"));
            if (lastDirectory == null)
            {
                dialog.InitialDirectory = "C:\\Users";
            }
            else
            {
                dialog.InitialDirectory = lastDirectory;
            }
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                lastDirectory = dialog.FileName;
                using (StreamReader file = File.OpenText(dialog.FileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    List<Stopwatch> timers = (List<Stopwatch>)serializer.Deserialize((file), typeof(List<Stopwatch>));
                    timerLists = timers;
                    PopulateCombobox();
                }
            }
        }

        private void SaveStatistics(object sender, RoutedEventArgs e)
        {
            if (isActive)
            {
                StartTimer(sender, e);
            }
            CommonSaveFileDialog dialog = new CommonSaveFileDialog
            {
                DefaultExtension = ".json",
                AlwaysAppendDefaultExtension = true
            };
            dialog.Filters.Add(new CommonFileDialogFilter("Json files", "*.json"));

            if (lastDirectory == null)
            {
                dialog.InitialDirectory = "C:\\Users";
            }
            else
            {
                dialog.InitialDirectory = lastDirectory;
            }
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                lastDirectory = dialog.FileName;
                using (StreamWriter file = File.CreateText(Path.Combine(dialog.FileName, dialog.FileName)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, timerLists);
                }
            }
        }
       
        public void StartTimer(object sender, RoutedEventArgs e)
        {
            if (!isActive)
            {
                timer.Start();
                isActive = true;
                button_activate.Content = "Stop";
            }
            else
            {
                timer.Stop();
                isActive = false;
                button_activate.Content = "Start";
            }
        }

        private void Shortcut_StartTimer(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            StartTimer(sender, e);
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            SaveStatistics(sender, e);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentTimer.Increment();
            RefreshLabels();
        }
        private void RefreshLabels()
        {
            d_seconds.Content = currentTimer.Seconds;
            d_minutes.Content = currentTimer.Minutes;
            d_hours.Content = currentTimer.Hours;
        }
        private void PopulateCombobox()
        {
            cbox_timers.Items.Clear();
            foreach(Stopwatch timers in timerLists)
            {
                cbox_timers.Items.Add(timers);
            }
        }
    }
}
