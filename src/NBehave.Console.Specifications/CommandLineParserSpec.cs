using System.Collections.Generic;
using NUnit.Framework;

namespace NBehave.Console.Specifications
{
    [TestFixture]
    public abstract class CommandLineParserSpec
    {
        public class When_parsing_parameters : CommandLineParserSpec
        {
            [Test]
            public void Should_parse_empty_array()
            {
                var args = new string[0];
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                CollectionAssert.IsEmpty(options.Parameters);
            }

            [Test]
            public void Should_parse_argument_with_slash()
            {
                var args = new[] { "/xml:out.xml" };
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                Assert.That(options.Xml, Is.EqualTo("out.xml"));
            }

            [Test]
            public void Should_parse_argument_with_equal_sign_as_value_separator()
            {
                var args = new[] { "/xml=out.xml" };
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                Assert.That(options.Xml, Is.EqualTo("out.xml"));
            }

            [Test]
            public void Should_parse_argument_with_list_as_backing_field()
            {
                var args = new[] { "--tags:@foo,~@bar" };
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                Assert.That(options.Tags, Is.EqualTo(new List<string> { "@foo,~@bar" }));
            }

            [Test]
            public void Should_each_value_of_parameter_to_list()
            {
                var args = new[] { "--tags:@foo,~@bar", "--tags:@baz" };
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                Assert.That(options.Tags, Is.EqualTo(new List<string> { "@foo,~@bar", "@baz" }));
            }

            [Test]
            public void Should_handle_boolean_parameter()
            {
                var args = new[] { "/console" };
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                Assert.That(options.Console, Is.True);
            }

            [Test]
            public void Should_handle_space_as_separator_for_minus_minus_parameter_switch()
            {
                var args = new[] { "--tags", "@foo,~@bar", "--tags", "@baz" };
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                Assert.That(options.Tags, Is.EqualTo(new List<string> { "@foo,~@bar", "@baz" }));
            }

            [Test]
            public void Should_find_property_to_set_by_short_name()
            {
                var args = new[] { "/con" };
                var options = CommandLineParser<ConsoleOptions>.Parse(args);
                Assert.That(options.Console, Is.True);
            }
        }
    }
}