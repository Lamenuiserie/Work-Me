using NaturalLanguageProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSystem;
using Parameters;

public class Player : MonoBehaviour {
	
	public Transform camera;
	public GUISkin skin;
	public static float yScale = 1f;
	private string stringToEdit;
	private EntityExtractor extractor;
	private GUIText textComponent;

	/// <summary>
	/// Create the plant, the language processor and the player.
	/// </summary>
	void Start () {
		reset();
	}
	
	public void reset () {
		string language = (GameObject.Find("Controller").GetComponent("CommunicationProcessing") as CommunicationProcessing).language;
		extractor = new EntityExtractor(GameConstants.ENGLISH_LANGUAGE);
		textComponent = GetComponent("GUIText") as GUIText;

		stringToEdit = "Write me something";
		if (language.Equals(GameConstants.FRENCH_LANGUAGE)) {
			stringToEdit = "Ecris-moi quelque chose";
		}
		
		textComponent.material.color = new Color (0, 0, 0);
		textComponent.text = stringToEdit;
	}
	
	void OnGUI () {
		GUI.skin = skin;
		GUI.SetNextControlName ("InputPlayer");
		GUI.FocusControl ("InputPlayer");
		
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
			//Debug.Log("Enter key pressed");
			
			//TODO check if it works in french
			
			//Extract sentences
			string[] sentences = extractor.extractSentences(stringToEdit);
			//Extract tokens, only take into account the first sentence
			string[] tokens = extractor.tokenize(sentences[0]);
			//Extract tags of part-of-speech
			string[] tags = extractor.posTag(tokens);
			//Chunk the tags and tokens together
			string[] chunk = extractor.chunk(tokens, tags);
			
			if (tokens.Length > 0) {
				//Process the tokens retrieved
				(GameObject.Find("Controller").GetComponent("CommunicationProcessing") as CommunicationProcessing).processInput(new List<string> (tokens), new List<string> (chunk), new List<string> (tags));
			}
			
			//Reset input text field
			stringToEdit = GUI.TextField(new Rect((Screen.width / 2) - 250, Screen.height - 40, 500, 20), "");
			textComponent.text = stringToEdit;
		}
		else {
			stringToEdit = GUI.TextField(new Rect((Screen.width / 2) - 250, Screen.height - 40, 500, 20), stringToEdit);
			textComponent.text = stringToEdit;
		}
	}
	
	IEnumerator doRotate() {
		camera.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, 20 * Time.deltaTime);
		//TODO how to make it work without being blocked by branch growth
		yield return new WaitForSeconds(0.1f);
	}

	// Update is called once per frame
	void Update () {
		StartCoroutine(doRotate());
		//TODO it stops because of the other other coroutine, check docs for multiple coroutines
		
		//Transfer input string to the GUIText
		foreach (char c in Input.inputString) {
            // Backspace - Remove the last character
            if (c.Equals("\b"[0])) {
                if (textComponent.text.Length != 0) {
                    textComponent.text = textComponent.text.Substring(0, textComponent.text.Length - 1);
				}
            }
			// Normal text input - just append to the end
            else {
                textComponent.text += c;
            }
        }
	}
}
