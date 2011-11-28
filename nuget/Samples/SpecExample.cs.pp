using NBehave.Spec.NUnit;
using NUnit.Framework;
 
namespace $rootnamespace$
{
	public class SpecExample
	{
		[Test]
		public void Some_extension_methods_you_can_use()
		{
			true.ShouldBeTrue();
			false.ShouldBeFalse();
			1.ShouldEqual(1);
			2.ShouldNotEqual(1);
			new int[]{1, 2, 3, 5, 8, 13}.ShouldContain(5);
		}
	}
}