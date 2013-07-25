#pragma strict
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * s3d Auto Depth Script revised 12.30.12
 * Usage:
 * This script uses information from the s3DdepthInfo script to automatically
 * set the interaxial and/or the convergence (zero parallax distance) to keep screen parallax within a
 * specified percentage of the screen width. The interaxial can be clamped between a minimum and a maximum,
 * and a minimum for zero parallax distance can be specified. 
 * 
 * Objects can be tagged "Ignore Raycast" to hide them from raycasts (this would typically be done with ground planes).
*/

//enum converge {none,percent,center,click,mouse,object};
public var convergenceMethod = converge.center;
// none: leave zpd alone
// auto: maintain ratio of negative to positive parallax as set by "percentageNegativeParallax". 
// center: keep convergence at center of scene
// click: click mouse button to set zero parallax distance to point on object
// mouse: zero parallax distance set continuously to mouse position
// object: continuously keep zero parallax distance at clicked object point

// autoInteraxial: Control interaxial. 
// True: keep within "parallaxPercentageOfWidth". 
// False: leaves interaxial alone
var autoInteraxial : boolean = true;

// parallaxPercentageOfWidth: Automatically calculates interaxial
// based on a total parallax value expressed as a percentage of image width.
var parallaxPercentageOfWidth: float = 3;

// percentageNegativeParallax: Automatically calculates zero parallax distance (convergence)
// based on a negative parallax value expressed as a percentage of total parallax.
var percentageNegativeParallax: float = 66;

// zeroPrlxDistanceMin: Set zero parallax distance minimum
var zeroPrlxDistanceMin : float = 1.0; // meters

// interaxialMin: Limit minimum allowed interaxial; overrides parallaxPercentageOfWidth
var interaxialMin: float = 30; // millimeters

// interaxialMax: Limit maximum allowed interaxial; overrides parallaxPercentageOfWidth
var interaxialMax: float = 120; // millimeters

// how gradually to change interaxial and zero parallax (bigger numbers are slower - more than 25 is very slow);
var lagTime : float = 10;

private var cameraWidth: float;
private var cameraParallaxNegative: float;
private var cameraParallaxPositive: float;
private var zeroPrlxNewDistance: float;
//private var farDistance: float;
private var nearPoint: Vector3;
private var farPoint: Vector3;
private var interaxial: float;

private var mainCam : Camera;
private var camScript : s3dCamera;
private var infoScript : s3dDepthInfo;
private var rays = [new Array(),new Array()];

@script RequireComponent(s3dCamera)
@script RequireComponent(s3dDepthInfo)
@script AddComponentMenu ("Stereoskopix/s3d Auto Depth")

function Start() {
	mainCam = gameObject.GetComponent(Camera);	// Main Stereo Camera Component
	camScript = gameObject.GetComponent(s3dCamera);	// Main Stereo Script
	infoScript = gameObject.GetComponent(s3dDepthInfo);	// Main Stereo Script
}

function Update () {
	if (infoScript.nearDistance < Mathf.Infinity && infoScript.farDistance > 0) {
		// calculate image width at far distance
		var cameraWidthFar = Mathf.Tan((mainCam.fieldOfView*mainCam.aspect)/2*Mathf.Deg2Rad)*infoScript.farDistance*2;
		// calculate total parallax
		var cameraParallaxTotal = ((parallaxPercentageOfWidth/100)*cameraWidthFar); 
		if (autoInteraxial) {
			// calculate interaxial
			interaxial = (cameraParallaxTotal*infoScript.nearDistance/(infoScript.farDistance-infoScript.nearDistance))*1000;	// convert from meters to millimeters
			// clamp interaxial between minimum & maximum values
			interaxial = Mathf.Clamp(interaxial,interaxialMin,interaxialMax);
		} else {
			interaxial = camScript.interaxial;
		}
		switch (convergenceMethod) {
			case converge.percent:
				// calculate negative parallax from percentage
				cameraParallaxNegative = cameraParallaxTotal*percentageNegativeParallax/100;
				// calculate zero parallax distance - convert from millimeters to meters
				zeroPrlxNewDistance = ((infoScript.nearDistance*cameraParallaxNegative)/(interaxial/1000))+infoScript.nearDistance;
				// clamp zero parallax distance to a minimum
				zeroPrlxNewDistance = Mathf.Max(zeroPrlxNewDistance, zeroPrlxDistanceMin);
				// calculate positive parallax from percentage
				cameraParallaxPositive = cameraParallaxTotal*(100-percentageNegativeParallax)/100;		
				break;
			case converge.center:
				zeroPrlxNewDistance = infoScript.distanceAtCenter;
				break;
			case converge.click:
			  	if (Input.GetMouseButtonDown(0)) {
 					zeroPrlxNewDistance = infoScript.distanceUnderMouse;
			  	}
				break;
			case converge.mouse:
				zeroPrlxNewDistance = infoScript.distanceUnderMouse;
				break;
			case converge.object:
				if (infoScript.selectedObject && infoScript.objectDistance > 0) {
					zeroPrlxNewDistance = infoScript.objectDistance;
				} else {
					zeroPrlxNewDistance = camScript.zeroPrlxDist;
				}
				break;
			default:
				zeroPrlxNewDistance = camScript.zeroPrlxDist;
		}
	}
}

// update interaxial and convergence
function FixedUpdate() {
	if (convergenceMethod != converge.none) {
		if (camScript.zeroPrlxDist > zeroPrlxNewDistance) {
			camScript.zeroPrlxDist -= (camScript.zeroPrlxDist-zeroPrlxNewDistance)/(lagTime+1);
		} else if (camScript.zeroPrlxDist < zeroPrlxNewDistance) {
			camScript.zeroPrlxDist += (zeroPrlxNewDistance-camScript.zeroPrlxDist)/(lagTime+1);
		}
	}
	if (autoInteraxial) {
		if (camScript.interaxial > interaxial) {
			camScript.interaxial -= (camScript.interaxial-interaxial)/(lagTime+1);
		} else if (camScript.interaxial < interaxial) {
			camScript.interaxial += (interaxial-camScript.interaxial)/(lagTime+1);
		}
	}
}

/*

Given: an on-screen parallax percentage AND image width AND near object distance AND screen plane distance - calculate interaxial
Assume that screen plane = far object distance, so that total parallax = negative parallax

                parallaxNeg                 interaxial
--------------------------------------- = -------------
screen plane distance - object distance    object distance

prlxNeg = (screenDist-objectDist) * interaxial / objectDist
prlxNeg * objectDist = (screenDist-objectDist) * interaxial
prlxNeg * objectDist / (screenDist-objectDist) = interaxial
interaxial = prlxNeg * objectDist / (screenDist-objectDist)

prlxPercent = prlxNeg / screenWidth
prlxNeg = prlxPercent * screenWidth

interaxial = (prlxPercent * screenWidth) * objectDist / (screenDist - objectDist)

to calculate positive parallax

                parallaxPos                 interaxial
--------------------------------------- = -------------
object distance - screen plane distance    object distance


*/
	
