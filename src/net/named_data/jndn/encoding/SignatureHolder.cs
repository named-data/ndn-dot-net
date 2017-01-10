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
	using net.named_data.jndn;
	
	public interface SignatureHolder {
		// When setSignature is called through a SignatureHolder, the returned Data is ignored.
		Data setSignature(Signature Signature);
	
		Signature getSignature();
	}
}
