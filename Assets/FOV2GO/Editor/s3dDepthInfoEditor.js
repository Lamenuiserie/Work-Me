/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dDepthInfo.
 */

@CustomEditor (s3dDepthInfo)
class s3dDepthInfoEditor extends Editor {
    function OnInspectorGUI () {
		EditorGUIUtility.LookLikeControls(120,30);
        target.raysH = EditorGUILayout.IntSlider ("Ray Columns",target.raysH, 3, 100);
        target.raysV = EditorGUILayout.IntSlider ("Ray Rows",target.raysV, 3, 100);
        target.maxSampleDistance = EditorGUILayout.Slider ("Max Sample Distance",target.maxSampleDistance, 1, 500);
	    EditorGUIUtility.LookLikeControls(130,70);
        target.drawDebugRays = EditorGUILayout.Toggle("Draw Debug Rays", target.drawDebugRays);
		var allowSceneObjects : boolean = !EditorUtility.IsPersistent (target);
		EditorGUILayout.BeginHorizontal();
		target.showScreenPlane = EditorGUILayout.Toggle(GUIContent("Show Screen Plane ","Translucent Plane at Zero Prlx Dist"), target.showScreenPlane);
	    EditorGUIUtility.LookLikeControls(150,70);
		target.clickToSelect = EditorGUILayout.Toggle("Click Selects Object", target.clickToSelect);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
	    EditorGUIUtility.LookLikeControls(130,70);
		target.showNearFarPlanes = EditorGUILayout.Toggle(GUIContent("Show Near/Far Planes ","Translucent Plane at Near/Far Dist"), target.showNearFarPlanes);
	    EditorGUIUtility.LookLikeControls(90,70);
		target.selectedObject = EditorGUILayout.ObjectField("Selected Object",target.selectedObject,GameObject,allowSceneObjects);
		EditorGUILayout.EndHorizontal();
        var r : Rect = EditorGUILayout.BeginVertical("TextField");
        EditorGUILayout.BeginHorizontal();
 		EditorGUILayout.LabelField("Distances: ");
  		EditorGUILayout.LabelField("Near: "+Mathf.Round(target.nearDistance*10)/10); 
		EditorGUILayout.EndHorizontal();
       	EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Mouse: "+Mathf.Round(target.distanceUnderMouse*10)/10); 
  		EditorGUILayout.LabelField("Center: "+Mathf.Round(target.distanceAtCenter*10)/10); 
       	EditorGUILayout.EndHorizontal();
       	EditorGUILayout.BeginHorizontal();
   		EditorGUILayout.LabelField("Object: "+Mathf.Round(target.objectDistance*10)/10); 
 		EditorGUILayout.LabelField("Far: "+Mathf.Round(target.farDistance*10)/10); 
		EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
		if (GUI.changed)
            EditorUtility.SetDirty (target);
	}

}
