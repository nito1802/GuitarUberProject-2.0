using GitarUberProject.Helperes;

namespace GitarUberProject.Models
{
    public class StrumNoteDetails
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public long DelayMs { get; set; }
        public long PlayTime { get; set; } //jak dlugo ma grac
        public int Struna { get; set; } //aby dzwieki z tej samej struny nie nakladaly sie na siebie

        public StrumNoteDetails(string path, string name, int struna)
        {
            this.Path = path;
            this.Name = name;
            this.Struna = struna;
        }

        public override string ToString()
        {
            return $"{Name} {Path} Delay: {DelayMs} Struna: {Struna} Time: {PlayTime}";
        }
    }

    public class StrumModel : ICloneable
    {
        public StrumDirection StrumDir { get; set; }
        public long DelayMs { get; set; } //pomiedzy strunami
        public long DelayBeforeMs { get; set; } //jesli jest to kolejny strum z kolei
        public List<StrumNoteDetails> PlayedNotes { get; set; }
        public int TakeNotes { get; set; }
        public int SkipNotes { get; set; }

        public object Clone()
        {
            StrumModel clone = new StrumModel(StrumDir, DelayMs, DelayBeforeMs, TakeNotes, SkipNotes);

            return clone;
        }

        public StrumModel()
        {
        }

        public StrumModel(List<string> notesPaths, StrumDirection strumDir, long delayMs, long delayBeforeMs, int takeNotes = -1, int skipNotes = 0)
        {
            this.StrumDir = strumDir;
            this.DelayMs = delayMs;
            this.DelayBeforeMs = delayBeforeMs;
            this.TakeNotes = takeNotes;
            this.SkipNotes = skipNotes;

            if (StrumDir == StrumDirection.Downward)
            {
                PlayedNotes = notesPaths.AsEnumerable()
                                        .Reverse()
                                        .Skip(SkipNotes)
                                        .Take(TakeNotes)
                                        .Where(b => !string.IsNullOrEmpty(b))
                                        .Select(a => new StrumNoteDetails(a, $"{NotesViewModel.NotesDict[a].Name}{NotesViewModel.NotesDict[a].Octave}", NotesViewModel.NotesDict[a].Struna))
                                        .ToList();
            }
            else
            {
                PlayedNotes = notesPaths.Skip(SkipNotes)
                                        .Take(TakeNotes)
                                        .Where(b => !string.IsNullOrEmpty(b))
                                        .Select(a => new StrumNoteDetails(a, $"{NotesViewModel.NotesDict[a].Name}{NotesViewModel.NotesDict[a].Octave}", NotesViewModel.NotesDict[a].Struna))
                                        .ToList();
            }
        }

        public StrumModel(StrumDirection strumDir, long delayMs, long delayBeforeMs, int takeNotes = -1, int skipNotes = 0)
        {
            this.StrumDir = strumDir;
            this.DelayMs = delayMs;
            this.DelayBeforeMs = delayBeforeMs;
            this.TakeNotes = takeNotes;
            this.SkipNotes = skipNotes;
        }

        public void AssignPaths(List<string> notesPaths)
        {
            if (StrumDir == StrumDirection.Downward)
            {
                try
                {
                    PlayedNotes = notesPaths.AsEnumerable()
                                            .Reverse()
                                            .Skip(SkipNotes)
                                            .Take(TakeNotes)
                                            .Where(b => !string.IsNullOrEmpty(b))
                                            .Select(a => new StrumNoteDetails(a, $"{NotesViewModel.NotesDict[a].Name}{NotesViewModel.NotesDict[a].Octave}", NotesViewModel.NotesDict[a].Struna))
                                            .ToList();
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                try
                {
                    PlayedNotes = notesPaths.Skip(SkipNotes)
                                            .Take(TakeNotes)
                                            .Where(b => !string.IsNullOrEmpty(b))
                                            .Select(a => new StrumNoteDetails(a, $"{NotesViewModel.NotesDict[a].Name}{NotesViewModel.NotesDict[a].Octave}", NotesViewModel.NotesDict[a].Struna))
                                            .ToList();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}