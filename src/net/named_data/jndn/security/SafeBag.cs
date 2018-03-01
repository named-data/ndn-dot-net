// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security.certificate;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.tpm;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A SafeBag represents a container for sensitive related information such as a
	/// certificate and private key.
	/// </summary>
	///
	public class SafeBag {
		/// <summary>
		/// Create a SafeBag with the given certificate and private key.
		/// </summary>
		///
		/// <param name="certificate">The certificate data packet. This copies the object.</param>
		/// <param name="privateKeyBag">PKCS #8 PrivateKeyInfo.</param>
		public SafeBag(Data certificate, Blob privateKeyBag) {
			this.certificate_ = null;
			this.privateKeyBag_ = new Blob();
			certificate_ = new Data(certificate);
			privateKeyBag_ = privateKeyBag;
		}
	
		/// <summary>
		/// Create a SafeBag with given private key and a new self-signed certificate
		/// for the given public key.
		/// </summary>
		///
		/// <param name="keyName">This copies the Name.</param>
		/// <param name="privateKeyBag">PKCS #8 PrivateKeyInfo.</param>
		/// <param name="publicKeyEncoding">The encoded public key for the certificate.</param>
		/// <param name="password">PKCS #8 EncryptedPrivateKeyInfo.</param>
		/// <param name="digestAlgorithm"></param>
		/// <param name="wireFormat"></param>
		public SafeBag(Name keyName, Blob privateKeyBag, Blob publicKeyEncoding,
				ByteBuffer password, DigestAlgorithm digestAlgorithm,
				WireFormat wireFormat) {
			this.certificate_ = null;
					this.privateKeyBag_ = new Blob();
			certificate_ = makeSelfSignedCertificate(keyName, privateKeyBag,
					publicKeyEncoding, password, digestAlgorithm, wireFormat);
			privateKeyBag_ = privateKeyBag;
		}
	
		/// <summary>
		/// Create a SafeBag with given private key and a new self-signed certificate
		/// for the given public key.
		/// Use getDefaultWireFormat() to encode the self-signed certificate in order
		/// to sign it.
		/// </summary>
		///
		/// <param name="keyName">This copies the Name.</param>
		/// <param name="privateKeyBag">PKCS #8 PrivateKeyInfo.</param>
		/// <param name="publicKeyEncoding">The encoded public key for the certificate.</param>
		/// <param name="password">PKCS #8 EncryptedPrivateKeyInfo.</param>
		/// <param name="digestAlgorithm"></param>
		public SafeBag(Name keyName, Blob privateKeyBag, Blob publicKeyEncoding,
				ByteBuffer password, DigestAlgorithm digestAlgorithm) {
			this.certificate_ = null;
					this.privateKeyBag_ = new Blob();
			certificate_ = makeSelfSignedCertificate(keyName, privateKeyBag,
					publicKeyEncoding, password, digestAlgorithm,
					net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
			privateKeyBag_ = privateKeyBag;
		}
	
		/// <summary>
		/// Create a SafeBag with given private key and a new self-signed certificate
		/// for the given public key, using DigestAlgorithm.SHA256 to sign it.
		/// Use getDefaultWireFormat() to encode the self-signed certificate in order
		/// to sign it.
		/// </summary>
		///
		/// <param name="keyName">This copies the Name.</param>
		/// <param name="privateKeyBag">PKCS #8 PrivateKeyInfo.</param>
		/// <param name="publicKeyEncoding">The encoded public key for the certificate.</param>
		/// <param name="password">PKCS #8 EncryptedPrivateKeyInfo.</param>
		public SafeBag(Name keyName, Blob privateKeyBag, Blob publicKeyEncoding,
				ByteBuffer password) {
			this.certificate_ = null;
					this.privateKeyBag_ = new Blob();
			certificate_ = makeSelfSignedCertificate(keyName, privateKeyBag,
					publicKeyEncoding, password, net.named_data.jndn.security.DigestAlgorithm.SHA256,
					net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
			privateKeyBag_ = privateKeyBag;
		}
	
		/// <summary>
		/// Create a SafeBag with given private key and a new self-signed certificate
		/// for the given public key, using DigestAlgorithm.SHA256 to sign it.
		/// Use getDefaultWireFormat() to encode the self-signed certificate in order
		/// to sign it.
		/// </summary>
		///
		/// <param name="keyName">This copies the Name.</param>
		/// <param name="privateKeyBag"></param>
		/// <param name="publicKeyEncoding">The encoded public key for the certificate.</param>
		public SafeBag(Name keyName, Blob privateKeyBag, Blob publicKeyEncoding) {
			this.certificate_ = null;
					this.privateKeyBag_ = new Blob();
			certificate_ = makeSelfSignedCertificate(keyName, privateKeyBag,
					publicKeyEncoding, null, net.named_data.jndn.security.DigestAlgorithm.SHA256,
					net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
			privateKeyBag_ = privateKeyBag;
		}
	
		/// <summary>
		/// Get the certificate data packet.
		/// </summary>
		///
		/// <returns>The certificate as a Data packet. If you need to process it as a
		/// certificate object then you must create a new CertificateV2(data).</returns>
		public Data getCertificate() {
			return certificate_;
		}
	
		/// <summary>
		/// Get the encoded private key.
		/// </summary>
		///
		/// <returns>The encoded private key. If encrypted, this is a PKCS #8 
		/// EncryptedPrivateKeyInfo. If not encrypted, this is an unencrypted PKCS #8
		/// PrivateKeyInfo.</returns>
		public Blob getPrivateKeyBag() {
			return privateKeyBag_;
		}
	
		private static CertificateV2 makeSelfSignedCertificate(Name keyName,
				Blob privateKeyBag, Blob publicKeyEncoding, ByteBuffer password,
				DigestAlgorithm digestAlgorithm, WireFormat wireFormat) {
			CertificateV2 certificate = new CertificateV2();
	
			// Set the name.
			double now = net.named_data.jndn.util.Common.getNowMilliseconds();
			Name certificateName = new Name(keyName);
			certificateName.append("self").appendVersion((long) now);
			certificate.setName(certificateName);
	
			// Set the MetaInfo.
			certificate.getMetaInfo().setType(net.named_data.jndn.ContentType.KEY);
			// Set a one-hour freshness period.
			certificate.getMetaInfo().setFreshnessPeriod(3600 * 1000.0d);
	
			// Set the content.
			PublicKey publicKey = null;
			try {
				publicKey = new PublicKey(publicKeyEncoding);
			} catch (UnrecognizedKeyFormatException ex) {
				// Promote to Pib.Error.
				throw new Pib.Error("Error decoding public key " + ex);
			}
			certificate.setContent(publicKey.getKeyDer());
	
			// Create a temporary in-memory Tpm and import the private key.
			Tpm tpm = new Tpm("", "", new TpmBackEndMemory());
			tpm.importPrivateKey_(keyName, privateKeyBag.buf(), password);
	
			// Set the signature info.
			if (publicKey.getKeyType() == net.named_data.jndn.security.KeyType.RSA)
				certificate.setSignature(new Sha256WithRsaSignature());
			else if (publicKey.getKeyType() == net.named_data.jndn.security.KeyType.EC)
				certificate.setSignature(new Sha256WithEcdsaSignature());
			else
				throw new AssertionError("Unsupported key type");
			Signature signatureInfo = certificate.getSignature();
			net.named_data.jndn.KeyLocator.getFromSignature(signatureInfo).setType(
					net.named_data.jndn.KeyLocatorType.KEYNAME);
			net.named_data.jndn.KeyLocator.getFromSignature(signatureInfo).setKeyName(keyName);
	
			// Set a 20-year validity period.
			net.named_data.jndn.security.ValidityPeriod.getFromSignature(signatureInfo).setPeriod(now,
					now + 20 * 365 * 24 * 3600 * 1000.0d);
	
			// Encode once to get the signed portion.
			SignedBlob encoding = certificate.wireEncode(wireFormat);
			Blob signatureBytes = tpm.sign(encoding.signedBuf(), keyName,
					digestAlgorithm);
			signatureInfo.setSignature(signatureBytes);
	
			// Encode again to include the signature.
			certificate.wireEncode(wireFormat);
	
			return certificate;
		}
	
		private Data certificate_;
		private Blob privateKeyBag_;
	}
}
