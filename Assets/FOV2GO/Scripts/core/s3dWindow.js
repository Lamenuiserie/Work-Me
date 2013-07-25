#pragma strict
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Stereo Window Script revised 12.30.12
 * Usage:
 * Attach to main camera. This script requires s3Dcamera script.
 * It automatically provides side masks to prevent stereo window violations.
 * Dependencies: s3dCamera.js
 */

public var on : boolean = true;
public var sideSamples : int = 15;
//enum maskDistance {MaxDistance, ScreenPlane, FarFrustum};
public var maskLimit = maskDistance.ScreenPlane;
@HideInInspector
public var maskStrings : String[] = ["Max Distance", "Screen Plane", "Far Frustum"];
public var maximumDistance : float = 2;
public var drawDebugRays: boolean = true;
private var mainCam : Camera;
private var camScript : s3dCamera;
private var leftMask : GameObject;
private var rightMask : GameObject;
private var lCam : Camera;
private var rCam : Camera;
private var cutInL : float = 0;
private var cutInR : float = 0;

@script RequireComponent(s3dCamera)
@script AddComponentMenu ("Stereoskopix/s3d Window")

function Start () {
	mainCam = gameObject.GetComponent(Camera);
	camScript = gameObject.GetComponent(s3dCamera);
	lCam = camScript.leftCam.camera;
	rCam = camScript.rightCam.camera;
	var masks : GameObject = new GameObject("masks");
	
	leftMask = new GameObject ("leftMask");
	leftMask.transform.parent = masks.transform;
	leftMask.layer = LayerMask.NameToLayer("Ignore Raycast");
	var filterL = leftMask.AddComponent(MeshFilter);
	leftMask.AddComponent(MeshRenderer);
	var leftMesh = filterL.mesh;
	leftMesh.Clear();
	leftMesh.vertices = [Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero];
	leftMesh.normals = [Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero];
	leftMesh.triangles = [0,2,1,0,3,2];
	leftMesh.uv = [Vector2(0,1),Vector2(0,0),Vector2(1,0),Vector2(1,1)];
	leftMask.renderer.material = new Material(Shader.Find("Self-Illumin/Diffuse"));
	leftMask.renderer.material.color = Color(0,0,0,1);
	leftMask.renderer.castShadows = false;

	rightMask = new GameObject ("rightMask");
	rightMask.transform.parent = masks.transform;
	rightMask.layer = LayerMask.NameToLayer("Ignore Raycast");
	var filterR = rightMask.AddComponent(MeshFilter);
	rightMask.AddComponent(MeshRenderer);
	var rightMesh = filterR.mesh;
	rightMesh.Clear();
	rightMesh.vertices = [Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero];
	rightMesh.normals = [Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero];
	rightMesh.triangles = [0,2,1,0,3,2];
	rightMesh.uv = [Vector2(0,1),Vector2(0,0),Vector2(1,0),Vector2(1,1)];
	rightMask.renderer.material = new Material(Shader.Find("Self-Illumin/Diffuse"));
	rightMask.renderer.material.color = Color(0,0,0,1);
	rightMask.renderer.castShadows = false;
}

function toggleVis(a) {
	if (a) {
		leftMask.renderer.enabled = true;
		rightMask.renderer.enabled = true;
	} else {
		leftMask.renderer.enabled = false;
		rightMask.renderer.enabled = false;
	}
}

function Update() {
	if (on) {
		var leftBool : boolean = false;
		var leftDepth : float = Mathf.Infinity;
		var leftCoord : Vector3 = Vector3.zero;

		for (var yy = 0; yy < sideSamples; yy++) {
			var ray : Ray = lCam.ViewportPointToRay(Vector3(1,yy/(sideSamples-1.0),0)); // test lCam along right edge
			var hit : RaycastHit;
			if (Physics.Raycast(ray,hit,Mathf.Infinity)) {
				if (drawDebugRays) {
					Debug.DrawRay (ray.origin, ray.direction*hit.distance, Color(0,1,1,1));
				}
				if (hit.distance < camScript.zeroPrlxDist) {
					leftBool = true;
					if (hit.distance < leftDepth) {
						leftDepth = hit.distance;
						leftCoord = hit.point; // absolute point in world space where right edge of left view was violated
					}
				}
			}
		}
		if (leftBool) {
			cutInR = rCam.WorldToViewportPoint(leftCoord).x; // x coord to cut in
		} else {
			cutInR = 1;
		}

		var rightBool : boolean = false;
		var rightDepth : float = Mathf.Infinity;
		var rightCoord : Vector3 = Vector3.zero;
		for (yy = 0; yy < sideSamples; yy++) {
			ray = rCam.ViewportPointToRay(Vector3(0,yy/(sideSamples-1.0),0)); // test rCam along left edge
			if (Physics.Raycast(ray,hit,Mathf.Infinity)) {
				if (drawDebugRays) {
					Debug.DrawRay (ray.origin, ray.direction*hit.distance, Color(1,0,0,1));
				}
				if (hit.distance < camScript.zeroPrlxDist) {
					rightBool = true;
					if (hit.distance < rightDepth) {
						rightDepth = hit.distance;
						rightCoord = hit.point; // absolute point in world space where left edge of right view was violated
					}
				}
			}
		}
		if (rightBool) {
			cutInL = lCam.WorldToViewportPoint(rightCoord).x;
		} else {
			cutInL = 0;
		}
	}	
}

function LateUpdate () {
	if (on) {
		var leftMesh : Mesh = leftMask.GetComponent(MeshFilter).mesh;
		var vertsL : Vector3[] = leftMesh.vertices;
		vertsL[0] = lCam.ViewportToWorldPoint(Vector3(0,1,lCam.nearClipPlane)); // near upper left
		vertsL[1] = lCam.ViewportToWorldPoint(Vector3(0,0,lCam.nearClipPlane)); // near lower left
		if (maskLimit == maskDistance.FarFrustum) {
			vertsL[2] = lCam.ViewportToWorldPoint(Vector3(cutInL,0,lCam.farClipPlane)); // far lower left
			vertsL[3] = lCam.ViewportToWorldPoint(Vector3(cutInL,1,lCam.farClipPlane)); // far upper left
		} else if (maskLimit == maskDistance.ScreenPlane) {
			vertsL[2] = lCam.ViewportToWorldPoint(Vector3(cutInL,0,camScript.zeroPrlxDist)); // far lower left
			vertsL[3] = lCam.ViewportToWorldPoint(Vector3(cutInL,1,camScript.zeroPrlxDist)); // far upper left
		} else if (maskLimit == maskDistance.MaxDistance) {
			vertsL[2] = lCam.ViewportToWorldPoint(Vector3(cutInL,0,maximumDistance)); // far lower left
			vertsL[3] = lCam.ViewportToWorldPoint(Vector3(cutInL,1,maximumDistance)); // far upper left
		}
	    var vecL1 : Vector3 = vertsL[3]-vertsL[0];
	    var vecL2 : Vector3 = vertsL[1]-vertsL[0];
	    var normL : Vector3 = Vector3.Cross(vecL1,vecL2);
	    leftMesh.vertices = vertsL;
		leftMesh.normals = [normL,normL,normL,normL];
		var leftBounds = Bounds (Vector3.zero, Vector3.zero);
		for(vert in vertsL) {
			leftBounds.Encapsulate(vert);
		}
		leftMesh.bounds = leftBounds;
		
		var rightMesh : Mesh = rightMask.GetComponent(MeshFilter).mesh;
		var vertsR : Vector3[] = rightMesh.vertices;
		if (maskLimit == maskDistance.FarFrustum) {
			vertsR[0] = rCam.ViewportToWorldPoint(Vector3(cutInR,1,rCam.farClipPlane)); // far upper right
			vertsR[1] = rCam.ViewportToWorldPoint(Vector3(cutInR,0,rCam.farClipPlane)); // far lower right
		} else if (maskLimit == maskDistance.ScreenPlane) {
			vertsR[0] = rCam.ViewportToWorldPoint(Vector3(cutInR,1,camScript.zeroPrlxDist)); // far upper right
			vertsR[1] = rCam.ViewportToWorldPoint(Vector3(cutInR,0,camScript.zeroPrlxDist)); // far lower right
		} else if (maskLimit == maskDistance.MaxDistance) {
			vertsR[0] = rCam.ViewportToWorldPoint(Vector3(cutInR,1,maximumDistance)); // far upper right
			vertsR[1] = rCam.ViewportToWorldPoint(Vector3(cutInR,0,maximumDistance)); // far lower right
		}
		vertsR[2] = rCam.ViewportToWorldPoint(Vector3(1,0,rCam.nearClipPlane)); // near lower right
		vertsR[3] = rCam.ViewportToWorldPoint(Vector3(1,1,rCam.nearClipPlane)); // near upper right
	    var vecR1 : Vector3 = vertsR[3]-vertsR[0];
	    var vecR2 : Vector3 = vertsR[1]-vertsR[0];
	    var normR : Vector3 = Vector3.Cross(vecR1,vecR2);
	    rightMesh.vertices = vertsR;
		rightMesh.normals = [normR,normR,normR,normR];
		var rightBounds = Bounds (Vector3.zero, Vector3.zero);
		for(vert in vertsR) {
			rightBounds.Encapsulate(vert);
		}
		rightMesh.bounds = rightBounds;
	}
}

