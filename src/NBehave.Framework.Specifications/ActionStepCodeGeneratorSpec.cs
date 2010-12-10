using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public class ActionStepCodeGeneratorSpec
    {
        private ActionStepCodeGenerator _codeGen;

        [SetUp]
        public virtual void GivenTheseConditions()
        {
            _codeGen = new ActionStepCodeGenerator();
        }

        [TestFixture]
        public class WhenAddingAMethodWithNoParameters : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void GivenTheseConditions()
            {
                base.GivenTheseConditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given some stuff", TypeOfStep.Given);
            }

            [Test]
            public void ShouldCreateMethod()
            {
                Assert.That(_generatedCode, Is.Not.Null);
            }

            [Test]
            public void MethodShouldHaveActionStepAttribute()
            {
                StringAssert.Contains(@"[Given(""some stuff"")]", _generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_some_stuff()", _generatedCode);
            }
        }

        [TestFixture]
        public class WhenAddingAMethodThatStartWithAnd : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void GivenTheseConditions()
            {
                base.GivenTheseConditions();
                _generatedCode = _codeGen.GenerateMethodFor("And some stuff", TypeOfStep.Given);
            }

            [Test]
            public void ShouldCreateMethod()
            {
                Assert.That(_generatedCode, Is.Not.Null);
            }

            [Test]
            public void MethodShouldHaveActionStepAttribute()
            {
                StringAssert.Contains(@"[Given(""some stuff"")]", _generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_some_stuff()", _generatedCode);
            }
        }
        [TestFixture]
        public class WhenAddingAMethodWithAnIntegerParameter : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void GivenTheseConditions()
            {
                base.GivenTheseConditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given 10 choices", TypeOfStep.Given);
            }

            [Test]
            public void MethodShouldHaveActionStepAttributeWithParameter()
            {
                StringAssert.Contains(@"[Given(""$param1 choices"")]", _generatedCode);
            }

            [Test]
            public void MethodShouldHaveValidName()
            {
                StringAssert.Contains(@"public void Given_param1_choices(int param1)", _generatedCode);
            }
        }

        [TestFixture]
        public class WhenAddingAMethodWithAStringParameter : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void GivenTheseConditions()
            {
                base.GivenTheseConditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given 'astring'", TypeOfStep.Given);
            }

            [Test]
            public void MethodShouldHaveActionStepAttributeWithParameter()
            {
                StringAssert.Contains(@"[Given(""$param1"")]", _generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_param1(string param1)", _generatedCode);
            }
        }

        [TestFixture]
        public class WhenAddingAMethodWithAApostrofInStep : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void GivenTheseConditions()
            {
                base.GivenTheseConditions();
                _generatedCode = _codeGen.GenerateMethodFor("Given I'm using a special char", TypeOfStep.Given);
            }

            [Test]
            public void MethodShouldHaveActionStepAttributeWithNoParameters()
            {
                StringAssert.Contains(@"[Given(""I'm using a special char"")]", _generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_I_m_using_a_special_char()", _generatedCode);
            }
        }

        [TestFixture]
        public class WhenAddingAMethodMoreThanOneParameter : ActionStepCodeGeneratorSpec
        {
            private string _generatedCode;

            public override void GivenTheseConditions()
            {
                base.GivenTheseConditions();
                _generatedCode = _codeGen.GenerateMethodFor("When 2 for 2", TypeOfStep.When);
            }

            [Test]
            public void MethodShouldHaveActionStepAttributeWithParameter()
            {
                StringAssert.Contains(@"[When(""$param1 for $param2"")]", _generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void When_param1_for_param2(int param1, int param2)", _generatedCode);
            }
        }
    }
}
