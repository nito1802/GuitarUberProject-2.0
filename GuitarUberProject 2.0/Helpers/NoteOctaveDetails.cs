using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GitarUberProject.Helpers
{
    public class NoteOctaveDetails : ICloneable
    {
        private string name;

        public NoteOctaveDetails()
        { }

        public NoteOctaveDetails(string name, string octave)
        {
            Name = name;
            Octave = octave;

            if (!string.IsNullOrEmpty(name))
                NoteColor = NotesHelper.ChordColorNoOpacity[name];
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (!string.IsNullOrEmpty(name))
                    NoteColor = NotesHelper.ChordColorNoOpacity[name];
            }
        }

        public string Octave { get; set; }
        public int Struna { get; set; }
        public int Prog { get; set; }

        [JsonIgnore]
        public Brush NoteColor { get; set; }

        public object Clone()
        {
            NoteOctaveDetails clone = new NoteOctaveDetails(this.name, this.Octave);
            clone.NoteColor = this.NoteColor;
            clone.Prog = this.Prog;
            clone.Struna = this.Struna;

            return clone;
        }

        public override string ToString()
        {
            return $"{Name}{Octave} EditChordsWindow.NoteOctaveDetails";
        }
    }
}
