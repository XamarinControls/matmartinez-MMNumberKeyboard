using System;

using UIKit;
using CoreGraphics;

public partial class Globals
{
	public partial class ViewController : 
		UIViewController , MMNumberKeyboardDelegate
	{
		public UITextField textField;



		public ViewController () : base ()
		{
		}
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			this.View.BackgroundColor = UIColor.White;

			// Create and configure the keyboard.
			MMNumberKeyboard keyboard = new MMNumberKeyboard(CGRect.Empty);
			keyboard.allowsDecimalPoint = true;
			keyboard.Delegate = this;

			// Configure an example UITextField.
			UITextField textField = new UITextField(CGRect.Empty);
//			textField.InputView = keyboard;
			textField.Text = "123456789";
			textField.Placeholder = "Type something...";
//			textField.Font = UIFont systemFontOfSize:24.0f];
			textField.VerticalAlignment = UIControlContentVerticalAlignment.Top;

			this.textField = textField;

			this.View.AddSubview(textField);
		}
		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
			// Perform any additional setup after loading the view, typically from a nib.

			CGRect bounds = new CGRect(){
					Size = this.View.Bounds.Size
				};

			CGRect contentRect =  new UIEdgeInsets(){
				Top= this.TopLayoutGuide.Length,
				Bottom = this.BottomLayoutGuide.Length
			}.InsetRect(bounds);

			float pad = 20.0f;

			this.textField.Frame = contentRect.Inset( pad, pad);
		}
		public override void ViewWillAppear ( bool animated )
		{
			base.ViewWillAppear (animated);
			// Perform any additional setup after loading the view, typically from a nib.

			this.textField.BecomeFirstResponder();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

