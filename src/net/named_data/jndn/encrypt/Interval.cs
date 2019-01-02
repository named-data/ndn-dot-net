// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// An Interval defines a time duration which contains a start timestamp and an
	/// end timestamp.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public class Interval {
		/// <summary>
		/// Interval.Error extends Exception for errors using Interval methods. Note
		/// that even though this is called "Error" to be consistent with the other
		/// libraries, it extends the Java Exception class, not Error.
		/// </summary>
		///
		[Serializable]
		public class Error : Exception {
			public Error(String message) : base(message) {
			}
		}
	
		/// <summary>
		/// Create an Interval that is either invalid or an empty interval.
		/// </summary>
		///
		/// <param name="isValid"></param>
		public Interval(bool isValid) {
			isValid_ = isValid;
			startTime_ = -System.Double.MaxValue;
			endTime_ = -System.Double.MaxValue;
		}
	
		/// <summary>
		/// Create a valid Interval with the given start and end times. The start
		/// time must be less than the end time. To create an empty interval (start
		/// time equals end time), use the constructor Interval(true).
		/// </summary>
		///
		/// <param name="startTime">The start time as milliseconds since Jan 1, 1970 UTC.</param>
		/// <param name="endTime">The end time as milliseconds since Jan 1, 1970 UTC.</param>
		public Interval(double startTime, double endTime) {
			if (!(startTime < endTime))
				throw new Exception(
						"Interval start time must be less than the end time");
	
			startTime_ = startTime;
			endTime_ = endTime;
			isValid_ = true;
		}
	
		/// <summary>
		/// Create an Interval, copying values from the other interval.
		/// </summary>
		///
		/// <param name="interval">The other Interval with values to copy</param>
		public Interval(Interval interval) {
			startTime_ = interval.startTime_;
			endTime_ = interval.endTime_;
			isValid_ = interval.isValid_;
		}
	
		/// <summary>
		/// Create an Interval that is invalid.
		/// </summary>
		///
		public Interval() {
			isValid_ = false;
			startTime_ = -System.Double.MaxValue;
			endTime_ = -System.Double.MaxValue;
		}
	
		/// <summary>
		/// Set this interval to have the same values as the other interval.
		/// </summary>
		///
		/// <param name="interval">The other Interval with values to copy.</param>
		public void set(Interval interval) {
			startTime_ = interval.startTime_;
			endTime_ = interval.endTime_;
			isValid_ = interval.isValid_;
		}
	
		/// <summary>
		/// Check if the time point is in this interval.
		/// </summary>
		///
		/// <param name="timePoint">The time point to check as milliseconds since Jan 1, 1970 UTC.</param>
		/// <returns>True if timePoint is in this interval.</returns>
		public bool covers(double timePoint) {
			if (!isValid_)
				throw new Exception(
						"Interval.covers: This Interval is invalid");
	
			if (isEmpty())
				return false;
			else
				return startTime_ <= timePoint && timePoint < endTime_;
		}
	
		/// <summary>
		/// Set this Interval to the intersection of this and the other interval.
		/// This and the other interval should be valid but either can be empty.
		/// </summary>
		///
		/// <param name="interval">The other Interval to intersect with.</param>
		/// <returns>This Interval.</returns>
		public Interval intersectWith(Interval interval) {
			if (!isValid_)
				throw new Exception(
						"Interval.intersectWith: This Interval is invalid");
			if (!interval.isValid_)
				throw new Exception(
						"Interval.intersectWith: The other Interval is invalid");
	
			if (isEmpty() || interval.isEmpty()) {
				// If either is empty, the result is empty.
				startTime_ = endTime_;
				return this;
			}
	
			if (startTime_ >= interval.endTime_ || endTime_ <= interval.startTime_) {
				// The two intervals don't have an intersection, so the result is empty.
				startTime_ = endTime_;
				return this;
			}
	
			// Get the start time.
			if (startTime_ <= interval.startTime_)
				startTime_ = interval.startTime_;
	
			// Get the end time.
			if (endTime_ > interval.endTime_)
				endTime_ = interval.endTime_;
	
			return this;
		}
	
		/// <summary>
		/// Set this Interval to the union of this and the other interval.
		/// This and the other interval should be valid but either can be empty.
		/// This and the other interval should have an intersection. (Contiguous
		/// intervals are not allowed.)
		/// </summary>
		///
		/// <param name="interval">The other Interval to union with.</param>
		/// <returns>This Interval.</returns>
		/// <exception cref="Interval.Error">if the two intervals do not have an intersection.</exception>
		public Interval unionWith(Interval interval) {
			if (!isValid_)
				throw new Exception(
						"Interval.intersectWith: This Interval is invalid");
			if (!interval.isValid_)
				throw new Exception(
						"Interval.intersectWith: The other Interval is invalid");
	
			if (isEmpty()) {
				// This interval is empty, so use the other.
				startTime_ = interval.startTime_;
				endTime_ = interval.endTime_;
				return this;
			}
	
			if (interval.isEmpty())
				// The other interval is empty, so keep using this one.
				return this;
	
			if (startTime_ >= interval.endTime_ || endTime_ <= interval.startTime_)
				throw new Interval.Error(
						"Interval.unionWith: The two intervals do not have an intersection");
	
			// Get the start time.
			if (startTime_ > interval.startTime_)
				startTime_ = interval.startTime_;
	
			// Get the end time.
			if (endTime_ < interval.endTime_)
				endTime_ = interval.endTime_;
	
			return this;
		}
	
		/// <summary>
		/// Get the start time.
		/// </summary>
		///
		/// <returns>The start time as milliseconds since Jan 1, 1970 UTC.</returns>
		public double getStartTime() {
			if (!isValid_)
				throw new Exception(
						"Interval.getStartTime: This Interval is invalid");
			return startTime_;
		}
	
		/// <summary>
		/// Get the end time.
		/// </summary>
		///
		/// <returns>The end time as milliseconds since Jan 1, 1970 UTC.</returns>
		public double getEndTime() {
			if (!isValid_)
				throw new Exception(
						"Interval.getEndTime: This Interval is invalid");
			return endTime_;
		}
	
		/// <summary>
		/// Check if this Interval is valid.
		/// </summary>
		///
		/// <returns>True if this interval is valid, false if invalid.</returns>
		public bool isValid() {
			return isValid_;
		}
	
		/// <summary>
		/// Check if this Interval is empty.
		/// </summary>
		///
		/// <returns>True if this Interval is empty (start time equals end time), false
		/// if not.</returns>
		public bool isEmpty() {
			if (!isValid_)
				throw new Exception(
						"Interval.isEmpty: This Interval is invalid");
			return startTime_ == endTime_;
		}
	
		private double startTime_; // MillisecondsSince1970 UTC
		private double endTime_; // MillisecondsSince1970 UTC
		private bool isValid_;
	}
}
