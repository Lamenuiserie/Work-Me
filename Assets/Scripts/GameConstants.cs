using System;
using UnityEngine;

namespace Parameters
{
	public static class GameConstants
	{
		//****  Languages  ****//
		public static string ENGLISH_LANGUAGE = "english";
		public static string FRENCH_LANGUAGE = "french";
		
		//****  Word processing  ****//
		public const double FUZZINESS = 0.7f;
		public const string HELP_WORD_EN = "help";
		public const string HELP_WORD_FR = "aide";
		public const string NOUN_PHRASE_B = "B-NP";
		public const string VERB_PHRASE_B = "B-VP";
		public const string ADJECTIVE_PHRASE_B = "B-ADJP";
		public const string ADVERB_PHRASE_B = "B-ADVP";
		public const string PREPOSITIONAL_PHRASE_B = "B-PP";
		public const string NOUN_PHRASE_I = "I-NP";
		public const string VERB_PHRASE_I = "I-VP";
		public const string ADJECTIVE_PHRASE_I = "I-ADJP";
		public const string ADVERB_PHRASE_I = "I-ADVP";
		public const string PREPOSITIONAL_PHRASE_I = "I-PP";
		public const string MODAL_WORD = "MD";
		public const string ADVERB_WORD = "RB";
		public const string NOUN_WORD = "NN";
		public const string VERB_WORD = "VB";
		
		
		//****  Plant  ****//
		public const float INITIAL_BRANCH_COLOR_VALUE_YOUNG = 1.0f;
		public const float MAX_BRANCH_COLOR_VALUE_YOUNG = 0.27f;
		public const float INITIAL_BRANCH_COLOR_VALUE_OLD = 0.46f;
		public const float MAX_BRANCH_COLOR_VALUE_OLD = 0.24f;
		public static Color INITIAL_BRANCH_COLOR_YOUNG = new Color (0, INITIAL_BRANCH_COLOR_VALUE_YOUNG, 0.059f);
		public static Color INITIAL_BRANCH_COLOR_OLD = new Color (INITIAL_BRANCH_COLOR_VALUE_OLD, 0.13f, 0.13f);
		
		public const float INITIAL_BRANCH_LENGTH = 0.7f;
		public const float INITIAL_BRANCH_RADIUS = 0.1f;
		public const float INITIAL_MINIMUM_ROTATION = 0.0f;
		public const float INITIAL_MAXIMUM_ROTATION = 0.0f;
		public const float INITIAL_MINIMUM_GROWING_LENGTH_FACTOR = 0.8f;
		public const float INITIAL_MINIMUM_GROWING_RADIUS_FACTOR = 0.8f;
		
		public const float MIN_GROWING_LENGTH_FACTOR = 0.2f;
		public const float MAX_GROWING_LENGTH_FACTOR = 1.2f;
		public const float MIN_GROWING_RADIUS_FACTOR = 0.2f;
		public const float MAX_GROWING_RADIUS_FACTOR = 1.0f;
		public const float MIN_GROWING_LEAVE_FACTOR = 0.0f;
		public const float MAX_GROWING_LEAVE_FACTOR = 1.0f;
		public const float MIN_GROWING_BRANCH_FACTOR = 0.0f;
		public const float MAX_GROWING_BRANCH_FACTOR = 1.0f;
		public const float MIN_GROWING_LARGE_FACTOR = 0.0f;
		public const float MAX_GROWING_LARGE_FACTOR = 1.0f;
		public const float MIN_GROWING_HIGH_FACTOR = 0.0f;
		public const float MAX_GROWING_HIGH_FACTOR = 1.0f;
		
		public const int MIN_BRANCH_NUMBER = 1;
		public const int MAX_BRANCH_NUMBER = 3;
		
		//****  Corpuses  ****//
		public const string CORPUS_DEATH = "death";
		public const string CORPUS_NEGATION = "negation";
		public const string CORPUS_HATEFUL = "hateful";
		public const string CORPUS_GROW = "grow";
		public const string CORPUS_MAJESTY = "majesty";
		public const string CORPUS_STRENGTH = "strength";
		public const string CORPUS_PRETTY = "pretty";
	}
}

