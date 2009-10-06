using System;
using System.IO;
using NUnit.Framework;

namespace NBehave.Narrator.Framework.Specifications
{
	[TestFixture]
	public class PlainTextOutputSpec
	{
		private PlainTextOutput output;
		private TextWriter writer;
		[SetUp]
		public void Establish_context()
		{
			writer = new StringWriter();
			output = new PlainTextOutput(writer);
			var results = new StoryResults();
			var story = new Story("story title");
			var scenarioResult = new ScenarioResult(story, "scenario title");
			scenarioResult.AddActionStepResult(new ActionStepResult("a", new Passed()));
			scenarioResult.AddActionStepResult(new ActionStepResult("b", new Passed()));
			scenarioResult.AddActionStepResult(new ActionStepResult("c", new Pending("pending reason")));
			scenarioResult.AddActionStepResult(new ActionStepResult("d", new Failed(new Exception("why it failed"))));
			results.AddResult(scenarioResult);
			
			output.WriteSummaryResults(results);
		}
		
		[Test]
		public void Should_write_number_of_actionsteps_in_summary()
		{
			Assert.That(writer.ToString(),Is.StringContaining("Steps 4"));
		}
		
		[Test]
		public void Should_write_number_of_failed_actionsteps_in_summary()
		{
			Assert.That(writer.ToString(),Is.StringContaining("failed 1"));
		}
		
		[Test]
		public void Should_write_number_of_pending_actionsteps_in_summary()
		{
			Assert.That(writer.ToString(),Is.StringContaining("pending 1"));
		}
	}
}
