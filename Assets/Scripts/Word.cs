using System;

namespace NaturalLanguageProcessing
{
	public class Word
	{
		public string stringValue;
		public float weight;
		public string tag;
		public float timesUsed;
		
		public Word (string word, float weight)
		{
			stringValue = word;
			this.weight = weight;
			timesUsed = 0;
		}
	}
}

