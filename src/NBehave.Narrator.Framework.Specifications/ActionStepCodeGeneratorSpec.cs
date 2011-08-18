using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
    [TestFixture]
    public abstract class ActionStepCodeGeneratorSpec
    {
        private ActionStepCodeGenerator _codeGen;
        private string _generatedCode;

        private string StepText { get; set; }

        [SetUp]
        public virtual void GivenTheseConditions()
        {
            _codeGen = new ActionStepCodeGenerator();
            _generatedCode = _codeGen.GenerateMethodFor(new StringStep(StepText, "a.feature"));
        }


        [TestFixture]
        public class WhenAddingAMethodWithNoParameters : ActionStepCodeGeneratorSpec
        {

            public override void GivenTheseConditions()
            {
                StepText = "Given some stuff";
                base.GivenTheseConditions();
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
            public override void GivenTheseConditions()
            {
                StepText = "And some stuff";
                base.GivenTheseConditions();
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
            public override void GivenTheseConditions()
            {
                StepText = "Given 10 choices";
                base.GivenTheseConditions();
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
            public override void GivenTheseConditions()
            {
                StepText = "Given 'astring'";
                base.GivenTheseConditions();
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
            public override void GivenTheseConditions()
            {
                StepText = "Given I'm using a special char";
                base.GivenTheseConditions();
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
            public override void GivenTheseConditions()
            {
                StepText = "When 2 for 2";
                base.GivenTheseConditions();
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
