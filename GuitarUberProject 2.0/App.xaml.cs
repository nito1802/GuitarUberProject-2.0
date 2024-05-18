using GitarUberProject.Games_and_Fun;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GitarUberProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static double CustomScaleX { get; set; }
        public static double CustomScaleY { get; set; }

        public static string FolderSettingsPath { get; set; }
        public static string ChordsImagesDefaultPath { get; set; }
        public static string ChordImagesWorkingPath { get; set; }
        public static string ChordImagesProfiles { get; set; }

        public static string ReadChordsImagesDefaultPath { get; set; }
        public static string ReadChordImagesWorkingPath { get; set; }
        public static string ReadChordImagesProfiles { get; set; }

        public static string ProfileName { get; set; }
        public static string PathMidi { get; set; } = "MIDI";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Stopwatch swApp = new Stopwatch();
            swApp.Start();

            string myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string applicationFolder = "Gitar Uber Project Data";
            FolderSettingsPath = Path.Combine(myDocumentPath, applicationFolder);

            string chordImages = "ChordImagesDefault";
            ChordsImagesDefaultPath = Path.Combine(FolderSettingsPath, chordImages);

            string chordImagesProfiles = "ChordImagesProfiles";
            ChordImagesProfiles = Path.Combine(FolderSettingsPath, chordImagesProfiles);

            string chordImagesWorkingPath = "ChordImagesWorkingPath";
            ChordImagesWorkingPath = Path.Combine(FolderSettingsPath, chordImagesWorkingPath);


            string readChordImages = "ReadChordImagesDefault";
            ReadChordsImagesDefaultPath = Path.Combine(FolderSettingsPath, readChordImages);

            string readChordImagesProfiles = "ReadChordImagesProfiles";
            ReadChordImagesProfiles = Path.Combine(FolderSettingsPath, readChordImagesProfiles);

            string readChordImagesWorkingPath = "ReadChordImagesWorkingPath";
            ReadChordImagesWorkingPath = Path.Combine(FolderSettingsPath, readChordImagesWorkingPath);


            if (!Directory.Exists(FolderSettingsPath)) Directory.CreateDirectory(FolderSettingsPath);
            if(!Directory.Exists(ChordsImagesDefaultPath)) Directory.CreateDirectory(ChordsImagesDefaultPath);
            if (!Directory.Exists(ChordImagesProfiles)) Directory.CreateDirectory(ChordImagesProfiles);
            if (!Directory.Exists(ChordImagesWorkingPath)) Directory.CreateDirectory(ChordImagesWorkingPath);

            if (!Directory.Exists(ReadChordsImagesDefaultPath)) Directory.CreateDirectory(ReadChordsImagesDefaultPath);
            if (!Directory.Exists(ReadChordImagesProfiles)) Directory.CreateDirectory(ReadChordImagesProfiles);
            if (!Directory.Exists(ReadChordImagesWorkingPath)) Directory.CreateDirectory(ReadChordImagesWorkingPath);

            AppOptions.OptionsPath = Path.Combine(FolderSettingsPath, "AppOptions.json");
            AppOptions.Load();

            if(AppOptions.Options != null)
            {
                ProfileName = Path.GetFileNameWithoutExtension(AppOptions.Options.LastUsedFile);
            }
            else
            {
                AppOptions.Options = new AppOptions();
                ProfileName = "Default";
            }

            HighScoreViewModel.RootFilePath = Path.Combine(FolderSettingsPath, "Terrain");


            Stopwatch sw = new Stopwatch();
            sw.Start();


            string imagesSourceDir = Path.Combine(ChordImagesProfiles, ProfileName);
            bool NeedToRefreshImages = false;
            if (!Directory.Exists(imagesSourceDir))
            {
                imagesSourceDir = ChordsImagesDefaultPath;

                if (!Directory.Exists(imagesSourceDir)) NeedToRefreshImages = true;
            }

            string readImagesSourceDir = Path.Combine(ReadChordImagesProfiles, ProfileName);
            if (!Directory.Exists(readImagesSourceDir))
            {
                readImagesSourceDir = ReadChordsImagesDefaultPath;

                if (!Directory.Exists(readImagesSourceDir)) NeedToRefreshImages = true;
            }

            var chordImageFiles = Directory.GetFiles(imagesSourceDir);
            var readChordImageFiles = Directory.GetFiles(readImagesSourceDir);

            
            Parallel.ForEach(chordImageFiles, chordImageFile =>
            {
                File.Copy(chordImageFile, Path.Combine(ChordImagesWorkingPath, Path.GetFileName(chordImageFile)), true);
            });

            Parallel.ForEach(readChordImageFiles, readChordImageFile =>
            {
                File.Copy(readChordImageFile, Path.Combine(ReadChordImagesWorkingPath, Path.GetFileName(readChordImageFile)), true);
            });

            //Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(imagesSourceDir, ChordImagesWorkingPath, true);
            //Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(readImagesSourceDir, ReadChordImagesWorkingPath, true);
            sw.Stop();
            //NeedToRefreshImages = true;
            MainWindow mainWindow = new GitarUberProject.MainWindow(NeedToRefreshImages);
            sw.Stop();
            swApp.Stop();
            Debug.WriteLine($"swApp: {swApp.Elapsed}  sw: {sw.Elapsed}");
            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);

            //if (e.Exception.GetType().Name != "ParserException")
            //{
            string myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string applicationFolder = "Gitar Uber Project Data";

            var errorDirectory = Path.Combine(myDocumentPath, applicationFolder, "Errors");

            if(!Directory.Exists(errorDirectory))
            {
                Directory.CreateDirectory(errorDirectory);
            }

            var errorLogFullname = Path.Combine(errorDirectory, "ErrorLogsGuitarUberProject.txt");

            string errorContent = $">>>>>{DateTime.Now}{Environment.NewLine}{e.Exception.ToString()}{Environment.NewLine}-----------------{Environment.NewLine}{Environment.NewLine}";

            using (StreamWriter sw = new StreamWriter(errorLogFullname, true))
            {
                sw.WriteLine(errorContent);
            }

            //}

            e.Handled = true;
        }
    }
}
