/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Enums Script revised 12.30.12
 * Contains all enums used in Stereoskopix FOV2GO package 
 */

#pragma strict
enum cams3D {Left_Right, Left_Only, Right_Only, Right_Left}
enum mode3D {SideBySide, Anaglyph, OverUnder, Interlace, Checkerboard};
enum anaType {Monochrome, HalfColor, FullColor, Optimized, Purple};
enum phoneType {GalaxyNexus_LandLeft, GalaxyNote_LandLeft, iPad2_LandLeft, iPad2_Portrait, iPad3_LandLeft, iPad3_Portrait, iPhone4_LandLeft, OneS_LandLeft, Rezound_LandLeft, Thrill_LandLeft, my3D_LandLeft};
enum maskDistance {MaxDistance, ScreenPlane, FarFrustum};
enum rayCastFilter {None,Tag,Layer}
enum Axes {MouseXandY, MouseX, MouseY} 
enum controlGui {none,move,turn,point}
enum controlPos {off,left,center,right}
enum converge {none,percent,center,click,mouse,object};
enum sPlane {near,screen,far}
