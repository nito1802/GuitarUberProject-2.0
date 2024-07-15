using EditChordsWindow;
using System.Windows;
using System.Windows.Media;

namespace GitarUberProject
{
    public static class NotesHelper
    {
        public static string[] AllNotes { get; set; } = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        public static SolidColorBrush GlobalStrunaBrush { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom("#72D8970B"));
        public static SolidColorBrush GlobalStrunaBrushInactive { get; set; } = (SolidColorBrush)(new BrushConverter().ConvertFrom("#72C40811"));

        public static Dictionary<int, int> OctaveToFontSizeDict = new Dictionary<int, int>
        {
            {2, 23 },
            {3, 25 },
            {4, 28 },
            {5, 30 },
            {6, 32 },
        };

        public static Dictionary<string, SolidColorBrush> NotesColor = new Dictionary<string, SolidColorBrush>
        {
                {"C", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFA285F0")) },
                {"C#", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF7947FF")) },
                {"D", (SolidColorBrush)(new BrushConverter().ConvertFrom("#00A0E8")) },
                {"D#", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF09709E")) },
                {"E", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0A910A")) },
                {"F", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFEC1C1C")) },
                {"F#", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFA40E0E")) },
                {"G", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF94790")) },
                {"G#", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB61473")) },
                {"A", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF3C31E")) },
                {"A#", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFA47004")) },
                {"B", (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFAAAEAD")) },
        };

        public static Dictionary<string, LinearGradientBrush> ChordColor = new Dictionary<string, LinearGradientBrush>
        {
                {"C", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#99A285F0"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#99774FE4"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"C#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#997947FF"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#996939EC"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"D", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#9900A0E8"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#990E8CC5"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"D#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#9909709E"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#9908577A"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"E", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#9906C106"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#990A910A"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"F", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#99EC1C1C"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#99C50E0E"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"F#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#99A40E0E"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#99810606"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"G", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#99E64286"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#99D41F6A"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"G#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#99B61473"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#99A40864"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"A", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#99F3C31E"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#99D1A616"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"A#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#999E6C04"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#99856012"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"B", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#99CFCFCF"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#998F8F8F"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
        };

        public static Dictionary<string, LinearGradientBrush> ChordColorNoOpacity = new Dictionary<string, LinearGradientBrush>
        {
                {"C", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FFA285F0"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF774FE4"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"C#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FF7947FF"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF6939EC"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"D", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FF00A0E8"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF0E8CC5"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"D#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FF09709E"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF08577A"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"E", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FF06C106"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF0A910A"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"F", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FFEC1C1C"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFC50E0E"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"F#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FFA40E0E"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF810606"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"G", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FFE64286"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFD41F6A"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"G#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FFB61473"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFA40864"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"A", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FFF3C31E"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFD1A616"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"A#", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FF9E6C04"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF856012"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
                {"B", new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FFCFCFCF"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF8F8F8F"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
        };

        public static Dictionary<CheckedFinger, Brush> FingerBrushes = new Dictionary<CheckedFinger, Brush>()
        {
            {CheckedFinger.None, Brushes.Transparent },
            {CheckedFinger.firstFinger, new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#E29A05"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFEBA619"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
            {CheckedFinger.secondFinger,new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#B71DE9"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFA50CD6"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
            {CheckedFinger.thirdFinger,new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color) ColorConverter.ConvertFromString("#00A0E8"), 0),  new GradientStop((Color) ColorConverter.ConvertFromString("#FF0784BD"), 1) }), new Point(0.5, 0), new Point(0.5, 1))},
            {CheckedFinger.fourthFinger,new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#E16449"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFDB5639"), 1) }), new Point(0.5, 0), new Point(0.5, 1))},
            {CheckedFinger.Other,new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FF036C50"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF07946E"), 1) }), new Point(0.5, 0), new Point(0.5, 1))}
        };

        public static Dictionary<CheckedFinger, Brush> EditStrumBrushes = new Dictionary<CheckedFinger, Brush>()
        {
            {CheckedFinger.None, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCCCC")) },
            {CheckedFinger.firstFinger, new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#E29A05"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFEBA619"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
            {CheckedFinger.secondFinger,new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#B71DE9"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFA50CD6"), 1) }), new Point(0.5, 0), new Point(0.5, 1)) },
        };

        public static string[] IntervalNames = new string[]
            {
                "Pryma",
                "Sekunda mała",
                "Sekunda wielka",
                "Tercja mała",
                "Tercja wielka",
                "Kwarta czysta",
                "Tryton",
                "Kwinta czysta",
                "Seksta mała",
                "Seksta wielka",
                "Septyma mała",
                "Septyma wielka",
                "Oktawa"
            };

        public static LiteNote NextNote(LiteNote startNote)
        {
            int startNoteIdx = Array.IndexOf(AllNotes, startNote.Name);

            LiteNote resNote = null;

            if (startNoteIdx != AllNotes.Length - 1)
            {
                resNote = new LiteNote(AllNotes[startNoteIdx + 1], startNote.Octave);
            }
            else
            {
                resNote = new LiteNote(AllNotes[0], startNote.Octave + 1);
            }

            return resNote;
        }

        public static NotesViewModelLiteVersion InputFacadeToViewModelLite(InputViewModelFacade inputViewModelFacade)
        {
            NotesViewModelLiteVersion res = new NotesViewModelLiteVersion();
            res.InitNotes();

            int counter = 0;
            for (int i = 0; i < InputViewModelFacade.StrunyCount; i++)
            {
                for (int j = 0; j < InputViewModelFacade.ProgCount; j++)
                {
                    NoteModelLiteVersion noteModel = new NoteModelLiteVersion(inputViewModelFacade.InputNotes[i, j].Struna, inputViewModelFacade.InputNotes[i, j].Prog);
                    noteModel.CheckedFinger = inputViewModelFacade.InputNotes[i, j].CheckedFingerProp;

                    if (noteModel.CheckedFinger != CheckedFinger.None)
                    {
                    }

                    res.Notes[counter] = noteModel;

                    counter++;
                }
            }

            res.Fr = inputViewModelFacade.Fr;

            for (int i = 0; i < inputViewModelFacade.NotesO.Length; i++)
            {
                switch (inputViewModelFacade.NotesO[i])
                {
                    case NotesOStates.None:
                        res.NotesO[i] = "";
                        break;

                    case NotesOStates.X:
                        res.NotesO[i] = "X";
                        break;

                    case NotesOStates.O:
                        res.NotesO[i] = "O";
                        break;

                    default:
                        break;
                }
            }

            res.UpdateAllStrunsNames();
            //res.ChordIntervalsNotes

            if (res.ChordIntervalsNotes != null)
                res.ChordCode = string.Join("-", res.ChordIntervalsNotes.OrderBy(b => b).Distinct().ToList());
            else
                res.ChordCode = string.Join("-", res.NoteOctaves.Where(c => !string.IsNullOrEmpty(c.Name)).Select(ax => ax.Name).OrderBy(b => b).Distinct().ToList());

            res.ChordCodeNormalized = string.Join("-", res.Notes.Where(a => a.CheckedFinger != CheckedFinger.None).Select(ax => $"s{ax.Struna}p{ax.Prog}").OrderBy(b => b).Distinct().ToList());

            if (MainWindow.CheckedFingerDict.ContainsKey(res.ChordCodeNormalized))
            {
                var ukladFingers = MainWindow.CheckedFingerDict[res.ChordCodeNormalized];
                for (int i = 0; i < ukladFingers.Count; i++)
                {
                    res.Notes[i].CheckedFinger = ukladFingers[i];
                }
            }

            return res;
        }

        public static Dictionary<string, int> GetAllNotesFromGuitar()
        {
            LiteNote startNote = new LiteNote("E", 3);
            LiteNote endNote = new LiteNote("F#", 6);
            Dictionary<string, int> res = new Dictionary<string, int>(); //name note, and idx

            int idx = Array.IndexOf(AllNotes, startNote.Name);
            int octave = startNote.Octave;
            int counter = 0;

            while (true)
            {
                var note = new LiteNote(AllNotes[idx], octave);
                string key = $"{AllNotes[idx]}{octave}";
                res.Add(key, counter++);

                if (note.Name == endNote.Name && note.Octave == endNote.Octave)
                {
                    break;
                }

                if (idx == (AllNotes.Length - 1))
                {
                    idx = 0;
                    octave++;
                }
                else idx++;
            }

            return res;
        }
    }

    internal static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }

    public class LiteNote
    {
        public LiteNote(string name, int octave)
        {
            Name = name;
            Octave = octave;
        }

        public string Name { get; set; }
        public int Octave { get; set; }

        public override string ToString()
        {
            return $"{Name}{Octave}";
        }
    }

    public class MediumNote
    {
        public string Name { get; set; }
        public int Octave { get; set; }
        public int Prog { get; set; }
        public int Struna { get; set; }

        public MediumNote(string name, int octave, int prog, int struna)
        {
            Name = name;
            Octave = octave;
            Prog = prog;
            Struna = struna;
        }

        public override string ToString()
        {
            return $"{Name}{Octave} P{Prog}_S{Struna}";
        }
    }
}