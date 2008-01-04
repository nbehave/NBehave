using System;
using System.Collections.Generic;
using System.Text;
using NBehave.Spec.Framework;

namespace NBehave.Runner.Console.Specifications
{
    [Context]
    public class ProgramInformationBehaviour
    {
        [Specification]
        public void ShouldRetriveCopyrightInformationFromAssembly()
        {
            string correct = "Copyright (C) 2007 Jimmy Bogard, Joe Ocampo, Morgan Persson, Tim Haughton.";
            string actual = ProgramInformation.Copyright;

            Specify.That( actual ).ShouldEqual( correct );
        }

        [Specification]
        public void ShouldRetrieveProductFromAssembly()
        {
            string correct = "NBehave 0.3";
            string actual = ProgramInformation.Product;

            Specify.That( actual ).ShouldEqual( correct );
        }
    }
}