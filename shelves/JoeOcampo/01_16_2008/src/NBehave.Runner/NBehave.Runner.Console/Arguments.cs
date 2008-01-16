using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NBehave.Runner.Console
{
    class Arguments
    {
        internal string Assembly;
        internal string Output = "NSpecOutput";
        internal string BehaviourOutput = "NSpecAssemblyBehaviour.txt";

        private Arguments()
        {
        }

        private static bool HelpRequested( string[] args )
        {
            foreach ( string arg in args )
            {
                if ( arg == "/?" )
                    return true;
            }

            return false;
        }

        internal static Arguments Parse(string[] args)
        {
            Arguments a = new Arguments();

            if ( args.Length == 0 || HelpRequested( args ) )
                return null;
            try
            {
                foreach ( string s in args )
                {
                    string[] components = s.Split( char.Parse( ":" ) );
                    bool ignoreCase = true;
                    if ( 0 == String.Compare( components[0], "/assembly", ignoreCase ) )
                    {
                        if ( File.Exists( Path.GetFullPath( components[1] ) ) )
                        {
                            a.Assembly = Path.GetFullPath( components[1] );
                        }
                        else if ( null != components[2] &&  File.Exists( Path.GetFullPath( components[2] ) ) )
                        {
                            // Cater for the user entering a full explicit path including the drive letter i.e. /assembly:D:\assemblyname.dll
                            a.Assembly = Path.GetFullPath( components[2] );
                        }
                    }
                    else if ( 0 == String.Compare( components[0], "/output", ignoreCase ) )
                    {
                        a.Output = Path.GetFullPath( components[1] );
                    }
                    else if (0 == String.Compare(components[0], "/behaviourOutput", ignoreCase))
                    {
                        a.BehaviourOutput = Path.GetFullPath(components[1]);
                    }
                    else if (null != components[2])
                    {
                        // Cater for the user entering a full explicit path including the drive letter i.e. /output:D:\outputfile.xml
                        a.Output = Path.GetFullPath(components[2]);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }

            return a;
        }

        internal static string Usage()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( "\r\nUsage: \r\n \r\n" );
            sb.Append("nspec.console /assembly:AssemblyFileName [/output:XmlOutputFileName] [/behaviourOutput:textSpecFileName] \r\n \r\n");

            return sb.ToString(); ;
        }
    }
}