using System;

namespace NBehave.Narrator.Framework
{
	public interface IMatchFiles
	{
		IFileMatcher FileMatcher {get;}
	}
	
	public interface IFileMatcher
	{
		bool IsMatch(string fileName);			
	}
	
	public class MatchAllFiles : IFileMatcher
	{
		bool IFileMatcher.IsMatch(string fileName)
		{
			return true;
		}
	}
}
