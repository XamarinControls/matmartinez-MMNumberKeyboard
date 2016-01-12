//using UIKit;
//
//namespace MMNumberKeyboard
//{
//	public class Application
//	{
//		// This is the main entry point of the application.
//		static void Main (string[] args)
//		{
//			// if you want to use a different Application Delegate class from "AppDelegate"
//			// you can specify it here.
//			UIApplication.Main (args, null, "AppDelegate");
//		}
//	}
//}


using UIKit;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreGraphics;
using System.Collections.Generic;
using System;
using MMNumberKeyboard;

public partial class Globals
{
	//
	//  main.m
	//  MMNumberKeyboard
	//
	//  Created by Matías Martínez on 1f2f/1f0f/1f5f.
	//  Copyright © 2f0f1f5f Matías Martínez. All rights reserved.
	//

	//#import <UIKit/UIKit.h>
	//#import "AppDelegate.h"

	public static void Main (string []args){
	//@autoreleasepool {

	UIApplication.Main (args, null, typeof(AppDelegate).Name);
	//}
}

}
