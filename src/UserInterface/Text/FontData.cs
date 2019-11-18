using System.Collections.Generic;

namespace Larx.UserInterface.Text
{
    public class FontData
    {
        public string Family { get; set; }
        public int Buffer { get; set; }
        public int Size { get; set; }
        public Dictionary<char, int[]> Chars { get; set; }
    }
}