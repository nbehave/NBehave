using System;

namespace NBehave.Narrator.Framework.Specifications.EventListeners
{
	public class XmlOutputEventListenerTestData
	{
		[Theme("T1")]
		public class ThemeClass1
		{
			[Story]
			public void Story1()
			{
				Story story = new Story("S1");
				
				story
					.AsA("somebody")
					.IWant("this to work")
					.SoThat("I can do something else");
				
				story.WithScenario("SC1")
					.Given("something",()=>{})
					.When("some event occurs",()=>{})
					.Then("there is some outcome",()=>{ });

				story.WithScenario("SC2")
					.Given("something two",()=>{})
					.When("some event #2 occurs",()=>{})
					.Then("there is some outcome #2",()=>{ });
			}
		}

		[Theme("T2")]
		public class ThemeClass2
		{
			[Story]
			public void Story1()
			{
				Story story = new Story("S3");
				
				story
					.AsA("somebody")
					.IWant("this to work")
					.SoThat("I can do something else");
				
				story.WithScenario("SC3")
					.Given("something",()=>{})
					.When("some event occurs",()=>{})
					.Then("there is some outcome",()=>{ });
			}
		}
	}
}
