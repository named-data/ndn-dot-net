// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.util {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	
	/// <summary>
	/// A CommandInterestGenerator keeps track of a timestamp and generates
	/// command interests according to the NFD Signed Command Interests protocol:
	/// https://redmine.named-data.net/projects/ndn-cxx/wiki/CommandInterest
	/// </summary>
	///
	public class CommandInterestGenerator : CommandInterestPreparer {
		/// <summary>
		/// Create a new CommandInterestGenerator and initialize the timestamp to now.
		/// </summary>
		///
		public CommandInterestGenerator() {
		}
	
		/// <summary>
		/// Append a timestamp component and a random value component to interest's
		/// name. This ensures that the timestamp is greater than the timestamp used in
		/// the previous call. Then use keyChain to sign the interest which appends a
		/// SignatureInfo component and a component with the signature bits. If the
		/// interest lifetime is not set, this sets it.
		/// </summary>
		///
		/// <param name="interest">The interest whose name is append with components.</param>
		/// <param name="keyChain">The KeyChain for calling sign.</param>
		/// <param name="certificateName">The certificate name of the key to use for signing.</param>
		/// <param name="wireFormat"></param>
		public void generate(Interest interest, KeyChain keyChain,
				Name certificateName, WireFormat wireFormat) {
			prepareCommandInterestName(interest, wireFormat);
			keyChain.sign(interest, certificateName, wireFormat);
	
			if (interest.getInterestLifetimeMilliseconds() < 0)
				// The caller has not set the interest lifetime, so set it here.
				interest.setInterestLifetimeMilliseconds(1000.0d);
		}
	
		/// <summary>
		/// Append a timestamp component and a random value component to interest's
		/// name. This ensures that the timestamp is greater than the timestamp used in
		/// the previous call. Then use keyChain to sign the interest which appends a
		/// SignatureInfo component and a component with the signature bits. If the
		/// interest lifetime is not set, this sets it. Use the default WireFormat to
		/// encode the SignatureInfo and to encode interest name for signing.
		/// </summary>
		///
		/// <param name="interest">The interest whose name is append with components.</param>
		/// <param name="keyChain">The KeyChain for calling sign.</param>
		/// <param name="certificateName">The certificate name of the key to use for signing.</param>
		public void generate(Interest interest, KeyChain keyChain,
				Name certificateName) {
			generate(interest, keyChain, certificateName,
					net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	}
}
