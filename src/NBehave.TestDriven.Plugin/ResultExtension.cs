using System;

using TestDriven.Framework;

namespace NBehave.TestDriven.Plugin
{
    static class ResultExtension
    {
        public static TestState ToTestState(this Result result)
        {
            if (result is Passed)
                return TestState.Passed;
            if (result is Failed)
                return TestState.Failed;
            if (result is Pending)
                return TestState.Ignored;

            throw new ArgumentException("Unable to translate Result " + result + " into a TestState", "result");
        }
    }
}
