using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
    public class ActionMatch
    {
        public Regex ActionStepMatcher { get; set; }

        public List<string> GetParameterNames()
        {
            var names = new List<string>();
            int index = 0;
            string name = ".";
            Regex regex = ActionStepMatcher;
            while (string.IsNullOrEmpty(name) == false)
            {
                name = regex.GroupNameFromNumber(index);
                if (string.IsNullOrEmpty(name) == false && name != index.ToString())
                    names.Add(name);
                index++;
            }
            return names;
        }
    }
}