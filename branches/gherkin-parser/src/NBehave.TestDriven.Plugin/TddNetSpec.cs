using System;

namespace NBehave.TestDriven.Plugin
{
    public class TddNetSpec
    {
        public static string Story = "Story: Testdriven.NET Support" + Environment.NewLine +
                                     "  As a fan of NBehave" + Environment.NewLine +
                                     "  I Want NBehave to work with Testdriven.NET" + Environment.NewLine +
                                     "  So that I get less friction developing code using NBehave" + Environment.NewLine +
                                     "Scenario: VS integration - right click method" + Environment.NewLine +
                                     //.Pending("Pending test")
                                     "  Given A story in a C# source file" + Environment.NewLine +
                                     "    And User have right clicked on a method in code window" + Environment.NewLine +
                                     "    And Left clicked 'Run Test(s)'" + Environment.NewLine +
                                     "  When Testdriven.Net runs story" + Environment.NewLine +
                                     "  Then NBehave framework should be invoked" + Environment.NewLine +
                                     "    And Testdriven.Net should be notified of the result" + Environment.NewLine +
                                     //
                                     "Scenario VS integration - right click .cs file" + Environment.NewLine +
                                     "  Given A story in a C# source file" + Environment.NewLine +
                                     "    And User have right clicked on file" + Environment.NewLine +
                                     "    And Left clicked 'Run Test(s)'" + Environment.NewLine +
                                     "  When Testdriven.Net runs story" + Environment.NewLine +
                                     "  Then NBehave framework should be invoked" + Environment.NewLine +
                                     "    And Testdriven.Net should be notified of the result";

        public static string SomeOtherStory = "Story: Testdriven.NET Support second story" + Environment.NewLine +
                                              "  As an addict of NBehave" + Environment.NewLine +
                                              "  I want NBehave to work with Testdriven.NET" + Environment.NewLine +
                                              "  So that I get my fix" + Environment.NewLine +
                                              "Scenario: VS integration - something" + Environment.NewLine +
                                              "  Given A story in a C# source file" + Environment.NewLine +
                                              "  When Testdriven.Net runs story" + Environment.NewLine +
                                              "  Then NBehave framework should be invoked" + Environment.NewLine +
                                              "    And Testdriven.Net should be notified of the result";
    }
}