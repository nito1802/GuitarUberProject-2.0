namespace AudioMaker.Interfaces.Models.MidiExport
{
    public class PlaylistKlocekChordModel
    {
        public double XPos { get; set; }
        //public double YPos { get; set; }
        //public double ZIndex { get; set; }
        //public double KlocekOpacity = 1;
        //public double ChordWidth;
        //public double ChordHeight;

        //private Brush chordBackground;
        //private Brush chordBorder;
        public string ChordName { get; set; }

        //private Visibility klocekVisibility;
        //public int ChannelNr;

        //public NotesViewModelLiteVersion ChordModel { get; set; }
        public List<PlaylistKlocekNoteDetails> NotesInChord { get; set; }

        //public string Name { get; set; }
        //public int Octave { get; set; }
        //public int Prog { get; set; }
        //public int Struna { get; set; }
        //public string Mp3Name { get; set; }
        public bool IsChord { get; set; }

        //public StrumViewModel StrumViewModel { get; set; }
        //public List<string> NotesToPlay { get; set; }
    }
}