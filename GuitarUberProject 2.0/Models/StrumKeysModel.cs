using GitarUberProject.EditChord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.Models
{
    public class StrumKeysModel
    {
        public StrumDirection Direction { get; set; }
        public int DelayMsBefore { get; set; }
        public int PlayTime { get; set; } = 15; //jak dlugo ma grac
        public long MsTimestamp { get; set; }

        public StrumKeysModel(StrumDirection direction, long msTimestamp)
        {
            Direction = direction;
            MsTimestamp = msTimestamp;
        }

        public override string ToString()
        {
            return $"{Direction } TS: {MsTimestamp} DelayBefore: {DelayMsBefore} Time: {PlayTime}";

        }
    }
}
