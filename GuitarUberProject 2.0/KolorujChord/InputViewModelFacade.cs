using System.Text;

namespace EditChordsWindow
{
    public enum NotesOStates
    {
        None,
        X,
        O
    }

    public class InputViewModelFacade
    {
        public static int StrunyCount { get; } = 6;
        public static int ProgCount { get; } = 6;

        public string Name { get; set; }
        public InputModel[,] InputNotes { get; set; } = new InputModel[6, 6];
        public int Fr { get; set; }
        public NotesOStates[] NotesO { get; set; } = new NotesOStates[6];

        public InputViewModelFacade()
        {
            for (int i = 0; i < StrunyCount; i++)
            {
                for (int j = 0; j < ProgCount; j++)
                {
                    var inputModel = new InputModel();
                    inputModel.Struna = i + 1;
                    inputModel.Prog = j;
                    InputNotes[i, j] = inputModel;
                }
            }

            for (int i = 0; i < NotesO.Length; i++)
            {
                NotesO[i] = NotesOStates.None;
            }
        }

        public string GetNotesShape()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < StrunyCount; i++)
            {
                for (int j = 0; j < ProgCount; j++)
                {
                    sb.Append(InputNotes[i, j].GetCheckedFingerCode());
                }
                sb.AppendLine();
            }

            string res = sb.ToString();

            return res;
        }
    }
}

/*

      public string Name { get; set; }
        public List<InputModel> InputNotes { get; set; }
        public int Fr { get; set; }
        public bool[] NotesO { get; set; }
    }

     */