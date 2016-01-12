using System;
using Foundation;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;


public static class XamarinMethodExtensions
{
	public static object Invoke(this INSObjectProtocol obj, string method){
		//return XamarinMethodExtensions.Invoke(obj,method, new object[]{});
		return obj.GetType().GetMethod (method).Invoke (obj,  new object[]{});
	}
	public static object Invoke(this INSObjectProtocol obj, string method, params object[] list ){
		return obj.GetType().GetMethod (method).Invoke (obj, list);
	}

	public delegate void enumerateKeysAndObjectsAction(NSObject key, NSObject obj , out bool stop);
	//TODO:revisar que funcione
	public static void enumerateKeysAndObjectsUsingBlock(this NSDictionary dictionary, enumerateKeysAndObjectsAction action){
		bool stop = false;
		for (nuint i = 0 ; i < dictionary.Count; i++) {
			//id key, id obj, BOOL *stop
			NSObject key = dictionary.Keys[i];
			NSObject obj = dictionary.Values[i];
			action (key,obj,out stop);
			if (stop == true) {
				break;
			}
		}	 	
	}
	public delegate void enumerateObjectsAction( object obj ,nuint idx, out bool stop);
	//TODO:revisar que funcione
	public static void enumerateObjectsUsingBlock(this NSArray array, enumerateObjectsAction action){
		bool stop = false;
		for (nuint i = 0 ; i < array.Count; i++) {
			//id key, id obj, BOOL *stop
			NSObject obj = NSObject.FromObject( array.ValueAt(i));
			action (obj,i,out stop);
			if (stop == true) {
				break;
			}
		}	 	
	}

	public static NSObject GetEnumerator(this IEnumerator<KeyValuePair<NSObject,NSObject>> o)
	{
		//return o.GetEnumerator ();
		return o.GetEnumerator ();
	}
	public static NSObject ToNSObject(this object o)
	{
		NSObject toReturn;
		// Specific types first - DateTime
		if (o is DateTime) {
			toReturn = (NSDate)((DateTime)o);
		}
		// Now a String
		else if (o is string) {
			toReturn = (NSString)((string)o);
		}
		// And a catch-all for number types
		else if (typeof(IConvertible).IsAssignableFrom (o.GetType ())) {
			try {
				// Most types will convert happily to a double
				toReturn = (NSNumber)(((IConvertible)o).ToDouble (CultureInfo.InvariantCulture.NumberFormat));
			} catch (InvalidCastException e) {
				throw new InvalidCastException ("Unable to convert from .net type to objC type.", e);
			}
		}
		// We can't do it
		else {
			throw new InvalidCastException ("Unable to convert from .net type to objC type.");
		}

		// Send the result back
		return toReturn;
	}

	//	public static Object ToObject (this NSObject nsO)
	//	{
	//		return nsO.ToObject (null);
	//	}

	//      public enum TypeCode
	//      {
	//          Empty,
	//          Object,
	//          DBNull,
	//          Boolean,
	//          Char,
	//          SByte,
	//          Byte,
	//          Int16,
	//          UInt16,
	//          Int32,
	//          UInt32,
	//          Int64,
	//          UInt64,
	//          Single,
	//          Double,
	//          Decimal,
	//          DateTime,
	//          String = 18
	//      }
	#region Data

	/// <summary>The NSDate from Xamarin takes a reference point form January 1, 2001, at 12:00</summary>
	/// <remarks>
	/// It also has calls for NIX reference point 1970 but appears to be problematic
	/// </remarks>
	private static DateTime nsRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); // last zero is milliseconds

	#endregion

	/// <summary>Returns the seconds interval for a DateTime from NSDate reference data of January 1, 2001</summary>
	/// <param name="dt">The DateTime to evaluate</param>
	/// <returns>The seconds since NSDate reference date</returns>
	public static double SecondsSinceNSRefenceDate(this DateTime dt) {
		return (dt - nsRef).TotalSeconds;
	}


	/// <summary>Convert a DateTime to NSDate</summary>
	/// <param name="dt">The DateTime to convert</param>
	/// <returns>An NSDate</returns>
	public static NSDate ToNSDate(this DateTime dt) {
		return NSDate.FromTimeIntervalSinceReferenceDate(dt.SecondsSinceNSRefenceDate());
	}


	/// <summary>Convert an NSDate to DateTime</summary>
	/// <param name="nsDate">The NSDate to convert</param>
	/// <returns>A DateTime</returns>
	public static DateTime ToDateTime(this NSDate nsDate) {
		// We loose granularity below millisecond range but that is probably ok
		return nsRef.AddSeconds(nsDate.SecondsSinceReferenceDate);
	}
	public static Object ToObject (this NSObject nsO, Type targetType)
	{
		if (nsO is NSString) {
			return nsO.ToString ();
		}

		if (nsO is NSDate) {
			var nsDate = (NSDate)nsO;
			return DateTime.SpecifyKind (nsDate.ToDateTime(), DateTimeKind.Unspecified);
		}

		if (nsO is NSDecimalNumber) {
			return decimal.Parse (nsO.ToString (), CultureInfo.InvariantCulture);
		}

		if (nsO is NSNumber) {
			var x = (NSNumber)nsO;

			switch (Type.GetTypeCode (targetType)) {
			case TypeCode.Boolean:
				return x.BoolValue;
			case TypeCode.Char:
				return Convert.ToChar (x.ByteValue);
			case TypeCode.SByte:
				return x.SByteValue;
			case TypeCode.Byte:
				return x.ByteValue;
			case TypeCode.Int16:
				return x.Int16Value;
			case TypeCode.UInt16:
				return x.UInt16Value;
			case TypeCode.Int32:
				return x.Int32Value;
			case TypeCode.UInt32:
				return x.UInt32Value;
			case TypeCode.Int64:
				return x.Int64Value;
			case TypeCode.UInt64:
				return x.UInt64Value;
			case TypeCode.Single:
				return x.FloatValue;
			case TypeCode.Double:
				return x.DoubleValue;
			}
		}

		if (nsO is NSValue) {
			var v = (NSValue)nsO;

			if (targetType == typeof(IntPtr)) {
				return v.PointerValue;
			}

			if (targetType == typeof(SizeF)) {
				return v.SizeFValue;
			}

			if (targetType == typeof(RectangleF)) {
				return v.RectangleFValue;
			}

			if (targetType == typeof(PointF)) {
				return v.PointFValue;
			}           
		}

		return nsO;
	}
}


