using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.Chunker;
using Parameters;

namespace NaturalLanguageProcessing
{
	public class EntityExtractor
	{
		private string modelsPath;
		private string language;

		public EntityExtractor (string language)
		{
			this.language = language;
			modelsPath = "Assets\\Tools\\SharpNLP\\Models\\";
		}

		public string[] extractSentences (string inputData)
		{
			string fileName = "EnglishSD.nbin";
			if (language.Equals(GameConstants.FRENCH_LANGUAGE)) {
				fileName = "FrenchSD.bin";
			}
			
			EnglishMaximumEntropySentenceDetector sentenceDetector = new EnglishMaximumEntropySentenceDetector (modelsPath + fileName);
			return sentenceDetector.SentenceDetect (inputData);
		}

		public string[] tokenize (string sentence)
		{
			string fileName = "EnglishTok.nbin";
			if (language.Equals(GameConstants.FRENCH_LANGUAGE)) {
				fileName = "FrenchTok.bin";
			}
			
			EnglishMaximumEntropyTokenizer tokenizer = new EnglishMaximumEntropyTokenizer (modelsPath + fileName);
			return tokenizer.Tokenize (sentence);
		}
		
		public string[] posTag (string[] tokens)
		{
			string fileName = "EnglishPOS.nbin";
			if (language.Equals(GameConstants.FRENCH_LANGUAGE)) {
				fileName = "FrenchPOS.bin";
			}
			
			EnglishMaximumEntropyPosTagger posTagger = new EnglishMaximumEntropyPosTagger(modelsPath + fileName);
			return posTagger.Tag(tokens);
		}
		
		public string[] chunk (string[] tokens, string[] tags)
		{
			string fileName = "EnglishChunk.nbin";
			if (language.Equals(GameConstants.FRENCH_LANGUAGE)) {
				fileName = "FrenchChunk.bin";
			}
			
			EnglishTreebankChunker chunker = new EnglishTreebankChunker(modelsPath + fileName);
			return chunker.Chunk(tokens, tags);
		}
		
		public string getChunks (string[] tokens, string[] tags)
		{
			string fileName = "EnglishChunk.nbin";
			if (language.Equals(GameConstants.FRENCH_LANGUAGE)) {
				fileName = "FrenchChunk.bin";
			}
			
			EnglishTreebankChunker chunker = new EnglishTreebankChunker(modelsPath + fileName);
			return chunker.GetChunks(tokens, tags);
		}
	}
}
