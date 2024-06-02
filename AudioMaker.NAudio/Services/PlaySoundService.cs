using AudioMaker.Interfaces.Consts;
using AudioMaker.Interfaces.Models.PlaySound;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioMaker.NAudiox.Services
{
    public interface IPlaySoundService
    {
    }

    public class PlaySoundService : IPlaySoundService
    {
        public WaveOut[] StrunyWaves { get; set; } = new WaveOut[6];
        public WaveOut MainWaveOut { get; set; } = new WaveOut();

        public PlaySoundService()
        {
            //poczatkowo bylo w metodzie PlayNote - mozna usunac komentarz
            for (int i = 0; i < StrunyWaves.Length; i++)
            {
                StrunyWaves[i] = new WaveOut();
            }
        }

        public double PlaylistMilisecond()
        {
            var pos = MainWaveOut.GetPosition();

            double ms = MainWaveOut.GetPosition() * 1000.0 / MainWaveOut.OutputWaveFormat.BitsPerSample / MainWaveOut.OutputWaveFormat.Channels * 8 / MainWaveOut.OutputWaveFormat.SampleRate;
            return ms;
        }

        public PlaylistState GetPlaylistState() => MainWaveOut.PlaybackState switch
        {
            PlaybackState.Paused => PlaylistState.Paused,
            PlaybackState.Stopped => PlaylistState.Stopped,
            PlaybackState.Playing => PlaylistState.Playing,
            _ => throw new ArgumentException("unknown PlaybackState!")
        };

        public void PausePlaylist() => MainWaveOut.Pause();

        public void ResumePlaylist() => MainWaveOut.Resume();

        public void StopPlaylist() => MainWaveOut.Stop();

        public void PlayNote(int nrStruny, string mp3Name)
        {
            int idx = nrStruny - 1;
            StrunyWaves[idx].Dispose();
            StrunyWaves[idx] = new WaveOut();
            var reader = new AudioFileReader($@"NotesMp3\GibsonSj200 New\{mp3Name}.wav");

            StrunyWaves[idx].Init(reader);
            StrunyWaves[idx].Play();
        }

        public void TryPlayChordFromPlaylist(PlaysoundStrumViewModel strumManager)
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

        public void MyExtraPlayChordWithStrumPattern(PlaysoundStrumViewModel strumManager)
        {
            //List<string> playedNotesPaths = notesViewModelLite == null ? PrepareToPlayForChord() : PrepareToPlayForChord(notesViewModelLite);

            //if (models == null || !models.Any())
            //{
            //    if (GlobalStrumPattern == null || !GlobalStrumPattern.Any()) return;
            //    models = GlobalStrumPattern;
            //}

            //models.ForEach(a => a.AssignPaths(playedNotesPaths));

            //List<string> GmChord = new List<string> { "s6p3", "s5p5", "s4p5", "s3p3", "s2p3", "s1p3" };
            //List<string> EbChord = new List<string> { "s5p6", "s4p5", "s3p3", "s2p4", "s1p3" };
            //List<string> BbChord = new List<string> { "s6p6", "s5p8", "s4p8", "s3p7", "s2p6", "s1p6" };

            //int offsetMs = 0;

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

            //Stopwatch myU = new Stopwatch();
            //myU.Start();
            ////StrumViewModel strumManager = new StrumViewModel(strumPattern);
            ////StrumViewModel strumManager = new StrumViewModel(models);
            //strumManager.CalculateDelays();
            //myU.Stop();

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

        public void PlayPlaylist(List<PlaysoundKlocekChordModel> klocki, List<PlaysoundMixerModel> mixerModels, double Bpm, double BeatWidth, double delayByMs = 0, bool exportToWavMp3 = false)
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
                volumeSampleProvider.Volume = (float)mixerModels[groupedByChannel[s].Key].Vol / 100;
                globalSamples.Add(volumeSampleProvider);
                //OffsetSampleProvider myOffsetSample = new OffsetSampleProvider(mixSample);
                //myOffsetSample.SkipOver = TimeSpan.FromMilliseconds(delayByMs);
            }

            MixingSampleProvider globalMixSample = new MixingSampleProvider(globalSamples);

            OffsetSampleProvider myOffsetSample = new OffsetSampleProvider(globalMixSample);
            myOffsetSample.SkipOver = TimeSpan.FromMilliseconds(delayByMs);

            if (exportToWavMp3)
            {
                /*
                string recordPath = Path.Combine(App.FolderSettingsPath, "lastRecorded");
                if (!Directory.Exists(recordPath)) Directory.CreateDirectory(recordPath);

                string wavPath = Path.Combine(recordPath, "recorded.wav");
                string mp3Path = Path.Combine(recordPath, "recorded.mp3");
                NAudioHelper.ConvertToFileWavMp3(globalMixSample, wavPath, mp3Path);
                */
            }

            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(myOffsetSample);
            MainWaveOut.Play();
        }

        public void PlayChordPiano(List<string> paths, int delayMs, int index = 0)
        {
            int offsetMs = 0;

            List<ISampleProvider> samples = new List<ISampleProvider>();

            foreach (var path in paths)
            {
                //OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\piano-mp3\{path}.mp3"));
                OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\piano-mp3-volumeUP5\{path}.wav"));

                offsetSample.DelayBy = TimeSpan.FromMilliseconds(offsetMs);
                samples.Add(offsetSample);
                offsetMs += delayMs;
            }

            if (!samples.Any()) return;

            MixingSampleProvider mixSample = new MixingSampleProvider(samples);

            //WaveFileWriter.CreateWaveFile16("mixed22.wav", mixSample);

            bool exportToWavMp3 = false;
            if (exportToWavMp3)
            {
                /*
                string recordPath = Path.Combine(App.FolderSettingsPath, "chordsRecordedNew2222");
                if (!Directory.Exists(recordPath)) Directory.CreateDirectory(recordPath);

                //string wavPath = Path.Combine(recordPath, $"{myChord.Name}_{index}.wav");
                //string mp3Path = Path.Combine(recordPath, $"{myChord.Name}_{index}.mp3");
                string wavPath = Path.Combine(recordPath, $"recordedPiano.wav");
                string mp3Path = Path.Combine(recordPath, $"recordedPiano.mp3");
                NAudioHelper.ConvertToFileWavMp3(mixSample, wavPath, mp3Path);
                */
            }
            else
            {
                MainWaveOut.Dispose();
                MainWaveOut = new WaveOut();
                MainWaveOut.Init(mixSample);
                MainWaveOut.Play();
            }
        }

        public void PlayChord(List<string> paths, int delayMs)
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

        public void PlayTwoChords(List<string> chordANotes, List<string> chordBNotes, int delayBetweenNotes, int delayBetweenChords)
        {
            List<ISampleProvider> samples = new List<ISampleProvider>();

            int idx = 0;
            int fullDelayBy = 0;

            foreach (var item in chordANotes)
            {
                OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{item}.wav"));

                fullDelayBy += delayBetweenNotes;
                offsetSample.DelayBy = TimeSpan.FromMilliseconds(fullDelayBy);
                samples.Add(offsetSample);
                idx++;
            }

            fullDelayBy += delayBetweenChords;
            idx = 0;
            foreach (var item in chordBNotes)
            {
                OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{item}.wav"));

                fullDelayBy += delayBetweenNotes;
                offsetSample.DelayBy = TimeSpan.FromMilliseconds(fullDelayBy);
                //var takeSample = strumItem.PlayTime == -1 ? offsetSample : offsetSample.Take(TimeSpan.FromMilliseconds(strumItem.PlayTime + strumItem.DelayMs + fullDelayMs));
                samples.Add(offsetSample);
                idx++;
            }

            MixingSampleProvider globalMixSample = new MixingSampleProvider(samples);

            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(globalMixSample);
            MainWaveOut.Play();
        }
    }
}