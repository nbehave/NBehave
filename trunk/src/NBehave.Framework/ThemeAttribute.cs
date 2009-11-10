using System;

namespace NBehave.Narrator.Framework
{
	[Obsolete("You should switch to text scenarios, read more here http://nbehave.codeplex.com/wikipage?title=With%20textfiles%20and%20ActionSteps&referringTitle=Examples")]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ThemeAttribute : Attribute
    {
        private readonly string _name;

        public ThemeAttribute() : this(null) {}

        public ThemeAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}