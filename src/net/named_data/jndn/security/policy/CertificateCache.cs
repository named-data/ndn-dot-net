// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2016 Regents of the University of California.
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
	using net.named_data.jndn.security.certificate;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A CertificateCache is used to save other users' certificate during
	/// verification.
	/// </summary>
	///
	public class CertificateCache {
		public CertificateCache() {
			this.cache_ = new Hashtable();
		}
	
		/// <summary>
		/// Insert the certificate into the cache. Assumes the timestamp is not yet
		/// removed from the name.
		/// </summary>
		///
		/// <param name="certificate">The certificate to copy and insert.</param>
		public void insertCertificate(IdentityCertificate certificate) {
			Name certName = certificate.getName().getPrefix(-1);
			ILOG.J2CsMapping.Collections.Collections.Put(cache_,certName.toUri(),certificate.wireEncode());
		}
	
		/// <summary>
		/// Remove a certificate from the cache. This does nothing if it is not present.
		/// </summary>
		///
		/// <param name="certificateName"></param>
		public void deleteCertificate(Name certificateName) {
			ILOG.J2CsMapping.Collections.Collections.Remove(cache_,certificateName.toUri());
		}
	
		/// <summary>
		/// Fetch a certificate from the cache.
		/// </summary>
		///
		/// <param name="certificateName"></param>
		/// <returns>A new copy of the IdentityCertificate, or null if not found.</returns>
		public IdentityCertificate getCertificate(Name certificateName) {
			Blob certData = (Blob) ILOG.J2CsMapping.Collections.Collections.Get(cache_,certificateName.toUri());
			if (certData == null)
				return null;
	
			IdentityCertificate cert = new IdentityCertificate();
			try {
				cert.wireDecode(certData.buf());
			} catch (EncodingException ex) {
				ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(CertificateCache).FullName).log(
						ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex);
				throw new Exception(ex.Message);
			}
	
			return cert;
		}
	
		/// <summary>
		/// Clear all certificates from the store.
		/// </summary>
		///
		public void reset() {
			cache_.clear();
		}
	
		// The key is the certificate name URI. The value is the wire encoding Blob.
		// Use HashMap without generics so it works with older Java compilers.
		private readonly Hashtable cache_;
	}
}
