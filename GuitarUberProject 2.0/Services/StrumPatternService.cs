using EditChordsWindow;
using GitarUberProject.EditStrumWindow;
using GitarUberProject.Helperes;
using GitarUberProject.Models;
using GitarUberProject.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace GitarUberProject.Services
{
    public static class StrumPatternService
    {
        public static void CreateStrumPattern(
            bool IsStrumming, 
            Stopwatch SwStrumPatternKeys, 
            List<StrumKeysModel> StrumKeys, 
            NotesViewModel NotesViewModel,
            EditStrumViewModel EditStrumViewModel,
            ToViewStrumViewModels ToViewStrumViewModels,
            DispatcherTimer StrumTimer,
            Stopwatch SwCurrentStrumGlobal,
            Stopwatch SwSingleNote,
            List<string> Bords,
            List<StrumDetails> Strums,
            List<NotesInStrum> NotesInStrum
            )
        {
            if (IsStrumming)
            {
                StrumStop(
                    StrumTimer,
                    SwCurrentStrumGlobal,
                    SwSingleNote,
                    Bords,
                    IsStrumming,
                    NotesViewModel,
                    Strums,
                    NotesInStrum
                    );
            }

            SwStrumPatternKeys.Stop();

            List<EditStrumModel> keyStrumsEdit = new List<EditStrumModel>();

            for (int i = 0; i < StrumKeys.Count; i++)
            {
                var item = StrumKeys[i];

                EditStrumModel keyStrum = new EditStrumModel();

                if (item.Direction == EditChord.StrumDirection.Downward)
                {
                    keyStrum.Notes[0].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[1].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[2].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[3].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[4].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[5].CheckedNote = CheckedFinger.secondFinger;
                }
                else
                {
                    keyStrum.Notes[0].CheckedNote = CheckedFinger.secondFinger;
                    keyStrum.Notes[1].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[2].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[3].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[4].CheckedNote = CheckedFinger.firstFinger;
                    keyStrum.Notes[5].CheckedNote = CheckedFinger.firstFinger;
                }

                keyStrum.DelayBetweenStrunaMs = item.PlayTime;

                if (i != 0)
                {
                    keyStrum.DelayBeforeMs = StrumKeys[i].MsTimestamp - StrumKeys[i - 1].MsTimestamp;
                }

                keyStrumsEdit.Add(keyStrum);
            }

            EditStrumViewModel.AssignStrumPattern(NotesViewModel.GlobalEditStrumModels.Any() ? NotesViewModel.GlobalEditStrumModels : keyStrumsEdit);
            EditStrumView editStrumView = new EditStrumView();
            editStrumView.DataContext = EditStrumViewModel;
            editStrumView.Owner = System.Windows.Application.Current.MainWindow;
            editStrumView.Left = 1150 * App.CustomScaleX;
            editStrumView.Top = 200 * App.CustomScaleY;

            editStrumView.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            editStrumView.Width *= App.CustomScaleX;
            editStrumView.Height *= App.CustomScaleY;

            NotesViewModelLiteVersion.MainWindowBlackAction?.Invoke(true);
            editStrumView.ShowDialog();
            NotesViewModelLiteVersion.MainWindowBlackAction?.Invoke(false);

            if (editStrumView.ResultDialog == true)
            {
                NotesViewModel.GlobalEditStrumModels = EditStrumViewModel.EditStrumModels.ToList();
                NotesViewModel.GlobalStrumPattern = NotesViewModel.ConvertEditStrumModelsToStrumPattern(NotesViewModel.GlobalEditStrumModels);

                ToViewStrumViewModels.AddStrumModel(NotesViewModel.GlobalEditStrumModels);
            }
        }

        public static void StrumStop(
            DispatcherTimer StrumTimer,
            Stopwatch SwCurrentStrumGlobal,
            Stopwatch SwSingleNote,
            List<string> Bords,
            bool IsStrumming,
            NotesViewModel NotesViewModel,
            List<StrumDetails> Strums,
            List<NotesInStrum> NotesInStrum
            )
        {
            StrumTimer.Stop();
            SwCurrentStrumGlobal.Stop();
            SwSingleNote.Stop();
            Bords.Clear();
            if (!IsStrumming) return;
            IsStrumming = false;

            if (NotesViewModel.GlobalEditStrumModels == null) NotesViewModel.GlobalEditStrumModels = new List<EditStrumModel>();
            else NotesViewModel.GlobalEditStrumModels.Clear();

            int idx = 0;
            foreach (var item in Strums)
            {
                EditStrumModel editStrumModel = new EditStrumModel();
                editStrumModel.DelayBeforeMs = idx != 0 ? item.DelayBeforeMs : 0;
                editStrumModel.DelayBetweenStrunaMs = item.TookMs;

                if (item.StrumDir != StrumDirection.None)
                {
                    int minStruna = item.Notes.Min(a => a.StrunaNr);
                    int maxStruna = item.Notes.Max(a => a.StrunaNr);

                    for (int i = minStruna - 1; i < maxStruna; i++)
                    {
                        editStrumModel.Notes[i].CheckedNote = CheckedFinger.firstFinger;
                    }

                    if (item.StrumDir == StrumDirection.Downward)
                        editStrumModel.Notes[minStruna - 1].CheckedNote = CheckedFinger.secondFinger;
                    else
                        editStrumModel.Notes[maxStruna - 1].CheckedNote = CheckedFinger.secondFinger;
                }
                else
                {

                }


                NotesViewModel.GlobalEditStrumModels.Add(editStrumModel);
                idx++;
            }



            Strums.Clear();
            NotesInStrum.Clear();

            NotesViewModel.GlobalStrumPattern = NotesViewModel.ConvertEditStrumModelsToStrumPattern(NotesViewModel.GlobalEditStrumModels);
        }
    }
}
