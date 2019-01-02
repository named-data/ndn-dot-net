// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// The KeyIdType enum represents the type of a KeyId component in a key name.
	/// </summary>
	///
	public enum KeyIdType {
		/// <summary>
		/// USER_SPECIFIED: A user-specified key ID. It is the user's responsibility to
		/// ensure the uniqueness of key names.
		/// </summary>
		///
		USER_SPECIFIED,
		/// <summary>
		/// SHA256: The SHA256 hash of the public key as the key id. This KeyId type
		/// guarantees the uniqueness of key names.
		/// </summary>
		///
		SHA256,
		/// <summary>
		/// RANDOM: A 64-bit random number as the key id. This KeyId provides rough
		/// uniqueness of key names.
		/// </summary>
		///
		RANDOM
	}
}
