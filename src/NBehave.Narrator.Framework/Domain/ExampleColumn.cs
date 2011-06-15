using System;

namespace NBehave.Narrator.Framework
{
    public class ExampleColumn : IEquatable<ExampleColumn>
    {
        public string Name { get; private set; }

        public ExampleColumn(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            Name = name;
        }

        public bool Equals(ExampleColumn other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ExampleColumn)) return false;
            return Equals((ExampleColumn) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(ExampleColumn left, ExampleColumn right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ExampleColumn left, ExampleColumn right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}