// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2016-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.lp {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// IncomingFaceId represents the incoming face ID header field in an NDNLPv2 packet.
	/// http://redmine.named-data.net/projects/nfd/wiki/NDNLPv2
	/// </summary>
	///
	public class IncomingFaceId {
		public IncomingFaceId() {
			this.faceId_ = -1;
		}
	
		/// <summary>
		/// Get the incoming face ID value.
		/// </summary>
		///
		/// <returns>The face ID value.</returns>
		public long getFaceId() {
			return faceId_;
		}
	
		/// <summary>
		/// Set the face ID value.
		/// </summary>
		///
		/// <param name="faceId">The incoming face ID value.</param>
		public void setFaceId(long faceId) {
			faceId_ = faceId;
		}
	
		/// <summary>
		/// Get the first header field in lpPacket which is an IncomingFaceId. This is
		/// an internal method which the application normally would not use.
		/// </summary>
		///
		/// <param name="lpPacket">The LpPacket with the header fields to search.</param>
		/// <returns>The first IncomingFaceId header field, or null if not found.</returns>
		static public IncomingFaceId getFirstHeader(LpPacket lpPacket) {
			for (int i = 0; i < lpPacket.countHeaderFields(); ++i) {
				Object field = lpPacket.getHeaderField(i);
				if (field  is  IncomingFaceId)
					return (IncomingFaceId) field;
			}
	
			return null;
		}
	
		private long faceId_;
	}
}
