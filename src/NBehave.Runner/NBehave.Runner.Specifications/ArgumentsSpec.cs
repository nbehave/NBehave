using System;
using NBehave.Spec.Framework;
using System.IO;

namespace NBehave.Runner.Console.Specifications
{
    [Context]
    public class ParseGoodArgumentsSpec
    {
        const string assemblyName = "NBehave.Runner.Console.exe";
        const string xmlFileName = "Bob.xml";
        const string txtFileName = "Bob.xml";

        const string assemblyArg = "/assembly:" + assemblyName;
        const string outputArg = "/output:" + xmlFileName;
        const string behaviourOutputArg = "/behaviourOutput:" + txtFileName;

        Arguments args;

        [SetUp]
        public void Setup()
        {
            string[] argArray = new string[] { assemblyArg, outputArg, behaviourOutputArg };
            args = Arguments.Parse( argArray );
        }

        [Specification]
        public void ShouldParseAssemblyFileName()
        {
            Specify.That( args.Assembly ).ShouldEqual( Path.GetFullPath( assemblyName ) );
        }

        [Specification]
        public void ShouldParseOutputFileName()
        {
            Specify.That( args.Output ).ShouldEqual( Path.GetFullPath( xmlFileName ) );
        }

        [Specification]
        public void ShouldParseBehaviourOutputTextFileName()
        {
            Specify.That( args.BehaviourOutput ).ShouldEqual( Path.GetFullPath( txtFileName ) );
        }
    }

    [Context]
    public class ParseBadArgumentsSpec
    {
        const string badArg = "badstring";
        Arguments args;

        [SetUp]
        public void Setup()
        {
            args = Arguments.Parse( new string[] { badArg } );
        }
    
        [Specification]
        public void ShouldReturnNull()
        {
            Specify.That( args ).ShouldBeNull();
        }
    }

    [Context]
    public class ParseNoArgumentsSpec
    {
        Arguments args;

        [SetUp]
        public void Setup()
        {
            string[] argArray = new string[0];
            args = Arguments.Parse( argArray );
        }

        [Specification]
        public void ShouldReturnNull()
        {
            Specify.That( args ).ShouldBeNull();
        }
    }

    [Context]
    public class ParseHelpRequestSpec
    {
        Arguments args;

        [SetUp]
        public void Setup()
        {
            string[] argArray = new string[] { "/?" };
            args = Arguments.Parse( argArray );
        }

        [Specification]
        public void ShouldReturnNull()
        {
            Specify.That( args ).ShouldBeNull();
        }
    }
}