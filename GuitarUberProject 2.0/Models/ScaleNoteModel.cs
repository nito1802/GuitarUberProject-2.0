namespace GitarUberProject.Models
{
    public class ScaleNoteModel
    {
        public string Note { get; set; }
        public string Step { get; set; }
        public int Indeks { get; set; }

        public ScaleNoteModel(string note, string step, int indeks)
        {
            Note = note;
            Step = step;
            Indeks = indeks;
        }

        public override string ToString()
        {
            return $"{Note} {Step}";
        }
    }
}