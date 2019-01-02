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
namespace net.named_data.jndn.tests.integration_tests {
	
	using ILOG.J2CsMapping.Collections;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.tpm;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	public class IdentityManagementFixture {
		public IdentityManagementFixture() {
			this.identityNames_ = new HashedSet<Name>();
					this.certificateFiles_ = new HashedSet<String>();
			keyChain_ = new KeyChain("pib-memory:", "tpm-memory:");
		}
	
		public bool saveCertificateToFile(Data data, String filePath) {
			ILOG.J2CsMapping.Collections.Collections.Add(certificateFiles_,filePath);
	
			try {
				Blob encoding = data.wireEncode();
				String encodedCertificate = net.named_data.jndn.util.Common.base64Encode(
						encoding.getImmutableArray(), true);
	
				var writer = (new StreamWriter(filePath));
				// Use "try/finally instead of "try-with-resources" or "using"
				// which are not supported before Java 7.
				try {
					writer.Write(encodedCertificate,0,encodedCertificate.Substring(0,encodedCertificate.Length));
					writer.flush();
				} finally {
					writer.close();
				}
	
				return true;
			} catch (Exception ex) {
				return false;
			}
		}
	
		/// <summary>
		/// Add an identity for the identityName.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <param name="params"></param>
		/// <returns>The created PibIdentity instance.</returns>
		public PibIdentity addIdentity(Name identityName, KeyParams paras) {
			PibIdentity identity = keyChain_.createIdentityV2(identityName, paras);
			ILOG.J2CsMapping.Collections.Collections.Add(identityNames_,identityName);
			return identity;
		}
	
		/// <summary>
		/// Add an identity for the identityName.
		/// Use KeyChain.getDefaultKeyParams().
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <returns>The created PibIdentity instance.</returns>
		public PibIdentity addIdentity(Name identityName) {
			return addIdentity(identityName, net.named_data.jndn.security.KeyChain.getDefaultKeyParams());
		}
	
		/// <summary>
		/// Save the identity's certificate to a file.
		/// </summary>
		///
		/// <param name="identity">The PibIdentity.</param>
		/// <param name="filePath">The file path, which should be writable.</param>
		/// <returns>True if successful.</returns>
		public bool saveCertificate(PibIdentity identity, String filePath) {
			try {
				CertificateV2 certificate = identity.getDefaultKey()
						.getDefaultCertificate();
				return saveCertificateToFile(certificate, filePath);
			} catch (Pib.Error ex) {
				return false;
			}
		}
	
		/// <summary>
		/// Issue a certificate for subIdentityName signed by issuer. If the identity
		/// does not exist, it is created. A new key is generated as the default key
		/// for the identity. A default certificate for the key is signed by the
		/// issuer using its default certificate.
		/// </summary>
		///
		/// <param name="subIdentityName">The name to issue the certificate for.</param>
		/// <param name="issuer">The identity of the signer.</param>
		/// <param name="params"></param>
		/// <returns>The sub identity.</returns>
		internal PibIdentity addSubCertificate(Name subIdentityName, PibIdentity issuer,
				KeyParams paras) {
			PibIdentity subIdentity = addIdentity(subIdentityName, paras);
	
			CertificateV2 request = subIdentity.getDefaultKey()
					.getDefaultCertificate();
	
			request.setName(request.getKeyName().append("parent").appendVersion(1));
	
			SigningInfo certificateParams = new SigningInfo(issuer);
			// Validity period of 20 years.
			double now = net.named_data.jndn.util.Common.getNowMilliseconds();
			certificateParams.setValidityPeriod(new ValidityPeriod(now, now + 20
					* 365 * 24 * 3600 * 1000.0d));
	
			// Skip the AdditionalDescription.
	
			keyChain_.sign(request, certificateParams);
			keyChain_.setDefaultCertificate(subIdentity.getDefaultKey(), request);
	
			return subIdentity;
		}
	
		/// <summary>
		/// Issue a certificate for subIdentityName signed by issuer. If the identity
		/// does not exist, it is created. A new key is generated as the default key
		/// for the identity. A default certificate for the key is signed by the
		/// issuer using its default certificate.
		/// Use KeyChain.getDefaultKeyParams().
		/// </summary>
		///
		/// <param name="subIdentityName">The name to issue the certificate for.</param>
		/// <param name="issuer">The identity of the signer.</param>
		/// <returns>The sub identity.</returns>
		internal PibIdentity addSubCertificate(Name subIdentityName, PibIdentity issuer) {
			return addSubCertificate(subIdentityName, issuer,
					net.named_data.jndn.security.KeyChain.getDefaultKeyParams());
		}
	
		/// <summary>
		/// Add a self-signed certificate made from the key and issuer ID.
		/// </summary>
		///
		/// <param name="key">The key for the certificate.</param>
		/// <param name="issuerId">The issuer ID name component for the certificate name.</param>
		/// <returns>The new certificate.</returns>
		internal CertificateV2 addCertificate(PibKey key, String issuerId) {
			Name certificateName = new Name(key.getName());
			certificateName.append(issuerId).appendVersion(3);
			CertificateV2 certificate = new CertificateV2();
			certificate.setName(certificateName);
	
			// Set the MetaInfo.
			certificate.getMetaInfo().setType(net.named_data.jndn.ContentType.KEY);
			// One hour.
			certificate.getMetaInfo().setFreshnessPeriod(3600 * 1000.0);
	
			// Set the content.
			certificate.setContent(key.getPublicKey());
	
			SigningInfo paras = new SigningInfo(key);
			// Validity period of 10 days.
			double now = net.named_data.jndn.util.Common.getNowMilliseconds();
			paras.setValidityPeriod(new ValidityPeriod(now, now + 10 * 24 * 3600
					* 1000.0d));
	
			keyChain_.sign(certificate, paras);
			return certificate;
		}
	
		public KeyChain keyChain_;
	
		private HashedSet<Name> identityNames_;
		private HashedSet<String> certificateFiles_;
	}
}
