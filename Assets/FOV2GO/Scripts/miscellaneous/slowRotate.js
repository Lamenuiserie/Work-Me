#pragma strict
public var rotateSpeed : float = 1.0;
public var randomize : boolean = false;
public var rotateAxis : Vector3 =  Vector3(0.5,0.5,0.5);
function Start () {

}

function Update () {
   	transform.Rotate(Vector3(Time.deltaTime*rotateSpeed*rotateAxis.x,Time.deltaTime*rotateSpeed*rotateAxis.y,Time.deltaTime*rotateSpeed*rotateAxis.z));
   	if (randomize) {
		rotateAxis += Vector3(Random.Range(-0.01,0.01),Random.Range(-0.01,0.01),Random.Range(-0.01,0.01));
   	}
}