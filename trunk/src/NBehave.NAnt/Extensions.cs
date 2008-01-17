namespace NBehave.NAnt
{
    public static class Extensions
    {
        public static bool Blank(this string value)
        {
            return value == null ? true : string.IsNullOrEmpty(value.Trim());
        }
    }
}
