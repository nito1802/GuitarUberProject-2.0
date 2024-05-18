using GitarUberProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.ViewModels
{
    public class StrumViewModel
    {
        public List<StrumModel> StrumPattern { get; set; }

        public StrumViewModel()
        {
        }

        public StrumViewModel(List<StrumModel> strumPattern)
        {
            StrumPattern = strumPattern;
        }

        public void CalculateDelays()
        {
            //ustawienie delayow
            SetDelays();
            var allPlayedNotes = StrumPattern.SelectMany(a => a.PlayedNotes).ToList();

            //foreach (var item in allPlayedNotes)
            for (int i = 0; i < allPlayedNotes.Count; i++)
            {
                var nextNoteOnStruna = allPlayedNotes.Skip(i + 1).FirstOrDefault(a => a.Struna == allPlayedNotes[i].Struna);

                if (nextNoteOnStruna != null)
                {
                    allPlayedNotes[i].PlayTime = nextNoteOnStruna.DelayMs - allPlayedNotes[i].DelayMs;
                }
                else
                {
                    allPlayedNotes[i].PlayTime = -1;
                }
            }

            //foreach (var item in StrumPattern)
            //{
            //    if(item.StrumDir == StrumDirection.Downward)
            //    {
            //        for (int i = 0; i < item.PlayedNotes.Count; i++)
            //        {
            //            item.PlayedNotes[i].Struna = i + 1;
            //        }
            //    }
            //}
        }

        private void SetDelays()
        {
            long offsetMs = 0;

            foreach (var item in StrumPattern)
            {
                int counter = 0;

                foreach (var strumItem in item.PlayedNotes)
                {
                    long delayMs = item.DelayMs + offsetMs;

                    if (counter == 0)
                    {
                        delayMs = item.DelayBeforeMs + offsetMs;
                    }

                    strumItem.DelayMs = delayMs;
                    offsetMs = delayMs;

                    counter++;
                }
            }
        }
    }
}
