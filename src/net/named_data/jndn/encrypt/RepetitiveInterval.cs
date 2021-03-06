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
	
	using ILOG.J2CsMapping.Util;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A RepetitiveInterval is an advanced interval which can repeat and can be used
	/// to find a simple Interval that a time point falls in.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public class RepetitiveInterval : IComparable {
		public enum RepeatUnit {
			NONE, DAY, MONTH, YEAR
		}
	
		/// <summary>
		/// Get the numeric value associated with the repeatUnit. This is a separate
		/// method for portability.
		/// </summary>
		///
		/// <param name="repeatUnit">The RepeatUnit.</param>
		/// <returns>The numeric value for repeatUnit.</returns>
		public static int getRepeatUnitNumericType(RepetitiveInterval.RepeatUnit  repeatUnit) {
			if (repeatUnit == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY)
				return 1;
			else if (repeatUnit == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.MONTH)
				return 2;
			else if (repeatUnit == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.YEAR)
				return 3;
			else
				return 0;
		}
	
		public class Result {
			public Result(bool isPositive, Interval interval) {
				this.isPositive = isPositive;
				this.interval = interval;
			}
	
			public bool isPositive;
			public Interval interval;
		}
	
		/// <summary>
		/// Create a default RepetitiveInterval with one day duration, non-repeating.
		/// </summary>
		///
		public RepetitiveInterval() {
			startDate_ = -System.Double.MaxValue;
			endDate_ = -System.Double.MaxValue;
			intervalStartHour_ = 0;
			intervalEndHour_ = 24;
			nRepeats_ = 0;
			repeatUnit_ = net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.NONE;
		}
	
		/// <summary>
		/// Create a RepetitiveInterval with the given values. startDate must be
		/// earlier than or same as endDate. intervalStartHour must be less than
		/// intervalEndHour.
		/// </summary>
		///
		/// <param name="startDate">The start date as milliseconds since Jan 1, 1970 UTC.</param>
		/// <param name="endDate">The end date as milliseconds since Jan 1, 1970 UTC.</param>
		/// <param name="intervalStartHour">The start hour in the day, from 0 to 23.</param>
		/// <param name="intervalEndHour">The end hour in the day from 1 to 24.</param>
		/// <param name="nRepeats"></param>
		/// <param name="repeatUnit"></param>
		/// <exception cref="System.Exception">if the above conditions are not met.</exception>
		public RepetitiveInterval(double startDate, double endDate,
				int intervalStartHour, int intervalEndHour, int nRepeats,
				RepetitiveInterval.RepeatUnit  repeatUnit) {
			startDate_ = toDateOnlyMilliseconds(startDate);
			endDate_ = toDateOnlyMilliseconds(endDate);
			intervalStartHour_ = intervalStartHour;
			intervalEndHour_ = intervalEndHour;
			nRepeats_ = nRepeats;
			repeatUnit_ = repeatUnit;
	
			validate();
		}
	
		/// <summary>
		/// Create a RepetitiveInterval with the given values, and no repetition.
		/// Because there is no repetition, startDate must equal endDate.
		/// intervalStartHour must be less than intervalEndHour.
		/// </summary>
		///
		/// <param name="startDate">The start date as milliseconds since Jan 1, 1970 UTC.</param>
		/// <param name="endDate">The end date as milliseconds since Jan 1, 1970 UTC.</param>
		/// <param name="intervalStartHour">The start hour in the day, from 0 to 23.</param>
		/// <param name="intervalEndHour">The end hour in the day from 1 to 24.</param>
		/// <exception cref="System.Exception">if the above conditions are not met.</exception>
		public RepetitiveInterval(double startDate, double endDate,
				int intervalStartHour, int intervalEndHour) {
			startDate_ = toDateOnlyMilliseconds(startDate);
			endDate_ = toDateOnlyMilliseconds(endDate);
			intervalStartHour_ = intervalStartHour;
			intervalEndHour_ = intervalEndHour;
			nRepeats_ = 0;
			repeatUnit_ = net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.NONE;
	
			validate();
		}
	
		/// <summary>
		/// Create a RepetitiveInterval, copying values from the given repetitiveInterval.
		/// </summary>
		///
		/// <param name="repetitiveInterval">The RepetitiveInterval to copy values from.</param>
		public RepetitiveInterval(RepetitiveInterval repetitiveInterval) {
			startDate_ = repetitiveInterval.startDate_;
			endDate_ = repetitiveInterval.endDate_;
			intervalStartHour_ = repetitiveInterval.intervalStartHour_;
			intervalEndHour_ = repetitiveInterval.intervalEndHour_;
			nRepeats_ = repetitiveInterval.nRepeats_;
			repeatUnit_ = repetitiveInterval.repeatUnit_;
		}
	
		private void validate() {
			if (!(intervalStartHour_ < intervalEndHour_))
				throw new Exception(
						"ReptitiveInterval: startHour must be less than endHour");
			if (!(startDate_ <= endDate_))
				throw new Exception(
						"ReptitiveInterval: startDate must be earlier than or same as endDate");
			if (!(intervalStartHour_ >= 0))
				throw new Exception(
						"ReptitiveInterval: intervalStartHour must be non-negative");
			if (!(intervalEndHour_ >= 1 && intervalEndHour_ <= 24))
				throw new Exception(
						"ReptitiveInterval: intervalEndHour must be from 1 to 24");
			if (repeatUnit_ == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.NONE) {
				if (!(startDate_ == endDate_))
					throw new Exception(
							"ReptitiveInterval: With RepeatUnit.NONE, startDate must equal endDate");
			}
		}
	
		/// <summary>
		/// Get an interval that covers the time point. If there is no interval
		/// covering the time point, this returns false for isPositive and returns a
		/// negative interval.
		/// </summary>
		///
		/// <param name="timePoint">The time point as milliseconds since Jan 1, 1970 UTC.</param>
		/// <returns>An object with fields (isPositive, interval) where isPositive is
		/// true if the returned interval is positive or false if negative, and
		/// interval is the Interval covering the time point or a negative interval if
		/// not found.</returns>
		public RepetitiveInterval.Result  getInterval(double timePoint) {
			bool isPositive_0;
			double startTime;
			double endTime;
	
			if (!hasIntervalOnDate(timePoint)) {
				// There is no interval on the date of timePoint.
				startTime = toDateOnlyMilliseconds(timePoint);
				endTime = toDateOnlyMilliseconds(timePoint) + 24
						* MILLISECONDS_IN_HOUR;
				isPositive_0 = false;
			} else {
				// There is an interval on the date of timePoint.
				startTime = toDateOnlyMilliseconds(timePoint) + intervalStartHour_
						* MILLISECONDS_IN_HOUR;
				endTime = toDateOnlyMilliseconds(timePoint) + intervalEndHour_
						* MILLISECONDS_IN_HOUR;
	
				// check if in the time duration
				if (timePoint < startTime) {
					endTime = startTime;
					startTime = toDateOnlyMilliseconds(timePoint);
					isPositive_0 = false;
				} else if (timePoint > endTime) {
					startTime = endTime;
					endTime = toDateOnlyMilliseconds(timePoint)
							+ MILLISECONDS_IN_DAY;
					isPositive_0 = false;
				} else
					isPositive_0 = true;
			}
	
			return new RepetitiveInterval.Result (isPositive_0, new Interval(startTime, endTime));
		}
	
		/// <summary>
		/// Compare this to the other RepetitiveInterval.
		/// </summary>
		///
		/// <param name="other">The other RepetitiveInterval to compare to.</param>
		/// <returns>-1 if this is less than the other, 1 if greater and 0 if equal.</returns>
		public int compare(RepetitiveInterval other) {
			if (startDate_ < other.startDate_)
				return -1;
			if (startDate_ > other.startDate_)
				return 1;
	
			if (endDate_ < other.endDate_)
				return -1;
			if (endDate_ > other.endDate_)
				return 1;
	
			if (intervalStartHour_ < other.intervalStartHour_)
				return -1;
			if (intervalStartHour_ > other.intervalStartHour_)
				return 1;
	
			if (intervalEndHour_ < other.intervalEndHour_)
				return -1;
			if (intervalEndHour_ > other.intervalEndHour_)
				return 1;
	
			if (nRepeats_ < other.nRepeats_)
				return -1;
			if (nRepeats_ > other.nRepeats_)
				return 1;
	
			// Lastly, compare the repeat units.
			// Compare without using Integer.compare so it works in older Java compilers.
			if (getRepeatUnitNumericType(repeatUnit_) < getRepeatUnitNumericType(other.repeatUnit_))
				return -1;
			else if (getRepeatUnitNumericType(repeatUnit_) == getRepeatUnitNumericType(other.repeatUnit_))
				return 0;
			else
				return 1;
		}
	
		public virtual int compareTo(Object other) {
			return compare((RepetitiveInterval) other);
		}
	
		// Also include this version for portability.
		public int CompareTo(Object other) {
			return compare((RepetitiveInterval) other);
		}
	
		public override bool Equals(Object other) {
			if (!(other  is  RepetitiveInterval))
				return false;
	
			return compare((RepetitiveInterval) other) == 0;
		}
	
		public override int GetHashCode() {
			long longStartDate = BitConverter.DoubleToInt64Bits(startDate_);
			long longEndDate = BitConverter.DoubleToInt64Bits(endDate_);
			int hash = 3;
			hash = 73 * hash + (int) (longStartDate ^ ((long) (((ulong) longStartDate) >> 32)));
			hash = 73 * hash + (int) (longEndDate ^ ((long) (((ulong) longEndDate) >> 32)));
			hash = 73 * hash + intervalStartHour_;
			hash = 73 * hash + intervalEndHour_;
			hash = 73 * hash + nRepeats_;
			hash = 73 * hash + getRepeatUnitNumericType(repeatUnit_);
			return hash;
		}
	
		/// <summary>
		/// Get the start date.
		/// </summary>
		///
		/// <returns>The start date as milliseconds since Jan 1, 1970 UTC.</returns>
		public double getStartDate() {
			return startDate_;
		}
	
		/// <summary>
		/// Get the end date.
		/// </summary>
		///
		/// <returns>The end date as milliseconds since Jan 1, 1970 UTC.</returns>
		public double getEndDate() {
			return endDate_;
		}
	
		/// <summary>
		/// Get the interval start hour.
		/// </summary>
		///
		/// <returns>The interval start hour.</returns>
		public int getIntervalStartHour() {
			return intervalStartHour_;
		}
	
		/// <summary>
		/// Get the interval end hour.
		/// </summary>
		///
		/// <returns>The interval end hour.</returns>
		public int getIntervalEndHour() {
			return intervalEndHour_;
		}
	
		/// <summary>
		/// Get the number of repeats.
		/// </summary>
		///
		/// <returns>The number of repeats.</returns>
		public int getNRepeats() {
			return nRepeats_;
		}
	
		/// <summary>
		/// Get the repeat unit.
		/// </summary>
		///
		/// <returns>The repeat unit.</returns>
		public RepetitiveInterval.RepeatUnit  getRepeatUnit() {
			return repeatUnit_;
		}
	
		/// <summary>
		/// Check if the date of the time point is in any interval.
		/// </summary>
		///
		/// <param name="timePoint">The time point as milliseconds since Jan 1, 1970 UTC.</param>
		/// <returns>True if the date of the time point is in any interval.</returns>
		private bool hasIntervalOnDate(double timePoint) {
			double timePointDateMilliseconds = toDateOnlyMilliseconds(timePoint);
	
			if (timePointDateMilliseconds < startDate_
					|| timePointDateMilliseconds > endDate_)
				return false;
	
			if (repeatUnit_ == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.NONE)
				return true;
			else if (repeatUnit_ == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY) {
				long durationDays = (long) (timePointDateMilliseconds - startDate_)
						/ MILLISECONDS_IN_DAY;
				if (durationDays % nRepeats_ == 0)
					return true;
			} else {
				Calendar timePointDate = toCalendar(timePointDateMilliseconds);
				Calendar startDate = toCalendar(startDate_);
	
				if (repeatUnit_ == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.MONTH
						&& timePointDate.get(ILOG.J2CsMapping.Util.Calendar.DAY_OF_MONTH) == startDate
								.get(ILOG.J2CsMapping.Util.Calendar.DAY_OF_MONTH)) {
					int yearDifference = timePointDate.get(ILOG.J2CsMapping.Util.Calendar.YEAR)
							- startDate.get(ILOG.J2CsMapping.Util.Calendar.YEAR);
					int monthDifference = 12 * yearDifference
							+ timePointDate.get(ILOG.J2CsMapping.Util.Calendar.MONTH)
							- startDate.get(ILOG.J2CsMapping.Util.Calendar.MONTH);
					if (monthDifference % nRepeats_ == 0)
						return true;
				} else if (repeatUnit_ == net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.YEAR
						&& timePointDate.get(ILOG.J2CsMapping.Util.Calendar.DAY_OF_MONTH) == startDate
								.get(ILOG.J2CsMapping.Util.Calendar.DAY_OF_MONTH)
						&& timePointDate.get(ILOG.J2CsMapping.Util.Calendar.MONTH) == startDate
								.get(ILOG.J2CsMapping.Util.Calendar.MONTH)) {
					int difference = timePointDate.get(ILOG.J2CsMapping.Util.Calendar.YEAR)
							- startDate.get(ILOG.J2CsMapping.Util.Calendar.YEAR);
					if (difference % nRepeats_ == 0)
						return true;
				}
			}
	
			return false;
		}
	
		/// <summary>
		/// Return a time point on the beginning of the date (without hours, minutes, etc.)
		/// </summary>
		///
		/// <param name="timePoint">The time point as milliseconds since Jan 1, 1970 UTC.</param>
		/// <returns>A time point as milliseconds since Jan 1, 1970 UTC.</returns>
		public static double toDateOnlyMilliseconds(double timePoint) {
			long result = (long) Math.Round(timePoint,MidpointRounding.AwayFromZero);
			result -= result % MILLISECONDS_IN_DAY;
			return result;
		}
	
		/// <summary>
		/// Return a Calendar for the time point.
		/// </summary>
		///
		/// <param name="timePoint">The time point as milliseconds since Jan 1, 1970 UTC.</param>
		/// <returns>The Calendar.</returns>
		private static Calendar toCalendar(double timePoint) {
			Calendar result = ILOG.J2CsMapping.Util.Calendar.getInstance(System.Collections.TimeZone.getTimeZone("UTC"));
			result.setTimeInMillis((long) timePoint);
			return result;
		}
	
		private const long MILLISECONDS_IN_HOUR = 3600 * 1000;
		private const long MILLISECONDS_IN_DAY = 24 * 3600 * 1000;
		private readonly double startDate_; // MillisecondsSince1970 UTC
		private readonly double endDate_; // MillisecondsSince1970 UTC
		private readonly int intervalStartHour_;
		private readonly int intervalEndHour_;
		private readonly int nRepeats_;
		private readonly RepetitiveInterval.RepeatUnit  repeatUnit_;
	}
}
