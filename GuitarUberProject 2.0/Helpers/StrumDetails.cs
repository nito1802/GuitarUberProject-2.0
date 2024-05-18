using System.Collections.Generic;
using System.Linq;

namespace GitarUberProject.Helperes
{
    public enum StrumDirection
    {
        Downward, Upward, None
    }

    public class StrumDetails
    {
        public long TookMs { get; set; }
        public long DelayBeforeMs { get; set; }
        public StrumDirection StrumDir { get; set; }
        public List<NotesInStrum> Notes { get; set; }

        public StrumDetails(long tookMs, long delayBeforeMs, StrumDirection strumDir, List<NotesInStrum> notes)
        {
            TookMs = tookMs;
            DelayBeforeMs = delayBeforeMs;
            StrumDir = strumDir;
            Notes = notes;
        }

        public override string ToString()
        {
            return $"Before: {DelayBeforeMs}ms Took: {TookMs}ms  {StrumDir} || {string.Join(", ", Notes.Select(a => a))}";
        }
    }
}