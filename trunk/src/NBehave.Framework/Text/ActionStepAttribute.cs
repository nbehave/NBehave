using System;
using System.Text.RegularExpressions;

namespace NBehave.Narrator.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	/// <summary>
	/// Use Given, When or Then Attribute
	/// </summary>
	public abstract class ActionStepAttribute : Attribute
	{
		public Regex ActionMatch { get; protected set; }
		public string Type { get; protected set; }

		protected ActionStepAttribute()
		{ }

		protected ActionStepAttribute(string tokenString)
			: this(tokenString.RemoveFirstWord().AsRegex())
		{
			Type = tokenString.GetFirstWord();
		}

		protected ActionStepAttribute(Regex actionMatch)
		{
			ActionMatch = actionMatch;
			Type = actionMatch.ToString().GetFirstWord();
		}
	}
}