/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
 
/* s3d Stereo Window Editor Script revised 12.30.12
 * This script goes in the Editor folder. It provides a custom inspector for s3dMouse.js.
 */

@CustomEditor (s3dWindow)
class s3dWindowEditor extends Editor {
	static var foldout1 : boolean = true;
    function OnInspectorGUI () {
 		foldout1 = EditorGUILayout.Foldout(foldout1, "Options"); 
		if (foldout1) {
	       	EditorGUILayout.BeginVertical("box");

	       	target.on = EditorGUILayout.Toggle("Active", target.on);
        	if (GUI.changed) {
            	EditorUtility.SetDirty (target);
            	target.toggleVis(target.on);
        	}
        	GUI.changed = false;
 	       	target.drawDebugRays = EditorGUILayout.Toggle("Draw Debug Rays", target.drawDebugRays);
       	
	        target.sideSamples = EditorGUILayout.Slider ("Side Samples",target.sideSamples, 3, 50);
			target.maskLimit = EditorGUILayout.EnumPopup("Mask Limit", target.maskLimit);
			if (target.maskLimit == 0) {
	        	target.maximumDistance = EditorGUILayout.Slider ("Maximum Distance",target.maximumDistance, 0, 50);
			}
	      	EditorGUILayout.EndVertical();
		}
        if (GUI.changed) {
            EditorUtility.SetDirty (target);
         }
  }

}
