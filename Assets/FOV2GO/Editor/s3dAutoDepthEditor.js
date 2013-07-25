/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * This script goes in the Editor folder. It provides a custom inspector for s3dAutoDepth.
 */

@CustomEditor (s3dAutoDepth)
class s3dAutoDepthEditor extends Editor {
    function OnInspectorGUI () {
    	target.convergenceMethod = EditorGUILayout.EnumPopup(GUIContent("Convergence Method","Pick dynamic convergence method"), target.convergenceMethod);
    	target.autoInteraxial = EditorGUILayout.Toggle(GUIContent("Auto Interaxial","Use dynamic interaxial"), target.autoInteraxial);
    	target.parallaxPercentageOfWidth = EditorGUILayout.Slider (GUIContent("Parallax Percentage","Total parallax percentage of image width"),target.parallaxPercentageOfWidth, 1, 100);
    	target.percentageNegativeParallax = EditorGUILayout.Slider (GUIContent("Negative/Positive Ratio","Ratio of negative to positive parallax"),target.percentageNegativeParallax, 0, 100);
   		target.zeroPrlxDistanceMin = EditorGUILayout.Slider (GUIContent("Min Zero Prlx Distance","Minimum allowable parallax (M)"),target.zeroPrlxDistanceMin, 1, 100);
   		target.interaxialMin = EditorGUILayout.Slider (GUIContent("Minimum Interaxial","Minimum allowable interaxial (mm)"),target.interaxialMin, 1, 100);
   		target.interaxialMax = EditorGUILayout.Slider (GUIContent("Maximum Interaxial","Maximum allowable interaxial (mm)"),target.interaxialMax, 1, 1000);
   		target.lagTime = EditorGUILayout.Slider (GUIContent("Lag Time","Smooth abrupt changes"),target.lagTime, 0, 100);
		if (GUI.changed)
            EditorUtility.SetDirty (target);
	}
}
