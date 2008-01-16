using System;

namespace NBehave.Narrator.Framework
{
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