namespace AudioMaker.Interfaces.Models.PlaySound
{
    public class PlaysoundStrumNoteDetails
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public long DelayMs { get; set; }
        public long PlayTime { get; set; } //jak dlugo ma grac
        //public int Struna { get; set; } //aby dzwieki z tej samej struny nie nakladaly sie na siebie
    }
}