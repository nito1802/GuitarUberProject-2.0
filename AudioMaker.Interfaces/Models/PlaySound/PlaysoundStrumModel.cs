namespace AudioMaker.Interfaces.Models.PlaySound
{
    public class PlaysoundStrumModel
    {
        //public StrumDirection StrumDir { get; set; }
        //public long DelayMs { get; set; } //pomiedzy strunami
        //public long DelayBeforeMs { get; set; } //jesli jest to kolejny strum z kolei
        public List<PlaysoundStrumNoteDetails> PlayedNotes { get; set; }

        //public int TakeNotes { get; set; }
        //public int SkipNotes { get; set; }
    }
}