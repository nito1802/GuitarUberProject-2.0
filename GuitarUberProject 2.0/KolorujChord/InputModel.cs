namespace EditChordsWindow
{
    public class InputModel
    {
        public CheckedFinger CheckedFingerProp { get; set; }
        public int Struna { get; set; }
        public int Prog { get; set; }

        public override string ToString()
        {
            return $"{CheckedFingerProp} Struna: {Struna} Prog: {Prog}";
        }

        public char GetCheckedFingerCode()
        {
            char res = 'O';

            switch (CheckedFingerProp)
            {
                case CheckedFinger.None:
                    res = 'O';
                    break;

                case CheckedFinger.firstFinger:
                    res = '1';
                    break;

                case CheckedFinger.secondFinger:
                    res = '2';
                    break;

                case CheckedFinger.thirdFinger:
                    res = '3';
                    break;

                case CheckedFinger.fourthFinger:
                    res = '4';
                    break;

                case CheckedFinger.Other:
                    res = '5';
                    break;

                default:
                    break;
            }

            return res;
        }
    }
}