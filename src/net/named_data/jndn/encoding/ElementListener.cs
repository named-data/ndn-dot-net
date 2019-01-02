// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encoding {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A class implements ElementListener if it has onReceivedElement which is used
	/// by Node.onReceivedData.
	/// </summary>
	///
	public interface ElementListener {
		/// <summary>
		/// This is called when an entire element is received.
		/// </summary>
		///
		/// <param name="element"></param>
		void onReceivedElement(ByteBuffer element);
	}
}
