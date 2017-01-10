// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2016-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A class implements OnNetworkNack if it has onNetworkNack, used to pass a
	/// callback to Face.expressInterest.
	/// </summary>
	///
	public interface OnNetworkNack {
		/// <summary>
		/// When a network Nack packet is received, onNetworkNack is called.
		/// </summary>
		///
		/// <param name="interest"></param>
		/// <param name="networkNack">The received NetworkNack object.</param>
		void onNetworkNack(Interest interest, NetworkNack networkNack);
	}
}
