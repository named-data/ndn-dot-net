// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2016 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security.identity {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.certificate;
	using net.named_data.jndn.util;
	
	public abstract class PrivateKeyStorage {
		/// <summary>
		/// Generate a pair of asymmetric keys.
		/// </summary>
		///
		/// <param name="keyName">The name of the key pair.</param>
		/// <param name="params">The parameters of the key.</param>
		/// <exception cref="System.Security.SecurityException"></exception>
		public abstract void generateKeyPair(Name keyName, KeyParams paras);
	
		/// <summary>
		/// Delete a pair of asymmetric keys. If the key doesn't exist, do nothing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key pair.</param>
		public abstract void deleteKeyPair(Name keyName);
	
		/// <summary>
		/// Get the public key
		/// </summary>
		///
		/// <param name="keyName">The name of public key.</param>
		/// <returns>The public key.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public abstract PublicKey getPublicKey(Name keyName);
	
		/// <summary>
		/// Fetch the private key for keyName and sign the data, returning a signature
		/// Blob.
		/// </summary>
		///
		/// <param name="data">Pointer the input byte buffer to sign.</param>
		/// <param name="keyName">The name of the signing key.</param>
		/// <param name="digestAlgorithm">the digest algorithm.</param>
		/// <returns>The signature Blob.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public abstract Blob sign(ByteBuffer data, Name keyName,
				DigestAlgorithm digestAlgorithm);
	
		/// <summary>
		/// Fetch the private key for keyName and sign the data using
		/// DigestAlgorithm.SHA256, returning a signature Blob.
		/// </summary>
		///
		/// <param name="data">Pointer the input byte buffer to sign.</param>
		/// <param name="keyName">The name of the signing key.</param>
		/// <returns>The signature Blob.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public Blob sign(ByteBuffer data, Name keyName) {
			return sign(data, keyName, net.named_data.jndn.security.DigestAlgorithm.SHA256);
		}
	
		/// <summary>
		/// Decrypt data.
		/// </summary>
		///
		/// <param name="keyName">The name of the decrypting key.</param>
		/// <param name="data"></param>
		/// <param name="isSymmetric"></param>
		/// <returns>The decrypted data.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public abstract Blob decrypt(Name keyName, ByteBuffer data,
				bool isSymmetric);
	
		/// <summary>
		/// Decrypt data using asymmetric encryption.
		/// </summary>
		///
		/// <param name="keyName">The name of the decrypting key.</param>
		/// <param name="data"></param>
		/// <returns>The decrypted data.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public Blob decrypt(Name keyName, ByteBuffer data) {
			return decrypt(keyName, data, false);
		}
	
		/// <summary>
		/// Encrypt data.
		/// </summary>
		///
		/// <param name="keyName">The name of the encrypting key.</param>
		/// <param name="data"></param>
		/// <param name="isSymmetric"></param>
		/// <returns>The encrypted data.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public abstract Blob encrypt(Name keyName, ByteBuffer data,
				bool isSymmetric);
	
		/// <summary>
		/// Encrypt data using asymmetric encryption.
		/// </summary>
		///
		/// <param name="keyName">The name of the encrypting key.</param>
		/// <param name="data"></param>
		/// <returns>The encrypted data.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public Blob encrypt(Name keyName, ByteBuffer data) {
			return encrypt(keyName, data, false);
		}
	
		/// <summary>
		/// Generate a symmetric key.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="params">The parameters of the key.</param>
		/// <exception cref="System.Security.SecurityException"></exception>
		public abstract void generateKey(Name keyName, KeyParams paras);
	
		/// <summary>
		/// Check if a particular key exists.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="keyClass"></param>
		/// <returns>True if the key exists, otherwise false.</returns>
		public abstract bool doesKeyExist(Name keyName, KeyClass keyClass);
	}
}
