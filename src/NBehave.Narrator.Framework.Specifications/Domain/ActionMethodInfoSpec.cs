using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications.Domain
{
    [TestFixture]
    public class ActionMethodInfoSpec
    {
        [Test]
        public void Should_set_MethodParametersType_to_TypedStep()
        {
            Action<ActionMethodInfo> action = _ => { };
            var x = new ActionMethodInfo(new Regex(".*"), action, action.Method, "Given");
            Assert.AreEqual(MethodParametersType.TypedStep, x.MethodParametersType);
        }

        [Test]
        public void Should_set_MethodParametersType_to_UntypedStep_when_parameter_is_primitive_type()
        {
            Action<int> action = _ => { };
            var x = new ActionMethodInfo(new Regex(".*"), action, action.Method, "Given");
            Assert.AreEqual(MethodParametersType.UntypedStep, x.MethodParametersType);
        }

        [Test]
        public void Should_set_MethodParametersType_to_UntypedStep_when_parameter_is_string_type()
        {
            Action<string> action = _ => { };
            var x = new ActionMethodInfo(new Regex(".*"), action, action.Method, "Given");
            Assert.AreEqual(MethodParametersType.UntypedStep, x.MethodParametersType);
        }

        [Test]
        public void Should_set_MethodParametersType_to_UntypedListStep_when_parameter_is_List_of_string_type()
        {
            Action<List<string>> action = _ => { };
            var x = new ActionMethodInfo(new Regex(".*"), action, action.Method, "Given");
            Assert.AreEqual(MethodParametersType.UntypedListStep, x.MethodParametersType);
        }

        [Test]
        public void Should_set_MethodParametersType_to_TypedListStep_when_parameter_is_List_of_object_type()
        {
            Action<List<ActionMethodInfo>> action = _ => { };
            var x = new ActionMethodInfo(new Regex(".*"), action, action.Method, "Given");
            Assert.AreEqual(MethodParametersType.TypedListStep, x.MethodParametersType);
        }
    }
}