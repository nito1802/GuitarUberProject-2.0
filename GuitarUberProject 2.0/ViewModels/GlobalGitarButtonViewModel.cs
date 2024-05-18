using GitarUberProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.ViewModels
{
    public class GlobalGitarButtonViewModel
    {
        public List<GlobalGitarButtonModel> GlobalButtons { get; set; } = new List<GlobalGitarButtonModel>();

        public GlobalGitarButtonViewModel()
        {
            for (int i = 0; i < 6; i++)
            {
                GlobalButtons.Add(new GlobalGitarButtonModel(i + 1));
            }
        }

    }
}
