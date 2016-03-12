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
	using System.spec;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.certificate;
	using net.named_data.jndn.util;
	
	public class MemoryPrivateKeyStorage : PrivateKeyStorage {
		public MemoryPrivateKeyStorage() {
			this.publicKeyStore_ = new Hashtable();
			this.privateKeyStore_ = new Hashtable();
		}
	
		/// <summary>
		/// Set the public key for the keyName.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <param name="keyType">The KeyType, such as KeyType.RSA.</param>
		/// <param name="publicKeyDer">The public key DER byte buffer.</param>
		/// <exception cref="System.Security.SecurityException">if can't decode the key DER.</exception>
		public void setPublicKeyForKeyName(Name keyName, KeyType keyType,
				ByteBuffer publicKeyDer) {
			ILOG.J2CsMapping.Collections.Collections.Put(publicKeyStore_,keyName.toUri(),new PublicKey(new Blob(
							publicKeyDer, true)));
		}
	
		/// <summary>
		/// Set the private key for the keyName.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <param name="keyType">The KeyType, such as KeyType.RSA.</param>
		/// <param name="privateKeyDer">The private key DER byte buffer.</param>
		/// <exception cref="System.Security.SecurityException">if can't decode the key DER.</exception>
		public void setPrivateKeyForKeyName(Name keyName, KeyType keyType,
				ByteBuffer privateKeyDer) {
			ILOG.J2CsMapping.Collections.Collections.Put(privateKeyStore_,keyName.toUri(),new MemoryPrivateKeyStorage.PrivateKey (keyType,
							privateKeyDer));
		}
	
		/// <summary>
		/// Set the public and private key for the keyName.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <param name="keyType">The KeyType, such as KeyType.RSA.</param>
		/// <param name="publicKeyDer">The public key DER byte buffer.</param>
		/// <param name="privateKeyDer">The private key DER byte buffer.</param>
		/// <exception cref="System.Security.SecurityException">if can't decode the key DER.</exception>
		public void setKeyPairForKeyName(Name keyName, KeyType keyType,
				ByteBuffer publicKeyDer, ByteBuffer privateKeyDer) {
			setPublicKeyForKeyName(keyName, keyType, publicKeyDer);
			setPrivateKeyForKeyName(keyName, keyType, privateKeyDer);
		}
	
		
		/// <param name="keyName">The key name.</param>
		/// <param name="publicKeyDer">The public key DER byte buffer.</param>
		/// <param name="privateKeyDer">The private key DER byte buffer.</param>
		/// <exception cref="System.Security.SecurityException">if can't decode the key DER.</exception>
		public void setKeyPairForKeyName(Name keyName,
				ByteBuffer publicKeyDer, ByteBuffer privateKeyDer) {
			setKeyPairForKeyName(keyName, net.named_data.jndn.security.KeyType.RSA, publicKeyDer, privateKeyDer);
		}
	
		/// <summary>
		/// Generate a pair of asymmetric keys.
		/// </summary>
		///
		/// <param name="keyName">The name of the key pair.</param>
		/// <param name="params">The parameters of the key.</param>
		/// <exception cref="System.Security.SecurityException"></exception>
		public override void generateKeyPair(Name keyName, KeyParams paras) {
			if (doesKeyExist(keyName, net.named_data.jndn.security.KeyClass.PUBLIC))
				throw new SecurityException("Public Key already exists");
			if (doesKeyExist(keyName, net.named_data.jndn.security.KeyClass.PRIVATE))
				throw new SecurityException("Private Key already exists");
	
			String keyAlgorithm;
			int keySize;
			if (paras.getKeyType() == net.named_data.jndn.security.KeyType.RSA) {
				keyAlgorithm = "RSA";
				keySize = ((RsaKeyParams) paras).getKeySize();
			} else if (paras.getKeyType() == net.named_data.jndn.security.KeyType.ECDSA) {
				keyAlgorithm = "EC";
				keySize = ((EcdsaKeyParams) paras).getKeySize();
			} else
				throw new SecurityException("Cannot generate a key pair of type "
						+ paras.getKeyType());
	
			KeyPairGenerator generator = null;
			try {
				generator = System.KeyPairGenerator.getInstance(keyAlgorithm);
			} catch (Exception e) {
				throw new SecurityException(
						"FilePrivateKeyStorage: Could not create the key generator: "
								+ e.Message);
			}
	
			generator.initialize(keySize);
			KeyPair pair = generator.generateKeyPair();
	
			setKeyPairForKeyName(keyName, paras.getKeyType(),
					ILOG.J2CsMapping.NIO.ByteBuffer.wrap(pair.getPublic().getEncoded()),
					ILOG.J2CsMapping.NIO.ByteBuffer.wrap(pair.getPrivate().getEncoded()));
		}
	
		/// <summary>
		/// Delete a pair of asymmetric keys. If the key doesn't exist, do nothing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key pair.</param>
		public override void deleteKeyPair(Name keyName) {
			String keyUri = keyName.toUri();
	
			ILOG.J2CsMapping.Collections.Collections.Remove(publicKeyStore_,keyUri);
			ILOG.J2CsMapping.Collections.Collections.Remove(privateKeyStore_,keyUri);
		}
	
		/// <summary>
		/// Get the public key
		/// </summary>
		///
		/// <param name="keyName">The name of public key.</param>
		/// <returns>The public key.</returns>
		/// <exception cref="System.Security.SecurityException"></exception>
		public override PublicKey getPublicKey(Name keyName) {
			PublicKey publicKey = (PublicKey) ILOG.J2CsMapping.Collections.Collections.Get(publicKeyStore_,keyName.toUri());
			if (publicKey == null)
				throw new SecurityException(
						"MemoryPrivateKeyStorage: Cannot find public key "
								+ keyName.toUri());
			return publicKey;
		}
	
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
		public override Blob sign(ByteBuffer data, Name keyName,
				DigestAlgorithm digestAlgorithm) {
			if (digestAlgorithm != net.named_data.jndn.security.DigestAlgorithm.SHA256)
				throw new SecurityException(
						"MemoryPrivateKeyStorage.sign: Unsupported digest algorithm");
	
			// Find the private key and sign.
			MemoryPrivateKeyStorage.PrivateKey  privateKey = (MemoryPrivateKeyStorage.PrivateKey ) ILOG.J2CsMapping.Collections.Collections.Get(privateKeyStore_,keyName
							.toUri());
			if (privateKey == null)
				throw new SecurityException(
						"MemoryPrivateKeyStorage: Cannot find private key "
								+ keyName.toUri());
	
			System.SecuritySignature signature = null;
			if (privateKey.getKeyType() == net.named_data.jndn.security.KeyType.RSA) {
				try {
					signature = System.SecuritySignature
							.getInstance("SHA256withRSA");
				} catch (Exception e) {
					// Don't expect this to happen.
					throw new SecurityException(
							"SHA256withRSA algorithm is not supported");
				}
			} else if (privateKey.getKeyType() == net.named_data.jndn.security.KeyType.ECDSA) {
				try {
					signature = System.SecuritySignature
							.getInstance("SHA256withECDSA");
				} catch (Exception e_0) {
					// Don't expect this to happen.
					throw new SecurityException(
							"SHA256withECDSA algorithm is not supported");
				}
			} else
				// We don't expect this to happen.
				throw new SecurityException("Unrecognized private key type");
	
			try {
				signature.initSign(privateKey.getPrivateKey());
			} catch (InvalidKeyException exception) {
				throw new SecurityException("InvalidKeyException: "
						+ exception.Message);
			}
			try {
				signature.update(data);
				return new Blob(signature.sign());
			} catch (SignatureException exception_1) {
				throw new SecurityException("SignatureException: "
						+ exception_1.Message);
			}
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
		public override Blob decrypt(Name keyName, ByteBuffer data, bool isSymmetric) {
			throw new NotSupportedException(
					"MemoryPrivateKeyStorage.decrypt is not implemented");
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
		public override Blob encrypt(Name keyName, ByteBuffer data, bool isSymmetric) {
			throw new NotSupportedException(
					"MemoryPrivateKeyStorage.encrypt is not implemented");
		}
	
		/// <summary>
		/// Generate a symmetric key.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="params">The parameters of the key.</param>
		/// <exception cref="System.Security.SecurityException"></exception>
		public override void generateKey(Name keyName, KeyParams paras) {
			throw new NotSupportedException(
					"MemoryPrivateKeyStorage.generateKey is not implemented");
		}
	
		/// <summary>
		/// Check if a particular key exists.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="keyClass"></param>
		/// <returns>True if the key exists, otherwise false.</returns>
		public override bool doesKeyExist(Name keyName, KeyClass keyClass) {
			if (keyClass == net.named_data.jndn.security.KeyClass.PUBLIC)
				return publicKeyStore_.Contains(keyName.toUri());
			else if (keyClass == net.named_data.jndn.security.KeyClass.PRIVATE)
				return privateKeyStore_.Contains(keyName.toUri());
			else
				// KeyClass.SYMMETRIC not implemented yet.
				return false;
		}
	
		/// <summary>
		/// PrivateKey is a simple class to hold a java.security.PrivateKey along with
		/// a KeyType.
		/// </summary>
		///
		internal class PrivateKey {
			public PrivateKey(KeyType keyType, ByteBuffer keyDer) {
				keyType_ = keyType;
	
				if (keyType == net.named_data.jndn.security.KeyType.RSA) {
					KeyFactory keyFactory = null;
					try {
						keyFactory = System.KeyFactory.getInstance("RSA");
					} catch (Exception exception) {
						// Don't expect this to happen.
						throw new SecurityException(
								"KeyFactory: RSA is not supported: "
										+ exception.Message);
					}
	
					try {
						privateKey_ = keyFactory
								.generatePrivate(new PKCS8EncodedKeySpec(new Blob(
										keyDer, false).getImmutableArray()));
					} catch (InvalidKeySpecException exception_0) {
						// Don't expect this to happen.
						throw new SecurityException(
								"KeyFactory: PKCS8EncodedKeySpec is not supported for RSA: "
										+ exception_0.Message);
					}
				} else if (keyType == net.named_data.jndn.security.KeyType.ECDSA) {
					KeyFactory keyFactory_1 = null;
					try {
						keyFactory_1 = System.KeyFactory.getInstance("EC");
					} catch (Exception exception_2) {
						// Don't expect this to happen.
						throw new SecurityException(
								"KeyFactory: EC is not supported: "
										+ exception_2.Message);
					}
	
					try {
						privateKey_ = keyFactory_1
								.generatePrivate(new PKCS8EncodedKeySpec(new Blob(
										keyDer, false).getImmutableArray()));
					} catch (InvalidKeySpecException exception_3) {
						// Don't expect this to happen.
						throw new SecurityException(
								"KeyFactory: PKCS8EncodedKeySpec is not supported for EC: "
										+ exception_3.Message);
					}
				} else
					throw new SecurityException(
							"PrivateKey constructor: Unrecognized keyType");
			}
	
			public KeyType getKeyType() {
				return keyType_;
			}
	
			public System.PrivateKey getPrivateKey() {
				return privateKey_;
			}
	
			private KeyType keyType_;
			private System.PrivateKey privateKey_;
		}
	
		// Use HashMap without generics so it works with older Java compilers.
		private readonly Hashtable publicKeyStore_;
		/// <summary>
		/// < The map key is the keyName.toUri().
		/// The value is security.certificate.PublicKey. 
		/// </summary>
		///
		private readonly Hashtable privateKeyStore_;
		/**< The map key is the keyName.toUri().
		   * The value is MemoryPrivateKeyStorage.PrivateKey. */
	}
}
