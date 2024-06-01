using AudioMaker.Interfaces.Models.MidiExport;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using NoteMy = Melanchall.DryWetMidi.Interaction.Note;

namespace GitarUberProject.Services
{
    public static class MidiService
    {
        public static void ExportToMidiFile(string outputPath, PlaylistKlocekChordViewModel klocekViewModel)
        {
            int mnoznik = 2;
            long NOTE_LENGTH = 192 * mnoznik;
            long TIME_STEP = 192 * mnoznik;

            var chordsInPlaylist = klocekViewModel.Klocki.Where(a => a.IsChord).OrderBy(b => b.XPos).ToList();
            var chorNames = chordsInPlaylist.Select(a => a.ChordName).ToList();

            List<NoteMy> midiNotes = new List<NoteMy>();

            for (int i = 0; i < chordsInPlaylist.Count; i++)
            {
                var chord = chordsInPlaylist[i];

                foreach (var item in chord.NotesInChord)
                {
                    NoteName noteName = ConvertStringNoteNameToEnum(item.Name);

                    NoteMy midiNote = new NoteMy(noteName, item.Octave - 1, NOTE_LENGTH, TIME_STEP * i);
                    midiNotes.Add(midiNote);
                }
            }

            var midiFile = midiNotes.ToFile();
            midiFile.Write(outputPath, overwriteFile: true);
        }

        private static NoteName ConvertStringNoteNameToEnum(string noteName)
        {
            NoteName res = NoteName.C;

            switch (noteName)
            {
                case "C":
                    res = NoteName.C;
                    break;

                case "C#":
                    res = NoteName.CSharp;
                    break;

                case "D":
                    res = NoteName.D;
                    break;

                case "D#":
                    res = NoteName.DSharp;
                    break;

                case "E":
                    res = NoteName.E;
                    break;

                case "F":
                    res = NoteName.F;
                    break;

                case "F#":
                    res = NoteName.FSharp;
                    break;

                case "G":
                    res = NoteName.G;
                    break;

                case "G#":
                    res = NoteName.GSharp;
                    break;

                case "A":
                    res = NoteName.A;
                    break;

                case "A#":
                    res = NoteName.ASharp;
                    break;

                case "B":
                    res = NoteName.B;
                    break;

                default:
                    break;
            }

            return res;
        }
    }
}