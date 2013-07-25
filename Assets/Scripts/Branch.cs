using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parameters;

namespace LSystem
{
	public class Branch : MonoBehaviour 
	{
		/// <summary>
		/// The sub L-Systems of the L-System.
		/// </summary>
		private List<Branch> branches;
		private int age = 0;
		public Branch branch;
		public float branchColor = GameConstants.INITIAL_BRANCH_COLOR_VALUE_YOUNG;
		public Vector3 branchRotationGap = new Vector3 (0.1f, 0, 0.1f);
		/// <summary>
		/// 
		/// </summary>
		private GameObject contents;
		private GameObject contentsRoot;
		/// <summary>
		/// Appearance of the current L-System.
		/// </summary>
		private GameObject appearance;
		/// <summary>
		/// Parent of this current L-System.
		/// </summary>
		public Branch parent = null;
		public Vector3 computedRotation;
		/// <summary>
		/// The factor used to know how much the branches colors becomes shallower.
		/// </summary>
		public float colorDecay = 0.98f;
		/// <summary>
		/// Factor diminishing the length of the current L-System compared to the previous one.
		/// </summary>
		public float lengthDecay = 0.80f;
		/// <summary>
		/// Factor diminishing the radius of the current L-System compared to the previous one.
		/// </summary>
		public float radiusDecay = 0.80f;
		/// <summary>
		/// Minimum radius for the current L-System.
		/// </summary>
		public float minimumRadius = 0.01f;
		public float minimumLength = 0.08f;
		private float computedLength = 0.0f;
		private float computedRadius = 0.0f;
		private float actualMinimumRadiusFactor = GameConstants.INITIAL_MINIMUM_GROWING_RADIUS_FACTOR;
		private float actualMinimumLengthFactor = GameConstants.INITIAL_MINIMUM_GROWING_LENGTH_FACTOR;
		private float actualMaximumRadiusFactor = GameConstants.MAX_GROWING_RADIUS_FACTOR;
		private float actualMaximumLengthFactor = GameConstants.MAX_GROWING_LENGTH_FACTOR;
		public float minimumRotation = 10.0f;
		public float maximumRotation = 60.0f;
		public float actualMinimumRotation = 1.0f;
		public float actualMaximumRotation = 70.0f;
		private bool growing = false;
		private float growingRadius = 0.0f;
		private float growingLength = 0.0f;
		private Color growingColor = new Color(0, 0, 0);
		private bool colorChanging = false;
		private Branch parentToChange = null;
		private Color colorToChange = new Color(0, 0, 0);
		private bool old = false;
		
		void Awake () {
			//Init
			growing = false;
			growingRadius = 0.0f;
			growingLength = 0.0f;
			growingColor = new Color(0, 0, 0);
			parent = null;
			parentToChange = null;
			colorChanging = false;
			colorToChange = new Color(0, 0, 0);
		}
		
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			if (growing) {
				StartCoroutine(growCubes(growingRadius, growingLength, growingColor));
				growing = false;
				growingRadius = 0.0f;
				growingLength = 0.0f;
				growingColor = new Color(0, 0, 0);
			}
			if (colorChanging) {
				StartCoroutine(changeColor(parentToChange, colorToChange));
				parentToChange = null;
				colorChanging = false;
				colorToChange = new Color(0, 0, 0);
			}
		}
		
		/// <summary>
		/// Grow new branches according to the factors given.
		/// </summary>
		/// <param name='growingFactor'>
		/// Growing factor.
		/// </param>
		/// <param name='growingLargeFactor'>
		/// Growing large factor.
		/// </param>
		/// <param name='growingHighFactor'>
		/// Growing high factor.
		/// </param>
		/// <param name='growingMessyFactor'>
		/// Growing messy factor.
		/// </param>
		public List<Branch> grow (Branch rootNode, float growingLengthFactor, float growingRadiusFactor, float growingBranchFactor, float growingLargeFactor, float growingHighFactor)
		{
			Debug.Log("--- Start of growing branches ---");
			
			//TODO change all colors over time
			
			//Compute the angle at which it will grow
			bool growWell = false;
			if (parent != null && growingLargeFactor > GameConstants.MIN_GROWING_LARGE_FACTOR) {
				growWell = true;
			}
			float minRotation = ((maximumRotation * growingLargeFactor) + minimumRotation) / 2;
			float maxRotation = ((maximumRotation * (1 - growingHighFactor)) + maximumRotation) / 2;
			if (minRotation > maxRotation) {
				float temp = minRotation;
				minRotation = maxRotation;
				maxRotation = temp;
			}
			//Compute the number of children according to the groingFactor and the growingLargeFactor
			float actualBranchFactor = Random.Range(GameConstants.MIN_GROWING_BRANCH_FACTOR + 0.5f, GameConstants.MAX_GROWING_BRANCH_FACTOR);
			if (growingBranchFactor > GameConstants.MIN_GROWING_RADIUS_FACTOR) {
				actualBranchFactor = Random.Range(0.8f * growingBranchFactor, growingBranchFactor);
			}
			float numChildren = Mathf.RoundToInt((GameConstants.MAX_BRANCH_NUMBER * (actualBranchFactor + (growingLargeFactor / 2))));
			if (numChildren < GameConstants.MIN_BRANCH_NUMBER) {
				numChildren = GameConstants.MIN_BRANCH_NUMBER;
			}
			if (numChildren > GameConstants.MAX_BRANCH_NUMBER) {
				numChildren = GameConstants.MAX_BRANCH_NUMBER;
			}
			
			//Increase the age of the branch
			age++;
			//Decrease the level of green in the color of the child branches and the parents
			if (parent != null) {
				actualMinimumRadiusFactor = parent.actualMinimumRadiusFactor * radiusDecay;
				actualMaximumRadiusFactor = parent.actualMaximumRadiusFactor * radiusDecay;
				actualMinimumLengthFactor = parent.actualMinimumLengthFactor * lengthDecay;
				actualMaximumLengthFactor = parent.actualMaximumLengthFactor * lengthDecay;
				contentsRoot = parent.contentsRoot;
				
				//TODO Change color with a yield or in the update
				
				Branch prevParent = this;
				while (prevParent != null) {
					prevParent.branchColor *= colorDecay;
					Color newColor = GameConstants.INITIAL_BRANCH_COLOR_YOUNG;
					if (prevParent.old && prevParent.branchColor > GameConstants.MAX_BRANCH_COLOR_VALUE_OLD) {
						newColor = GameConstants.INITIAL_BRANCH_COLOR_OLD;
						newColor.r = prevParent.branchColor;
					} else if (prevParent.old && prevParent.branchColor < GameConstants.MAX_BRANCH_COLOR_VALUE_OLD) {
						newColor = GameConstants.INITIAL_BRANCH_COLOR_OLD;
						newColor.r = GameConstants.MAX_BRANCH_COLOR_VALUE_OLD;
						prevParent.branchColor = newColor.r;
					} else if (!prevParent.old) {
						newColor = GameConstants.INITIAL_BRANCH_COLOR_YOUNG;
						newColor.g = prevParent.branchColor;
					}
					if (!prevParent.old && prevParent.branchColor < GameConstants.MAX_BRANCH_COLOR_VALUE_YOUNG) {
						newColor = GameConstants.INITIAL_BRANCH_COLOR_OLD;
						prevParent.branchColor = newColor.r;
						prevParent.old = true;
					}
					prevParent.colorChanging = true;
					prevParent.colorToChange = newColor;
					prevParent.parentToChange = prevParent;
					prevParent = prevParent.parent;
				}
			} else {
				actualMinimumRadiusFactor *= radiusDecay;
				actualMaximumRadiusFactor *= radiusDecay;
				actualMinimumLengthFactor *= lengthDecay;
				actualMaximumLengthFactor *= lengthDecay;
				parent = rootNode;
				contentsRoot = rootNode.contentsRoot;
				rootNode.parent = null;
				rootNode.branchColor *= colorDecay;
				Color newColor = GameConstants.INITIAL_BRANCH_COLOR_YOUNG;
				newColor.g = rootNode.branchColor;
				colorChanging = true;
				colorToChange = newColor;
				parentToChange = rootNode;
			}
			float childBranchColor = branchColor * colorDecay;
			
			//Creation of the L-System
			branches = new List<Branch> ();
			Branch lastChild = null;
			GameObject progenitor = new GameObject ();
			progenitor.name = "Root for children";
			progenitor.transform.parent = contents.transform;
			progenitor.transform.localPosition = new Vector3 (0, 0, 0);
			progenitor.transform.localEulerAngles = new Vector3 (0, 0, 0);
			progenitor.transform.Translate (0, 2 * appearance.transform.localPosition.y, 0);
			for (int i = 0; i < numChildren; i++) {
				Branch child = Instantiate(branch) as Branch;
				child.branch = branch;
				child.contentsRoot = contentsRoot;
				branches.Add (child);
				child.parent = this;
				child.branchColor = childBranchColor;
				
				Color newColor = GameConstants.INITIAL_BRANCH_COLOR_YOUNG;
				newColor.g = childBranchColor;
				
				//Change radius of the next branch only if its factor was modified
				float actualRadiusFactor = Random.Range(actualMinimumRadiusFactor, actualMaximumRadiusFactor);
				if (growingRadiusFactor > GameConstants.INITIAL_MINIMUM_GROWING_RADIUS_FACTOR) {
					float minGrowingRadiusFactor = actualMinimumRadiusFactor;
					float maxGrowingRadiusFactor = actualMaximumRadiusFactor;
					if (growingRadiusFactor > actualMaximumRadiusFactor) {
						growingRadiusFactor = actualMaximumRadiusFactor;
						minGrowingRadiusFactor = (growingRadiusFactor * actualMinimumRadiusFactor) / actualMaximumRadiusFactor;
					} else if (growingRadiusFactor < actualMinimumRadiusFactor) {
						growingRadiusFactor = actualMinimumRadiusFactor;
						maxGrowingRadiusFactor = (growingRadiusFactor * actualMaximumRadiusFactor) / actualMinimumRadiusFactor;
					} else {
						minGrowingRadiusFactor = growingRadiusFactor;
						maxGrowingRadiusFactor = growingRadiusFactor;
					}
					actualRadiusFactor = Random.Range(0.8f * minGrowingRadiusFactor, maxGrowingRadiusFactor);
				}
				
				//float newRadius = appearance.transform.localScale.x * (radiusDecay * actualRadiusFactor);
				float newRadius = appearance.transform.localScale.x * actualRadiusFactor;
				//Check that the decay does not make the radius too small
				if (newRadius < minimumRadius) {
					newRadius = minimumRadius;
				}
				//Change length of the next branch only if its factor was modified
				float actualLengthFactor = Random.Range(actualMinimumLengthFactor, actualMaximumLengthFactor);
				if (growingLengthFactor > GameConstants.INITIAL_MINIMUM_GROWING_LENGTH_FACTOR) {
					float minGrowingLengthFactor = actualMinimumLengthFactor;
					float maxGrowingLengthFactor = actualMaximumLengthFactor;
					if (growingLengthFactor > actualMaximumLengthFactor) {
						//growingLengthFactor = actualMaximumLengthFactor;
						actualMaximumLengthFactor = growingLengthFactor * lengthDecay;
						maxGrowingLengthFactor = actualMaximumLengthFactor;
						minGrowingLengthFactor = (growingLengthFactor * actualMinimumLengthFactor) / actualMaximumLengthFactor;
					} else if (growingLengthFactor < actualMinimumLengthFactor) {
						growingLengthFactor = actualMinimumLengthFactor;
						maxGrowingLengthFactor = (growingLengthFactor * actualMaximumLengthFactor) / actualMinimumLengthFactor;
					} else {
						minGrowingLengthFactor = growingLengthFactor;
						maxGrowingLengthFactor = growingLengthFactor;
					}
					if (maxGrowingLengthFactor > actualMaximumLengthFactor) {
						maxGrowingLengthFactor = actualMaximumLengthFactor;
					}
					actualLengthFactor = Random.Range(minGrowingLengthFactor, maxGrowingLengthFactor);
				}
				
				
				//TODO limit length by a decreased length decay from age
				
				
				//float newLength = appearance.transform.localScale.y * (lengthDecay * actualLengthFactor);
				float newLength = appearance.transform.localScale.y * actualLengthFactor;
				//Check that the decay does not make the length too small
				if (newLength < minimumLength) {
					newLength = minimumLength;
				}
				
				Debug.Log("* Number of children: " + numChildren);
				Debug.Log("* New length: " + newLength);
				Debug.Log("* New radius: " + newRadius);
				Debug.Log("* Min rotation: " + minRotation);
				Debug.Log("* Max rotation: " + maxRotation);
				
				Debug.Log("--- End of growing branches ---");
				
				child.construct (progenitor, lastChild, newLength, newRadius, minRotation, maxRotation, newColor, growWell);
				lastChild = child;
			}
			return branches;
		}
	
		public void pivot ()
		{
			contents.transform.Rotate (0, .1f, 0);
		}
	
		public void doRotate (float amt)
		{
			contents.transform.Rotate (0, amt, 0);
			if (branches == null)
				return;
			for (int i = 0; i < branches.Count; i++) {
				branches [i].doRotate (amt);
			}
		}
	
		public void reset ()
		{
			GameObject.Destroy (contents);
			GameObject.Destroy (contentsRoot);
			branches = new List<Branch> ();
			branchColor = GameConstants.INITIAL_BRANCH_COLOR_VALUE_YOUNG;
			age = 0;
			parent = null;
		}
		
		/// <summary>
		/// Grows the cubes progressively.
		/// </summary>
		/// <param name='radius'>
		/// Radius.
		/// </param>
		/// <param name='length'>
		/// Length.
		/// </param>
		/// <param name='color'>
		/// Color.
		/// </param>
		IEnumerator growCubes (float radius, float length, Color color) {
			Transform parentToAssociate = contents.transform;
			if (parent == null) {
				parentToAssociate = contentsRoot.transform;
			}
			Vector3 scaleVector = new Vector3 (radius / 2, length / ((length / radius) * 2), radius / 2);
			float cubeSide = length / ((length / radius) * 2);
			float cubeSideXZ = radius / 2;
			float posX = -cubeSideXZ / 2;
			float posY = cubeSide / 2;
			float posZ = -cubeSideXZ / 2;
			for (int i = 0; i < (length / radius) * 2; i++) {
				for (int j = 0; j < 2; j++) {
					for (int k = 0; k < 2; k++) {
						GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
						cube.name = "Cube";
						cube.renderer.material = (Material)Resources.Load("Materials/Branch", typeof(Material));
						cube.renderer.material.color = color;
						cube.transform.parent = parentToAssociate;
						cube.transform.localPosition = new Vector3 (posX, posY, posZ);
						cube.transform.localEulerAngles = new Vector3 (0, 0, 0);
						cube.transform.localScale = scaleVector;
						posZ += cubeSideXZ;
					}
					posX += cubeSideXZ;
					posZ = -cubeSideXZ / 2;
				}
				yield return new WaitForSeconds(0.00001f);
				posX = -cubeSideXZ / 2;
				posY += cubeSide;
				posZ = -cubeSideXZ / 2;
			}
		}
		
		IEnumerator changeColor (Branch parent, Color color) {
			Component[] renderers = parent.contents.GetComponentsInChildren<Renderer>();
			if (parent.parent == null) {
				renderers = parent.contentsRoot.GetComponentsInChildren<Renderer>();
			}
			foreach (Renderer rdr in renderers) {
				rdr.material.color = color;
				yield return new WaitForSeconds(0.00001f);
			}
		}
		
		/// <summary>
		/// Build the plant.
		/// </summary>
		/// <param name='parentTree'>
		/// Parent tree.
		/// </param>
		/// <param name='length'>
		/// Length.
		/// </param>
		/// <param name='radius'>
		/// Radius.
		/// </param>
		/// <param name='color'>
		/// Color.
		/// </param>
		public void construct (GameObject parentTree, Branch previousBranch, float length, float radius, float minRotation, float maxRotation, Color color, bool isGrowingWell)
		{
			//Debug.Log("--- Start of constructing a branch ---");
			
			//Actual structure of the branch
			contents = new GameObject ();
			contents.name = "Contents";
			//Container for the branch cubes
			appearance = new GameObject ();
			if (parentTree != null) {
				contents.transform.parent = parentTree.transform;
			} else {
				contentsRoot = new GameObject ();
				contentsRoot.name = "ContentsRoot";
				contentsRoot.transform.parent = contents.transform;
			}
			contents.transform.localPosition = new Vector3 (0, 0, 0);
			contents.transform.localEulerAngles = new Vector3 (0, 0, 0);
			appearance.transform.parent = contents.transform;
			appearance.transform.localPosition = new Vector3 (0, 0, 0);
			appearance.transform.localEulerAngles = new Vector3 (0, 0, 0);
			
			//If there is no parent tree then it is the first branch and it must not have any rotations
			if (parentTree != null) {
				//Compute the minimum and maximum rotations between the maximum value given and the actual maximum value
				float minMinRotation = (minRotation + actualMaximumRotation) / 100;
				float maxMinRotation = (((minRotation * actualMaximumRotation) / maximumRotation) + actualMaximumRotation) / 100;
				float newMinRotation = Random.Range(minMinRotation, maxMinRotation);
				float minMaxRotation = (maxRotation + actualMaximumRotation) / 100;
				float maxMaxRotation = (((maxRotation * actualMaximumRotation) / maximumRotation) + actualMaximumRotation) / 100;
				float newMaxRotation = Random.Range(minMaxRotation, maxMaxRotation);
				
				while (newMinRotation > newMaxRotation) {
					newMinRotation = Random.Range(minMinRotation, maxMinRotation);
					newMaxRotation = Random.Range(minMaxRotation, maxMaxRotation);
				}
				
				//Check if a previous branch of the same parent was created before and retrieve the rotations values
				float lastBranchXRotation = 0;
				float lastBranchZRotation = 0;
				if (previousBranch != null) {
					lastBranchXRotation = previousBranch.computedRotation.x;
					lastBranchZRotation = previousBranch.computedRotation.z;
					//TODO If no strength or pretty words were used let branches grow into each other
				}
				
				//Find the rotation on x between the computed min and max rotations
				float rotationX = Random.Range(newMinRotation, newMaxRotation);
				//Find the rotation on z between the computed min and max rotations
				float rotationZ = Random.Range(newMinRotation, newMaxRotation);
				
				//Prevent branches from growing aside or even inside each other
				if (Mathf.Abs(rotationX - lastBranchXRotation) < 0.5 && Mathf.Abs(rotationZ - lastBranchZRotation) < 0.5) {
					if (Random.value > 0.5) {
						rotationX = Random.Range(newMinRotation, newMaxRotation);
						if (Mathf.Abs(newMinRotation - lastBranchXRotation) > 0.5 || Mathf.Abs(newMaxRotation - lastBranchXRotation) > 0.5) {
							while (Mathf.Abs(rotationX - lastBranchXRotation) < 0.5) {
								rotationX = Random.Range(newMinRotation, newMaxRotation);
							}
						}
					} else {
						rotationZ = Random.Range(newMinRotation, newMaxRotation);
						if (Mathf.Abs(newMinRotation - lastBranchZRotation) > 0.5 || Mathf.Abs(newMaxRotation - lastBranchZRotation) > 0.5) {
							while (Mathf.Abs(rotationZ - lastBranchZRotation) < 0.5) {
								rotationZ = Random.Range(newMinRotation, newMaxRotation);
							}
						}
					}
				}
				
				//Save the rotations values
				computedRotation = new Vector3(rotationX, 0, rotationZ);
				
				//Translate rotation values to the actual values in degrees
				rotationX = rotationX * 100 - actualMaximumRotation;
				rotationZ = rotationZ * 100 - actualMaximumRotation;
				
				//Debug.Log(rotationX);
				//Debug.Log(rotationZ);

				//If the factors are different from min then prevent tree from growing messy
				bool noPositiveZRotation = false;
				bool noNegativeZRotation = false;
				bool noPositiveXRotation = false;
				bool noNegativeXRotation = false;
				if (isGrowingWell) {
					//Debug.Log("Grow well");
					//If at the right of the rootNode then prevent positive z rotation
					if (contents.transform.position.x - contentsRoot.transform.position.x > 0) {
						//Debug.Log("GOING RIGHT");
						noPositiveZRotation = true;
					//If at the left of the rootNode prevent negative z rotation
					} else if (contents.transform.position.x - contentsRoot.transform.position.x < 0) {
						//Debug.Log("GOING LEFT");
						noNegativeZRotation = true;
					}
					
					//If at the back of the rootNode prevent negative x rotation
					if (contents.transform.position.z - contentsRoot.transform.position.z > 0) {
						//Debug.Log("GOING BACK");
						noNegativeXRotation = true;
					//If at the front of the rootNode prevent positive x rotation
					} else if (contents.transform.position.z - contentsRoot.transform.position.z < 0) {
						//Debug.Log("GOING FRONT");
						noPositiveXRotation = true;
					}
					
					//Grow in a direction or another
					if (Random.value > 0.5 && ((rotationX < 0 || !noNegativeXRotation) && (rotationX > 0 || !noPositiveXRotation))) {
						rotationX = -rotationX;
					}
					if (Random.value > 0.5 && ((rotationZ < 0 || !noNegativeZRotation) && (rotationZ > 0 || !noPositiveZRotation))) {
						rotationZ = -rotationZ;
					}
					//Make one axis rotate anywhere from negative to positive, the angle of the branch being given by the other axis already
					if (Random.value > 0.5 && !noNegativeXRotation && !noPositiveXRotation) {
						rotationX = Random.Range(-rotationX, rotationX);
					} else if (!noNegativeZRotation && !noPositiveZRotation) {
						rotationZ = Random.Range(-rotationZ, rotationZ);
					} else if (Random.value > 0.5) {
						if (noNegativeXRotation) {
							rotationX = Random.Range(0, (newMaxRotation * 100) - actualMaximumRotation);
							computedRotation.x = rotationX;
						} else if (noPositiveXRotation) {
							rotationX = Random.Range(-rotationX, -((newMinRotation * 100) - actualMinimumRotation));
							computedRotation.x = -rotationX;
						}
					} else {
						if (noNegativeZRotation) {
							rotationZ = Random.Range(0, (newMaxRotation * 100) - actualMaximumRotation);
							computedRotation.z = rotationZ;
						} else if (noPositiveZRotation) {
							rotationZ = Random.Range(-rotationZ, -((newMinRotation * 100) - actualMinimumRotation));
							computedRotation.z = -rotationZ;
						}
					}
				} else {
					//Debug.Log("Grow messy");
					//Grow in a direction or another
					if (Random.value > 0.5) {
						rotationX = -rotationX;
					}
					if (Random.value > 0.5) {
						rotationZ = -rotationZ;
					}
					//Make one axis rotate anywhere from negative to positive, the angle of the branch being given by the other axis already
					if (Random.value > 0.5) {
						rotationX = Random.Range(-rotationX, rotationX);
					} else {
						rotationZ = Random.Range(-rotationZ, rotationZ);
					}
				}
				
				//Debug.Log("* Computed x rotation: " + rotationX);
				//Debug.Log("* Computed z rotation: " + rotationZ);
				
				//Rotate based on the position of the first root node
				contents.transform.parent = contents.transform.root.transform;
				contents.transform.localEulerAngles = new Vector3 (0, 0, 0);
				contents.transform.Rotate (rotationX, 0, rotationZ);
			}
			
			growing = true;
			growingRadius = radius;
			growingLength = length;
			growingColor = color;

			//TODO faire briller les branches qui sont majestueuses
			
			//Save length and radius
			computedLength = length;
			computedRadius = radius;
			
			//Scale and translate the branch
			appearance.name = "Appearance";
			Vector3 scaleVector = new Vector3 (radius, length, radius);
			appearance.transform.localScale = scaleVector;
			appearance.transform.Translate (0, 0.5f * Player.yScale * length, 0);

			//Debug.Log("--- End of constructing a branch ---");
		}
		
		public GameObject getContents ()
		{
			return contents;
		}
		
		public int getAge ()
		{
			return age;
		}
	}
}