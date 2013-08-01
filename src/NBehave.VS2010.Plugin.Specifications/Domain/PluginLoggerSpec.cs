using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using NBehave.VS2010.Plugin.Domain;
using NUnit.Framework;

namespace NBehave.VS2010.Plugin.Specifications.Domain
{
    [TestFixture]
    public class PluginLoggerSpec
    {
        [Test]
        public void Should_handle_case_where_stacktrace_is_null()
        {
            var p = new PluginLogger(null);
            Assert.DoesNotThrow(() =>
            {
                var args = new FirstChanceExceptionEventArgs(new COMException("blerg"));
                p.LogFirstChanceException(args);
            });
        }
    }
}
