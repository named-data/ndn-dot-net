// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2016 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security.identity {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.certificate;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// MemoryIdentityStorage extends IdentityStorage and implements its methods to
	/// store identity, public key and certificate objects in memory. The application
	/// must get the objects through its own means and add the objects to the
	/// MemoryIdentityStorage object. To use permanent file-based storage, see
	/// BasicIdentityStorage.
	/// </summary>
	///
	public class MemoryIdentityStorage : IdentityStorage {
		public MemoryIdentityStorage() {
			this.identityStore_ = new Hashtable();
			this.defaultIdentity_ = "";
			this.keyStore_ = new Hashtable();
			this.certificateStore_ = new Hashtable();
		}
	
		/// <summary>
		/// Check if the specified identity already exists.
		/// </summary>
		///
		/// <param name="identityName">The identity name.</param>
		/// <returns>True if the identity exists, otherwise false.</returns>
		public override bool doesIdentityExist(Name identityName) {
			return identityStore_.Contains(identityName.toUri());
		}
	
		/// <summary>
		/// Add a new identity. Do nothing if the identity already exists.
		/// </summary>
		///
		/// <param name="identityName">The identity name to be added.</param>
		public override void addIdentity(Name identityName) {
			String identityUri = identityName.toUri();
			if (identityStore_.Contains(identityUri))
				return;
	
			ILOG.J2CsMapping.Collections.Collections.Put(identityStore_,identityUri,new MemoryIdentityStorage.IdentityRecord ());
		}
	
		/// <summary>
		/// Revoke the identity.
		/// </summary>
		///
		/// <returns>True if the identity was revoked, false if not.</returns>
		public override bool revokeIdentity() {
			throw new NotSupportedException(
					"MemoryIdentityStorage.revokeIdentity not implemented");
		}
	
		/// <summary>
		/// Check if the specified key already exists.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>true if the key exists, otherwise false.</returns>
		public override bool doesKeyExist(Name keyName) {
			return keyStore_.Contains(keyName.toUri());
		}
	
		/// <summary>
		/// Add a public key to the identity storage. Also call addIdentity to ensure
		/// that the identityName for the key exists.
		/// </summary>
		///
		/// <param name="keyName">The name of the public key to be added.</param>
		/// <param name="keyType">Type of the public key to be added.</param>
		/// <param name="publicKeyDer">A blob of the public key DER to be added.</param>
		/// <exception cref="System.Security.SecurityException">if a key with the keyName already exists.</exception>
		public override void addKey(Name keyName, KeyType keyType, Blob publicKeyDer) {
			Name identityName = keyName.getSubName(0, keyName.size() - 1);
	
			addIdentity(identityName);
	
			if (doesKeyExist(keyName))
				throw new SecurityException(
						"a key with the same name already exists!");
	
			ILOG.J2CsMapping.Collections.Collections.Put(keyStore_,keyName.toUri(),new MemoryIdentityStorage.KeyRecord (keyType, publicKeyDer));
		}
	
		/// <summary>
		/// Get the public key DER blob from the identity storage.
		/// </summary>
		///
		/// <param name="keyName">The name of the requested public key.</param>
		/// <returns>The DER Blob.  If not found, return a Blob with a null pointer.</returns>
		public override Blob getKey(Name keyName) {
			MemoryIdentityStorage.KeyRecord  keyRecord = (MemoryIdentityStorage.KeyRecord ) ILOG.J2CsMapping.Collections.Collections.Get(keyStore_,keyName.toUri());
			if (keyRecord == null)
				// Not found.  Silently return a null Blob.
				return new Blob();
	
			return keyRecord.getKeyDer();
		}
	
		/// <summary>
		/// Activate a key.  If a key is marked as inactive, its private part will not
		/// be used in packet signing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		public override void activateKey(Name keyName) {
			throw new NotSupportedException(
					"MemoryIdentityStorage.activateKey not implemented");
		}
	
		/// <summary>
		/// Deactivate a key. If a key is marked as inactive, its private part will not
		/// be used in packet signing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		public override void deactivateKey(Name keyName) {
			throw new NotSupportedException(
					"MemoryIdentityStorage.deactivateKey not implemented");
		}
	
		/// <summary>
		/// Check if the specified certificate already exists.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <returns>True if the certificate exists, otherwise false.</returns>
		public override bool doesCertificateExist(Name certificateName) {
			return certificateStore_.Contains(certificateName.toUri());
		}
	
		/// <summary>
		/// Add a certificate to the identity storage.
		/// </summary>
		///
		/// <param name="certificate"></param>
		/// <exception cref="System.Security.SecurityException">if the certificate is already installed.</exception>
		public override void addCertificate(IdentityCertificate certificate) {
			Name certificateName = certificate.getName();
			Name keyName = certificate.getPublicKeyName();
	
			if (!doesKeyExist(keyName))
				throw new SecurityException(
						"No corresponding Key record for certificate! "
								+ keyName.toUri() + " " + certificateName.toUri());
	
			// Check if certificate already exists.
			if (doesCertificateExist(certificateName))
				throw new SecurityException(
						"Certificate has already been installed!");
	
			// Check if the public key of certificate is the same as the key record.
			Blob keyBlob = getKey(keyName);
			if (keyBlob.isNull()
					|| !keyBlob.equals(certificate.getPublicKeyInfo().getKeyDer()))
				throw new SecurityException(
						"Certificate does not match the public key!");
	
			// Insert the certificate.
			ILOG.J2CsMapping.Collections.Collections.Put(certificateStore_,certificateName.toUri(),certificate.wireEncode());
		}
	
		/// <summary>
		/// Get a certificate from the identity storage.
		/// </summary>
		///
		/// <param name="certificateName">The name of the requested certificate.</param>
		/// <param name="allowAny"></param>
		/// <returns>The requested certificate. If not found, return null.</returns>
		public override IdentityCertificate getCertificate(Name certificateName,
				bool allowAny) {
			if (!allowAny)
				throw new NotSupportedException(
						"MemoryIdentityStorage.getCertificate for !allowAny is not implemented");
	
			Blob certificateDer = (Blob) ILOG.J2CsMapping.Collections.Collections.Get(certificateStore_,certificateName
							.toUri());
			if (certificateDer == null)
				// Not found.  Silently return null.
				return new IdentityCertificate();
	
			IdentityCertificate certificate = new IdentityCertificate();
			try {
				certificate.wireDecode(certificateDer);
			} catch (EncodingException ex) {
				// Don't expect this to happen. Silently return null.
				return new IdentityCertificate();
			}
			return certificate;
		}
	
		/*****************************************
		 *           Get/Set Default             *
		 *****************************************/
	
		/// <summary>
		/// Get the default identity.
		/// </summary>
		///
		/// <returns>The name of default identity.</returns>
		/// <exception cref="System.Security.SecurityException">if the default identity is not set.</exception>
		public override Name getDefaultIdentity() {
			if (defaultIdentity_.Length == 0)
				throw new SecurityException(
						"MemoryIdentityStorage.getDefaultIdentity: The default identity is not defined");
	
			return new Name(defaultIdentity_);
		}
	
		/// <summary>
		/// Get the default key name for the specified identity.
		/// </summary>
		///
		/// <param name="identityName">The identity name.</param>
		/// <returns>The default key name.</returns>
		/// <exception cref="System.Security.SecurityException">if the default key name for the identity is not set.</exception>
		public override Name getDefaultKeyNameForIdentity(Name identityName) {
			String identity = identityName.toUri();
			if (identityStore_.Contains(identity)) {
				if (((MemoryIdentityStorage.IdentityRecord ) ILOG.J2CsMapping.Collections.Collections.Get(identityStore_,identity)).hasDefaultKey()) {
					return ((MemoryIdentityStorage.IdentityRecord ) ILOG.J2CsMapping.Collections.Collections.Get(identityStore_,identity))
							.getDefaultKey();
				} else {
					throw new SecurityException("No default key set.");
				}
			} else {
				throw new SecurityException("Identity not found.");
			}
		}
	
		/// <summary>
		/// Get the default certificate name for the specified key.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <returns>The default certificate name.</returns>
		/// <exception cref="System.Security.SecurityException">if the default certificate name for the key nameis not set.</exception>
		public override Name getDefaultCertificateNameForKey(Name keyName) {
			String key = keyName.toUri();
			if (keyStore_.Contains(key)) {
				if (((MemoryIdentityStorage.KeyRecord ) ILOG.J2CsMapping.Collections.Collections.Get(keyStore_,key)).hasDefaultCertificate()) {
					return ((MemoryIdentityStorage.KeyRecord ) ILOG.J2CsMapping.Collections.Collections.Get(keyStore_,key)).getDefaultCertificate();
				} else {
					throw new SecurityException("No default certificate set.");
				}
			} else {
				throw new SecurityException("Key not found.");
			}
		}
	
		/// <summary>
		/// Append all the key names of a particular identity to the nameList.
		/// </summary>
		///
		/// <param name="identityName">The identity name to search for.</param>
		/// <param name="nameList">Append result names to nameList.</param>
		/// <param name="isDefault"></param>
		public override void getAllKeyNamesOfIdentity(Name identityName, ArrayList nameList,
				bool isDefault) {
			throw new NotSupportedException(
					"MemoryIdentityStorage.getAllKeyNamesOfIdentity not implemented");
		}
	
		/// <summary>
		/// Set the default identity.  If the identityName does not exist, then clear
		/// the default identity so that getDefaultIdentity() throws an exception.
		/// </summary>
		///
		/// <param name="identityName">The default identity name.</param>
		public override void setDefaultIdentity(Name identityName) {
			String identityUri = identityName.toUri();
			if (identityStore_.Contains(identityUri))
				defaultIdentity_ = identityUri;
			else
				// The identity doesn't exist, so clear the default.
				defaultIdentity_ = "";
		}
	
		/// <summary>
		/// Set a key as the default key of an identity. The identity name is inferred
		/// from keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="identityNameCheck"></param>
		public override void setDefaultKeyNameForIdentity(Name keyName,
				Name identityNameCheck) {
			Name identityName = keyName.getPrefix(-1);
	
			if (identityNameCheck.size() > 0
					&& !identityNameCheck.equals(identityName))
				throw new SecurityException(
						"The specified identity name does not match the key name");
	
			String identity = identityName.toUri();
			if (identityStore_.Contains(identity)) {
				((MemoryIdentityStorage.IdentityRecord ) ILOG.J2CsMapping.Collections.Collections.Get(identityStore_,identity))
						.setDefaultKey(new Name(keyName));
			}
		}
	
		/// <summary>
		/// Set the default key name for the specified identity.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <param name="certificateName">The certificate name.</param>
		public override void setDefaultCertificateNameForKey(Name keyName,
				Name certificateName) {
			String key = keyName.toUri();
			if (keyStore_.Contains(key)) {
				((MemoryIdentityStorage.KeyRecord ) ILOG.J2CsMapping.Collections.Collections.Get(keyStore_,key)).setDefaultCertificate(new Name(
						certificateName));
			}
		}
	
		/*****************************************
		 *            Delete Methods             *
		 *****************************************/
	
		/// <summary>
		/// Delete a certificate.
		/// </summary>
		///
		/// <param name="certificateName">The certificate name.</param>
		public override void deleteCertificateInfo(Name certificateName) {
			throw new NotSupportedException(
					"MemoryIdentityStorage.deleteCertificateInfo is not implemented");
		}
	
		/// <summary>
		/// Delete a public key and related certificates.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		public override void deletePublicKeyInfo(Name keyName) {
			throw new NotSupportedException(
					"MemoryIdentityStorage.deletePublicKeyInfo is not implemented");
		}
	
		/// <summary>
		/// Delete an identity and related public keys and certificates.
		/// </summary>
		///
		/// <param name="identity">The identity name.</param>
		public override void deleteIdentityInfo(Name identity) {
			throw new NotSupportedException(
					"MemoryIdentityStorage.deleteIdentityInfo is not implemented");
		}
	
		private class IdentityRecord {
	
			internal void setDefaultKey(Name key) {
				defaultKey_ = key;
			}
	
			internal bool hasDefaultKey() {
				return defaultKey_ != null;
			}
	
			internal Name getDefaultKey() {
				return defaultKey_;
			}
	
			private Name defaultKey_;
		} 
	
		private class KeyRecord {
			public KeyRecord(KeyType keyType, Blob keyDer) {
				keyType_ = keyType;
				keyDer_ = keyDer;
			}
	
			internal KeyType getKeyType() {
				return keyType_;
			}
	
			internal Blob getKeyDer() {
				return keyDer_;
			}
	
			internal void setDefaultCertificate(Name certificate) {
				defaultCertificate_ = certificate;
			}
	
			internal bool hasDefaultCertificate() {
				return defaultCertificate_ != null;
			}
	
			internal Name getDefaultCertificate() {
				return defaultCertificate_;
			}
	
			private KeyType keyType_;
			private Blob keyDer_;
			private Name defaultCertificate_;
		} 
	
		// Use HashMap without generics so it works with older Java compilers.
		private readonly Hashtable identityStore_;
		/// <summary>
		/// < The map key is the identityName.toUri(). The value is an IdentityRecord. 
		/// </summary>
		///
		private String defaultIdentity_;
		/// <summary>
		/// < The default identity in identityStore_, or "" if not defined. 
		/// </summary>
		///
		private readonly Hashtable keyStore_;
		/// <summary>
		/// < The map key is the keyName.toUri(). The value is a KeyRecord. 
		/// </summary>
		///
		private readonly Hashtable certificateStore_;
		/**< The map key is the certificateName.toUri(). The value is the certificate Blob. */
	}
}
