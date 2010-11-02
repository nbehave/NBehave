using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NBehave.Runner.Console
{
    static class ProgramInformation
    {
        internal static string Copyright
        {
            get
            {
                object[] attrs = Assembly.GetExecutingAssembly().GetCustomAttributes(
                    typeof( AssemblyCopyrightAttribute ), false );

                AssemblyCopyrightAttribute aca = attrs[0] as AssemblyCopyrightAttribute;
                return aca.Copyright;
            }
        }

        internal static string Product
        {
            get
            {
                object[] attrs = Assembly.GetExecutingAssembly().GetCustomAttributes(
                    typeof( AssemblyProductAttribute ), false );

                AssemblyProductAttribute apa = attrs[0] as AssemblyProductAttribute;

                Version version = Assembly.GetExecutingAssembly().GetName().Version;

                return apa.Product + " " + version.ToString( 2 );
            }
        }
    }
}