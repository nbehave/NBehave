namespace NBehave.Gherkin
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
    }
}