using EditChordsWindow;
using GitarUberProject.Models;
using GitarUberProject.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GitarUberProject.Services
{
    public  class PlaylistService
    {
        public static void StopPlaylist(
            double BorderPlaylistOffset,
            NotesViewModel NotesViewModel,
            DispatcherTimer PlaylistTimer,
            Border borderPositionPlaylist
            )
        {
            BorderPlaylistOffset = 0;
            NotesViewModel.MainWaveOut.Stop();
            PlaylistTimer.Stop();
            Canvas.SetLeft(borderPositionPlaylist, 0);
        }


        public static void MoveNoteToPlaylistAction(
            NoteModel note,
            KlocekChordViewModel KlocekViewModel,
            ComboBox cbCurrentChannel,
            MixerViewModel MixerViewModel,
            Stack<List<KlocekChordModel>> PlaylistRedoStack,
            Stack<List<KlocekChordModel>> PlaylistStack
            )
        {
            AddPlaylistActionToStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel);

            var selectedNotes = KlocekViewModel.SelectedItems.Where(a => !a.IsChord).ToList();

            if (selectedNotes.Any())
            {
                foreach (var item in selectedNotes)
                {
                    KlocekChordModel newKlocek = new KlocekChordModel(note.Name, note.Octave, note.Prog, note.Struna) { Mp3Name = note.Mp3Name };
                    newKlocek.XPos = item.XPos;
                    newKlocek.YPos = item.YPos;
                    newKlocek.KlocekOpacity = item.KlocekOpacity;

                    newKlocek.ChannelNr = cbCurrentChannel.SelectedIndex;

                    List<int> visibleChannels = new List<int>();

                    for (int i = 0; i < MixerViewModel.MixerModels.Count; i++)
                    {
                        if (MixerViewModel.MixerModels[i].IsTurn) visibleChannels.Add(i);
                    }

                    if (visibleChannels.Contains(newKlocek.ChannelNr))
                    {
                        newKlocek.KlocekVisibility = Visibility.Visible;
                    }
                    else
                    {
                        newKlocek.KlocekVisibility = Visibility.Hidden;
                    }

                    KlocekViewModel.Klocki.Remove(item);
                    KlocekViewModel.Klocki.Add(newKlocek);
                }
            }
            else
            {
                KlocekChordModel klocek = new KlocekChordModel(note.Name, note.Octave, note.Prog, note.Struna) { Mp3Name = note.Mp3Name };
                klocek.YPos = KlocekChordViewModel.ItemHeight * (klocek.Struna - 1);

                var notesKlocki = KlocekViewModel.Klocki.Where(b => b.IsChord == false && b.ChannelNr == cbCurrentChannel.SelectedIndex).ToList();
                var chordsKlocki = KlocekViewModel.Klocki.Where(b => b.IsChord == true && b.ChannelNr == cbCurrentChannel.SelectedIndex).ToList();

                double xPosMaxNote = notesKlocki.Any() ? notesKlocki.Max(b => b.XPos) + KlocekChordViewModel.ItemWidth : 0;
                double xPosMaxChordNote = 0;
                if (chordsKlocki.Any())
                {
                    List<double> xPosy = new List<double>();
                    foreach (var item in chordsKlocki)
                    {
                        xPosy.Add(item.XPos + item.NotesInChord.Max(c => c.XPos) + KlocekChordViewModel.ItemWidth);
                    }
                    xPosMaxChordNote = xPosy.Max();
                }

                klocek.ChannelNr = cbCurrentChannel.SelectedIndex;
                List<int> visibleChannels = new List<int>();

                for (int i = 0; i < MixerViewModel.MixerModels.Count; i++)
                {
                    if (MixerViewModel.MixerModels[i].IsTurn) visibleChannels.Add(i);
                }

                if (visibleChannels.Contains(klocek.ChannelNr))
                {
                    klocek.KlocekVisibility = Visibility.Visible;
                }
                else
                {
                    klocek.KlocekVisibility = Visibility.Hidden;
                }

                klocek.XPos = Math.Max(xPosMaxNote, xPosMaxChordNote);
                KlocekViewModel.Klocki.Add(klocek);
            }
        }


        public static void AddChordToPlaylist(
            NotesViewModelLiteVersion chord,
            KlocekChordViewModel KlocekViewModel,
            NotesViewModel NotesViewModel,
            MixerViewModel MixerViewModel,
            ComboBox cbCurrentChannel,
            Stack<List<KlocekChordModel>> PlaylistRedoStack,
            Stack<List<KlocekChordModel>> PlaylistStack
            )
        {
            NotesViewModelLiteVersion clone = (NotesViewModelLiteVersion)chord.Clone();
            ChordMode chordMode = ChordMode.Read;
            //clone.UpdateChordImage(chordMode);

            AddPlaylistActionToStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel);

            KlocekChordModel klocek = new KlocekChordModel();
            klocek.ChordModel = chord;
            klocek.IsChord = true;
            var notesToPlay = NotesViewModel.PrepareToPlayForChord();
            var oct = chord.NoteOctaves;

            var myStrumModel = NotesViewModel.GetStrumModels(notesToPlay);
            double delayBpm = 60_000 / NotesViewModel.Bpm;

            Dictionary<string, KlocekNoteDetails> klocekNoteDetailsDict = new Dictionary<string, KlocekNoteDetails>();
            for (int i = 0; i < notesToPlay.Count; i++)
            {
                if (string.IsNullOrEmpty(notesToPlay[i])) continue;

                KlocekNoteDetails klocekNoteDetails = new KlocekNoteDetails()
                {
                    Name = oct[i].Name,
                    Octave = int.Parse(oct[i].Octave),
                    Struna = oct[i].Struna,
                    Prog = oct[i].Prog,
                    Mp3Name = notesToPlay[i]
                };

                klocekNoteDetailsDict.Add(notesToPlay[i], klocekNoteDetails);
            }

            //klocek.YPos = KlocekChordViewModel.ItemHeight * (klocek.Struna - 1);


            var notesKlocki = KlocekViewModel.Klocki.Where(b => b.IsChord == false && b.ChannelNr == cbCurrentChannel.SelectedIndex).ToList();
            var chordsKlocki = KlocekViewModel.Klocki.Where(b => b.IsChord == true && b.ChannelNr == cbCurrentChannel.SelectedIndex).ToList();

            double xPosMaxNote = notesKlocki.Any() ? notesKlocki.Max(b => b.XPos) + KlocekChordViewModel.ItemWidth : 0;
            double xPosMaxChordNote = 0;
            if (chordsKlocki.Any())
            {
                List<double> xPosy = new List<double>();
                foreach (var item in chordsKlocki)
                {
                    xPosy.Add(item.XPos + item.NotesInChord.Max(c => c.XPos) + KlocekChordViewModel.ItemWidth);
                }
                xPosMaxChordNote = xPosy.Max();
            }

            klocek.XPos = Math.Max(xPosMaxNote, xPosMaxChordNote);

            double currentDelay = 0;
            int idx = 0;
            foreach (var item in myStrumModel.StrumPattern)
            {
                currentDelay += item.DelayBeforeMs;
                foreach (var strumItem in item.PlayedNotes)
                {
                    var dictItem = klocekNoteDetailsDict[strumItem.Path];
                    KlocekNoteDetails klocekNoteDetails = new KlocekNoteDetails()
                    {
                        Name = dictItem.Name,
                        Octave = dictItem.Octave,
                        Struna = dictItem.Struna,
                        Prog = dictItem.Prog,
                        Mp3Name = dictItem.Mp3Name
                    };
                    klocekNoteDetails.AssignBackground();

                    if (idx != 0)
                    {
                        currentDelay += item.DelayMs;
                    }

                    klocekNoteDetails.XPos = currentDelay * NotesViewModel.BeatWidth / delayBpm;
                    klocekNoteDetails.YPos = KlocekChordViewModel.ItemHeight * (klocekNoteDetails.Struna - 1);
                    klocek.NotesInChord.Add(klocekNoteDetails);

                    idx++;
                }
            }

            klocek.YPos = klocek.NotesInChord.Min(b => b.YPos);

            foreach (var item in klocek.NotesInChord)
            {
                item.YPos -= klocek.YPos;
            }

            klocek.NotesToPlay = notesToPlay;
            klocek.StrumViewModel = myStrumModel;
            klocek.ChordWidth = klocek.NotesInChord.Max(c => c.XPos) - klocek.NotesInChord.Min(c => c.XPos) + KlocekChordViewModel.ItemWidth;
            klocek.ChordHeight = klocek.NotesInChord.Max(c => c.YPos) - klocek.NotesInChord.Min(c => c.YPos) + 20;

            if (string.IsNullOrEmpty(chord.ChordName))
            {
                var filteredNoteOctaves = chord.NoteOctaves
                                        .Where(a => !string.IsNullOrEmpty(a.Name))
                                        .Reverse()
                                        .ToList();

                chord.ChordName = filteredNoteOctaves.First().Name;
            }

            klocek.ChordBackground = NotesHelper.ChordColor[chord.ChordName];
            klocek.ChordBackground = NotesHelper.ChordColor[chord.ChordName];
            klocek.ChordBorder = NotesHelper.ChordColorNoOpacity[chord.ChordName];
            klocek.ChordName = clone.ChordFullName;

            klocek.ChannelNr = cbCurrentChannel.SelectedIndex;
            List<int> visibleChannels = new List<int>();

            for (int i = 0; i < MixerViewModel.MixerModels.Count; i++)
            {
                if (MixerViewModel.MixerModels[i].IsTurn) visibleChannels.Add(i);
            }
            if (visibleChannels.Contains(klocek.ChannelNr))
            {
                klocek.KlocekVisibility = Visibility.Visible;
            }
            else
            {
                klocek.KlocekVisibility = Visibility.Hidden;
            }

            KlocekViewModel.Klocki.Add(klocek);
        }

        public static void SavePlaylistSong(
            string path, 
            TextBox tbBpm, 
            KlocekChordViewModel KlocekViewModel,
            TextBox tbPlaylistWidth
            )
        {
            var myText = tbBpm.Text.Replace(',', '.');
            bool parseResult = double.TryParse(myText, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed);

            if (parseResult)
            {
                KlocekViewModel.Bpm = parsed;
            }
            else KlocekViewModel.Bpm = 100;


            bool parseResultCanvas = int.TryParse(tbPlaylistWidth.Text, out var parsedCanvas);

            if (parseResultCanvas)
            {
                KlocekViewModel.CanvasWidth = parsedCanvas;
            }
            else KlocekViewModel.CanvasWidth = 300;


            string serializedText = JsonConvert.SerializeObject(KlocekViewModel, Formatting.Indented);
            File.WriteAllText(path, serializedText);
        }

        public static void LoadPlaylist(
            string path,
            KlocekChordViewModel KlocekViewModel,
            TextBox tbBpm,
            TextBox tbPlaylistWidth,
            MixerViewModel MixerViewModel
            )
        {
            string text = File.ReadAllText(path);

            var loadedPlaylistKlocki = JsonConvert.DeserializeObject<KlocekChordViewModel>(text);

            KlocekViewModel.Klocki.Clear();

            tbBpm.Text = loadedPlaylistKlocki.Bpm.ToString();
            tbPlaylistWidth.Text = loadedPlaylistKlocki.CanvasWidth.ToString();

            List<int> visibleChannels = new List<int>();

            for (int i = 0; i < MixerViewModel.MixerModels.Count; i++)
            {
                if (MixerViewModel.MixerModels[i].IsTurn) visibleChannels.Add(i);
            }

            foreach (var item in loadedPlaylistKlocki.Klocki)
            {
                if (visibleChannels.Contains(item.ChannelNr))
                {
                    item.KlocekVisibility = Visibility.Visible;
                }
                else
                {
                    item.KlocekVisibility = Visibility.Hidden;
                }

                if (!item.IsChord)
                {
                    item.NoteBackground = NotesHelper.ChordColor[item.Name];
                    KlocekViewModel.Klocki.Add(item);
                }
                else
                {
                    item.ChordBackground = NotesHelper.ChordColor[item.ChordModel.ChordName];
                    item.ChordBorder = NotesHelper.ChordColorNoOpacity[item.ChordModel.ChordName];
                    foreach (var notInChord in item.NotesInChord)
                    {
                        notInChord.AssignBackground();
                    }


                    KlocekViewModel.Klocki.Add(item);
                }
            }
        }

        public static void UpdateChordsInPlaylist(
            KlocekChordViewModel KlocekViewModel, 
            NotesViewModel NotesViewModel,
            Stack<List<KlocekChordModel>> PlaylistRedoStack,
            Stack<List<KlocekChordModel>> PlaylistStack,
            bool selectedItemsOnly = true, 
            bool changeStrumPattern = true
            )
        {
            List<KlocekChordModel> selectedChords = null;

            if (selectedItemsOnly)
            {
                selectedChords = KlocekViewModel.SelectedItems.Where(a => a.IsChord).ToList();
            }
            else
            {
                selectedChords = KlocekViewModel.Klocki.Where(a => a.IsChord).ToList();
            }

            if (selectedChords.Any())
            {
                AddPlaylistActionToStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel);
            }

            foreach (var selectedChord in selectedChords)
            {
                StrumViewModel myStrumModel = null;
                if (changeStrumPattern)
                {
                    myStrumModel = NotesViewModel.GetStrumModels(selectedChord.NotesToPlay);
                }
                else
                {
                    myStrumModel = selectedChord.StrumViewModel;
                }

                double delayBpm = 60_000 / NotesViewModel.Bpm;
                var oct = selectedChord.ChordModel.NoteOctaves;

                Dictionary<string, KlocekNoteDetails> klocekNoteDetailsDict = new Dictionary<string, KlocekNoteDetails>();
                for (int i = 0; i < selectedChord.NotesToPlay.Count; i++)
                {
                    if (string.IsNullOrEmpty(selectedChord.NotesToPlay[i])) continue;

                    KlocekNoteDetails klocekNoteDetails = new KlocekNoteDetails()
                    {
                        Name = oct[i].Name,
                        Octave = int.Parse(oct[i].Octave),
                        Struna = oct[i].Struna,
                        Prog = oct[i].Prog,
                        Mp3Name = selectedChord.NotesToPlay[i]
                    };

                    klocekNoteDetailsDict.Add(selectedChord.NotesToPlay[i], klocekNoteDetails);
                }

                selectedChord.NotesInChord.Clear();
                double currentDelay = 0;
                int idx = 0;
                foreach (var item in myStrumModel.StrumPattern)
                {
                    currentDelay += item.DelayBeforeMs;
                    foreach (var strumItem in item.PlayedNotes)
                    {
                        var dictItem = klocekNoteDetailsDict[strumItem.Path];
                        KlocekNoteDetails klocekNoteDetails = new KlocekNoteDetails()
                        {
                            Name = dictItem.Name,
                            Octave = dictItem.Octave,
                            Struna = dictItem.Struna,
                            Prog = dictItem.Prog,
                            Mp3Name = dictItem.Mp3Name
                        };
                        klocekNoteDetails.AssignBackground();

                        if (idx != 0)
                        {
                            currentDelay += item.DelayMs;
                        }

                        klocekNoteDetails.XPos = currentDelay * NotesViewModel.BeatWidth / delayBpm;
                        klocekNoteDetails.YPos = KlocekChordViewModel.ItemHeight * (klocekNoteDetails.Struna - 1);
                        selectedChord.NotesInChord.Add(klocekNoteDetails);

                        idx++;
                    }
                }

                selectedChord.YPos = selectedChord.NotesInChord.Min(b => b.YPos);

                foreach (var item in selectedChord.NotesInChord)
                {
                    item.YPos -= selectedChord.YPos;
                }

                selectedChord.ChordWidth = selectedChord.NotesInChord.Max(c => c.XPos) - selectedChord.NotesInChord.Min(c => c.XPos) + KlocekChordViewModel.ItemWidth;
                selectedChord.ChordHeight = selectedChord.NotesInChord.Max(c => c.YPos) - selectedChord.NotesInChord.Min(c => c.YPos) + 20;

                selectedChord.StrumViewModel = myStrumModel;
            }
        }

        public static void AddPlaylistActionToStack(
            Stack<List<KlocekChordModel>> PlaylistRedoStack,
            Stack<List<KlocekChordModel>> PlaylistStack,
            KlocekChordViewModel KlocekViewModel
            )
        {
            PlaylistRedoStack.Clear();
            var cloneOfKlocki = KlocekViewModel.Klocki.Select(a => (KlocekChordModel)a.Clone()).ToList();

            PlaylistStack.Push(cloneOfKlocki);
        }

        public static void GetPlaylistActionFromStack(
            Stack<List<KlocekChordModel>> PlaylistRedoStack,
            Stack<List<KlocekChordModel>> PlaylistStack,
            KlocekChordViewModel KlocekViewModel,
            MixerViewModel MixerViewModel
            )
        {
            if (!PlaylistStack.Any()) return;

            var lastCloneOfKlocki = PlaylistStack.Pop();

            var cloneOfKlocki = KlocekViewModel.Klocki.Select(a => (KlocekChordModel)a.Clone()).ToList();
            PlaylistRedoStack.Push(cloneOfKlocki);

            KlocekViewModel.Klocki.Clear();


            List<int> visibleChannels = new List<int>();

            for (int i = 0; i < MixerViewModel.MixerModels.Count; i++)
            {
                if (MixerViewModel.MixerModels[i].IsTurn) visibleChannels.Add(i);
            }

            foreach (var item in lastCloneOfKlocki)
            {
                if (visibleChannels.Contains(item.ChannelNr))
                {
                    item.KlocekVisibility = Visibility.Visible;
                }
                else
                {
                    item.KlocekVisibility = Visibility.Hidden;
                }

                KlocekViewModel.Klocki.Add(item);
            }
        }

        public static void GetPlaylistActionFromRedo(
            Stack<List<KlocekChordModel>> PlaylistRedoStack,
            Stack<List<KlocekChordModel>> PlaylistStack,
            KlocekChordViewModel KlocekViewModel
            )
        {
            if (!PlaylistRedoStack.Any()) return;

            var lastCloneOfKlocki = PlaylistRedoStack.Pop();

            var cloneOfKlocki = KlocekViewModel.Klocki.Select(a => (KlocekChordModel)a.Clone()).ToList();
            PlaylistStack.Push(cloneOfKlocki);

            KlocekViewModel.Klocki.Clear();

            foreach (var item in lastCloneOfKlocki)
            {
                KlocekViewModel.Klocki.Add(item);
            }

        }
    }
}
