using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pal.Model
{
    public class Option
    {
        public string Title { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public Option()
        {
        }

        public Option(string title)
        {
            Title = title;
        }
    }

}
