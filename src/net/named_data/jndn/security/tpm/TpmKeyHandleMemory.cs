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
namespace net.named_data.jndn.security.tpm {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// TpmKeyHandleMemory extends TpmKeyHandle to implement a TPM key handle that
	/// keeps the private key in memory.
	/// </summary>
	///
	public class TpmKeyHandleMemory : TpmKeyHandle {
		/// <summary>
		/// Create a TpmKeyHandleMemory to use the given in-memory key.
		/// </summary>
		///
		/// <param name="key">The in-memory key.</param>
		public TpmKeyHandleMemory(TpmPrivateKey key) {
			if (key == null)
				throw new AssertionError("The key is null");
	
			key_ = key;
		}
	
		protected internal override Blob doSign(DigestAlgorithm digestAlgorithm, ByteBuffer data) {
			if (digestAlgorithm == net.named_data.jndn.security.DigestAlgorithm.SHA256) {
				try {
					return key_.sign(data, digestAlgorithm);
				} catch (TpmPrivateKey.Error ex) {
					throw new TpmBackEnd.Error("Error in TpmPrivateKey.sign: " + ex);
				}
			} else
				return new Blob();
		}
	
		protected internal override Blob doDecrypt(ByteBuffer cipherText) {
			try {
				return key_.decrypt(cipherText);
			} catch (TpmPrivateKey.Error ex) {
				throw new TpmBackEnd.Error("Error in TpmPrivateKey.decrypt: " + ex);
			}
		}
	
		protected internal override Blob doDerivePublicKey() {
			try {
				return key_.derivePublicKey();
			} catch (TpmPrivateKey.Error ex) {
				throw new TpmBackEnd.Error(
						"Error in TpmPrivateKey.derivePublicKey: " + ex);
			}
		}
	
		private TpmPrivateKey key_;
	}
}
