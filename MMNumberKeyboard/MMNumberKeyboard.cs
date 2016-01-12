using System;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using System.Collections.Generic;

//http://blog.adamkemp.com/2015/04/objective-c-categories-in-xamarinios.html
//[Category("UIResponder")]


//[Category(typeof(UIResponder))]
public static class UIResponderFirstResponder
{
	public static object _currentFirstResponder ;
	//[Export("MM_currentFirstResponder")]
	public static object MM_currentFirstResponder(this UIResponder uIResponder)
	{
		UIResponderFirstResponder._currentFirstResponder = null;
		UIApplication.SharedApplication.SendAction(new Selector("MM_findFirstResponder:") ,uIResponder,null,null);
		return UIResponderFirstResponder._currentFirstResponder;
	} 
	//[Export("MM_findFirstResponder:")]
	public static void MM_findFirstResponder(this UIResponder UIResponder, object sender)
	{
		UIResponderFirstResponder._currentFirstResponder = UIResponder;
	}

}	

public partial class Globals
{
	//https://developer.xamarin.com/api/type/MonoTouch.Foundation.INSObjectProtocol/
	public interface MMNumberKeyboardDelegate:INSObjectProtocol
	{

		/**
 *  Asks whether the specified text should be inserted.
 *
 *  @param numberKeyboard The keyboard instance proposing the text insertion.
 *  @param text           The proposed text to be inserted.
 *
 *  @return Returns	@c YES if the text should be inserted or @c NO if it should not.
 */
//		- (BOOL)numberKeyboard:(MMNumberKeyboard *)numberKeyboard shouldInsertText:(NSString *)text;
		//bool ShouldInsertText( MMNumberKeyboard  numberKeyboard,  string  text);

		/**
 *  Asks the delegate if the keyboard should process the pressing of the return button.
 *
 *  @param numberKeyboard The keyboard whose return button was pressed.
 *
 *  @return Returns	@c YES if the keyboard should implement its default behavior for the return button; otherwise, @c NO.
 */
		//- (BOOL)numberKeyboardShouldReturn:(MMNumberKeyboard *)numberKeyboard;
		//bool ShouldReturn( MMNumberKeyboard numberKeyboard);

		//@end
	}
		/**
 *  A simple keyboard to use with numbers and, optionally, a decimal point.
 */
		//@interface IMMNumberKeyboard //: UIInputView
	public interface IMMNumberKeyboard{
		/**
 *  Initializes and returns a number keyboard view using the specified style information and locale.
 *
 *  An initialized view object or @c nil if the view could not be initialized.
 *
 *  @param frame          The frame rectangle for the view, measured in points. The origin of the frame is relative to the superview in which you plan to add it.
 *  @param inputViewStyle The style to use when altering the appearance of the view and its subviews. For a list of possible values, see @c UIInputViewStyle
 *  @param locale         An @c NSLocale object that specifies options (specifically the @c NSLocaleDecimalSeparator) used for the keyboard. Specify @c nil if you want to use the current locale.
 *
 *  @returns An initialized view object or @c nil if the view could not be initialized.
 */
		//- (instancetype)initWithFrame:(CGRect)frame inputViewStyle:(UIInputViewStyle)inputViewStyle locale:(NSLocale *)locale;
		//- (instancetype)initWithFrame:(CGRect)frame inputViewStyle:(UIInputViewStyle)inputViewStyle locale:(NSLocale *)locale;

		/**
 *  The receiver key input object. If @c nil the object at top of the responder chain is used.
 */
		//@property (weak, nonatomic) id <IUIKeyInput> keyInput;
		IUIKeyInput keyInput{get;set;}

		/**
 *  Delegate to change text insertion or return key behavior.
 */
//		@property (weak, nonatomic) id <MMNumberKeyboardDelegate> delegate;
		MMNumberKeyboardDelegate Delegate {get;set;}

		/**
 *  Configures the special key with an image and an action block.
 *
 *  @param image   The image to display in the key.
 *  @param handler A handler block.
 */
//		- (void)configureSpecialKeyWithImage:(UIImage *)image actionHandler:(dispatch_block_t)handler;

		/**
 *  Configures the special key with an image and a target-action.
 *
 *  @param image  The image to display in the key.
 *  @param target The target object—that is, the object to which the action message is sent.
 *  @param action A selector identifying an action message. It cannot be NULL.
 */
//		- (void)configureSpecialKeyWithImage:(UIImage *)image target:(id)target action:(SEL)action;

		/**
 *  If @c YES, the decimal separator key will be displayed.
 *
 *  @note The default value of this property is @c NO.
 */
		//@property (assign, nonatomic) BOOL allowsDecimalPoint;
		bool allowsDecimalPoint { get;set; }

		//@interface MMNumberKeyboard () <UIInputViewAudioFeedback>

		NSDictionary buttonDictionary { get;set; }
		NSArray separatorViews { get;set; }
		NSLocale locale { get;set; }

		Action specialKeyHandler { get;set; }

	}


	public enum  MMNumberKeyboardButton {
		NumberMin=0,
		NumberMax =  10, // Ten digits.
		Backspace,
		Done,
		Special,
		DecimalPoint,
		None = -1,
	};

	public enum MMNumberKeyboardButtonType{
		White,
		Gray,
		Done
	};

	public class MMNumberKeyboard : UIInputView , IMMNumberKeyboard
	{
		//public IUIKeyInput keyInput{get;set;}
		public IUIKeyInput _keyInput;
		public MMNumberKeyboardDelegate Delegate {get;set;}
		public bool _allowsDecimalPoint;

		public NSDictionary buttonDictionary { get;set; }
		public NSArray separatorViews { get;set; }
		public NSLocale locale { get;set; }

		public Action specialKeyHandler { get;set; }
		public MMNumberKeyboard ()
		{
		}
		public /*const*/ nint MMNumberKeyboardRows = 4;
		public /*const*/ nfloat MMNumberKeyboardRowHeight = 55.0f;
		public /*const*/ nfloat MMNumberKeyboardPadBorder = 7.0f;
		public /*const*/ nfloat MMNumberKeyboardPadSpacing = 8.0f;


		public string UIKitLocalizedString( string key){
			return new  NSBundle ("com.apple.UIKit").LocalizedString (key, "");
		}

		public MMNumberKeyboard (CGRect Frame ):base(Frame ,UIInputViewStyle.Default){
			this._commonInit();
		}
		public MMNumberKeyboard (CGRect Frame ,/*inputViewStyle*/ UIInputViewStyle inputViewStyle ):base(Frame ,/*inputViewStyle*/ inputViewStyle){
			//this = base.init(Frame ,/*inputViewStyle*/ inputViewStyle);
			//if (this) {
				this._commonInit();
			//}
			//return this;
		}

		public MMNumberKeyboard(CGRect  Frame,/*inputViewStyle*/ UIInputViewStyle inputViewStyle ,/*locale*/  NSLocale  locale):base(Frame ,/*inputViewStyle*/ inputViewStyle){
			//this = base.init(Frame ,/*inputViewStyle*/ inputViewStyle);
			//if (this) {
				this.locale = locale;
				this._commonInit();
			//}
			//return this;
		}

	public void _commonInit(){
		NSMutableDictionary buttonDictionary = new NSMutableDictionary();

		/*const*/ MMNumberKeyboardButton numberMin = MMNumberKeyboardButton.NumberMin;
		/*const*/ MMNumberKeyboardButton numberMax = MMNumberKeyboardButton.NumberMax;

		UIFont buttonFont = UIFont.FromName("Helvetica-Bold", 20f);
		if (buttonFont.RespondsToSelector(new Selector ( "weight:"))) {
			buttonFont = UIFont.SystemFontOfSize(28.0f ,/*weight*/ UIFontWeight.Light);
		} else {
			buttonFont = UIFont.FromName("HelveticaNeue-Light" ,/*size*/ 28.0f);
		}

		UIFont doneButtonFont = UIFont.SystemFontOfSize(17.0f);

		//for (MMNumberKeyboardButton key =  numberMin;key < numberMax; key++) {
		foreach (MMNumberKeyboardButton key in Enum.GetValues(typeof(MMNumberKeyboardButton))) {
			UIButton button = new  _MMNumberKeyboardButton(MMNumberKeyboardButtonType.White);
			string Title = (key - numberMin).ToString();

			button.SetTitle(Title ,/*forState*/ UIControlState.Normal);
			button.TitleLabel.Font = buttonFont;
			int keyInt = (int)key;
				Console.WriteLine ("keyInt:"+keyInt);
				var number = new NSNumber (keyInt);
				Console.WriteLine ("number:"+number);
			buttonDictionary.Add(button ,number);
		}

		UIImage backspaceImage = this._keyboardImageNamed("MMNumberKeyboardDeleteKey.png");
		UIImage dismissImage = this._keyboardImageNamed("MMNumberKeyboardDismissKey.png");

		_MMNumberKeyboardButton backspaceButton = new _MMNumberKeyboardButton(MMNumberKeyboardButtonType.Gray);
		backspaceButton.SetImage( /*setImage*/ backspaceImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate ), UIControlState.Normal);

		backspaceButton.AddTarget( /*addTarget*/ this ,/*action*/ new Selector ("_backspaceRepeat: ") ,/*.forContinuousPressWithTimeInterval*/ 0.15f);

		buttonDictionary.Add(backspaceButton ,/*forKey*/ NSObject.FromObject(MMNumberKeyboardButton.Backspace));

		UIButton specialButton = new _MMNumberKeyboardButton(MMNumberKeyboardButtonType.Gray);

		buttonDictionary.Add(specialButton ,/*forKey*/ NSObject.FromObject(MMNumberKeyboardButton.Special));

		UIButton doneButton = new _MMNumberKeyboardButton(MMNumberKeyboardButtonType.Done);
		doneButton.TitleLabel.Font = doneButtonFont;
		doneButton.SetTitle(UIKitLocalizedString("Done") ,/*forState*/ UIControlState.Normal);

		buttonDictionary.Add(doneButton ,/*forKey*/ NSObject.FromObject(MMNumberKeyboardButton.Done));

		_MMNumberKeyboardButton decimalPointButton = new _MMNumberKeyboardButton(MMNumberKeyboardButtonType.White);

		NSLocale locale = this.locale ??  NSLocale.CurrentLocale;
		string decimalSeparator = @".";//locale.ObjectForKey(NSLocale.DecimalSeparator);
		decimalPointButton.SetTitle(decimalSeparator ??  "." ,/*forState*/ UIControlState.Normal);

		buttonDictionary.Add(decimalPointButton ,/*forKey*/ NSObject.FromObject(MMNumberKeyboardButton.DecimalPoint));

		foreach (UIButton button in buttonDictionary.Values) {
			button.ExclusiveTouch = true;
			button.AddTarget(this ,/*action*/ new Selector ("_buttonInput") ,/*forControlEvents*/ UIControlEvent.TouchUpInside);
			button.AddTarget(this ,/*action*/ new Selector ("_buttonPlayClick") ,/*forControlEvents*/ UIControlEvent.TouchDown);

			this.AddSubview(button);
		}

		UIPanGestureRecognizer highlightGestureRecognizer = new UIPanGestureRecognizer(this ,/*action*/ new Selector ("_handleHighlightGestureRecognizer"));
		this.AddGestureRecognizer(highlightGestureRecognizer);

		this.buttonDictionary = buttonDictionary;

		// Add default action.
		this.configureSpecialKeyWithImage(dismissImage ,/*target*/ this ,/*action*/ new Selector ("_dismissKeyboard"));

		// Size to fit.
		this.SizeToFit();
	}

	//#pragma mark - Input.

	public void _handleHighlightGestureRecognizer( UIPanGestureRecognizer gestureRecognizer ){
		CGPoint point = gestureRecognizer.LocationInView(this);

		if (gestureRecognizer.State == UIGestureRecognizerState.Changed || gestureRecognizer.State == UIGestureRecognizerState.Ended) {
			foreach (UIButton button in this.buttonDictionary.Values) {
				bool  points = button.Frame.Contains( point) && !button.Hidden;

				if (gestureRecognizer.State == UIGestureRecognizerState.Changed) {
					button.Highlighted = points;
				} else {
						button.Highlighted = false;
				}

				if (gestureRecognizer.State == UIGestureRecognizerState.Ended && points) {
						button.SendActionForControlEvents(UIControlEvent.TouchUpInside);
				}
			}
		}
	}

	public void _buttonPlayClick( UIButton  button ){
		UIDevice.CurrentDevice.PlayInputClick();
	}

	public void _buttonInput( UIButton button ){
		MMNumberKeyboardButton keyboardButton = MMNumberKeyboardButton.None;
			//https://github.com/xamarin/monotouch-samples/blob/master/AppPrefs/Settings.cs
		//NSArray.FromArray<NSDictionary> (
		//	foreach (KeyValuePair<NSObject,NSObject> t in this.buttonDictionary.GetEnumerator<KeyValuePair<NSObject,NSObject>>()) {
		//foreach (var t in this.buttonDictionary.GetEnumerator()) {
		//	this.buttonDictionary.
		//foreach (var item  in NSArray.FromArray<NSDictionary> (this.buttonDictionary.GetEnumerator)) {
		for (nuint i = 0 ; i < this.buttonDictionary.Count; i++) {
			//id key, id obj, BOOL *stop
			NSObject key = this.buttonDictionary.Keys[i];
			NSObject obj = this.buttonDictionary.Values[i];
			
			MMNumberKeyboardButton k = (MMNumberKeyboardButton)Enum.Parse( typeof(MMNumberKeyboardButton), key.ToString());
			if (button == obj) {
			keyboardButton = k;
			//stop = true;
					break;
			}
		}

		if (keyboardButton == MMNumberKeyboardButton.None) {
			return;
		}

		// Get first responder.
		IUIKeyInput keyInput = this.keyInput;
		MMNumberKeyboardDelegate Delegate = this.Delegate;

			if (!(keyInput!=null)) {
			return;
		}

		// Handle number.
		/*const*/ MMNumberKeyboardButton numberMin = MMNumberKeyboardButton.NumberMin;
		/*const*/ MMNumberKeyboardButton numberMax = MMNumberKeyboardButton.NumberMax;

		if (keyboardButton >= numberMin && keyboardButton < numberMax) {
				NSNumber number = NSNumber.FromNInt(keyboardButton - numberMin);
			string @string = number.ToString();

			if (Delegate.RespondsToSelector(new Selector ("numberKeyboard:shouldAddText:"))) {
				//bool  shouldAdd = Delegate.numberKeyboard(this ,/*shouldAddText*/ @string);
				//bool  shouldAdd = Delegate.Invoke("numberKeyboard", new [] {this ,/*shouldAddText*/ @string});
				bool  shouldAdd = (bool) Delegate.Invoke("numberKeyboard", this ,/*shouldAddText*/ @string);
				if (!shouldAdd) {
				return;
				}
				}

				keyInput.InsertText(@string);
			}

			// Handle backspace.
			else if (keyboardButton == MMNumberKeyboardButton.Backspace) {
				keyInput.DeleteBackward();
			}

			// Handle done.
			else if (keyboardButton == MMNumberKeyboardButton.Done) {
			bool  shouldReturn = true;

			if (Delegate.RespondsToSelector(new Selector ("numberKeyboardShouldReturn*/ "))) {
				shouldReturn = (bool) Delegate.Invoke("numberKeyboardShouldReturn", this);
			}

			if (shouldReturn) {
			this._dismissKeyboard(button);
			}
			}

			// Handle special key.
			else if (keyboardButton == MMNumberKeyboardButton.Special) {
			Action handler = this.specialKeyHandler;
			if (handler!=null) {
			handler();
			}
			}

			// Handle .
			else if (keyboardButton == MMNumberKeyboardButton.DecimalPoint) {
			string decimalText = button.Title(UIControlState.Normal);
			if (Delegate.RespondsToSelector(new Selector ("numberKeyboard:shouldAddText:"))) {
					bool  shouldAdd = (bool) Delegate.Invoke("numberKeyboard", new object[] {this ,/*shouldAddText*/ decimalText});
			if (!shouldAdd) {
			return;
			}
			}

			keyInput.InsertText(decimalText);
		}
	}

	public void _backspaceRepeat( UIButton   button){
		IUIKeyInput keyInput = this.keyInput;

		if (!keyInput.HasText) {
			return;
		}

		this._buttonPlayClick(button);
		this._buttonInput(button);
	}

	public IUIKeyInput keyInput {
		get {
			IUIKeyInput keyInput = _keyInput;
			if (keyInput!=null) {
				return keyInput;
			}

				keyInput = (IUIKeyInput) new UIResponder().MM_currentFirstResponder();
			if (!(keyInput is  IUITextInput)) {
				//NSLog ,/*"Warning*/  First responder {} does not conform to the IUIKeyInput protocol.", keyInput);
				return null;
			}

			_keyInput = keyInput;

			return keyInput;
		}

		set
		{
			_keyInput = value;
		}
	}

	//#pragma mark - Default special action.

	public void _dismissKeyboard(object sender){
		UIResponder firstResponder = (UIResponder)this.keyInput;
		if (firstResponder!=null) {
		firstResponder.ResignFirstResponder();
		}
	}

	//#pragma mark - Public.

	public void configureSpecialKeyWithImage( UIImage  image ,/*actionHandler*/  Action handler){
		if (image!=null) {
		this.specialKeyHandler = handler;
		} else {
		this.specialKeyHandler = null;
		}

		UIButton button = (UIButton)this.buttonDictionary[""+MMNumberKeyboardButton.Special];
		button.SetImage(image ,/*forState*/ UIControlState.Normal);
	}

	public void configureSpecialKeyWithImage( UIImage  image ,/*target*/ object target ,/*action*/ Selector action){
//		/*__weak*/ var weakTarget = target;
//		/*__weak*/ var weakSelf = this;
//
//		this.configureSpecialKeyWithImage( image ,/*actionHandler*/ () => {
//			/*__strong*/ var strongTarget = weakTarget;
//			/*__strong*/ var strongSelf = weakSelf;
//
//			if (strongTarget==null) {
//				NSMethodSignature methodSignature = strongTarget.methodSignatureForSelector(action);
//				NSInvocation invocation = NSInvocation.invocationWithMethodSignature(methodSignature);
//				invocation.setSelector(action);
//				if (methodSignature.numberOfArguments > 2f) {
//					invocation.setArgument(&strongSelf ,/*atIndex*/ 2f);
//				}
//				invocation.invokeWithTarget(strongTarget);
//			}
//		});
	}

	public bool allowsDecimalPoint {
			get {
				return _allowsDecimalPoint;
			}
			set {
				if (value != _allowsDecimalPoint) {
					_allowsDecimalPoint = value;

					this.SetNeedsLayout ();
				}
			}
		}

		//#pragma mark - Layout.

	public CGRect MMButtonRectMake(CGRect rect, CGRect contentRect, UIUserInterfaceIdiom interfaceIdiom){
		rect.Offset( contentRect.X, contentRect.Y);

		if (interfaceIdiom == UIUserInterfaceIdiom.Pad) {
		nfloat inset = MMNumberKeyboardPadSpacing / 2.0f;
		rect = rect.Inset( inset, inset);
		}

		return rect;
	}

	public override void LayoutSubviews(){
		base.LayoutSubviews();

		CGRect bounds = new CGRect(){
			Size = this.Bounds.Size
		};

		NSDictionary buttonDictionary = this.buttonDictionary;

		// Settings.
		/*const*/ UIUserInterfaceIdiom interfaceIdiom = UI_USER_INTERFACE_IDIOM();
		/*const*/ nfloat spacing = (interfaceIdiom == UIUserInterfaceIdiom.Pad) ? MMNumberKeyboardPadBorder : 0.0f;
			/*const*/ nfloat maximumWidth = (interfaceIdiom == UIUserInterfaceIdiom.Pad) ? (nfloat)400.0 : bounds.Width;
		/*const*/ bool  allowsDecimalPoint = this.allowsDecimalPoint;

		/*const*/ nfloat width = (nfloat)Math.Min(maximumWidth, bounds.Width);
		/*const*/ CGRect contentRect = new CGRect(){
				X = (nfloat)Math.Round((bounds.Width - width) / (nfloat)2.0f),
				Y = spacing,
				Size = new CGSize(){
					Width = width,
					Height = bounds.Height - spacing * 2.0f 
				}
		};

		// Layout.
		/*const*/ nfloat columnWidth = contentRect.Width / 4.0f;
		/*const*/ nfloat rowHeight = MMNumberKeyboardRowHeight;

		CGSize numberSize = new CGSize(columnWidth, rowHeight);

		// Layout numbers.
		/*const*/ MMNumberKeyboardButton numberMin = MMNumberKeyboardButton.NumberMin;
		/*const*/ MMNumberKeyboardButton numberMax = MMNumberKeyboardButton.NumberMax;

		/*const*/ nint numbersPerLine = 3;

		for (MMNumberKeyboardButton key = numberMin; key < numberMax; key++) {
			UIButton button = (UIButton)buttonDictionary[""+key];
			nint digit = key - numberMin;

			CGRect rect = new CGRect(){ Size = numberSize };

			if (digit == 0f) {
				rect.Y = numberSize.Height * 3f;
				rect.X = numberSize.Width;

				if (!allowsDecimalPoint) {
					rect.Size=  new CGSize(){Width = numberSize.Width * 2.0f};
					button.ContentEdgeInsets = new UIEdgeInsets(0f, 0f, 0f, numberSize.Width);
				}

			} else {
				nint idx = (digit - 1);

				nint line = idx / numbersPerLine;
				nint pos = idx % numbersPerLine;

				rect.Y = line * numberSize.Height;
				rect.X = pos * numberSize.Width;
			}

			button.Frame = MMButtonRectMake(rect, contentRect, interfaceIdiom);
		}

		// Layout special key.
		UIButton specialKey = (UIButton) buttonDictionary[""+MMNumberKeyboardButton.Special];
		if (specialKey!=null) {
		CGRect rect = new CGRect(){ Size = numberSize };
		rect.Y = numberSize.Height * 3f;

		specialKey.Frame = MMButtonRectMake(rect, contentRect, interfaceIdiom);
		}

		// Layout decimal point.
		UIButton decimalPointKey = (UIButton) buttonDictionary[""+MMNumberKeyboardButton.DecimalPoint];
		if (decimalPointKey!=null) {
				CGRect rect = new CGRect(){ Size = numberSize };
		rect.Y = numberSize.Height * 3f;
		rect.X = numberSize.Width * 2f;

		decimalPointKey.Frame = MMButtonRectMake(rect, contentRect, interfaceIdiom);

		decimalPointKey.Hidden = !allowsDecimalPoint;
		}

		// Layout utility column.
		/*const*/ MMNumberKeyboardButton []utilityButtonKeys = new MMNumberKeyboardButton[] { MMNumberKeyboardButton.Backspace, MMNumberKeyboardButton.Done };
		/*const*/ CGSize utilitySize = new CGSize(columnWidth, rowHeight * 2.0f);

		for (nint idx = 0; idx < utilityButtonKeys.Length; idx++) {
			MMNumberKeyboardButton key = utilityButtonKeys[idx];

			UIButton button = (UIButton) buttonDictionary[""+key];
			CGRect rect = new CGRect(){ Size = utilitySize };

			rect.X = columnWidth * 3.0f;
			rect.Y = idx * utilitySize.Height;

			button.Frame = MMButtonRectMake(rect, contentRect, interfaceIdiom);
		}

		// Layout separators if phone.
		if (interfaceIdiom != UIUserInterfaceIdiom.Pad) {
			NSArray separatorViews = this.separatorViews;

			/*const*/ nuint totalColumns = 4;
			/*const*/ nuint totalRows = (nuint) numbersPerLine + 1;
			/*const*/ nuint numberOfSeparators = totalColumns * totalRows - 1;

			if (separatorViews.Count != numberOfSeparators) {
					separatorViews.PerformSelector(new Selector("makeObjectsPerformSelector"),NSObject.FromObject(new Selector("removeFromSuperview")));

			NSMutableArray neueSeparatorViews = new NSMutableArray(numberOfSeparators);

			for (nuint idx = 0; idx < numberOfSeparators; idx++) {
				UIView separator = new UIView(CGRect.Empty);
				separator.BackgroundColor = UIColor.FromWhiteAlpha(0.0f ,/*alpha*/ 0.1f);

				this.AddSubview(separator);
				neueSeparatorViews.Add(separator);
			}

			this.separatorViews = neueSeparatorViews;
			}

				/*const*/ nfloat separatorDimension = 1.0f / ((this.Window.Screen.Scale != 0) ? this.Window.Screen.Scale:  1.0f);

			separatorViews.enumerateObjectsUsingBlock( (object obj, nuint idx, out bool  stop) =>{
				UIView separator = (UIView)obj;

				CGRect rect = CGRect.Empty;

				if (idx < totalRows) {
					rect.Y = idx * rowHeight;
					if (idx % 2 != 0) {
						rect.Size = new CGSize(rect.Size){	Width = contentRect.Width - columnWidth};
					} else {
						rect.Size = new CGSize(rect.Size){	Width = contentRect.Width};
					}
					rect.Size = new CGSize(rect.Size){	Height = separatorDimension};
				} else {
					nuint col = (idx - totalRows);

					rect.X = (col + 1) * columnWidth;
						rect.Size = new CGSize(rect.Size){	Width = separatorDimension};

					if (col == 1 && !allowsDecimalPoint) {
						rect.Size = new CGSize(rect.Size){	Height = contentRect.Height - rowHeight};
					} else {
						rect.Size = new CGSize(rect.Size){	Height = contentRect.Height};
					}
				}

				separator.Frame = MMButtonRectMake(rect, contentRect, interfaceIdiom);
				stop = false;
			});
		}
	}

	public CGSize sizeThatFits( CGSize size ){
		/*const*/ UIUserInterfaceIdiom interfaceIdiom = UI_USER_INTERFACE_IDIOM();
		/*const*/ nfloat spacing = (interfaceIdiom == UIUserInterfaceIdiom.Pad) ? MMNumberKeyboardPadBorder : 0.0f;

		size.Height = MMNumberKeyboardRowHeight * MMNumberKeyboardRows + (spacing * 2.0f);

		if (size.Width == 0.0f) {
		size.Width = UIScreen.MainScreen.Bounds.Size.Width;
		}

		return size;
	}

	//#pragma mark - Audio feedback.

	public bool  enableInputClicksWhenVisible(){
		return true;
	}

		//#pragma mark - Accessing keyboard images.

	public UIImage _keyboardImageNamed( string name)
	{
		string resource = new NSString(name).DeletePathExtension();
		string extension = new NSString(name).PathExtension;

		if (resource!=null) {
				NSBundle bundle = NSBundle.FromClass(this.Class);
			if (bundle!=null) {
				string resourcePath = bundle.PathForResource(resource ,/*ofType*/ extension);

				return UIImage.FromFile(resourcePath);
			} else {
				return UIImage.FromBundle(name);
			}
		}
		return null;
	}

}

	public interface _IMMNumberKeyboardButton{

		NSTimer continuousPressTimer{get;set;}
		double continuousPressTimeInterval{get;set;}

		UIColor fillColor{get;set;}
		UIColor highlightedFillColor{get;set;}

		UIColor controlColor{get;set;}
		UIColor highlightedControlColor{get;set;}

	}

	public class _MMNumberKeyboardButton: UIButton, _IMMNumberKeyboardButton{


		public NSTimer continuousPressTimer{get;set;}
		public double continuousPressTimeInterval{get;set;}

		public UIColor fillColor{get;set;}
		public UIColor highlightedFillColor{get;set;}

		public UIColor controlColor{get;set;}
		public UIColor highlightedControlColor{get;set;}

		public  _MMNumberKeyboardButton(/*keyboardButtonWithType*/  MMNumberKeyboardButtonType buttonType)
		{
			//_MMNumberKeyboardButton button = new _MMNumberKeyboardButton(MMNumberKeyboardButtonType.Done);

			/*const*/ UIUserInterfaceIdiom interfaceIdiom = UI_USER_INTERFACE_IDIOM();

			UIColor fillColor = null;
			UIColor highlightedFillColor = null;
			if (buttonType == MMNumberKeyboardButtonType.White) {
				fillColor = UIColor.White;
				highlightedFillColor = UIColor.FromRGBA(0.82f ,/*green*/ 0.837f ,/*blue*/ 0.863f ,/*alpha*/ 1f);
			} else if (buttonType == MMNumberKeyboardButtonType.Gray) {
				if (interfaceIdiom == UIUserInterfaceIdiom.Pad) {
					fillColor =  UIColor.FromRGBA(0.674f ,/*green*/ 0.7f ,/*blue*/ 0.744f ,/*alpha*/ 1f);
				} else {
					fillColor = UIColor.FromRGBA(0.81f ,/*green*/ 0.837f ,/*blue*/ 0.86f ,/*alpha*/ 1f);
				}
				highlightedFillColor = UIColor.White;
			} else if (buttonType == MMNumberKeyboardButtonType.Done) {
				fillColor = UIColor.FromRGBA(0f ,/*green*/ 0.479f ,/*blue*/ 1f ,/*alpha*/ 1f);
				highlightedFillColor = UIColor.White;
			}

			UIColor controlColor = null;
			UIColor highlightedControlColor = null;
			if (buttonType == MMNumberKeyboardButtonType.Done) {
				controlColor = UIColor.White;
				highlightedControlColor = UIColor.Black;
			} else {
				controlColor = UIColor.Black;
				highlightedControlColor = UIColor.Black;
			}

			this.SetTitleColor(controlColor ,/*forState*/ UIControlState.Normal);
			this.SetTitleColor(highlightedControlColor ,/*forState*/ UIControlState.Selected);
			this.SetTitleColor(highlightedControlColor ,/*forState*/ UIControlState.Highlighted);

			this.fillColor = fillColor;
			this.highlightedFillColor = highlightedFillColor;
			this.controlColor = controlColor;
			this.highlightedControlColor = highlightedControlColor;

			if (interfaceIdiom == UIUserInterfaceIdiom.Pad) {
				CALayer buttonLayer = this.Layer;
				buttonLayer.CornerRadius = 4.0f;
				buttonLayer.ShadowColor = UIColor.FromRGBA(0.533f ,/*green*/ 0.541f ,/*blue*/ 0.556f ,/*alpha*/ 1f).CGColor;
				buttonLayer.ShadowOffset = new CGSize(0f, 1.0f);
				buttonLayer.ShadowOpacity = 1.0f;
				buttonLayer.ShadowRadius = 0.0f;
			}

			//return button;
		}

		public override void WillMoveToWindow( UIWindow  newWindow){
			base.WillMoveToWindow(newWindow);

			if (newWindow!= null) {
				this._updateButtonAppearance();
			}
		}

		public void _updateButtonAppearance(){
			if (this.Highlighted || this.Selected) {
				this.BackgroundColor = this.highlightedFillColor;
				this.ImageView.TintColor = this.controlColor;
			} else {
				this.BackgroundColor = this.fillColor;
				this.ImageView.TintColor = this.highlightedControlColor;
			}
		}

		public void setHighlighted(bool highlighted
		){
			base.Highlighted = highlighted;

			this._updateButtonAppearance();
		}

		//#pragma mark - Continuous press.

		public void AddTarget( object target ,/*action*/ Selector action ,/*forContinuousPressWithTimeInterval*/  double timeInterval){
			this.continuousPressTimeInterval = timeInterval;

			this.AddTarget(NSObject.FromObject(target) ,/*action*/ action ,/*forControlEvents*/ UIControlEvent.ValueChanged);
		}

		public bool  beginTrackingWithTouch( UITouch  touch ,/*withEvent*/  UIEvent  @event){
			bool  begins = base.BeginTracking(touch ,/*withEvent*/ @event);
			/*const*/ double continuousPressTimeInterval = this.continuousPressTimeInterval;

			if (begins && continuousPressTimeInterval > 0f) {
				this._beginContinuousPressDelayed();
			}

			return begins;
		}

		public void endTrackingWithTouch( UITouch  touch ,/*withEvent*/  UIEvent   @event){
			base.EndTracking(touch ,/*withEvent*/ @event);
			this._cancelContinousPressIfNeeded();
		}

		public void dealloc(){
			this._cancelContinousPressIfNeeded();
		}

		public void _beginContinuousPress(){
			/*const*/ double continuousPressTimeInterval = this.continuousPressTimeInterval;

			if (!this.Tracking || continuousPressTimeInterval == 0f) {
				return;
			}

			this.continuousPressTimer = NSTimer.CreateScheduledTimer(continuousPressTimeInterval ,/*target*/ this ,/*selector*/ new Selector ("_handleContinuousPressTimer") ,/*userInfo*/ null ,/*repeats*/ true);
			}

		public void _handleContinuousPressTimer( NSTimer  timer){
			if (!this.Tracking) {
				this._cancelContinousPressIfNeeded();
				return;
			}

			this.SendActionForControlEvents(UIControlEvent.ValueChanged);
		}

		public void _beginContinuousPressDelayed(){
			this.PerformSelector(new Selector("_beginContinuousPress") ,/*withObject*/ null ,/*afterDelay*/ this.continuousPressTimeInterval * 2.0f);
		}

		public void _cancelContinousPressIfNeeded(){
			NSObject.CancelPreviousPerformRequest(this ,/*selector*/ new Selector("_beginContinuousPress") ,/*object*/ null);

			NSTimer timer = this.continuousPressTimer;
			if (timer!=null) {
				timer.Invalidate();

				this.continuousPressTimer = null;
			}
		}


	}
}

