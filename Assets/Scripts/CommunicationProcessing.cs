using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using LSystem;
using Parameters;

namespace NaturalLanguageProcessing
{
	public class CommunicationProcessing : MonoBehaviour
	{	
		public Branch branch;
		public string language;
		
		private Branch rootNode;
		private List<Branch> lastBranches;
		private List<Word> deathCorpus;
		private List<Word> negationCorpus;
		private List<Word> hatefulCorpus;
		private List<Word> growCorpus;
		private List<Word> majestyCorpus;
		private List<Word> strengthCorpus;
		private List<Word> prettyCorpus;
		private int totalWordsCounter;
		
		//Sounds
		private AudioSource audioLoopGrow;
		private AudioSource audioLoopStrength;
		private AudioSource audioLoopPretty;
		private AudioSource audioLoopMajesty;
		private AudioSource audioLoopHateful;
		private AudioSource audioFeedback;
		public AudioClip growLoop;
		public AudioClip hatefulLoop;
		public AudioClip majestyLoop;
		public AudioClip prettyLoop;
		public AudioClip strengthLoop;
		public AudioClip deathFeedback;
		public AudioClip majestyFeedback;
		public AudioClip negativeFeedback1;
		public AudioClip negativeFeedback2;
		public AudioClip positiveFeedback;
		
		private bool fadeInGrowLoop;
		private bool fadeInPrettyLoop;
		private bool fadeInStrengthLoop;
		private bool fadeInMajestyLoop;
		private bool fadeInHatefulLoop;
		private float audioLoopGrowTargetVolumeFadeIn;
		private float audioLoopPrettyTargetVolumeFadeIn;
		private float audioLoopStrengthTargetVolumeFadeIn;
		private float audioLoopMajestyTargetVolumeFadeIn;
		private float audioLoopHatefulTargetVolumeFadeIn;
		
		private bool fadeOutGrowLoop;
		private bool fadeOutPrettyLoop;
		private bool fadeOutStrengthLoop;
		private bool fadeOutMajestyLoop;
		private bool fadeOutHatefulLoop;
		private float audioLoopGrowTargetVolumeFadeOut;
		private float audioLoopPrettyTargetVolumeFadeOut;
		private float audioLoopStrengthTargetVolumeFadeOut;
		private float audioLoopMajestyTargetVolumeFadeOut;
		private float audioLoopHatefulTargetVolumeFadeOut;
		
		// Use this for initialization
		void Start () {
			rootNode = transform.FindChild("RootNode").GetComponent("Branch") as Branch;
			rootNode.branch = branch;
			rootNode.construct (null, null, GameConstants.INITIAL_BRANCH_LENGTH, GameConstants.INITIAL_BRANCH_RADIUS, GameConstants.INITIAL_MINIMUM_ROTATION, GameConstants.INITIAL_MAXIMUM_ROTATION, GameConstants.INITIAL_BRANCH_COLOR_YOUNG, true);
			
			//TODO mots composés, split on ' ' and see if there's word after if yes then this is the complete word.
			
			//Load sounds
			fadeInGrowLoop = false;
			fadeInPrettyLoop = false;
			fadeInStrengthLoop = false;
			fadeInMajestyLoop = false;
			fadeInHatefulLoop = false;
			fadeOutGrowLoop = false;
			fadeOutPrettyLoop = false;
			fadeOutStrengthLoop = false;
			fadeOutMajestyLoop = false;
			fadeOutHatefulLoop = false;
			audioLoopGrowTargetVolumeFadeOut = 0;
			audioLoopPrettyTargetVolumeFadeOut = 0;
			audioLoopStrengthTargetVolumeFadeOut = 0;
			audioLoopMajestyTargetVolumeFadeOut = 0;
			audioLoopHatefulTargetVolumeFadeOut = 0;
			audioLoopGrowTargetVolumeFadeIn = 0;
			audioLoopPrettyTargetVolumeFadeIn = 0;
			audioLoopStrengthTargetVolumeFadeIn = 0;
			audioLoopMajestyTargetVolumeFadeIn = 0;
			audioLoopHatefulTargetVolumeFadeIn = 0;
			audioFeedback = GetComponents<AudioSource>()[0];
			audioLoopGrow = GetComponents<AudioSource>()[1];
			audioLoopPretty = GetComponents<AudioSource>()[2];
			audioLoopStrength = GetComponents<AudioSource>()[3];
			audioLoopMajesty = GetComponents<AudioSource>()[4];
			audioLoopHateful = GetComponents<AudioSource>()[5];
			
			lastBranches = new List<Branch>();
			lastBranches.Add(rootNode);
			negationCorpus = new List<Word> ();
			hatefulCorpus = new List<Word> ();
			deathCorpus = new List<Word> ();
			growCorpus = new List<Word> ();
			majestyCorpus = new List<Word> ();
			strengthCorpus = new List<Word> ();
			prettyCorpus = new List<Word> ();
			totalWordsCounter = 0;
			StreamReader deathReader = new StreamReader (@"Assets\Corpuses\death.txt");
			StreamReader negationReader = new StreamReader (@"Assets\Corpuses\negation.txt");
			StreamReader hatefulReader = new StreamReader (@"Assets\Corpuses\hateful.txt");
			StreamReader growReader = new StreamReader (@"Assets\Corpuses\grow.txt");
			StreamReader majestyReader = new StreamReader (@"Assets\Corpuses\majesty.txt");
			StreamReader strengthReader = new StreamReader (@"Assets\Corpuses\strength.txt");
			StreamReader prettyReader = new StreamReader (@"Assets\Corpuses\pretty.txt");
			if (language.Equals ("french")) {
				deathReader = new StreamReader (@"Assets\Corpuses\death_fr.txt");
				negationReader = new StreamReader (@"Assets\Corpuses\negation_fr.txt");
				hatefulReader = new StreamReader (@"Assets\Corpuses\hateful_fr.txt");
				growReader = new StreamReader (@"Assets\Corpuses\grow_fr.txt");
				majestyReader = new StreamReader (@"Assets\Corpuses\majesty_fr.txt");
				strengthReader = new StreamReader (@"Assets\Corpuses\strength_fr.txt");
				prettyReader = new StreamReader (@"Assets\Corpuses\pretty_fr.txt");
			}
			//Fill the list of death words
			fillCorpus (deathReader, GameConstants.CORPUS_DEATH);
			deathReader.Close ();
			//Fill the list of negation words
			fillCorpus (negationReader, GameConstants.CORPUS_NEGATION);
			negationReader.Close ();
			//Fill the list of hateful words
			fillCorpus (hatefulReader, GameConstants.CORPUS_HATEFUL);
			hatefulReader.Close ();
			//Fill the list of grow words
			fillCorpus (growReader, GameConstants.CORPUS_GROW);
			growReader.Close ();
			//Fill the list of majesty words
			fillCorpus (majestyReader, GameConstants.CORPUS_MAJESTY);
			majestyReader.Close ();
			//Fill the list of death words
			fillCorpus (strengthReader, GameConstants.CORPUS_STRENGTH);
			strengthReader.Close ();
			//Fill the list of death words
			fillCorpus (prettyReader, GameConstants.CORPUS_PRETTY);
			prettyReader.Close ();
		}
		
		// Update is called once per frame
		void Update () {
			//Fade ins
			if (fadeInGrowLoop) {
				fadeInGrowLoop = fadeIn(0.2f, audioLoopGrow, audioLoopGrowTargetVolumeFadeIn);
			}
			if (fadeInPrettyLoop) {
				fadeInPrettyLoop = fadeIn(0.2f, audioLoopPretty, audioLoopPrettyTargetVolumeFadeIn);
			}
			if (fadeInStrengthLoop) {
				fadeInStrengthLoop = fadeIn(0.2f, audioLoopStrength, audioLoopStrengthTargetVolumeFadeIn);
			}
			if (fadeInMajestyLoop) {
				fadeInMajestyLoop = fadeIn(0.2f, audioLoopMajesty, audioLoopMajestyTargetVolumeFadeIn);
			}
			if (fadeInHatefulLoop) {
				fadeInHatefulLoop = fadeIn(0.2f, audioLoopHateful, audioLoopHatefulTargetVolumeFadeIn);
			}
			
			//Fade outs
			if (fadeOutGrowLoop) {
				fadeOutGrowLoop = fadeOut(0.2f, audioLoopGrow, audioLoopGrowTargetVolumeFadeOut);
			}
			if (fadeOutPrettyLoop) {
				fadeOutPrettyLoop = fadeOut(0.2f, audioLoopPretty, audioLoopPrettyTargetVolumeFadeOut);
			}
			if (fadeOutStrengthLoop) {
				fadeOutStrengthLoop = fadeOut(0.2f, audioLoopStrength, audioLoopStrengthTargetVolumeFadeOut);
			}
			if (fadeOutMajestyLoop) {
				fadeOutMajestyLoop = fadeOut(0.2f, audioLoopMajesty, audioLoopMajestyTargetVolumeFadeOut);
			}
			if (fadeOutHatefulLoop) {
				fadeOutHatefulLoop = fadeOut(0.2f, audioLoopHateful, audioLoopHatefulTargetVolumeFadeOut);
			}
		}
		
		/// <summary>
		/// Processes the input.
		/// </summary>
		/// <param name='tokens'>
		/// Tokens.
		/// </param>
		public void processInput (List<string> tokens, List<string> phraseTypes, List<string> tags)
		{
			//Debug.Log("Processing sentences");
			
			//Grow the plant by starting from the oldest one
			Branch lastOldest = lastBranches[0];
			foreach (Branch br in lastBranches) {
				if (br.getAge() < lastOldest.getAge()) {
					lastOldest = br;
				}
			}
			Branch branch = lastOldest;
			
			//Recognize words from the tokens
			string token = "";
			string keyword = "";
			bool unknown = true;
			List<Word> words = new List<Word> ();
			List<Word> deathWords = new List<Word> ();
			List<Word> negationWords = new List<Word> ();
			List<Word> hatefulWords = new List<Word> ();
			List<Word> growWords = new List<Word> ();
			List<Word> majestyWords = new List<Word> ();
			List<Word> strengthWords = new List<Word> ();
			List<Word> prettyWords = new List<Word> ();
			List<Word> unknownWords = new List<Word> ();
			for (int i = 0; i < tokens.Count; i++) {
				token = tokens[i].ToLower();
				totalWordsCounter++;
				keyword = "";
				unknown = true;
				//TODO try to find words based on the morpheme/stem
				//TODO Check for keywords
				//TODO Check for keyword such as 2d to make the plant grow 2D
				//TODOCheck for keywords such as turn around and so on
				//TODO consider smileys
				if (token.Equals(GameConstants.HELP_WORD_EN) || token.Equals(GameConstants.HELP_WORD_FR)) {
					keyword = GameConstants.HELP_WORD_EN;
					unknown = false;
				}
				if (unknown) {
					//Look for words that are present in the death corpus
					unknown = retrieveWord(token, tags[i], deathCorpus, deathWords, words);
				}
				if (unknown) {
					//Look for words that are present in the negation corpus
					unknown = retrieveWord(token, tags[i], negationCorpus, negationWords, words);
				}
				if (unknown) {
					//Look for words that are present in the hateful corpus
					unknown = retrieveWord(token, tags[i], hatefulCorpus, hatefulWords, words);
				}
				if (unknown) {
					//Look for words that are present in the grow corpus
					unknown = retrieveWord(token, tags[i], growCorpus, growWords, words);
				}
				if (unknown) {
					//Look for words that are present in the strength corpus
					unknown = retrieveWord(token, tags[i], strengthCorpus, strengthWords, words);
				}
				if (unknown) {
					//Look for words that are present in the pretty corpus
					unknown = retrieveWord(token, tags[i], prettyCorpus, prettyWords, words);
				}
				if (unknown) {
					//Look for words that are present in the majesty corpus
					unknown = retrieveWord(token, tags[i], majestyCorpus, majestyWords, words);
				}
				
				//If the word is unknow add it to the list of unknown words
				if (unknown) {
					unknownWords.Add (new Word (token, 1));
				}
			}

			//Adjusting factors
			float growingLengthFactor = GameConstants.INITIAL_MINIMUM_GROWING_LENGTH_FACTOR;
			float growingRadiusFactor = GameConstants.INITIAL_MINIMUM_GROWING_RADIUS_FACTOR;
			float growingLeaveFactor = GameConstants.MIN_GROWING_LEAVE_FACTOR;
			float growingBranchFactor = GameConstants.MIN_GROWING_BRANCH_FACTOR;
			float growingLargeFactor = GameConstants.MIN_GROWING_LARGE_FACTOR;
			float growingHighFactor = GameConstants.MIN_GROWING_HIGH_FACTOR;
			bool canBePrettyAndStrong = false;
			bool died = false;
			
			//TODO make it so that too much highly weighted words in a row make them less powerful or even degrading
			
			//Keywords
			if (keyword != "") {
				switch (keyword) {
				case GameConstants.HELP_WORD_EN:
					//TODO display help
					break;
				}
			} else {
				//TODO pour le français certains trucs sont inversés, ne pousse pas, pousse grand majestueusement, etc.
				
				//Check for negated death words
				List<Word> wordsToRemove = new List<Word> ();
				foreach (Word deathWord in deathWords) {
					int positionIndex = words.IndexOf(deathWord);
					if ((positionIndex - 1) >= 0 && negationWords.Contains(words[positionIndex - 1])) {
						growingLengthFactor += (((totalWordsCounter / deathWord.timesUsed) * deathWord.weight) / totalWordsCounter) / deathWords.Count;
						if (growingLengthFactor > GameConstants.MAX_GROWING_LENGTH_FACTOR) {
							growingLengthFactor = GameConstants.MAX_GROWING_LENGTH_FACTOR;
						}
						growingRadiusFactor += growingLengthFactor;
						if (growingRadiusFactor > GameConstants.MAX_GROWING_RADIUS_FACTOR) {
							growingRadiusFactor = GameConstants.MAX_GROWING_RADIUS_FACTOR;
						}
						growingLeaveFactor += growingLengthFactor;
						wordsToRemove.Add(deathWord);
						//Debug.Log("Negated death word");
					}
				}
				//Remove the negated death words
				foreach (Word deathWord in wordsToRemove) {
					deathWords.Remove(deathWord);
				}
				
				//If too much death words are used kill the plant
				if (deathWords.Count > (growWords.Count + strengthWords.Count + prettyWords.Count)) {
					died = true;
					//Debug.Log("Too much death words");
					
					//Play death feedback
					audioFeedback.PlayOneShot(deathFeedback);
					//Stop all other sounds
					fadeInGrowLoop = false;
					fadeInPrettyLoop = false;
					fadeInStrengthLoop = false;
					fadeInMajestyLoop = false;
					fadeInHatefulLoop = false;
					fadeOutGrowLoop = false;
					fadeOutPrettyLoop = false;
					fadeOutStrengthLoop = false;
					fadeOutMajestyLoop = false;
					fadeOutHatefulLoop = false;
					audioLoopGrowTargetVolumeFadeOut = 0;
					audioLoopPrettyTargetVolumeFadeOut = 0;
					audioLoopStrengthTargetVolumeFadeOut = 0;
					audioLoopMajestyTargetVolumeFadeOut = 0;
					audioLoopHatefulTargetVolumeFadeOut = 0;
					audioLoopGrowTargetVolumeFadeIn = 0;
					audioLoopPrettyTargetVolumeFadeIn = 0;
					audioLoopStrengthTargetVolumeFadeIn = 0;
					audioLoopMajestyTargetVolumeFadeIn = 0;
					audioLoopHatefulTargetVolumeFadeIn = 0;
					audioLoopGrow.Stop();
					audioLoopPretty.Stop();
					audioLoopStrength.Stop();
					audioLoopMajesty.Stop();
					audioLoopHateful.Stop();
					reset();
				} else {
					//Make the plant grow way thinier and shorther
					foreach (Word hatefulWord in hatefulWords) {
						float temp = (totalWordsCounter / (1 / hatefulWord.timesUsed)) * hatefulWord.weight;
						growingLengthFactor -= (temp / totalWordsCounter) / hatefulWords.Count;
						if (growingLengthFactor < GameConstants.MIN_GROWING_LENGTH_FACTOR) {
							growingLengthFactor = GameConstants.MIN_GROWING_LENGTH_FACTOR;
						}
						growingRadiusFactor -= growingLengthFactor;
						if (growingRadiusFactor < GameConstants.MIN_GROWING_RADIUS_FACTOR) {
							growingRadiusFactor = GameConstants.MIN_GROWING_RADIUS_FACTOR;
						}
						growingLeaveFactor -= growingLengthFactor;
						branch.colorDecay -= 0.08f;
						branch.lengthDecay -= 0.08f;
						branch.radiusDecay -= 0.08f;
						//TODO reduce number of branches
						//Debug.Log("Hateful word");
						
						///Play ambiance
						//Fade in 5 sec
						fadeInHatefulLoop = true;
						audioLoopHatefulTargetVolumeFadeIn = 0.98f;
						
						//Decrease grow ambiance to 0
						if (audioLoopGrow.isPlaying) {
							//Fade out 5 sec
							fadeOutGrowLoop = true;
							audioLoopGrowTargetVolumeFadeOut = 0;
						}
						
						//Decrease pretty ambiance to 0
						if (audioLoopPretty.isPlaying) {
							//Fade out 5 sec
							fadeOutPrettyLoop = true;
							audioLoopPrettyTargetVolumeFadeOut = 0;
						}
						
						//Decrease strength ambiance to 0
						if (audioLoopStrength.isPlaying) {
							//Fade out 5 sec
							fadeOutStrengthLoop = true;
							audioLoopStrengthTargetVolumeFadeOut = 0;
						}
						
						//Decrease majesty ambiance to 0
						if (audioLoopMajesty.isPlaying) {
							//Fade out 5 sec
							fadeOutMajestyLoop = true;
							audioLoopMajestyTargetVolumeFadeOut = 0;
						}
					}
					//Make the plant grow longer and with more leaves and less thinier and allow the use of strength and pretty words
					foreach (Word growWord in growWords) {
						int positionIndex = words.IndexOf(growWord);
						float temp = ((totalWordsCounter * 1.5f) / growWord.timesUsed) * growWord.weight;
						if ((positionIndex - 1) < 0 || !negationWords.Contains(words[positionIndex - 1])) {
							growingLengthFactor += (temp / totalWordsCounter) / growWords.Count;
							if (growingLengthFactor > GameConstants.MAX_GROWING_LENGTH_FACTOR) {
								growingLengthFactor = GameConstants.MAX_GROWING_LENGTH_FACTOR;
							}
							growingRadiusFactor += growingLengthFactor;
							if (growingRadiusFactor > GameConstants.MAX_GROWING_RADIUS_FACTOR) {
								growingRadiusFactor = GameConstants.MAX_GROWING_RADIUS_FACTOR;
							}
							growingLeaveFactor += growingLengthFactor;
							if (growingLeaveFactor > GameConstants.MAX_GROWING_LEAVE_FACTOR) {
								growingLeaveFactor = GameConstants.MAX_GROWING_LEAVE_FACTOR;
							}
							growingBranchFactor *= growingLengthFactor / 2;
							if (growingBranchFactor > GameConstants.MAX_GROWING_BRANCH_FACTOR) {
								growingBranchFactor = GameConstants.MAX_GROWING_BRANCH_FACTOR;
							}
							canBePrettyAndStrong = true;
							//Debug.Log("Grow word");
							
							//Play ambiance
							//Fade in 5 sec
							fadeInGrowLoop = true;
							audioLoopGrowTargetVolumeFadeIn = 0.98f;
							
							//Decrease hateful ambiance to 0
							if (audioLoopHateful.isPlaying) {
								//Fade out 5 sec
								fadeOutHatefulLoop = true;
								audioLoopHatefulTargetVolumeFadeOut = 0;
							}
						} else if (negationWords.Contains(words[positionIndex - 1])) {
							growingLengthFactor -= (temp / totalWordsCounter) / growWords.Count;
							if (growingLengthFactor < GameConstants.MIN_GROWING_LENGTH_FACTOR) {
								growingLengthFactor = GameConstants.MIN_GROWING_LENGTH_FACTOR;
							}
							growingRadiusFactor -= growingLengthFactor;
							if (growingRadiusFactor < GameConstants.MIN_GROWING_RADIUS_FACTOR) {
								growingRadiusFactor = GameConstants.MIN_GROWING_RADIUS_FACTOR;
							}
							growingLeaveFactor -= growingLengthFactor;
							if (growingLeaveFactor < GameConstants.MIN_GROWING_LEAVE_FACTOR) {
								growingLeaveFactor = GameConstants.MIN_GROWING_LEAVE_FACTOR;
							}
							growingBranchFactor *= growingLengthFactor;
							if (growingBranchFactor < GameConstants.MIN_GROWING_BRANCH_FACTOR) {
								growingBranchFactor = GameConstants.MIN_GROWING_BRANCH_FACTOR;
							}
							branch.colorDecay -= 0.02f;
							branch.lengthDecay -= 0.02f;
							branch.radiusDecay -= 0.02f;
							//Debug.Log("Negated grow word");
						}
					}
					
					if (canBePrettyAndStrong) {
						//Play positive feedback
						audioFeedback.PlayOneShot(positiveFeedback);
						
						//Make the plant grow larger and thicker
						foreach (Word strengthWord in strengthWords) {
							int positionIndex = words.IndexOf(strengthWord);
							if (((positionIndex - 1) < 0 || !negationWords.Contains(words[positionIndex - 1])) && ((positionIndex - 2 < 0) || !negationWords.Contains(words[positionIndex - 2]))) {
								float temp = (totalWordsCounter / strengthWord.timesUsed) * strengthWord.weight;
								growingLargeFactor += (temp / totalWordsCounter) / strengthWords.Count;
								if (growingLargeFactor > GameConstants.MAX_GROWING_LARGE_FACTOR) {
									growingLargeFactor = GameConstants.MAX_GROWING_LARGE_FACTOR;
								}
								growingBranchFactor += ((temp * 0.8f) / totalWordsCounter) / strengthWords.Count;
								if (growingBranchFactor > GameConstants.MAX_GROWING_BRANCH_FACTOR) {
									growingBranchFactor = GameConstants.MAX_GROWING_BRANCH_FACTOR;
								}
								growingRadiusFactor += growingLargeFactor;
								if (growingRadiusFactor > GameConstants.MAX_GROWING_RADIUS_FACTOR) {
									growingRadiusFactor = GameConstants.MAX_GROWING_RADIUS_FACTOR;
								}
								//Debug.Log("Strength word");
								
								//Play ambiance
								//Fade in 5 sec
								fadeInStrengthLoop = true;
								audioLoopStrengthTargetVolumeFadeIn = 0.98f;
								
								//Decrease grow ambiance to 0.93
								if (audioLoopGrow.isPlaying) {
									//Fade out 5 sec
									fadeOutGrowLoop = true;
									audioLoopGrowTargetVolumeFadeOut = 0.93f;
								}
								
								//Decrease pretty ambiance to 0
								if (audioLoopPretty.isPlaying) {
									//Fade out
									fadeOutPrettyLoop = true;
									audioLoopPrettyTargetVolumeFadeOut = 0;
								}
								
								//Decrease majesty ambiance to 0
								if (audioLoopMajesty.isPlaying) {
									//Fade out
									fadeOutMajestyLoop = true;
									audioLoopMajestyTargetVolumeFadeOut = 0;
								}
								
								//Decrease hateful ambiance to 0
								if (audioLoopHateful.isPlaying) {
									//Fade out 5 sec
									fadeOutHatefulLoop = true;
									audioLoopHatefulTargetVolumeFadeOut = 0;
								}
							}
						}
						//Make the plant grow higher, longer, with less branch and thinier
						foreach (Word prettyWord in prettyWords) {
							int positionIndex = words.IndexOf(prettyWord);
							if (((positionIndex - 1) < 0 || !negationWords.Contains(words[positionIndex - 1])) && ((positionIndex - 2 < 0) || !negationWords.Contains(words[positionIndex - 2]))) {
								float temp = (totalWordsCounter / prettyWord.timesUsed) * prettyWord.weight;
								growingHighFactor += (temp / totalWordsCounter) / prettyWords.Count;
								if (growingHighFactor > GameConstants.MAX_GROWING_HIGH_FACTOR) {
									growingHighFactor = GameConstants.MAX_GROWING_HIGH_FACTOR;
								}
								growingLengthFactor += (temp / totalWordsCounter) / prettyWords.Count;
								if (growingLengthFactor > GameConstants.MAX_GROWING_LENGTH_FACTOR) {
									growingLengthFactor = GameConstants.MAX_GROWING_LENGTH_FACTOR;
								}
								growingBranchFactor -= ((temp * 0.8f) / totalWordsCounter) / prettyWords.Count;
								if (growingBranchFactor < GameConstants.MIN_GROWING_BRANCH_FACTOR) {
									growingBranchFactor = GameConstants.MIN_GROWING_BRANCH_FACTOR;
								}
								growingRadiusFactor -= growingHighFactor;
								if (growingRadiusFactor < GameConstants.MIN_GROWING_RADIUS_FACTOR) {
									growingRadiusFactor = GameConstants.MIN_GROWING_RADIUS_FACTOR;
								}
								//Debug.Log("Pretty word");
								
								//Play ambiance
								//Fade in 5 sec
								fadeInPrettyLoop = true;
								audioLoopPrettyTargetVolumeFadeIn = 0.98f;
								
								//Decrease grow ambiance to 0.93
								if (audioLoopGrow.isPlaying) {
									//Fade out 5 sec
									fadeOutGrowLoop = true;
									audioLoopGrowTargetVolumeFadeOut = 0.93f;
								}
								
								//Decrease strength ambiance to 0
								if (audioLoopStrength.isPlaying) {
									//Fade out
									fadeOutStrengthLoop = true;
									audioLoopStrengthTargetVolumeFadeOut = 0;
								}
								
								//Decrease majesty ambiance to 0
								if (audioLoopMajesty.isPlaying) {
									//Fade out
									fadeOutMajestyLoop = true;
									audioLoopMajestyTargetVolumeFadeOut = 0;
								}
								
								//Decrease hateful ambiance to 0
								if (audioLoopHateful.isPlaying) {
									//Fade out 5 sec
									fadeOutHatefulLoop = true;
									audioLoopHatefulTargetVolumeFadeOut = 0;
								}
							}
						}
						
						//Make the plant grow more branches, prettier, larger and with way more leaves
						foreach (Word majestyWord in majestyWords) {
							int positionIndex = words.IndexOf(majestyWord);
							if ((positionIndex - 1) < 0 || !negationWords.Contains(words[positionIndex - 1])) {
								//Check the presence of a majestic adverb or noun after and before the strength or pretty word
								positionIndex = words.IndexOf(majestyWord);
								if ((positionIndex + 1 < words.Count && strengthWords.Contains(words[positionIndex + 1]) || (positionIndex - 1 > 0 && strengthWords.Contains(words[positionIndex - 1]))) 
									&& (majestyWord.tag.Equals(GameConstants.ADVERB_WORD) || majestyWord.tag.Equals(GameConstants.NOUN_WORD))) {
									float temp = (totalWordsCounter / majestyWord.timesUsed) * (majestyWord.weight * 1.5f);
									growingLeaveFactor += (temp / totalWordsCounter) / majestyWords.Count;
									if (growingLeaveFactor > GameConstants.MAX_GROWING_LEAVE_FACTOR) {
										growingLeaveFactor = GameConstants.MAX_GROWING_LEAVE_FACTOR;
									}
									growingBranchFactor *= (temp / totalWordsCounter) / majestyWords.Count;
									if (growingBranchFactor > GameConstants.MAX_GROWING_BRANCH_FACTOR) {
										growingBranchFactor = GameConstants.MAX_GROWING_BRANCH_FACTOR;
									}
									growingLargeFactor *= 1 + growingBranchFactor;
									if (growingLargeFactor > GameConstants.MAX_GROWING_LARGE_FACTOR) {
										growingLargeFactor = GameConstants.MAX_GROWING_LARGE_FACTOR;
									}
									//Debug.Log("Majesty strength bonus!");
									
									//Play majesty positive feedback
									audioFeedback.PlayOneShot(majestyFeedback);
									
									//Play ambiance
									//Fade in 5 sec
									fadeInMajestyLoop = true;
									audioLoopMajestyTargetVolumeFadeIn = 0.98f;
									
									//Decrease grow ambiance to 0.93
									if (audioLoopGrow.isPlaying) {
										//Fade out 5 sec
										fadeOutGrowLoop = true;
										audioLoopGrowTargetVolumeFadeOut = 0.93f;
									}
									
									//Decrease pretty ambiance to 0.93
									if (audioLoopPretty.isPlaying) {
										//Fade out 5 sec
										fadeOutPrettyLoop = true;
										audioLoopPrettyTargetVolumeFadeOut = 0.93f;
									}
									
									//Decrease strength ambiance to 0.93
									if (audioLoopStrength.isPlaying) {
										//Fade out 5 sec
										fadeOutStrengthLoop = true;
										audioLoopStrengthTargetVolumeFadeOut = 0.93f;
									}
									
									//Decrease hateful ambiance to 0
									if (audioLoopHateful.isPlaying) {
										//Fade out 5 sec
										fadeOutHatefulLoop = true;
										audioLoopHatefulTargetVolumeFadeOut = 0;
									}
								}
								if ((positionIndex + 1 < words.Count && prettyWords.Contains(words[positionIndex + 1]) || (positionIndex - 1 > 0 && prettyWords.Contains(words[positionIndex - 1]))) 
									&& (majestyWord.tag.Equals(GameConstants.ADVERB_WORD) || majestyWord.tag.Equals(GameConstants.NOUN_WORD))) {
									float temp = (totalWordsCounter / majestyWord.timesUsed) * (majestyWord.weight * 1.5f);
									growingLeaveFactor += (temp / totalWordsCounter) / majestyWords.Count;
									if (growingLeaveFactor > GameConstants.MAX_GROWING_LEAVE_FACTOR) {
										growingLeaveFactor = GameConstants.MAX_GROWING_LEAVE_FACTOR;
									}
									//TODO less branch, larger
									growingBranchFactor *= (temp / totalWordsCounter) / majestyWords.Count;
									if (growingBranchFactor > GameConstants.MAX_GROWING_BRANCH_FACTOR) {
										growingBranchFactor = GameConstants.MAX_GROWING_BRANCH_FACTOR;
									}
									growingHighFactor *= 1 + growingBranchFactor;
									if (growingHighFactor > GameConstants.MAX_GROWING_HIGH_FACTOR) {
										growingHighFactor = GameConstants.MAX_GROWING_HIGH_FACTOR;
									}
									//Debug.Log("Majesty pretty bonus!");
									
									//Play majesty positive feedback
									audioFeedback.PlayOneShot(majestyFeedback);
									
									//Play ambiance
									//Fade in 5 sec
									fadeInMajestyLoop = true;
									audioLoopMajestyTargetVolumeFadeIn = 0.98f;
									
									//Decrease grow ambiance to 0.93
									if (audioLoopGrow.isPlaying) {
										//Fade out 5 sec
										fadeOutGrowLoop = true;
										audioLoopGrowTargetVolumeFadeOut = 0.93f;
									}
									
									//Decrease pretty ambiance to 0.93
									if (audioLoopPretty.isPlaying) {
										//Fade out 5 sec
										fadeOutPrettyLoop = true;
										audioLoopPrettyTargetVolumeFadeOut = 0.93f;
									}
									
									//Decrease strength ambiance to 0.93
									if (audioLoopStrength.isPlaying) {
										//Fade out 5 sec
										fadeOutStrengthLoop = true;
										audioLoopStrengthTargetVolumeFadeOut = 0.93f;
									}
									
									//Decrease hateful ambiance to 0
									if (audioLoopHateful.isPlaying) {
										//Fade out 5 sec
										fadeOutHatefulLoop = true;
										audioLoopHatefulTargetVolumeFadeOut = 0;
									}
								}
							}
						}
					}
				}
			}
			
			if (!died) {
				//If no words are recongnized, let it grow a bit better than the min
				//Check the syntax and reset factors if it's not well formed
				if ((words.Count > 0 && unknownWords.Count / (words.Count + unknownWords.Count) > 0.3) || (!phraseTypes.Contains(GameConstants.NOUN_PHRASE_B) && !phraseTypes.Contains(GameConstants.VERB_PHRASE_B)
					&& !phraseTypes.Contains(GameConstants.ADJECTIVE_PHRASE_B) && !phraseTypes.Contains(GameConstants.NOUN_PHRASE_I) && !phraseTypes.Contains(GameConstants.VERB_PHRASE_I) 
					&& !phraseTypes.Contains(GameConstants.ADJECTIVE_PHRASE_I) && (tags.Count == 1 && !tags[0].Equals(GameConstants.VERB_WORD)))) {
					//Debug.Log("Unknown words or sentence badly built");
					
					//Play negative feedback
					if (UnityEngine.Random.value > 0.5) {
						audioFeedback.PlayOneShot(negativeFeedback1);
					} else {
						audioFeedback.PlayOneShot(negativeFeedback2);
					}
					///Play ambiance
					//Fade in 5 sec
					fadeInHatefulLoop = true;
					audioLoopHatefulTargetVolumeFadeIn = 0.98f;
					
					//Decrease grow ambiance to 0
					if (audioLoopGrow.isPlaying) {
						//Fade out 5 sec
						fadeOutGrowLoop = true;
						audioLoopGrowTargetVolumeFadeOut = 0;
					}
					
					//Decrease pretty ambiance to 0
					if (audioLoopPretty.isPlaying) {
						//Fade out 5 sec
						fadeOutPrettyLoop = true;
						audioLoopPrettyTargetVolumeFadeOut = 0;
					}
					
					//Decrease strength ambiance to 0
					if (audioLoopStrength.isPlaying) {
						//Fade out 5 sec
						fadeOutStrengthLoop = true;
						audioLoopStrengthTargetVolumeFadeOut = 0;
					}
					
					//Decrease majesty ambiance to 0
					if (audioLoopMajesty.isPlaying) {
						//Fade out 5 sec
						fadeOutMajestyLoop = true;
						audioLoopMajestyTargetVolumeFadeOut = 0;
					}
					
					growingLengthFactor = GameConstants.INITIAL_MINIMUM_GROWING_LENGTH_FACTOR - 0.2f;
					growingRadiusFactor = GameConstants.INITIAL_MINIMUM_GROWING_RADIUS_FACTOR - 0.2f;
					growingLeaveFactor = GameConstants.MIN_GROWING_LEAVE_FACTOR;
					growingBranchFactor = GameConstants.MIN_GROWING_BRANCH_FACTOR;
					growingLargeFactor = GameConstants.MIN_GROWING_LARGE_FACTOR;
					growingHighFactor = GameConstants.MIN_GROWING_HIGH_FACTOR;
					branch.colorDecay -= 0.04f;
					branch.lengthDecay -= 0.04f;
					branch.radiusDecay -= 0.04f;
				}
				//Check that a modal word is present and make the 
				if (tags.Contains(GameConstants.MODAL_WORD)) {
					//TODO
				}
				//TODO Check the presence of a question mark and do something about it
				
				//Debug.Log("End of processing sentences");
				
				lastBranches.AddRange(branch.grow (rootNode, growingLengthFactor, growingRadiusFactor, growingBranchFactor, growingLargeFactor, growingHighFactor));
				lastBranches.Remove(branch);
				
			}
		}
		
		//Reset the plant, the words lists etc.
		public void reset ()
		{
			//(GetComponent("Player") as Player).reset();
			rootNode.reset();
			rootNode = transform.FindChild("RootNode").GetComponent("Branch") as Branch;
			rootNode.branch = branch;
			lastBranches = new List<Branch>();
			lastBranches.Add(rootNode);
			rootNode.construct (null, null, GameConstants.INITIAL_BRANCH_LENGTH, GameConstants.INITIAL_BRANCH_RADIUS, GameConstants.INITIAL_MINIMUM_ROTATION, GameConstants.INITIAL_MAXIMUM_ROTATION, GameConstants.INITIAL_BRANCH_COLOR_YOUNG, true);
		}
		
		bool fadeIn(float time, AudioSource audioSource, float targetVolume) {
			if (!audioSource.isPlaying) {
				audioSource.Play();
				audioSource.volume = 0;
			}
			if (audioSource.volume < targetVolume) {
				audioSource.volume += time * Time.deltaTime;
				return true;
			} else {
				return false;
			}
		}
		
		bool fadeOut(float time, AudioSource audioSource, float targetVolume) {
			if(audioSource.volume > targetVolume) {
				audioSource.volume -= time * Time.deltaTime;
				return true;
			} else {
				if (audioSource.volume <= 0) {
					audioSource.Stop();
				}
				return false;
			}
		}
		
		/// <summary>
		/// Fills the corpus.
		/// </summary>
		/// <param name='reader'>
		/// Reader.
		/// </param>
		private void fillCorpus (StreamReader reader, string corpusType)
		{
			string line;
			while ((line = reader.ReadLine()) != null) {
				string[] lineArray = line.Split(':');
				int weight = 1;
				if (lineArray.Length > 1) {
					weight = Convert.ToInt16(lineArray[1]);
				}
				switch(corpusType) {
					case GameConstants.CORPUS_DEATH:
						deathCorpus.Add (new Word(lineArray[0], weight));
						break;
					case GameConstants.CORPUS_NEGATION:
						negationCorpus.Add (new Word(lineArray[0], weight));
						break;
					case GameConstants.CORPUS_HATEFUL:
						hatefulCorpus.Add (new Word(lineArray[0], weight));
						break;
					case GameConstants.CORPUS_GROW:
						growCorpus.Add (new Word(lineArray[0], weight));
						break;
					case GameConstants.CORPUS_MAJESTY:
						majestyCorpus.Add (new Word(lineArray[0], weight));
						break;
					case GameConstants.CORPUS_STRENGTH:
						strengthCorpus.Add (new Word(lineArray[0], weight));
						break;
					case GameConstants.CORPUS_PRETTY:
						prettyCorpus.Add (new Word(lineArray[0], weight));
						break;
				}
			}
		}
		
		/// <summary>
		/// Retrieves the word.
		/// </summary>
		/// <returns>
		/// The word.
		/// </returns>
		/// <param name='token'>
		/// If set to <c>true</c> token.
		/// </param>
		/// <param name='corpus'>
		/// If set to <c>true</c> corpus.
		/// </param>
		/// <param name='wordsList'>
		/// If set to <c>true</c> words list.
		/// </param>
		private bool retrieveWord (string token, string tag, List<Word> corpus, List<Word> specificWordsList, List<Word> wordsList)
		{
			Word word = retrieveBestMatchingWord (token, corpus, GameConstants.FUZZINESS);
			if (word != null) {
				word.timesUsed++;
				word.tag = tag;
				specificWordsList.Add (word);
				wordsList.Add (word);
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Levenshteins the distance.
		/// </summary>
		/// <returns>
		/// The distance.
		/// </returns>
		/// <param name='src'>
		/// Source.
		/// </param>
		/// <param name='dest'>
		/// Destination.
		/// </param>
		private int LevenshteinDistance (string src, string dest)
		{
			int[,] d = new int[src.Length + 1, dest.Length + 1];
			int i, j, cost;
			char[] str1 = src.ToCharArray ();
			char[] str2 = dest.ToCharArray ();

			for (i = 0; i <= str1.Length; i++) {
				d [i, 0] = i;
			}
			for (j = 0; j <= str2.Length; j++) {
				d [0, j] = j;
			}
			for (i = 1; i <= str1.Length; i++) {
				for (j = 1; j <= str2.Length; j++) {

					if (str1 [i - 1] == str2 [j - 1])
						cost = 0;
					else
						cost = 1;

					d [i, j] =
                Math.Min (
                    d [i - 1, j] + 1, // Deletion
                    Math.Min (
                        d [i, j - 1] + 1, // Insertion
                        d [i - 1, j - 1] + cost)); // Substitution

					if ((i > 1) && (j > 1) && (str1 [i - 1] == 
                str2 [j - 2]) && (str1 [i - 2] == str2 [j - 1])) {
						d [i, j] = Math.Min (d [i, j], d [i - 2, j - 2] + cost);
					}
				}
			}

			return d [str1.Length, str2.Length];
		}
	
		/// <summary>
		/// Retrieves the best matching word.
		/// </summary>
		/// <returns>
		/// The best matching word.
		/// </returns>
		/// <param name='word'>
		/// Word.
		/// </param>
		/// <param name='wordList'>
		/// Word list.
		/// </param>
		/// <param name='fuzzyness'>
		/// Fuzzyness.
		/// </param>
		public Word retrieveBestMatchingWord (string word, List<Word> wordList, double fuzzyness)
		{
			Word foundWord = null;
			double bestScore = 0.0;
			foreach (Word w in wordList) {
				// Calculate the Levenshtein-distance:
				int levenshteinDistance = LevenshteinDistance (word, w.stringValue);

				// Length of the longer string:
				int length = Math.Max (word.Length, w.stringValue.Length);

				// Calculate the score:
				double score = 1.0 - (double)levenshteinDistance / length;

				// Match?
				if (score > fuzzyness) {
					if (score > bestScore) {
						bestScore = score;
						foundWord = w;
					}
				}
			}
			return foundWord;
		}
		
		public List<Word> getDeathCorpus ()
		{
			return deathCorpus;
		}
		
		public List<Word> getStrengthCorpus ()
		{
			return strengthCorpus;
		}
		
		public List<Word> getPrettyCorpus ()
		{
			return prettyCorpus;
		}
	}
}

