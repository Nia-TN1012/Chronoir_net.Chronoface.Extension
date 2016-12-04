#region Veision Info.
/**
*	@file WatchFaceUtility.cs
*	@brief Provides functions to make Watchface development more convenient.
*
*	@par Version
*	0.6.0
*	@par Author
*	Nia Tomonaka
*	@par Copyright
*	Copyright (C) 2016 Chronoir.net
*	@par Released Day
*	2016/12/03
*	@par Last modified Day
*	2016/12/03
*	@par Licence
*	MIT Licence
*	@par Contact
*	@@nia_tn1012Åi https://twitter.com/nia_tn1012/ Åj
*	@par Homepage
*	- http://chronoir.net/ (Homepage)
*	- https://github.com/Nia-TN1012/Chrooir_net.Chronoface.Utility/ (GitHub)
*	- https://www.nuget.org/packages/Chronoir_net.Chronoface.Utility/ (NuGet Gallery)
*/
#endregion

using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Text.Format;

namespace Chronoir_net.Chronoface.Utility {

	/// <summary>
	///		Provides functions to make Watchface development more convenient.
	/// </summary>
	public static class WatchfaceUtility {

		/// <summary>
		///		Converts the integer value containing the ARGB value to <see cref="Color"/> type.
		/// </summary>
		/// <param name="argb">ARGB value</param>
		/// <returns><See cref = "Color" /> type object equivalent to ARGB value</returns>
		public static Color ConvertARGBToColor( int argb ) =>
			Color.Argb( ( argb >> 24 ) & 0xFF, ( argb >> 16 ) & 0xFF, ( argb >> 8 ) & 0xFF, argb & 0xFF );

		/// <summary>
		///		Converts the <see cref="Time"/> type object to <see cref="DateTime"/> type.
		/// </summary>
		/// <param name="time"><see cref="Time"/> type object</param>
		/// <returns><see cref="DateTime"/> type object equivalent to argment</returns>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public static DateTime ConvertToDateTime( Time time ) =>
			new DateTime( time.Year, time.Month, time.MonthDay, time.Hour, time.Minute, time.Second, DateTimeKind.Local );

		/// <summary>
		///		Converts the <see cref="Java.Lang.Character"/> type object to <see cref="DateTime"/> type.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> type object</param>
		/// <returns><see cref="DateTime"/> type object equivalent to argment</returns>
		public static DateTime ConvertToDateTime( Java.Util.Calendar time ) =>
			new DateTime(
				time.Get( Java.Util.CalendarField.Year ), time.Get( Java.Util.CalendarField.Month ), time.Get( Java.Util.CalendarField.Date ),
				time.Get( Java.Util.CalendarField.HourOfDay ), time.Get( Java.Util.CalendarField.Minute ), time.Get( Java.Util.CalendarField.Second ),
				DateTimeKind.Local
			);

		/// <summary>
		///		Returns the <see cref="DateTime"/> type object specified as an argument as it is.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> type object</param>
		/// <returns>Specified argument itself</returns>
		/// <remarks>
		///		This is defined to ensure compatibility with <see cref="ConvertToDateTime(Time)"/> method and <see cref="ConvertToDateTime(Java.Util.Calendar)"/> method.
		/// </remarks>
		public static DateTime ConvertToDateTime( DateTime time ) => time;
	}

	#region BroadcastReciever Extention

	/// <summary>
	///		Provides a receiver class that receives the broadcasted <see cref="Intent"/> object and performs preset processing.
	/// </summary>
	public class ActionReservedBroadcastReceiver : BroadcastReceiver {

		/// <summary>
		///		Represents a delegate executed when a <see cref="Intent"/> object is received.
		/// </summary>
		private Action<Intent> receiver;

		/// <summary>
		///		Represents the MIME type for identifying <see cref="Intent"/> information.
		/// </summary>
		private IntentFilter intentFilter;

		/// <summary>
		///		Indicates whether <see cref="BroadcastReceiver"/> object is registered in <see cref="Application.Context"/>.
		/// </summary>
		private bool isRegistered = false;
		/// <summary>
		///		Gets and sets ( register to <see cref="Application.Context"/> or unregister ) registration state of <see cref="BroadcastReceiver"/> object to <see cref="Application.Context"/>.
		/// </summary>
		public bool IsRegistered {
			get { return isRegistered; }
			set {
				if( value != isRegistered ) {
					if( value ) {
						Application.Context.RegisterReceiver( this, intentFilter );
					}
					else {
						Application.Context.UnregisterReceiver( this );
					}
					isRegistered = value;
				}
			}
		}

		/// <summary>
		///		Creates a new instance of <see cref="ActionReservedBroadcastReceiver"/> class from the specified delegate and <see cref="Intent"/> MIME type.
		/// </summary>
		/// <param name="action">Delegate to execute when receiving <see cref="Intent"/> object</param>
		/// <param name="filter">MIME type for identifying <see cref="Intent"/> information</param>
		public ActionReservedBroadcastReceiver( Action<Intent> action, string filter ) {
			receiver = action;
			intentFilter = new IntentFilter( filter );
		}

		/// <summary>
		///		Invoked when receives a broadcast <see cref="Intent"/> object.
		/// </summary>
		/// <param name="context"><see cref="Context"/> object registering receiver</param>
		/// <param name="intent">Broadcasted <see cref="Intent"/> object</param>
		public override void OnReceive( Context context, Intent intent ) {
			receiver?.Invoke( intent );
		}
	}

	#endregion

	#region Analog hands stroke

	/// <summary>
	///		Represents the basis of the analog meter stroke function stored a <see cref="Android.Graphics.Paint"/> object, a XY coordinate and length of a hand tip.
	/// </summary>
	public abstract class AnalogHandStroke {
		/// <summary>
		///		Gets the <see cref="Android.Graphics.Paint"/> object of the hand.
		/// </summary>
		public Paint Paint { get; private set; }

		/// <summary>
		///		Gets the X coordinate of the hand tip.
		/// </summary>
		public float X { get; protected set; } = 0.0f;
		/// <summary>
		///		Gets the Y coordinate of the hand tip.
		/// </summary>
		public float Y { get; protected set; } = 0.0f;

		/// <summary>
		///		Gets the length of the hand.
		/// </summary>
		public float Length { get; set; }

		/// <summary>
		///		Creates a new instance of <see cref="AnalogHandStroke"/> class from the specified <see cref="Android.Graphics.Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Android.Graphics.Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public AnalogHandStroke( Paint paint, float length = 0.0f ) {
			Paint = paint ?? new Paint();
			Length = length;
		}
	}

	/// <summary>
	///		Provides analog meter stroke function for second hand.
	/// </summary>
	public class SecondAnalogHandStroke : AnalogHandStroke {

		/// <summary>
		///		Creates a new instance of <see cref="SecondAnalogHandStroke"/> class from the specified <see cref="Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public SecondAnalogHandStroke( Paint paint, float length = 0.0f ) : base( paint, length ) { }

		/// <summary>
		///		Calculates the XY coordinates of the second hand for the specified seconds.
		/// </summary>
		/// <param name="second">Second ( 0Å`59 )</param>
		public void SetTime( int second ) {
			// Because 2ÉŒ = 360 degrees is divided into 60, the angle per second is 6 degrees.
			// É∆_sec = second / 30 * ÉŒ
			float handRotation = second / 30f * ( float )Math.PI;
			// Converts from polar coordinates to XY coordinates at the second hand tip coordinates.
			X = ( float )Math.Sin( handRotation ) * Length;
			Y = ( float )-Math.Cos( handRotation ) * Length;
		}

		/// <summary>
		///		Calculates the XY coordinates of the second hand for the seconds of the specified <see cref="Time"/> object.
		/// </summary>
		/// <param name="time"><see cref="Time"/> object storing the time</param>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public void SetTime( Time time ) =>
			SetTime( time.Second );

		/// <summary>
		///		Calculates the XY coordinates of the second hand for the seconds of the specified <see cref="Java.Util.Calendar"/> object.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> object storing the time</param>
		public void SetTime( Java.Util.Calendar time ) =>
			SetTime( time.Get( Java.Util.CalendarField.Second ) );

		/// <summary>
		///		Calculates the XY coordinates of the second hand for the seconds of the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> object storing the time</param>
		public void SetTime( DateTime time ) =>
			SetTime( time.Second );
	}

	/// <summary>
	///		Provides analog meter stroke function for minute hand.
	/// </summary>
	public class MinuteAnalogHandStroke : AnalogHandStroke {

		/// <summary>
		///		Creates a new instance of <see cref="MinuteAnalogHandStroke"/> class from the specified <see cref="Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public MinuteAnalogHandStroke( Paint paint, float length = 0.0f ) : base( paint, length ) { }

		/// <summary>
		///		Calculates the XY coordinates of the minute hand for the specified minutes.
		/// </summary>
		/// <param name="minute">Minute ( 0Å`59 )</param>
		public void SetTime( int minute ) {
			// As with the second hand, the angle per minute is 6 degrees.
			float handRotation = minute / 30f * ( float )Math.PI;
			X = ( float )Math.Sin( handRotation ) * Length;
			Y = ( float )-Math.Cos( handRotation ) * Length;
		}

		/// <summary>
		///		Calculates the XY coordinates of the minute hand for the minutes of the specified <see cref="Time"/> object.
		/// </summary>
		/// <param name="time"><see cref="Time"/> object storing the time</param>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public void SetTime( Time time ) =>
			SetTime( time.Minute );

		/// <summary>
		///		Calculates the XY coordinates of the minute hand for the minutes of the specified <see cref="Java.Util.Calendar"/> object.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> object storing the time</param>
		public void SetTime( Java.Util.Calendar time ) =>
			SetTime( time.Get( Java.Util.CalendarField.Minute ) );

		/// <summary>
		///		Calculates the XY coordinates of the minute hand for the minutes of the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> object storing the time</param>
		public void SetTime( DateTime time ) =>
			SetTime( time.Minute );
	}

	/// <summary>
	///		Provides analog meter stroke function for hour hand.
	/// </summary>
	public class HourAnalogHandStroke : AnalogHandStroke {

		/// <summary>
		///		Creates a new instance of <see cref="HourAnalogHandStroke"/> class from the specified <see cref="Paint"/> object and hand length.
		/// </summary>
		/// <param name="paint"><see cref="Paint"/> object of the hand</param>
		/// <param name="length">Hand lendth</param>
		public HourAnalogHandStroke( Paint paint, float length = 0.0f ) : base( paint, length ) { }

		/// <summary>
		///		Calculates the XY coordinates of the hour hand for the specified hours and minutes.
		/// </summary>
		/// <param name="minute">Hour ( 0Å`23 )</param>
		/// <param name="minute">Minute ( 0Å`59 )</param>
		public void SetTime( int hour, int minute ) {
			float handRotation = ( ( hour + ( minute / 60f ) ) / 6f ) * ( float )Math.PI;
			X = ( float )Math.Sin( handRotation ) * Length;
			Y = ( float )-Math.Cos( handRotation ) * Length;
		}

		/// <summary>
		///		Calculates the XY coordinates of the hour hand for the hours and minutes of the specified <see cref="Time"/> object.
		/// </summary>
		/// <param name="time"><see cref="Time"/> object storing the time</param>
#if __ANDROID_22__
		[Obsolete( "This method is obsoleted in this android platform." )]
#endif
		public void SetTime( Time time ) =>
			SetTime( time.Hour, time.Minute );

		/// <summary>
		///		Calculates the XY coordinates of the hour hand for the hours and minutes of the specified <see cref="Java.Util.Calendar"/> object.
		/// </summary>
		/// <param name="time"><see cref="Java.Util.Calendar"/> object storing the time</param>
		public void SetTime( Java.Util.Calendar time ) =>
			SetTime( time.Get( Java.Util.CalendarField.Hour ), time.Get( Java.Util.CalendarField.Minute ) );

		/// <summary>
		///		Calculates the XY coordinates of the hour hand for the hours and minutes of the specified <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="time"><see cref="DateTime"/> object storing the time</param>
		public void SetTime( DateTime time ) =>
			SetTime( time.Hour, time.Minute );
	}

	#endregion

	#region Text style for digital

	/// <summary>
	///		Represents the text style stored the <see cref="Android.Graphics.Paint"/> object of the text and the top left XY coordinates.
	/// </summary>
	public class DigitalTextStyle {
		/// <summary>
		///		Gets the text <see cref="Android.Graphics.Paint"/> object.
		/// </summary>
		public Paint Paint { get; private set; }

		/// <summary>
		///		Gets and sets the left top X coordinate.
		/// </summary>
		public float XOffset { get; set; } = 0.0f;
		/// <summary>
		///		Gets and sets the left top Y coordinate.
		/// </summary>
		public float YOffset { get; set; } = 0.0f;

		/// <summary>
		///		Creates a new instance of the <see cref="DigitalTextStyle"/> class from the specified <see cref="Android.Graphics.Paint"/> object and the left top XY coordinates.
		/// </summary>
		/// <param name="paint">Text <see cref="Android.Graphics.Paint"/> object</param>
		/// <param name="xOffset">Left top X coordinate</param>
		/// <param name="yOffset">Left top Y coordinate</param>
		public DigitalTextStyle( Paint paint, float xOffset = 0.0f, float yOffset = 0.0f ) {
			Paint = paint ?? new Paint();
			XOffset = xOffset;
			YOffset = yOffset;
		}
	}

	#endregion
}