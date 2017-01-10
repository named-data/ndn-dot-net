// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encoding {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A TlvWireFormat extends WireFormat to override its methods to
	/// implement encoding and decoding using the preferred implementation of
	/// NDN-TLV.
	/// </summary>
	///
	public class TlvWireFormat : Tlv0_2WireFormat {
		/// <summary>
		/// Get a singleton instance of a TlvWireFormat.  Assuming that the default
		/// wire format was set with
		/// WireFormat.setDefaultWireFormat(TlvWireFormat.get()), you can check if this
		/// is the default wire encoding with
		/// if (WireFormat.getDefaultWireFormat() == TlvWireFormat.get()).
		/// </summary>
		///
		/// <returns>The singleton instance.</returns>
		public static TlvWireFormat get() {
			return instance_;
		}
	
		private static TlvWireFormat instance_ = new TlvWireFormat();
	}
}
