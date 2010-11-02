using System;
using System.Collections.Generic;
using System.Text;
using NBehave.Runner.Console;
using NBehave.Spec.Framework;

namespace NBehave.Runner.Console
{
    class ConsoleOutput
    {
        static int _passes = 0;
        static List<Failure> _failures = new List<Failure>();

        internal static void WriteInfo( Arguments arguments )
        {
            ConsoleColor startingColour = System.Console.ForegroundColor;
            try
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine( ProductString() );

                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.Write( "Specification assembly : " );
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine( arguments.Assembly );

                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.Write( ".Net Runtime Version   : " );
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine( Environment.Version.ToString() );

                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.Write( "OS Version             : " );
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine( Environment.OSVersion.ToString() + "\n\r" );
            }
            finally
            {
                System.Console.ForegroundColor = startingColour;
            }
        }

        internal static string ProductString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( "\n\r" );
            sb.Append( ProgramInformation.Product );
            sb.Append( "\n\r" );
            sb.Append( ProgramInformation.Copyright );
            sb.Append( "\n\r" + "\n\r" );

            return sb.ToString();
        }

        internal static int WriteResults()
        {
            ConsoleColor startingColour = System.Console.ForegroundColor;
            int returnValue = 0;
            try
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine( "\n\r" );
                System.Console.WriteLine( "Specifications Passed : " + _passes.ToString() );

                if ( _failures.Count > 0 )
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    returnValue = 1;
                }
                System.Console.WriteLine( "Specifications Failed : " + _failures.Count.ToString() );
                System.Console.WriteLine( "\n\r" );

                if ( _failures.Count > 0 )
                {
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine( "Failures: \n\r" );
                }

                for ( int count = 0; count < _failures.Count; count++ )
                {
                    System.Console.WriteLine( ( count + 1 ).ToString() + ") " + _failures[count].ToString() + "\n\r" );
                }
            }
            finally 
            {
                System.Console.ForegroundColor = startingColour;
            }

            return returnValue;
        }

        static internal void runner_SpecificationFailed( Failure failure )
        {
            ConsoleColor startingColour = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write( "F" );
            _failures.Add( failure );
            System.Console.ForegroundColor = startingColour;
        }

        static internal void runner_SpecificationPassed()
        {
            ConsoleColor startingColour = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.Write( "." );
            _passes++;
            System.Console.ForegroundColor = startingColour;
        }

        static internal void runner_InvalidAssemblyEvent( string assemblyName )
        {
            ConsoleColor startingColour = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine( " Unable to open assembly {0}. ", assemblyName );
            System.Console.ForegroundColor = startingColour;
        }
    }
}