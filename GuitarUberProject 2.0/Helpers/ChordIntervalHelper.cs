using EditChordsWindow;
using GitarUberProject.ViewModels;

namespace GitarUberProject.Helperes
{
    public static class ChordIntervalHelper
    {
        public static string[] AllNotes { get; set; } = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        public static NotesGroup GetNoteGroup(string lookingNote, List<NoteOctaveIntervalDetails> notes)
        {
            var filteredNotes = notes.Where(a => a.Note == lookingNote).ToList();

            foreach (var item in filteredNotes)
            {
                notes.Remove(item);
            }

            NotesGroup res = new NotesGroup(filteredNotes);
            return res;
        }

        private static void AssignIntervals(List<int> intervals, ChordIntervalsDetailsViewModel chordIntervalDetails)
        {
            var firstFromGroups = chordIntervalDetails.NotesGroup.Select(a => a.Notes.First()).ToList();

            if (firstFromGroups.Count < 2) return; //jesli nie ma min 2 elementow, nie ma interwalu

            for (int i = 0; i < intervals.Count; i++)
            {
                firstFromGroups[i].Interval = intervals[i].ToString();
            }
        }

        public static List<int> GetIntervalsFromNotes(string rootNote, List<string> notes)
        {
            var processedNotes = notes.Distinct().ToList();
            processedNotes.Remove(rootNote);

            if (!processedNotes.Any()) return new List<int>();

            int idx = Array.IndexOf(AllNotes, rootNote);
            //List<int> res = new List<int> { 1 };
            List<int> res = new List<int>();

            int intervalCounter = 0;
            for (int i = idx; ; i++, intervalCounter++)
            {
                if (i == AllNotes.Length)
                    i = 0;

                string currentNote = AllNotes[i];

                if (processedNotes.Contains(currentNote))
                {
                    res.Add(intervalCounter);
                    processedNotes.Remove(currentNote);

                    if (processedNotes.Count == 0) break;
                }
            }

            return res;
        }

        private static int GetOffsetedIntervalNote(int rootIdx, int noteIdx)
        {
            int res = -1;

            int wholeIdx = rootIdx + noteIdx;

            if (wholeIdx >= AllNotes.Length)
            {
                res = wholeIdx - AllNotes.Length;
            }
            else
            {
                res = rootIdx + noteIdx;
            }

            return res;
        }

        public static List<string> GetNotesFromIntervals(string rootNote, List<int> intervals)
        {
            var processedIntervals = intervals
                                    .Distinct()
                                    .OrderBy(a => a)
                                    .ToList();
            //processedIntervals.Remove(1);

            if (!processedIntervals.Any()) return new List<string>();

            List<string> res = new List<string>() { rootNote };

            int idx = Array.IndexOf(AllNotes, rootNote);

            var offsetedIntervals = processedIntervals.Select(a => GetOffsetedIntervalNote(idx, a)).ToList();

            foreach (var item in offsetedIntervals)
            {
                string note = AllNotes[item];
                res.Add(note);
            }

            return res;
        }

        public static ChordIntervalsDetailsViewModel ConvertIntoChordIntervalDetails(string rootNote, string chordType, List<NoteOctaveIntervalDetails> notes)
        {
            ChordIntervalsDetailsViewModel result = new ChordIntervalsDetailsViewModel() { RootNoteName = rootNote, ChordType = chordType };

            var processedNotes = notes.Select(a => a.Note).Distinct().ToList();

            if (!AllNotes.Contains(rootNote))
            {
                rootNote = processedNotes.First();
            }

            var keyGroup = GetNoteGroup(rootNote, notes);
            if (keyGroup.Notes.Any()) result.NotesGroup.Add(keyGroup);

            processedNotes.Remove(rootNote);

            if (processedNotes.Count == 0) return result;

            int idx = Array.IndexOf(AllNotes, rootNote);
            List<int> intervals = new List<int> { };

            int intervalCounter = 0;
            for (int i = idx; ; i++, intervalCounter++)
            {
                if (i == AllNotes.Length)
                    i = 0;

                string currentNote = AllNotes[i];

                if (processedNotes.Contains(currentNote))
                {
                    intervals.Add(intervalCounter);
                    processedNotes.Remove(currentNote);

                    var group = GetNoteGroup(currentNote, notes);
                    result.NotesGroup.Add(group);

                    if (processedNotes.Count == 0) break;
                }
            }

            AssignIntervals(intervals, result);

            return result;
        }

        public static string ConvertChord(NotesViewModelLiteVersion chord)
        {
            if (chord.ChordType == "Major")
                return chord.ChordName;
            else if (chord.ChordType == "moll")
            {
                return $"{chord.ChordName}m";
            }

            return string.Empty;
        }
    }
}