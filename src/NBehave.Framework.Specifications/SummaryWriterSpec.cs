using System;
using System.IO;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
	[TestFixture]
	public class SummaryWriterSpec
	{
		private SummaryWriter _output;
		private TextWriter _writer;
		[SetUp]
		public void EstablishContext()
		{
			_writer = new StringWriter();
			_output = new SummaryWriter(_writer);
			var results = new FeatureResults(this);
			var feature = new Feature("feature title");
			var scenarioResult = new ScenarioResult(feature, "scenario title");
			scenarioResult.AddActionStepResult(new ActionStepResult("a", new Passed()));
			scenarioResult.AddActionStepResult(new ActionStepResult("b", new Passed()));
			scenarioResult.AddActionStepResult(new ActionStepResult("c", new Pending("pending reason")));
			scenarioResult.AddActionStepResult(new ActionStepResult("d", new Failed(new Exception("why it failed"))));
			results.AddResult(scenarioResult);
			
			_output.WriteSummaryResults(results);
		}
		
		[Test]
		public void ShouldWriteNumberOfActionstepsInSummary()
		{
			Assert.That(_writer.ToString(),Is.StringContaining("Steps 4"));
		}
		
		[Test]
		public void ShouldWriteNumberOfFailedActionstepsInSummary()
		{
			Assert.That(_writer.ToString(),Is.StringContaining("failed 1"));
		}
		
		[Test]
		public void ShouldWriteNumberOfPendingActionstepsInSummary()
		{
			Assert.That(_writer.ToString(),Is.StringContaining("pending 1"));
		}
	}
}
