#pragma strict
public var spinSpeed : float = 10.0;
public var counterClockwise : boolean = false;

function Update () {
	if (!counterClockwise) {
   		transform.Rotate(Vector3(0,Time.deltaTime*spinSpeed,0));
	} else {
   		transform.Rotate(Vector3(0,Time.deltaTime*-spinSpeed,0));
	}
}