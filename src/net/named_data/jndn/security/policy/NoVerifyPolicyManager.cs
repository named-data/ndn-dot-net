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
namespace net.named_data.jndn.security.policy {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	
	public class NoVerifyPolicyManager : PolicyManager {
		/// <summary>
		/// Override to always skip verification and trust as valid.
		/// </summary>
		///
		/// <param name="data">The received data packet.</param>
		/// <returns>true.</returns>
		public sealed override bool skipVerifyAndTrust(Data data) {
			return true;
		}
	
		/// <summary>
		/// Override to always skip verification and trust as valid.
		/// </summary>
		///
		/// <param name="interest">The received interest.</param>
		/// <returns>true.</returns>
		public sealed override bool skipVerifyAndTrust(Interest interest) {
			return true;
		}
	
		/// <summary>
		/// Override to return false for no verification rule for the received data.
		/// </summary>
		///
		/// <param name="data">The received data packet.</param>
		/// <returns>false.</returns>
		public sealed override bool requireVerify(Data data) {
			return false;
		}
	
		/// <summary>
		/// Override to return false for no verification rule for the received interest.
		/// </summary>
		///
		/// <param name="interest">The received interest.</param>
		/// <returns>false.</returns>
		public sealed override bool requireVerify(Interest interest) {
			return false;
		}
	
		/// <summary>
		/// Override to call onVerified.onVerified(data) and to indicate no further
		/// verification step.
		/// </summary>
		///
		/// <param name="data">The Data object with the signature to check.</param>
		/// <param name="stepCount"></param>
		/// <param name="onVerified">better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onValidationFailed">Override to ignore this.</param>
		/// <returns>null for no further step.</returns>
		public sealed override ValidationRequest checkVerificationPolicy(Data data,
				int stepCount, OnVerified onVerified,
				OnDataValidationFailed onValidationFailed) {
			try {
				onVerified.onVerified(data);
			} catch (Exception ex) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onVerified", ex);
			}
			return null;
		}
	
		/// <summary>
		/// Override to call onVerified.onVerifiedInterest(interest) and to indicate no
		/// further verification step.
		/// </summary>
		///
		/// <param name="interest">The interest with the signature (to ignore).</param>
		/// <param name="stepCount"></param>
		/// <param name="onVerified">better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onValidationFailed">Override to ignore this.</param>
		/// <returns>null for no further step.</returns>
		public sealed override ValidationRequest checkVerificationPolicy(Interest interest,
				int stepCount, OnVerifiedInterest onVerified,
				OnInterestValidationFailed onValidationFailed, WireFormat wireFormat) {
			try {
				onVerified.onVerifiedInterest(interest);
			} catch (Exception ex) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onVerifiedInterest", ex);
			}
			return null;
		}
	
		/// <summary>
		/// Override to always indicate that the signing certificate name and data name
		/// satisfy the signing policy.
		/// </summary>
		///
		/// <param name="dataName">The name of data to be signed.</param>
		/// <param name="certificateName">The name of signing certificate.</param>
		/// <returns>true to indicate that the signing certificate can be used to sign
		/// the data.</returns>
		public sealed override bool checkSigningPolicy(Name dataName, Name certificateName) {
			return true;
		}
	
		/// <summary>
		/// Override to indicate that the signing identity cannot be inferred.
		/// </summary>
		///
		/// <param name="dataName">The name of data to be signed.</param>
		/// <returns>An empty name because cannot infer.</returns>
		public sealed override Name inferSigningIdentity(Name dataName) {
			return new Name();
		}
	
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(NoVerifyPolicyManager).FullName);
	}
}
