using NBehave.EventListeners.CodeGeneration;
using NBehave.Extensions;
using NUnit.Framework;

namespace NBehave.Specifications
{
    [TestFixture]
    public abstract class ActionStepCodeGeneratorSpec
    {
        private ActionStepCodeGenerator codeGen;
        private string generatedCode;

        private string StepText { get; set; }

        [SetUp]
        public virtual void GivenTheseConditions()
        {
            codeGen = new ActionStepCodeGenerator();
            generatedCode = codeGen.GenerateMethodFor(new StringStep(StepText.GetFirstWord(), StepText.RemoveFirstWord(), "a.feature"));
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
                Assert.That(generatedCode, Is.Not.Null);
            }

            [Test]
            public void MethodShouldHaveActionStepAttribute()
            {
                StringAssert.Contains(@"[Given(""some stuff"")]", generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_some_stuff()", generatedCode);
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
                Assert.That(generatedCode, Is.Not.Null);
            }

            [Test]
            public void MethodShouldHaveActionStepAttribute()
            {
                StringAssert.Contains(@"[Given(""some stuff"")]", generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_some_stuff()", generatedCode);
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
                StringAssert.Contains(@"[Given(""$param1 choices"")]", generatedCode);
            }

            [Test]
            public void MethodShouldHaveValidName()
            {
                StringAssert.Contains(@"public void Given_param1_choices(int param1)", generatedCode);
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
                StringAssert.Contains(@"[Given(""$param1"")]", generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_param1(string param1)", generatedCode);
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
                StringAssert.Contains(@"[Given(""I'm using a special char"")]", generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void Given_I_m_using_a_special_char()", generatedCode);
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
                StringAssert.Contains(@"[When(""$param1 for $param2"")]", generatedCode);
            }

            [Test]
            public void MethodShouldHaveName()
            {
                StringAssert.Contains(@"public void When_param1_for_param2(int param1, int param2)", generatedCode);
            }
        }
    }
}
