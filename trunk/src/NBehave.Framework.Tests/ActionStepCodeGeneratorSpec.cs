using NUnit.Framework;
using Context = NUnit.Framework.TestFixtureAttribute;
using Specification = NUnit.Framework.TestAttribute;

namespace NBehave.Narrator.Framework.Specifications
{
    [Context]
    public class ActionStepCodeGeneratorSpec
    {
        private ActionStepCodeGenerator _codeGen;

        [SetUp]
        public virtual void Given_these_conditions()
        {
            _codeGen = new ActionStepCodeGenerator();
        }

        [Context]
        public class When_adding_a_method_with_no_parameters : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void Given_these_conditions()
            {
                base.Given_these_conditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given some stuff");
            }

            [Specification]
            public void Should_create_method()
            {
                Assert.That(_generatedCode, Is.Not.Null);
            }

            [Specification]
            public void Method_Should_have_ActionStep_attribute()
            {
                StringAssert.Contains(@"[Given(""some stuff"")]", _generatedCode);
            }

            [Specification]
            public void Method_Should_have_name()
            {
                StringAssert.Contains(@"public void Given_some_stuff()", _generatedCode);
            }
        }

        [Context]
        public class When_adding_a_method_with_an_integer_parameter : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void Given_these_conditions()
            {
                base.Given_these_conditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given 10 choices");
            }

            [Specification]
            public void Method_Should_have_ActionStep_attribute_with_parameter()
            {
                StringAssert.Contains(@"[Given(""$param1 choices"")]", _generatedCode);
            }

            [Specification]
            public void Method_Should_have_valid_name()
            {
                StringAssert.Contains(@"public void Given_param1_choices(int param1)", _generatedCode);
            }
        }

        [Context]
        public class When_adding_a_method_with_a_string_parameter : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void Given_these_conditions()
            {
                base.Given_these_conditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given 'astring'");
            }

            [Specification]
            public void Method_Should_have_ActionStep_attribute_with_parameter()
            {
                StringAssert.Contains(@"[Given(""$param1"")]", _generatedCode);
            }

            [Specification]
            public void Method_Should_have_name()
            {
                StringAssert.Contains(@"public void Given_param1(string param1)", _generatedCode);
            }
        }

        [Context]
        public class When_adding_a_method_more_than_one_parameter : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void Given_these_conditions()
            {
                base.Given_these_conditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given 2 for 'one'");
            }

            [Specification]
            public void Method_Should_have_ActionStep_attribute_with_parameter()
            {
                StringAssert.Contains(@"[Given(""$param1 for $param2"")]", _generatedCode);
            }

            [Specification]
            public void Method_Should_have_name()
            {
                StringAssert.Contains(@"public void Given_param1_for_param2(int param1, string param2)", _generatedCode);
            }
        }
    }
}
