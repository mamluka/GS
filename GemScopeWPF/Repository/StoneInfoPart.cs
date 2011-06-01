using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GemScopeWPF.Repository
{
    public class StoneInfoPart
    {
        public string Title { get; set; }
        public string  Value { get; set; }

        public StoneInfoPart(string title, string value)
        {
            Title = title;
            Value = value;
        }
        public StoneInfoPart()
        {
        }
    }
}
