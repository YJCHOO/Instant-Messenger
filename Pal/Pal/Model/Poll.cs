using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pal.Model
{
    public class Poll
    {
        public string Title { get; set; }
        public ObservableCollection<Option> _Option { get; set; }

    }
}
