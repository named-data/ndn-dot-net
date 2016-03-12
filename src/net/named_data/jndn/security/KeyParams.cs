// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2016 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// KeyParams is a base class for key parameters. Its subclasses are used to
	/// store parameters for key generation.
	/// </summary>
	///
	public class KeyParams {
		public KeyType getKeyType() {
			return keyType_;
		}
	
		protected internal KeyParams(KeyType keyType) {
			keyType_ = keyType;
		}
	
		private readonly KeyType keyType_;
	}
}
