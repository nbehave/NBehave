Files:
Gherkin\Simple.feature - A very simple feature file
Gherkin\Simple.cs - Step implementations for steps in the above feature file

FluentExample.cs - contains example(s) of how to use the fluent example
SpecSample.cs - contains a few examples of extension methods you can use to assert outcomes.

The Spec samples are based on nunit but you may use xunit, mbunit or mstest if you want. choose the one that suits you best.
The same is true for the fluent samples.

To run the features, just go to the same package manager console where you installed the samples and run.
.\packages\nbehave.0.1.0.0\tools\net40\NBehave-Console.exe .\bin\debug\YourTestProjectAssembly.dll /sf=Gherkin\*.feature

You will have to change 0.1.0.0 to the version of nbehave you installed, and you will have to change .\bin\debug\YourTestProjectAssembly.dll to point to the dll that will be created when you compile the samples.
If you dont want to run from the command line install the Visual Studio and/or R# packages.

For more info see http://nbehave.org/

