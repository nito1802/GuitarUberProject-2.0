using EditChordsWindow;
using GitarUberProject.Helperes;
using GitarUberProject.Helpers;
using GitarUberProject.Models;
using GitarUberProject.ViewModels;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject
{
    public class NoteDescription
    {
        public string NoteName { get; set; }
        public string NotePath { get; set; }

        public NoteDescription(string noteName, string notePath)
        {
            NoteName = noteName;
            NotePath = notePath;
        }
    }

  

    

    

    public class NotesViewModel : INotifyPropertyChanged
    {
        public int ChordDelayMsDefault { get; } = 20;
        public static double Bpm { get; set; } = 100;
        public double BeatWidth { get; } = 100;
        public List<StrumModel> GlobalStrumPattern { get; set; } = new List<StrumModel>();
        public List<EditStrumModel> GlobalEditStrumModels { get; set; } = new List<EditStrumModel>();

        private RelayCommand moveChordLeft;
        private RelayCommand moveChordRight;
        private RelayCommand playAtOnce;
        private RelayCommand playUp;
        private RelayCommand playDown;
        private RelayCommand clearChords;
        private int strumDelayMs;

        public Dictionary<string, string> BrushNotesDict { get; set; }
        public Dictionary<int, string> BrushOctavesDict { get; set; }
        
        public string[] AllNotes { get; set; } = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };

        public List<NoteModel> Notes { get; set; }
        public static Dictionary<string, NoteModel> NotesDict { get; set; }
        public static Dictionary<string, NoteModel> NotesNameDict { get; set; } = new Dictionary<string, NoteModel>();

        public WaveOut[] StrunyWaves { get; set; } = new WaveOut[6];
        public WaveOut MainWaveOut { get; set; } = new WaveOut();
        public NotesViewModel()
        {

            //LiteNote[] startNotes = new LiteNote[] {"E", "A", "D", "G", "B", "E" };
            InitDicts();
            InitNotes();
            InitPlayNotes();
            StrumDelayMs = ChordDelayMsDefault;

            if(NotesViewModelLiteVersion.PlayDownAction == null)
            {
                NotesViewModelLiteVersion.PlayDownAction = (a) => PlayDownMethod(a);
            }
        }

        public ICommand MoveChordLeft
        {
            get
            {
                if (moveChordLeft == null)
                {
                    moveChordLeft = new RelayCommand(param =>
                    {
                        MoveNotesHorizontally(-1);
                    }
                     , param => true);
                }
                return moveChordLeft;
            }
        }

        public ICommand MoveChordRight
        {
            get
            {
                if (moveChordRight == null)
                {
                    moveChordRight = new RelayCommand(param =>
                    {
                        MoveNotesHorizontally(1);
                    }
                     , param => true);
                }
                return moveChordRight;
            }
        }

        public ICommand PlayAtOnce
        {
            get
            {
                if (playAtOnce == null)
                {
                    playAtOnce = new RelayCommand(param =>
                    {
                        if (GlobalStrumPattern.Any())
                        {
                            MyExtraPlayChordWithStrumPattern(null);
                        }
                        else
                        {
                            var playedNotes = PrepareToPlay();
                            playedNotes.Reverse();
                            PlayChord(playedNotes, strumDelayMs);
                        }
                    }
                     , param => true);
                }
                return playAtOnce;
            }
        }



        public ICommand PlayDown
        {
            get
            {
                if (playDown == null)
                {
                    playDown = new RelayCommand(param =>
                    {
                        PlayDownMethod();
                    }
                     , param => true);
                }
                return playDown;
            }
        }

        public ICommand PlayDownGuitarControl
        {
            get
            {
                if (playDown == null)
                {
                    playDown = new RelayCommand(param =>
                    {
                        var playedNotes = PrepareToPlay();
                        playedNotes.Reverse();
                        PlayChord(playedNotes, strumDelayMs);
                    }
                     , param => true);
                }
                return playDown;
            }
        }


        public ICommand PlayUp
        {
            get
            {
                if (playUp == null)
                {
                    playUp = new RelayCommand(param =>
                    {
                        var playedNotes = PrepareToPlay();
                        PlayChord(playedNotes, strumDelayMs);
                    }
                     , param => true);
                }
                return playUp;
            }
        }
        
        public ICommand ClearChords
        {
            get
            {
                if (clearChords == null)
                {
                    clearChords = new RelayCommand(param =>
                    {
                        Notes.ForEach(a => a.ClearNote());
                        NoteModel.UpdateGitarMainChord();
                    }
                     , param => true);
                }
                return clearChords;
            }
        }

        public int StrumDelayMs
        {
            get
            {
                return strumDelayMs;
            }

            set
            {
                strumDelayMs = value;
                OnPropertyChanged("StrumDelayMs");
            }
        }

        private void InitPlayNotes()
        {
            for (int i = 0; i < StrunyWaves.Length; i++)
            {
                StrunyWaves[i] = new WaveOut();
            }
            
            Action<int, string> PlayNoteAction = (nrStruny, mp3Name) =>
            {
                try
                {
                    int idx = nrStruny - 1;
                    StrunyWaves[idx].Dispose();
                    StrunyWaves[idx] = new WaveOut();
                    var reader = new AudioFileReader($@"NotesMp3\GibsonSj200 New\{mp3Name}.wav");

                    StrunyWaves[idx].Init(reader);
                    StrunyWaves[idx].Play();
                }
                catch (Exception ex)
                {
                    //tymczasowe: na wypadek jak by nie bylo odpowiedniego pliku
                }
            };

            Action<int> RefreshNotesOnStrunaAction = (nrStruny) =>
            {
                RefreshStrunaGuitar(nrStruny);

                //tbAkordContent.Text = string.Join("   ", checkedNotes);
            };


            NoteModel.PlayNoteAction = PlayNoteAction;
            NoteModel.RefreshNotesOnStrunaAction = RefreshNotesOnStrunaAction;
        }

        private void InitNotes()
        {
            Notes = new List<NoteModel>();
            NotesDict = new Dictionary<string, NoteModel>();

            int notesColumnCount = 15;

            int rowCounter = 1;

            LiteNote[] startNotes = new LiteNote[]
            {
                new LiteNote("E", 5),
                new LiteNote("B", 4),
                new LiteNote("G", 4),
                new LiteNote("D", 4),
                new LiteNote("A", 3),
                new LiteNote("E", 3),
            };

            foreach (var item in startNotes)
            {
                LiteNote currentNote = new LiteNote(item.Name, item.Octave);
                for (int i = 0; i < notesColumnCount; i++)
                {
                    NoteModel noteModel = new NoteModel(currentNote.Name, currentNote.Octave, i, rowCounter);
                    noteModel.NoteBrush = (Brush)(new BrushConverter().ConvertFrom(BrushNotesDict[currentNote.Name]));
                    noteModel.OctaveBrush = (Brush)(new BrushConverter().ConvertFrom(BrushOctavesDict[currentNote.Octave]));
                    noteModel.NoteBackground = NotesHelper.ChordColor[currentNote.Name];

                    Notes.Add(noteModel);
                    NotesDict.Add($"s{rowCounter}p{i}", noteModel);

                    string notesNameDictKey = $"{noteModel.Name}{noteModel.Octave}";
                    if (!NotesNameDict.ContainsKey(notesNameDictKey))
                    {
                        NotesNameDict.Add(notesNameDictKey, noteModel);
                    }
                    //S1_P0
                    currentNote = NotesHelper.NextNote(currentNote);
                }

                rowCounter++;
            }
        }

        void InitDicts()
        {
            BrushNotesDict = new Dictionary<string, string>
            {
                { "A", "#C9006A"},
                { "A#", "#000000"},
                { "B", "#BA7706"},
                { "C", "#129107"},
                { "C#", "#000000"},
                { "D", "#0649AD"},
                { "D#", "#000000"},
                { "E", "#6A0A82"},
                { "F", "#005E4A"},
                { "F#", "#000000"},
                { "G", "#D63E00"},
                { "G#", "#000000"},
            };

            BrushOctavesDict = new Dictionary<int, string>
            {
                { 3, "#E8BD00"},
                { 4, "#1D47C2"},
                { 5, "#008730"},
                { 6, "#B50461"},
            };
        }

        public void PlayDownMethod()
        {
            if(GlobalStrumPattern.Any())
            {
                MyExtraPlayChordWithStrumPattern(null);
            }
            else
            {
                var playedNotes = PrepareToPlay();
                playedNotes.Reverse();
                PlayChord(playedNotes, strumDelayMs);
            }
        }

        public void PlayDownMethod(NotesViewModelLiteVersion notesViewModelLite)
        {
            if (GlobalStrumPattern.Any())
            {
                MyExtraPlayChordWithStrumPattern(null, notesViewModelLite);
            }
            else
            {
                var playedNotes = PrepareToPlay(notesViewModelLite);
                playedNotes.Reverse();
                PlayChord(playedNotes, strumDelayMs);
            }
        }


        public StrumViewModel GetStrumModels(List<string> playedNotes)
        {
            List<StrumModel> myModel = null;

            if (GlobalStrumPattern != null && GlobalStrumPattern.Any())
            {
                myModel = GlobalStrumPattern.Select(a => (StrumModel)a.Clone()).ToList();
                myModel.ForEach(a => a.AssignPaths(playedNotes));
            }
            else
            {
                myModel = new List<StrumModel>()
                {
                    new StrumModel(playedNotes, StrumDirection.Downward, 20, 0, 6, 0),
                };
            }
            
            Stopwatch myU = new Stopwatch();
            myU.Start();
            //StrumViewModel strumManager = new StrumViewModel(strumPattern);
            StrumViewModel strumManager = new StrumViewModel(myModel);
            strumManager.CalculateDelays();

            return strumManager;
        }


        public void TryPlayChordFromPlaylist(StrumViewModel strumManager)
        {
            List<ISampleProvider> samples = new List<ISampleProvider>();

            int strumCounter = 0;
            foreach (var item in strumManager.StrumPattern)
            {
                int counter = 0;
                foreach (var strumItem in item.PlayedNotes)
                {
                    OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{strumItem.Path}.wav"));

                    offsetSample.DelayBy = TimeSpan.FromMilliseconds(strumItem.DelayMs);
                    var takeSample = strumItem.PlayTime == -1 ? offsetSample : offsetSample.Take(TimeSpan.FromMilliseconds(strumItem.PlayTime + strumItem.DelayMs));
                    samples.Add(takeSample);
                    counter++;
                }
                strumCounter++;
            }

            if (!samples.Any()) return;

            MixingSampleProvider mixSample = new MixingSampleProvider(samples);

            //NAudioHelper.ConvertToFileWavMp3(mixSample, "mixedBest.wav", "mixedBest.mp3");

            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(mixSample);
            MainWaveOut.Play();
        }



        public void MyExtraPlayChordWithStrumPattern(List<StrumModel> models, NotesViewModelLiteVersion notesViewModelLite = null)
        {
            List<string> playedNotesPaths = notesViewModelLite == null ? PrepareToPlayForChord() : PrepareToPlayForChord(notesViewModelLite);
            
            if (models == null || !models.Any())
            {
                if (GlobalStrumPattern == null || !GlobalStrumPattern.Any()) return;
                models = GlobalStrumPattern;
            }

            models.ForEach(a => a.AssignPaths(playedNotesPaths));

            List<string> GmChord = new List<string> { "s6p3", "s5p5", "s4p5", "s3p3", "s2p3", "s1p3" };
            List<string> EbChord = new List<string> { "s5p6", "s4p5", "s3p3", "s2p4", "s1p3" };
            List<string> BbChord = new List<string> { "s6p6", "s5p8", "s4p8", "s3p7", "s2p6", "s1p6" };


            int offsetMs = 0;

            List<ISampleProvider> samples = new List<ISampleProvider>();

            //List<StrumModel> strumPattern = new List<StrumModel>()
            //{
            //    new StrumModel(GmChord, StrumDirection.Downward, 40, 0, 3, 0),
            //    new StrumModel(GmChord, StrumDirection.Downward, 40, 800, 4, 0),

            //    new StrumModel(GmChord, StrumDirection.Upward, 15, 400, 6, 1),
            //    new StrumModel(GmChord, StrumDirection.Downward, 10, 100, 5, 0),


            //    new StrumModel(EbChord, StrumDirection.Upward, 15, 160, 6, 1),
            //    new StrumModel(EbChord, StrumDirection.Downward, 15, 160, EbChord.Count-1, 0),
            //    new StrumModel(EbChord, StrumDirection.Upward, 15, 160, 6, 1),
            //    new StrumModel(EbChord, StrumDirection.Downward, 20, 350, EbChord.Count-1, 0),

            //    new StrumModel(EbChord, StrumDirection.Upward, 15, 360, 6, 1),
            //    new StrumModel(EbChord, StrumDirection.Downward, 25, 100, EbChord.Count-1, 0),

            //    new StrumModel(BbChord, StrumDirection.Downward, 40, 160, BbChord.Count-1, 0),
            //    new StrumModel(BbChord, StrumDirection.Downward, 40, 700, BbChord.Count-1, 0),

            //    new StrumModel(BbChord, StrumDirection.Downward, 40, 400, BbChord.Count-1, 0),
            //    new StrumModel(BbChord, StrumDirection.Downward, 25, 300, BbChord.Count-1, 0),
            //    new StrumModel(BbChord, StrumDirection.Upward, 25, 100, 6, 1),

            //    new StrumModel(BbChord, StrumDirection.Downward, 25, 400, BbChord.Count-1, 0),
            //    new StrumModel(BbChord, StrumDirection.Upward, 25, 100, 6, 1),
            //    new StrumModel(BbChord, StrumDirection.Downward, 25, 100, BbChord.Count-1, 0),
            //    new StrumModel(BbChord, StrumDirection.Upward, 40, 100, 6, 1),
            //};

            Stopwatch myU = new Stopwatch();
            myU.Start();
            //StrumViewModel strumManager = new StrumViewModel(strumPattern);
            StrumViewModel strumManager = new StrumViewModel(models);
            strumManager.CalculateDelays();
            myU.Stop();

            //Gm     "s6p3", "s5p5", "s4p5", "s3p3", "s2p3", "s1p3"
            //Eb     "s5p6", "s4p5", "s3p3", "s2p4", "s1p3"
            //Bb     "s6p6", "s5p8", "s4p8", "s3p7", "s2p6", "s1p6"
            int strumCounter = 0;
            foreach (var item in strumManager.StrumPattern)
            {
                int counter = 0;
                foreach (var strumItem in item.PlayedNotes)
                {
                    OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{strumItem.Path}.wav"));

                    offsetSample.DelayBy = TimeSpan.FromMilliseconds(strumItem.DelayMs);
                    var takeSample = strumItem.PlayTime == -1 ? offsetSample : offsetSample.Take(TimeSpan.FromMilliseconds(strumItem.PlayTime + strumItem.DelayMs));
                    samples.Add(takeSample);
                    counter++;
                }
                strumCounter++;
            }

            if (!samples.Any()) return;

            MixingSampleProvider mixSample = new MixingSampleProvider(samples);

            //NAudioHelper.ConvertToFileWavMp3(mixSample, "mixedBest.wav", "mixedBest.mp3");

            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(mixSample);
            MainWaveOut.Play();
        }

        public List<StrumModel> ConvertEditStrumModelsToStrumPattern(List<EditStrumModel> editStrumModels)
        {
            List<StrumModel> res = new List<StrumModel>();

            foreach (var item in editStrumModels)
            {
                var playedNotes = item.Notes.Where(a => a.CheckedNote != CheckedFinger.None).ToList();
                var firstNote = playedNotes.First();
                var lastNote = playedNotes.Last();

                StrumDirection strumDir = (firstNote.CheckedNote == CheckedFinger.secondFinger) ? StrumDirection.Downward : StrumDirection.Upward;
                long delayMs = item.DelayBetweenStrunaMs;

                int skipNotes = 0;
                if (strumDir == StrumDirection.Downward)
                {
                    skipNotes = 5 - lastNote.MyIdx;
                }
                else
                {
                    skipNotes = firstNote.MyIdx;
                }

                StrumModel strumModel = new StrumModel(strumDir, delayMs, item.DelayBeforeMs, playedNotes.Count, skipNotes);
                res.Add(strumModel);
            }

            return res;
        }

        public List<StrumModel> ConvertToViewStrumModelsToStrumPattern(List<ToViewStrumModels> editStrumModels)
        {
            List<StrumModel> res = new List<StrumModel>();

            foreach (var item in editStrumModels)
            {
                var playedNotes = item.Notes.Where(a => a.CheckedNote != CheckedFinger.None).ToList();
                var firstNote = playedNotes.First();
                var lastNote = playedNotes.Last();

                StrumDirection strumDir = (firstNote.CheckedNote == CheckedFinger.secondFinger) ? StrumDirection.Downward : StrumDirection.Upward;
                long delayMs = item.DelayBetweenStrunaMs;

                int skipNotes = 0;
                if (strumDir == StrumDirection.Downward)
                {
                    skipNotes = 5 - lastNote.MyIdx;
                }
                else
                {
                    skipNotes = firstNote.MyIdx;
                }

                StrumModel strumModel = new StrumModel(strumDir, delayMs, item.DelayBeforeMs, playedNotes.Count, skipNotes);
                res.Add(strumModel);
            }

            return res;
        }

        public void PlayPlaylist(ObservableCollection<KlocekChordModel> klocki, List<MixerModel> mixerModels, double delayByMs = 0, bool exportToWavMp3 = false)
        {
            if (klocki == null || !klocki.Any()) return;

            double delayBpm = 60_000 / Bpm;

            List<ISampleProvider> globalSamples = new List<ISampleProvider>();
            var groupedByChannel = klocki.GroupBy(c => c.ChannelNr).ToList();

            for (int s = 0; s < groupedByChannel.Count; s++)
            {
                var klocekInChannel = groupedByChannel[s];

                List<ISampleProvider> samples = new List<ISampleProvider>();


                var notesKlocki = klocekInChannel.Where(b => b.IsChord == false).ToList();
                var chordsKlocki = klocekInChannel.Where(b => b.IsChord == true).ToList();

                var orderedKlocki = notesKlocki.OrderBy(a => a.XPos).ToList();

                int strumCounter = 0;
                for (int i = 0; i < orderedKlocki.Count; i++)
                {
                    var item = orderedKlocki[i];

                    OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{item.Mp3Name}.wav"));
                    double fullDelayMs = item.XPos / BeatWidth * delayBpm;
                    offsetSample.DelayBy = TimeSpan.FromMilliseconds(fullDelayMs);

                    //if (i == klocki.Count - 1)
                    //{
                    //    double playDuration = KlocekChordViewModel.ItemWidth / BeatWidth * fullDelayMs;
                    //    var takeSample = offsetSample.Take(TimeSpan.FromMilliseconds(playDuration + fullDelayMs));
                    //    samples.Add(takeSample);
                    //}
                    //else
                    {
                        //VolumeSampleProvider volumeSampleProvider = new VolumeSampleProvider(offsetSample);
                        //volumeSampleProvider.Volume = 1f;
                        samples.Add(offsetSample);
                    }
                }


                foreach (var chordKlocek in chordsKlocki)
                {
                    double fullDelayMs = chordKlocek.XPos / BeatWidth * delayBpm;


                    foreach (var item in chordKlocek.StrumViewModel.StrumPattern)
                    {
                        int counter = 0;
                        foreach (var strumItem in item.PlayedNotes)
                        {
                            OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{strumItem.Path}.wav"));

                            offsetSample.DelayBy = TimeSpan.FromMilliseconds(strumItem.DelayMs + fullDelayMs);
                            var takeSample = strumItem.PlayTime == -1 ? offsetSample : offsetSample.Take(TimeSpan.FromMilliseconds(strumItem.PlayTime + strumItem.DelayMs + fullDelayMs));
                            samples.Add(takeSample);
                            counter++;
                        }
                        strumCounter++;
                    }
                }


                if (!samples.Any()) return;
                MixingSampleProvider mixSample = new MixingSampleProvider(samples);

                VolumeSampleProvider volumeSampleProvider = new VolumeSampleProvider(mixSample);
                volumeSampleProvider.Volume = (float)mixerModels[groupedByChannel[s].Key].Vol/100;
                globalSamples.Add(volumeSampleProvider);
                //OffsetSampleProvider myOffsetSample = new OffsetSampleProvider(mixSample);
                //myOffsetSample.SkipOver = TimeSpan.FromMilliseconds(delayByMs);
            }

            MixingSampleProvider globalMixSample = new MixingSampleProvider(globalSamples);

            OffsetSampleProvider myOffsetSample = new OffsetSampleProvider(globalMixSample);
            myOffsetSample.SkipOver = TimeSpan.FromMilliseconds(delayByMs);

            if(exportToWavMp3)
            {
                string recordPath = Path.Combine(App.FolderSettingsPath, "lastRecorded");
                if (!Directory.Exists(recordPath)) Directory.CreateDirectory(recordPath);

                string wavPath = Path.Combine(recordPath, "recorded.wav");
                string mp3Path = Path.Combine(recordPath, "recorded.mp3");
                NAudioHelper.ConvertToFileWavMp3(globalMixSample, wavPath, mp3Path);
            }

            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(myOffsetSample);
            MainWaveOut.Play();
        }


        public void PlayAtOnceMethod()
        {
            var playedNotes = PrepareToPlay();
            playedNotes.Reverse();

            PlayChord(playedNotes, 0);
        }

        public void RefreshStrunaGuitar(int nrStruny)
        {
            var notesOnStruna = Notes.Where(a => a.Struna == nrStruny).ToList();

            int checkedIdx = -1;
            for (int i = notesOnStruna.Count - 1; i >= 0; i--)
            {
                if (notesOnStruna[i].IsSelected)
                {
                    checkedIdx = i;
                    break;
                }
            }

            for (int i = 0; i < notesOnStruna.Count; i++)
            {
                var note = notesOnStruna[i];

                if (i <= checkedIdx)
                {
                    note.PlayedOrBefore = true;

                    if (note.IsSelected) continue;
                    if (i != checkedIdx) note.ClearIsSelected();
                }
                else
                {
                    if (note.IsSelected) continue;
                    else note.PlayedOrBefore = false;

                    if (i != checkedIdx) note.ClearIsSelected();
                }
            }
        }

        public void RefreshAllStrunyOnGuitar()
        {
            for (int i = 1; i < 7; i++)
            {
                RefreshStrunaGuitar(i);
            }
        }

        public List<NoteModel> GetCheckedNotes()
        {
            List<NoteModel> res = Notes.Where(a => a.IsSelected).ToList();
            return res;
        }

        private void PlayChord(List<string> paths, int delayMs)
        {
            int offsetMs = 0;

            List<ISampleProvider> samples = new List<ISampleProvider>();

            foreach (var path in paths)
            {
                OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{path}.wav"));
                offsetSample.DelayBy = TimeSpan.FromMilliseconds(offsetMs);
                samples.Add(offsetSample);
                offsetMs += delayMs;
            }

            if (!samples.Any()) return;

            MixingSampleProvider mixSample = new MixingSampleProvider(samples);
            //WaveFileWriter.CreateWaveFile16("mixed22.wav", mixSample);
            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(mixSample);
            MainWaveOut.Play();
        }

        public List<string> PrepareToPlay()
        {
            var groupByStruny = Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();

            foreach (var strunaNotsy in groupByStruny)
            {
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.IsSelected && note.IsNoteEnabled)
                    {
                        playedNotes.Add(note.Mp3Name);
                        break;
                    }
                }
            }

            return playedNotes;
        }

        public static List<string> PrepareToPlay(NotesViewModelLiteVersion notesViewModelLite)
        {
            var groupByStruny = notesViewModelLite.Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();
            
            int currStruna = 1;
            int offsetFr = notesViewModelLite.Fr > 1 ? notesViewModelLite.Fr - 1 : 0; //nie do konca rozumiem czemu tak....

            foreach (var strunaNotsy in groupByStruny)
            {
                bool foundNote = false;
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.CheckedFinger != CheckedFinger.None)
                    {
                        playedNotes.Add($"s{note.Struna}p{note.Prog + 1 + offsetFr}");
                        foundNote = true;
                        break;
                    }
                }

                if (!foundNote)
                {
                    if(notesViewModelLite.NotesO[currStruna - 1] == "O")
                    {
                        playedNotes.Add($"s{currStruna}p0");

                    }
                }

                currStruna++;
            }

            return playedNotes;
        }

        public List<string> PrepareToPlayForChord()
        {
            var groupByStruny = Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();

            foreach (var strunaNotsy in groupByStruny)
            {
                bool found = false;
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.IsSelected && note.IsNoteEnabled)
                    {
                        playedNotes.Add(note.Mp3Name);
                        found = true;
                        break;
                    }
                }

                if(!found)
                {
                    playedNotes.Add("");
                }
            }

            return playedNotes;
        }

        public List<string> PrepareToPlayForChord(NotesViewModelLiteVersion notesViewModelLite)
        {
            var groupByStruny = notesViewModelLite.Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();

            int currStruna = 1;
            int offsetFr = notesViewModelLite.Fr > 1 ? notesViewModelLite.Fr - 1 : 0; //nie do konca rozumiem czemu tak....
            foreach (var strunaNotsy in groupByStruny)
            {
                bool found = false;
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.CheckedFinger != CheckedFinger.None)
                    {
                        playedNotes.Add($"s{note.Struna}p{note.Prog + 1 + offsetFr}");
                        found = true;
                        break;
                    }
                }


                if (!found)
                {
                    if (notesViewModelLite.NotesO[currStruna - 1] == "O")
                    {
                        playedNotes.Add($"s{currStruna}p0");
                        found = true;
                    }
                }

                if (!found)
                {
                    playedNotes.Add("");
                }

                currStruna++;
            }

            return playedNotes;
        }

        public void MoveToChooseProg(int prog)
        {
            var selectedNotes = Notes.Where(a => a.IsSelected).ToList();
            var minProg = selectedNotes.Min(a => a.Prog);

            int treshold = prog - minProg;

            bool changed = false;
            var usedNotes = new List<NoteModel>();

            for (int i = 0; i < selectedNotes.Count; i++)
            {
                var myNote = selectedNotes[i];

                var nextNote = Notes.FirstOrDefault(a => a.Struna == myNote.Struna && a.Prog == (myNote.Prog + treshold));

                if (nextNote != null)
                {
                    nextNote.IsSelecteOnlySet = true;
                    usedNotes.Add(nextNote);

                    if (!usedNotes.Contains(myNote)) myNote.IsSelecteOnlySet = false;

                    changed = true;
                }
            }

            if (changed)
            {
                NoteModel.UpdateGitarMainChordAction?.Invoke();
                NoteModel.SetGitarFocusAction.Invoke();
            }
        }

        private void MoveNotesHorizontally(int treshold)
        {
            var selectedNotes = Notes.Where(a => a.IsSelected).ToList();

            bool changed = false;
            var usedNotes = new List<NoteModel>();

            for (int i = 0; i < selectedNotes.Count; i++)
            {
                var myNote = selectedNotes[i];

                var nextNote = Notes.FirstOrDefault(a => a.Struna == myNote.Struna && a.Prog == (myNote.Prog + treshold));

                if (nextNote != null)
                {
                    nextNote.IsSelecteOnlySet = true;
                    usedNotes.Add(nextNote);

                    if (!usedNotes.Contains(myNote)) myNote.IsSelecteOnlySet = false;

                    changed = true;
                }
            }

            if (changed)
            {
                NoteModel.UpdateGitarMainChordAction?.Invoke();
                NoteModel.SetGitarFocusAction.Invoke();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; //INotifyPropertyChanged

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
