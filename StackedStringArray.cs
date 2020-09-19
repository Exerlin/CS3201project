using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

namespace Covid19Analysis
{
    public class StackedStringArray
    {

        private readonly List<List<string>> theListOfLists;

        public StackedStringArray()
        {
            this.theListOfLists = new List<List<string>>();
        }

        public List<List<string>> getListOfList()
        {
            return this.theListOfLists;
        }

        public void addListToList(List<string> stringToAdd)
        {
            this.theListOfLists.Add(stringToAdd);
        }

        public string getString(int listIndex, int valueIndex)
        {
            return this.theListOfLists.ElementAt(listIndex).ElementAt(valueIndex);
        }

    }
}
