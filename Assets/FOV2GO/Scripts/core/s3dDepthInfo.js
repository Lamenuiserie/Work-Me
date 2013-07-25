#pragma strict
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * s3d Depth Info Script revised 12.30.12
 * Usage:
 * This script performs an array of raycasts into the view frustum and finds:
 * A. the distance of the nearest object in the scene.
 * B. the distance of the farthest object in the scene.
 * C. the distance to the object under the mouse position in the scene.
 * D. the distance to a selected object (set either by script or by mouse click) in the scene.
 * There are currently three additional scripts that can use this information to control various stereoscopic parameters:
 * 1. s3DdepthScene: automatically adjusts interaxial, convergence and parallax based on total scene depth
 * 2. s3DdepthMouse: automatically adjust convergence based on mouse position
 * 3. s3DdepthGUI: attached to a GUIelement, automatically adjusts its depth - can be used to create a 3D mouse pointer
*/

// number of rays to cast (more = more accurate, especially horizontal)
var raysH:int = 25;
var raysV:int = 12;
// maximum distance to cast rays
var maxSampleDistance:float = 100;

// draw debug rays in scene
var drawDebugRays : boolean = true;
public var nearDistance: float;	// distance to the nearest visible object
public var farDistance: float; // distance to the farthest visible object
public var distanceAtCenter : float; // distance to center of scene
public var distanceUnderMouse : float;
public var clickToSelect : boolean = false;
public var selectedObject : GameObject;
private var objectUnderMouse : GameObject;
public var objectDistance: float = 0;
private var nearPoint: Vector3;
private var farPoint: Vector3;
private var interaxial: float;

private var mainCam : Camera;
private var camScript : s3dCamera;
private var rays = [new Array(),new Array()];

public var showScreenPlane : boolean;
public var showNearFarPlanes : boolean;

@script RequireComponent(s3dCamera)
@script AddComponentMenu ("Stereoskopix/s3d Depth Info")

function Start() {
	mainCam = gameObject.GetComponent(Camera);	// Main Stereo Camera Component
	camScript = gameObject.GetComponent(s3dCamera);	// Main Stereo Script
}

function Update () {
	findNearAndFarDistances();
	findDistanceUnderMousePosition();
	if (clickToSelect) {
		if (Input.GetMouseButtonDown(0)) {
			selectedObject = objectUnderMouse;
		}
	}
	if (selectedObject) {
		findObjectDistance();
	}
	if (showScreenPlane) {
		if (!camScript.planeZero.renderer.enabled) {
			camScript.planeZero.renderer.enabled = true;
		}
		var cameraWidth : float = Mathf.Tan(camScript.horizontalFOV/2*Mathf.Deg2Rad)*camScript.zeroPrlxDist*2;
		var cameraHeight : float = Mathf.Tan(camScript.verticalFOV/2*Mathf.Deg2Rad)*camScript.zeroPrlxDist*2;
		var screenSize : Vector2 = Vector2(cameraWidth,cameraHeight);
  		camScript.planeZero.transform.localPosition = Vector3(0,0, camScript.zeroPrlxDist);
		camScript.planeZero.transform.localScale = Vector3(screenSize.x,screenSize.y,0);
	} else {
		if (camScript.planeZero.renderer.enabled) {
			camScript.planeZero.renderer.enabled = false;
		}
	}
	if (showNearFarPlanes) {
		if (!camScript.planeNear.renderer.enabled) {
			camScript.planeNear.renderer.enabled = true;
			camScript.planeFar.renderer.enabled = true;
		}
		var nearWidth : float = Mathf.Tan(camScript.horizontalFOV/2*Mathf.Deg2Rad)*nearDistance*2;
		var nearHeight : float = Mathf.Tan(camScript.verticalFOV/2*Mathf.Deg2Rad)*nearDistance*2;
		var nearSize : Vector2 = Vector2(nearWidth,nearHeight);
  		camScript.planeNear.transform.localPosition = Vector3(0,0, nearDistance);
		camScript.planeNear.transform.localScale = Vector3(nearSize.x,nearSize.y,0);
		
		var farWidth : float = Mathf.Tan(camScript.horizontalFOV/2*Mathf.Deg2Rad)*farDistance*2;
		var farHeight : float = Mathf.Tan(camScript.verticalFOV/2*Mathf.Deg2Rad)*farDistance*2;
		var farSize : Vector2 = Vector2(farWidth,farHeight);
  		camScript.planeFar.transform.localPosition = Vector3(0,0, farDistance);
		camScript.planeFar.transform.localScale = Vector3(farSize.x,farSize.y,0);
	} else {
		if (camScript.planeNear.renderer.enabled) {
			camScript.planeNear.renderer.enabled = false;
			camScript.planeFar.renderer.enabled = false;
		}
	}

}

function findNearAndFarDistances() {
	// find the smallest and largest distance from any raycast
	rays[0] = [];
	rays[1] = [];
	var n : int = 0;
	nearDistance = Mathf.Infinity;
	farDistance = 0;
	var nearObjectName : String = "none";
	var farObjectName : String = "none";
	for (var col=0; col<raysH; col++) {
		for (var row=0; row<raysV; row++) {
			var ray : Ray = mainCam.ScreenPointToRay(Vector3( (col*mainCam.pixelWidth)/(raysH-1), (row*mainCam.pixelHeight)/(raysV-1), 0));
			var hit : RaycastHit;
			// draw all the rays in gray
			if (drawDebugRays) Debug.DrawRay (ray.origin, ray.direction * maxSampleDistance, Color(0.5,0.5,0.5,0.25));
			if (Physics.Raycast(ray, hit, maxSampleDistance)) {
				// draw the rays that actually hit valid objects in yellow
				if (drawDebugRays) Debug.DrawLine (ray.origin, ray.GetPoint(hit.distance), Color(1,1,0,0.5));
				rays[0][n] = ray.origin;
				rays[1][n] = ray.GetPoint(hit.distance);
				// plane of camera position (perpendicular to camera z axis)
				var camPlane : Plane = Plane(mainCam.transform.forward, mainCam.transform.position);
				// get hit point of ray (world coordinates)
				nearPoint = ray.GetPoint(hit.distance);
				// calculate distance measured along camera's local z axis
				var currentDistance = camPlane.GetDistanceToPoint(nearPoint);
				n++;
				if (currentDistance < nearDistance) {
					nearDistance = currentDistance;
				} else if (currentDistance > farDistance) {
					farDistance = currentDistance;
				}
				if (col == raysH/2 && row == raysV/2) {
					if (drawDebugRays) Debug.DrawLine (ray.origin, ray.GetPoint(hit.distance), Color(0,0,1,1));
					distanceAtCenter = currentDistance;
				}
			}
		}
	}
}
	
function findDistanceUnderMousePosition() {
	var hit: RaycastHit;
	var ray : Ray = mainCam.ScreenPointToRay (Input.mousePosition);	// converge to clicked point
	if (Physics.Raycast (ray, hit, 100.0)) {
		objectUnderMouse = hit.collider.gameObject;
		var camPlane : Plane = Plane(mainCam.transform.forward, mainCam.transform.position);
		var thePoint = ray.GetPoint(hit.distance);
		distanceUnderMouse =  camPlane.GetDistanceToPoint(thePoint);
	}
}

function findObjectDistance() {
  	var planes = GeometryUtility.CalculateFrustumPlanes(mainCam);
  	//test whether object is on camera
  	if (GeometryUtility.TestPlanesAABB(planes,selectedObject.collider.bounds)) { 
 		var vec : Vector3 = mainCam.WorldToViewportPoint(selectedObject.transform.position);
 		//alternate to bounds - just check object center
   		//if(vec.x>0 && vec.x<1 && vec.y>0 && vec.y<1 && vec.z>0) { 
		var ray : Ray = Camera.main.ViewportPointToRay (vec);
		var hit: RaycastHit;
 		if (Physics.Raycast(ray,hit,100.0)) {
 			 //make sure object isn't hidden by another object
			if (hit.collider.gameObject == selectedObject && hit.distance > mainCam.nearClipPlane) {
				var camPlane : Plane = Plane(mainCam.transform.forward, mainCam.transform.position);
				var thePoint = ray.GetPoint(hit.distance);
				objectDistance = camPlane.GetDistanceToPoint(thePoint);
			}
 		}
   	}
}

