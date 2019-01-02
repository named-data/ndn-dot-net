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
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// The TPM (Trusted Platform Module) stores the private portion of a user's
	/// cryptography keys. The format and location of stored information is indicated
	/// by the TPM locator. The TPM is designed to work with a PIB (Public
	/// Information Base) which stores public keys and related information such as
	/// certificates.
	/// The TPM also provides functionalities of cryptographic transformation, such
	/// as signing and decryption.
	/// A TPM consists of a unified front-end interface and a backend implementation.
	/// The front-end caches the handles of private keys which are provided by the
	/// backend implementation.
	/// Note: A Tpm instance is created and managed only by the KeyChain. It is
	/// returned by the KeyChain getTpm() method, through which it is possible to
	/// check for the existence of private keys, get public keys for the private
	/// keys, sign, and decrypt the supplied buffers using managed private keys.
	/// </summary>
	///
	public class Tpm {
		/// <summary>
		/// A Tpm.Error extends Exception and represents a semantic error in TPM
		/// processing.
		/// Note that even though this is called "Error" to be consistent with the
		/// other libraries, it extends the Java Exception class, not Error.
		/// </summary>
		///
		[Serializable]
		public class Error : Exception {
			public Error(String message) : base(message) {
			}
		}
	
		public String getTpmLocator() {
			return scheme_ + ":" + location_;
		}
	
		/// <summary>
		/// Check if the key with name keyName exists in the TPM.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>True if the key exists.</returns>
		public bool hasKey(Name keyName) {
			return backEnd_.hasKey(keyName);
		}
	
		/// <summary>
		/// Get the public portion of an asymmetric key pair with name keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The encoded public key, or an isNull Blob if the key does not exist.</returns>
		public Blob getPublicKey(Name keyName) {
			TpmKeyHandle key = findKey(keyName);
	
			if (key == null)
				return new Blob();
			else
				return key.derivePublicKey();
		}
	
		/// <summary>
		/// Compute a digital signature from the byte buffer using the key with name
		/// keyName.
		/// </summary>
		///
		/// <param name="data">The input byte buffer.</param>
		/// <param name="keyName">The name of the key.</param>
		/// <param name="digestAlgorithm">The digest algorithm for the signature.</param>
		/// <returns>The signature Blob, or an isNull Blob if the key does not exist, or
		/// for an unrecognized digestAlgorithm.</returns>
		public Blob sign(ByteBuffer data, Name keyName,
				DigestAlgorithm digestAlgorithm) {
			TpmKeyHandle key = findKey(keyName);
	
			if (key == null)
				return new Blob();
			else
				return key.sign(digestAlgorithm, data);
		}
	
		/// <summary>
		/// Return the plain text which is decrypted from cipherText using the key
		/// with name keyName.
		/// </summary>
		///
		/// <param name="cipherText">The cipher text byte buffer.</param>
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The decrypted data, or an isNull Blob if the key does not exist.</returns>
		public Blob decrypt(ByteBuffer cipherText, Name keyName) {
			TpmKeyHandle key = findKey(keyName);
	
			if (key == null)
				return new Blob();
			else
				return key.decrypt(cipherText);
		}
	
		// TPM Management
	
		/// <summary>
		/// Check if the TPM is in terminal mode.
		/// </summary>
		///
		/// <returns>True if in terminal mode.</returns>
		public bool isTerminalMode() {
			return backEnd_.isTerminalMode();
		}
	
		/// <summary>
		/// Set the terminal mode of the TPM. In terminal mode, the TPM will not ask
		/// for a password from the GUI.
		/// </summary>
		///
		/// <param name="isTerminal">True to enable terminal mode.</param>
		public void setTerminalMode(bool isTerminal) {
			backEnd_.setTerminalMode(isTerminal);
		}
	
		/// <summary>
		/// Check if the TPM is locked.
		/// </summary>
		///
		/// <returns>True if the TPM is locked, otherwise false.</returns>
		public bool isTpmLocked() {
			return backEnd_.isTpmLocked();
		}
	
		/// <summary>
		/// Unlock the TPM. If !isTerminalMode(), prompt for a password from the GUI.
		/// </summary>
		///
		/// <param name="password">The password to unlock TPM.</param>
		/// <returns>True if the TPM was unlocked.</returns>
		public bool unlockTpm(ByteBuffer password) {
			return backEnd_.unlockTpm(password);
		}
	
		/*
		 * Create a new TPM instance with the specified location. This constructor
		 * should only be called by KeyChain.
		 * @param scheme The scheme for the TPM.
		 * @param location The location for the TPM.
		 * @param backEnd The TPM back-end implementation.
		 */
		public Tpm(String scheme, String location, TpmBackEnd backEnd) {
			this.keys_ = new Hashtable<Name, TpmKeyHandle>();
			scheme_ = scheme;
			location_ = location;
			backEnd_ = backEnd;
		}
	
		/// <summary>
		/// Get the TpmBackEnd.
		/// This should only be called by KeyChain.
		/// </summary>
		///
		public TpmBackEnd getBackEnd_() {
			return backEnd_;
		}
	
		/// <summary>
		/// Create a key for the identityName according to params. The created key is
		/// named /{identityName}/[keyId]/KEY .
		/// This should only be called by KeyChain.
		/// </summary>
		///
		/// <param name="identityName">The name if the identity.</param>
		/// <param name="params">The KeyParams for creating the key.</param>
		/// <returns>The name of the created key.</returns>
		/// <exception cref="Tpm.Error">if params is invalid or the key type is unsupported.</exception>
		/// <exception cref="TpmBackEnd.Error">if the key already exists or cannot be created.</exception>
		public Name createKey_(Name identityName, KeyParams paras) {
			if (paras.getKeyType() == net.named_data.jndn.security.KeyType.RSA
					|| paras.getKeyType() == net.named_data.jndn.security.KeyType.EC) {
				TpmKeyHandle keyHandle = backEnd_.createKey(identityName, paras);
				Name keyName = keyHandle.getKeyName();
				ILOG.J2CsMapping.Collections.Collections.Put(keys_,keyName,keyHandle);
				return keyName;
			} else
				throw new Tpm.Error ("createKey: Unsupported key type");
		}
	
		/// <summary>
		/// Delete the key with name keyName. If the key doesn't exist, do nothing.
		/// Note: Continuing to use existing Key handles on a deleted key results in
		/// undefined behavior.
		/// This should only be called by KeyChain.
		/// </summary>
		///
		/// <exception cref="TpmBackEnd.Error">if the deletion fails.</exception>
		public void deleteKey_(Name keyName) {
			ILOG.J2CsMapping.Collections.Collections.Remove(keys_,keyName);
			backEnd_.deleteKey(keyName);
		}
	
		/// <summary>
		/// Get the encoded private key with name keyName in PKCS #8 format, possibly
		/// encrypted.
		/// This should only be called by KeyChain.
		/// </summary>
		///
		/// <param name="keyName">The name of the key in the TPM.</param>
		/// <param name="password">it to return a PKCS #8 EncryptedPrivateKeyInfo. If the password is null, return an unencrypted PKCS #8 PrivateKeyInfo.</param>
		/// <returns>The private key encoded in PKCS #8 format, or an isNull Blob if
		/// the key does not exist.</returns>
		/// <exception cref="TpmBackEnd.Error">if the key does not exist or if the key cannot beexported, e.g., insufficient privileges.</exception>
		public Blob exportPrivateKey_(Name keyName, ByteBuffer password) {
			return backEnd_.exportKey(keyName, password);
		}
	
		/// <summary>
		/// Import an encoded private key with name keyName in PKCS #8 format, possibly
		/// password-encrypted.
		/// This should only be called by KeyChain.
		/// </summary>
		///
		/// <param name="keyName">The name of the key to use in the TPM.</param>
		/// <param name="pkcs8">unencrypted PKCS #8 PrivateKeyInfo.</param>
		/// <param name="password">it to decrypt the PKCS #8 EncryptedPrivateKeyInfo. If the password is null, import an unencrypted PKCS #8 PrivateKeyInfo.</param>
		/// <exception cref="TpmBackEnd.Error">if the key cannot be imported.</exception>
		public void importPrivateKey_(Name keyName, ByteBuffer pkcs8,
				ByteBuffer password) {
			backEnd_.importKey(keyName, pkcs8, password);
		}
	
		/// <summary>
		/// Get the TpmKeyHandle with name keyName, using backEnd_.getKeyHandle if it
		/// is not already cached in keys_.
		/// </summary>
		///
		/// <param name="keyName">The name of the key, which is copied.</param>
		/// <returns>The key handle in the keys_ cache, or null if no key exists with
		/// name keyName.</returns>
		private TpmKeyHandle findKey(Name keyName) {
			TpmKeyHandle handle = ILOG.J2CsMapping.Collections.Collections.Get(keys_,keyName);
	
			if (handle != null)
				return handle;
	
			handle = backEnd_.getKeyHandle(keyName);
	
			if (handle != null) {
				// Copy the Name.
				ILOG.J2CsMapping.Collections.Collections.Put(keys_,new Name(keyName),handle);
				return handle;
			}
	
			return null;
		}
	
		private readonly String scheme_;
		private readonly String location_;
	
		internal Hashtable<Name, TpmKeyHandle> keys_;
	
		private readonly TpmBackEnd backEnd_;
	}
}
