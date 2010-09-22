using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using NBehave.Runner.Console;
using NBehave.Spec.Framework;


namespace NBehave.Runner.Console
{
    public class Program
    {
       
        public static int Main( string[] args )
        {
            Arguments arguments = Arguments.Parse( args );

            if ( arguments == null )
            {
                System.Console.WriteLine( Arguments.Usage() );
                return 1;
            }

            int returnValue = 0;

            ConsoleOutput.WriteInfo( arguments );
            SpecRunner runner = new SpecRunner();
            AddHandlers( runner );
            runner.LoadAssembly( arguments.Assembly );
            XmlOutputListener xmlOutputListener = new XmlOutputListener( arguments.Assembly, ProgramInformation.Product.ToString(), arguments.Output );
            xmlOutputListener.Add( runner );

            TextBehaviourListener textListener = new TextBehaviourListener(arguments.Assembly, ProgramInformation.Product.ToString(), arguments.BehaviourOutput);
            textListener.Add(runner);

            try
            {
                runner.Run();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.StackTrace);
                returnValue = 1;
            }

            int results = ConsoleOutput.WriteResults();

            if (results != 0)
                returnValue = results;

            xmlOutputListener.Remove();
            textListener.Remove();

            return returnValue;
        }

        private static void AddHandlers( SpecRunner runner )
        {
            runner.InvalidAssembly += new InvalidAssemblyHandler( ConsoleOutput.runner_InvalidAssemblyEvent );
            runner.SpecificationPassed += new SpecficationPassedHandler( ConsoleOutput.runner_SpecificationPassed );
            runner.SpecificationFailed += new SpecificationFailedHandler( ConsoleOutput.runner_SpecificationFailed );

            Specify.StopListening += new RunnersStopListeningHandler(runner.StopListening);
            Specify.StartListening += new RunnersStartListeningHandler( runner.StartListening );
        }
    }
}