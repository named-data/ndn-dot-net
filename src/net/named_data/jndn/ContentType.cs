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
namespace net.named_data.jndn {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A ContentType specifies the content type in a MetaInfo object. If the
	/// content type in the packet is not a recognized enum value, then we use
	/// ContentType.OTHER_CODE and you can call MetaInfo.getOtherTypeCode(). We do
	/// this to keep the recognized content type values independent of packet
	/// encoding formats.
	/// </summary>
	///
	public enum ContentType {
		BLOB, LINK, KEY, NACK, OTHER_CODE}
}
