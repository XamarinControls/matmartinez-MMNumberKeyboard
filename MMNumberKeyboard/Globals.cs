using System;
using Foundation;
using System.Drawing;
using System.Globalization;
using System;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using System.Collections.Generic;

public partial class Globals
{
	//TODO:crear un nsmutabledictionary con tipos de datos de c#

	/// <summary>
	/// Attempts to convert native .net types to objC types.
	/// Works for:
	///  - DateTime
	///  - String
	///  - Number types
	/// </summary>
	/// <returns>The objective C representation</returns>
	/// <param name="o">The .net object</param>

	public static  UIUserInterfaceIdiom UI_USER_INTERFACE_IDIOM() {
		return (UIDevice.CurrentDevice.RespondsToSelector(new Selector("userInterfaceIdiom"))) ?
			UIDevice.CurrentDevice.UserInterfaceIdiom :
			UIUserInterfaceIdiom.Phone;
	}
}

