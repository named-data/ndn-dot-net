// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.util;
	
	public class MetaInfo : ChangeCountable {
		/// <summary>
		/// Create a new MetaInfo with default values.
		/// </summary>
		///
		public MetaInfo() {
			this.type_ = net.named_data.jndn.ContentType.BLOB;
			this.otherTypeCode_ = -1;
			this.freshnessPeriod_ = -1;
			this.finalBlockId_ = new Name.Component();
			this.changeCount_ = 0;
		}
	
		/// <summary>
		/// Create a new MetaInfo with a copy of the fields in the given metaInfo.
		/// </summary>
		///
		/// <param name="metaInfo">The MetaInfo to copy.</param>
		public MetaInfo(MetaInfo metaInfo) {
			this.type_ = net.named_data.jndn.ContentType.BLOB;
			this.otherTypeCode_ = -1;
			this.freshnessPeriod_ = -1;
			this.finalBlockId_ = new Name.Component();
			this.changeCount_ = 0;
			type_ = metaInfo.type_;
			freshnessPeriod_ = metaInfo.freshnessPeriod_;
			// Name.Component is read-only, so we don't need a deep copy.
			finalBlockId_ = metaInfo.finalBlockId_;
		}
	
		/// <summary>
		/// Get the content type.
		/// </summary>
		///
		/// <returns>The content type enum value. If this is ContentType.OTHER_CODE,
		/// then call getOtherTypeCode() to get the unrecognized content type code.</returns>
		public ContentType getType() {
			return type_;
		}
	
		/// <summary>
		/// Get the content type code from the packet which is other than a recognized
		/// ContentType enum value. This is only meaningful if getType() is
		/// ContentType.OTHER_CODE.
		/// </summary>
		///
		/// <returns>The type code.</returns>
		public int getOtherTypeCode() {
			return otherTypeCode_;
		}
	
		public double getFreshnessPeriod() {
			return freshnessPeriod_;
		}
	
		public int getFreshnessSeconds() {
			return (freshnessPeriod_ < 0) ? -1 : (int) Math.Round(freshnessPeriod_ / 1000.0d,MidpointRounding.AwayFromZero);
		}
	
		public Name.Component getFinalBlockId() {
			return finalBlockId_;
		}
	
		public Name.Component getFinalBlockID() {
			return getFinalBlockId();
		}
	
		/// <summary>
		/// Set the content type.
		/// </summary>
		///
		/// <param name="type">call setOtherTypeCode().</param>
		public void setType(ContentType type) {
			type_ = type;
			++changeCount_;
		}
	
		/// <summary>
		/// Set the packet's content type code to use when the content type enum is
		/// ContentType.OTHER_CODE. If the packet's content type code is a recognized
		/// enum value, just call setType().
		/// </summary>
		///
		/// <param name="otherTypeCode"></param>
		public void setOtherTypeCode(int otherTypeCode) {
			if (otherTypeCode < 0)
				throw new Exception("MetaInfo other type code must be non-negative");
	
			otherTypeCode_ = otherTypeCode;
			++changeCount_;
		}
	
		public void setFreshnessPeriod(double freshnessPeriod) {
			freshnessPeriod_ = freshnessPeriod;
			++changeCount_;
		}
	
		public void setFreshnessSeconds(int freshnessSeconds) {
			setFreshnessPeriod((freshnessSeconds < 0) ? -1.0d
					: (double) freshnessSeconds * 1000.0d);
		}
	
		public void setFinalBlockId(Name.Component finalBlockId) {
			finalBlockId_ = ((finalBlockId == null) ? new Name.Component()
					: finalBlockId);
			++changeCount_;
		}
	
		public void setFinalBlockID(Name.Component finalBlockId) {
			setFinalBlockId(finalBlockId);
		}
	
		/// <summary>
		/// Get the change count, which is incremented each time this object is changed.
		/// </summary>
		///
		/// <returns>The change count.</returns>
		public long getChangeCount() {
			return changeCount_;
		}
	
		private ContentType type_;
		/// <summary>
		/// < default is ContentType.BLOB. 
		/// </summary>
		///
		private int otherTypeCode_;
		private double freshnessPeriod_;
		/// <summary>
		/// < -1 for none 
		/// </summary>
		///
		private Name.Component finalBlockId_;
		/// <summary>
		/// < size 0 for none 
		/// </summary>
		///
		private long changeCount_;
	}
}
