namespace GitarUberProject.Helperes
{
    public class NotesInStrum
    {
        public int StrunaNr { get; set; }
        public long DelayBeforeMs { get; set; }

        public NotesInStrum(int strunaNr, long delayBeforeMs)
        {
            StrunaNr = strunaNr;
            DelayBeforeMs = delayBeforeMs;
        }

        public override string ToString()
        {
            return $"({DelayBeforeMs})ms {StrunaNr}";
        }
    }
}