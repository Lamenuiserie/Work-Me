#pragma strict

public var randomAvoid : float = 10.0;
public var randomAttract : float = 5.0;
public var moveSpeed : float = 1.0;
private var currentMoveSpeed : float;
public var nearHeight : float = 2;
public var farHeight : float = 1;
public var attractState : boolean = true; // true means attract, false means avoid
public var nearThreshold : float = 10.0; 
public var farThreshold : float = 20.0;
private var attractAmount : float = 0;
private var nextPoint : Vector3;
private var attractOffset : float;

private var mainCam : GameObject;

function Start () {
	mainCam = GameObject.FindWithTag("MainCamera");
	nextPoint = mainCam.transform.position;
	currentMoveSpeed = moveSpeed+Random.Range(moveSpeed,moveSpeed*2);
}

function Update () {
	// update position
	gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,nextPoint,attractAmount);
	// attract
	if (attractState) {
		nextPoint = mainCam.transform.position + Vector3(0, Random.Range(nearHeight,nearHeight*2), 0);
		attractAmount += currentMoveSpeed/20000;
		// switch to avoid
		if (Vector3.Distance(gameObject.transform.position,nextPoint) < 0.1 || Vector3.Distance(gameObject.transform.position,mainCam.transform.position) < nearThreshold || attractAmount > 1.0) {
			attractState = false;
			attractAmount = 0;
			//currentMoveSpeed = moveSpeed+Random.Range(moveSpeed,moveSpeed*2);
			var dice = Random.Range(-1, 1);
			var drctn : int;
			if (dice<0) { 
				drctn = -1; 
			} else { 
				drctn = 1; 
			}
			var ray = new Ray (mainCam.transform.position, transform.position);
			nextPoint = ray.GetPoint(farThreshold*3*drctn);
			nextPoint = Vector3(nextPoint.x+Random.Range(-randomAvoid,randomAvoid), Random.Range(-farHeight,farHeight), nextPoint.z+Random.Range(-randomAvoid,randomAvoid));
		}
	// avoid
	} else {
		attractAmount += currentMoveSpeed/10000;
		// switch to attract
		if (Vector3.Distance(gameObject.transform.position,mainCam.transform.position) > farThreshold || attractAmount > 1.0) {
			attractState = true;
			attractAmount = 0;
			currentMoveSpeed = moveSpeed+Random.Range(moveSpeed,moveSpeed*2);
		}
	}
}		
			
