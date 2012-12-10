namespace NBehave.Internal.Gherkin
{
    public struct Token
    {
        public readonly string Content;
        public readonly LineInFile LineInFile;

        public Token(string content, LineInFile lineInFile)
        {
            LineInFile = lineInFile;
            Content = content;
        }

        public bool Equals(Token other)
        {
            return LineInFile.Line == other.LineInFile.Line && string.Equals(Content, other.Content);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Token && Equals((Token) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (LineInFile.GetHashCode() * 397) ^ (Content != null ? Content.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Token left, Token right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !left.Equals(right);
        }
    }
}