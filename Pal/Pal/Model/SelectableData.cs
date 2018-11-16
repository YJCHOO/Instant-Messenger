using System;
using System.Collections.Generic;
using System.Text;

namespace Pal.Model
{
    public class SelectableData<T>
    {
        public T Data { get; set; }
        public bool Selected { get; set; }

        public SelectableData(T data, bool selected)
        {
            Data = data;
            Selected = selected;
        }
    }
}
