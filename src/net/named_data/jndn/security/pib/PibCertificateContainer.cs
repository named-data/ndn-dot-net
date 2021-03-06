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
namespace net.named_data.jndn.security.pib {
	
	using ILOG.J2CsMapping.Collections;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A PibCertificateContainer is used to search/enumerate the certificates of a
	/// key. (A PibCertificateContainer object can only be created by PibKey.)
	/// </summary>
	///
	public class PibCertificateContainer {
		/// <summary>
		/// Get the number of certificates in the container.
		/// </summary>
		///
		/// <returns>The number of certificates.</returns>
		public int size() {
			return certificateNames_.Count;
		}
	
		/// <summary>
		/// Add certificate into the container. If the certificate already exists,
		/// this replaces it.
		/// </summary>
		///
		/// <param name="certificate">The certificate to add. This copies the object.</param>
		/// <exception cref="System.ArgumentException">if the name of the certificate does notmatch the key name.</exception>
		public void add(CertificateV2 certificate) {
			if (!keyName_.equals(certificate.getKeyName()))
				throw new ArgumentException("The certificate name `"
						+ certificate.getKeyName().toUri()
						+ "` does not match the key name");
	
			Name certificateName = new Name(certificate.getName());
			ILOG.J2CsMapping.Collections.Collections.Add(certificateNames_,certificateName);
			// Copy the certificate.
			ILOG.J2CsMapping.Collections.Collections.Put(certificates_,certificateName,new CertificateV2(certificate));
			pibImpl_.addCertificate(certificate);
		}
	
		/// <summary>
		/// Remove the certificate with name certificateName from the container. If the
		/// certificate does not exist, do nothing.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <exception cref="System.ArgumentException">if certificateName does not match the keyname.</exception>
		public void remove(Name certificateName) {
			if (!net.named_data.jndn.security.v2.CertificateV2.isValidName(certificateName)
					|| !net.named_data.jndn.security.v2.CertificateV2.extractKeyNameFromCertName(certificateName)
							.equals(keyName_))
				throw new ArgumentException("Certificate name `"
						+ certificateName.toUri()
						+ "` is invalid or does not match key name");
	
			ILOG.J2CsMapping.Collections.Collections.Remove(certificateNames_,certificateName);
			ILOG.J2CsMapping.Collections.Collections.Remove(certificates_,certificateName);
			pibImpl_.removeCertificate(certificateName);
		}
	
		/// <summary>
		/// Get the certificate with certificateName from the container.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <returns>A copy of the certificate.</returns>
		/// <exception cref="System.ArgumentException">if certificateName does not match the keyname</exception>
		/// <exception cref="Pib.Error">if the certificate does not exist.</exception>
		public CertificateV2 get(Name certificateName) {
			CertificateV2 cachedCertificate = ILOG.J2CsMapping.Collections.Collections.Get(certificates_,certificateName);
	
			if (cachedCertificate != null) {
				try {
					// Make a copy.
					// TODO: Copy is expensive. Can we just tell the caller not to modify it?
					return new CertificateV2(cachedCertificate);
				} catch (CertificateV2.Error ex) {
					// We don't expect this for the copy constructor.
					throw new Pib.Error("Error copying certificate: " + ex);
				}
			}
	
			// Get from the PIB and cache.
			if (!net.named_data.jndn.security.v2.CertificateV2.isValidName(certificateName)
					|| !net.named_data.jndn.security.v2.CertificateV2.extractKeyNameFromCertName(certificateName)
							.equals(keyName_))
				throw new ArgumentException("Certificate name `"
						+ certificateName.toUri()
						+ "` is invalid or does not match key name");
	
			CertificateV2 certificate = pibImpl_.getCertificate(certificateName);
			// Copy the certificate Name.
			ILOG.J2CsMapping.Collections.Collections.Put(certificates_,new Name(certificateName),certificate);
			try {
				// Make a copy.
				// TODO: Copy is expensive. Can we just tell the caller not to modify it?
				return new CertificateV2(certificate);
			} catch (CertificateV2.Error ex_0) {
				// We don't expect this for the copy constructor.
				throw new Pib.Error("Error copying certificate: " + ex_0);
			}
		}
	
		/// <summary>
		/// Check if the container is consistent with the backend storage.
		/// </summary>
		///
		/// <returns>True if the container is consistent, false otherwise.</returns>
		/// @note This method is heavy-weight and should be used in a debugging mode
		/// only.
		public bool isConsistent() {
			return certificateNames_
					.equals(pibImpl_.getCertificatesOfKey(keyName_));
		}
	
		/// <summary>
		/// Create a PibCertificateContainer for a key with keyName. This constructor
		/// should only be called by PibKeyImpl.
		/// </summary>
		///
		/// <param name="keyName">The name of the key, which is copied.</param>
		/// <param name="pibImpl">The PIB backend implementation.</param>
		public PibCertificateContainer(Name keyName, PibImpl pibImpl) {
			this.certificates_ = new Hashtable<Name, CertificateV2>();
			keyName_ = new Name(keyName);
			pibImpl_ = pibImpl;
	
			if (pibImpl == null)
				throw new AssertionError("The pibImpl is null");
	
			certificateNames_ = pibImpl_.getCertificatesOfKey(keyName);
		}
	
		/// <summary>
		/// Get the certificates_ map, which should only be used for testing.
		/// </summary>
		///
		public Hashtable<Name, CertificateV2> getCertificates_() {
			return certificates_;
		}
	
		private readonly Name keyName_;
		private HashedSet<Name> certificateNames_;
		// Cache of loaded certificates.
		private readonly Hashtable<Name, CertificateV2> certificates_;
	
		private readonly PibImpl pibImpl_;
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
