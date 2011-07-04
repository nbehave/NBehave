namespace NBehave.ReSharper.Plugin.UnitTestProvider
{
    /// <summary>
    /// This is a hack to circumvent R# sorting nodes
    /// </summary>
    public static class IdentityGenerator
    {
        private static int _id;

        public static int NextValue()
        {
            _id++;
            return _id;
        }

        public static void Reset()
        {
            _id = 0;
        }
    }
}