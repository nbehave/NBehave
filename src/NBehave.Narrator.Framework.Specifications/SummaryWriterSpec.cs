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
			var feature = new Feature("feature title");
            var featureResult = new FeatureResult();
			var scenarioResult = new ScenarioResult(feature, "scenario title");
            scenarioResult.AddActionStepResult(new StepResult("a".AsStringStep(""), new Passed()));
            scenarioResult.AddActionStepResult(new StepResult("b".AsStringStep(""), new Passed()));
            scenarioResult.AddActionStepResult(new StepResult("c".AsStringStep(""), new Pending("pending reason")));
            scenarioResult.AddActionStepResult(new StepResult("c".AsStringStep(""), new PendingNotImplemented("not implemented")));
            scenarioResult.AddActionStepResult(new StepResult("d".AsStringStep(""), new Failed(new Exception("why it failed"))));
            scenarioResult.AddActionStepResult(new StepResult("c".AsStringStep(""), new Skipped("previous step failed")));
            featureResult.AddResult(scenarioResult);
			_output.WriteSummaryResults(featureResult);
		}
		
		[Test]
		public void ShouldWriteNumberOfActionstepsInSummary()
		{
			Assert.That(_writer.ToString(),Is.StringContaining("Steps 6"));
		}
		
		[Test]
		public void ShouldWriteNumberOfFailedActionstepsInSummary()
		{
			Assert.That(_writer.ToString(),Is.StringContaining("failed 1"));
		}
		
		[Test]
		public void ShouldWriteNumberOfPendingActionstepsInSummary()
		{
			Assert.That(_writer.ToString(),Is.StringContaining("pending 3"));
		}
	}
}
