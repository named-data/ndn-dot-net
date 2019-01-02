// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security.identity {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	
	/// <summary>
	/// SqliteIdentityStorageBase is an abstract base class for the storage of
	/// identity, public keys and certificates using SQLite. This base class has
	/// protected SQL strings and helpers so the subclasses can work with similar
	/// tables using their own SQLite libraries.
	/// </summary>
	///
	public abstract class Sqlite3IdentityStorageBase : IdentityStorage {
		/// <summary>
		/// Activate a key.  If a key is marked as inactive, its private part will not
		/// be used in packet signing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		public sealed override void activateKey(Name keyName) {
			updateKeyStatus(keyName, true);
		}
	
		/// <summary>
		/// Deactivate a key. If a key is marked as inactive, its private part will not
		/// be used in packet signing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		public sealed override void deactivateKey(Name keyName) {
			updateKeyStatus(keyName, false);
		}
	
		/// <summary>
		/// In table Key, set 'active' to isActive for the keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="isActive">The value for the 'active' field.</param>
		protected abstract internal void updateKeyStatus(Name keyName, bool isActive);
	
		/// <summary>
		/// Throw an exception if it is an error for setDefaultKeyNameForIdentity to
		/// set it.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <param name="identityNameCheck">The identity name to check the keyName.</param>
		/// <exception cref="System.Security.SecurityException">if the identity name does not match the key nameor other problem.</exception>
		protected internal void checkSetDefaultKeyNameForIdentity(Name keyName,
				Name identityNameCheck) {
			Name identityName = keyName.getPrefix(-1);
	
			if (identityNameCheck.size() > 0
					&& !identityNameCheck.equals(identityName))
				throw new SecurityException(
						"The specified identity name does not match the key name");
		}
	
		protected internal const String SELECT_MASTER_TPM_INFO_TABLE = "SELECT name FROM sqlite_master WHERE type='table' And name='TpmInfo'";
		protected internal const String SELECT_MASTER_ID_TABLE = "SELECT name FROM sqlite_master WHERE type='table' And name='Identity'";
		protected internal const String SELECT_MASTER_KEY_TABLE = "SELECT name FROM sqlite_master WHERE type='table' And name='Key'";
		protected internal const String SELECT_MASTER_CERT_TABLE = "SELECT name FROM sqlite_master WHERE type='table' And name='Certificate'";
	
		protected internal const String INIT_TPM_INFO_TABLE = "CREATE TABLE IF NOT EXISTS                                           \n"
				+ "  TpmInfo(                                                           \n"
				+ "      tpm_locator BLOB NOT NULL,                                     \n"
				+ "      PRIMARY KEY (tpm_locator)                                      \n"
				+ "  );                                                                 \n";
	
		protected internal const String INIT_ID_TABLE1 = "CREATE TABLE IF NOT EXISTS                                           \n"
				+ "  Identity(                                                          \n"
				+ "      identity_name     BLOB NOT NULL,                               \n"
				+ "      default_identity  INTEGER DEFAULT 0,                           \n"
				+ "                                                                     \n"
				+ "      PRIMARY KEY (identity_name)                                    \n"
				+ "  );                                                                 \n"
				+ "                                                                     \n";
		protected internal const String INIT_ID_TABLE2 = "CREATE INDEX identity_index ON Identity(identity_name);              \n";
	
		protected internal const String INIT_KEY_TABLE1 = "CREATE TABLE IF NOT EXISTS                                           \n"
				+ "  Key(                                                               \n"
				+ "      identity_name     BLOB NOT NULL,                               \n"
				+ "      key_identifier    BLOB NOT NULL,                               \n"
				+ "      key_type          INTEGER,                                     \n"
				+ "      public_key        BLOB,                                        \n"
				+ "      default_key       INTEGER DEFAULT 0,                           \n"
				+ "      active            INTEGER DEFAULT 0,                           \n"
				+ "                                                                     \n"
				+ "      PRIMARY KEY (identity_name, key_identifier)                    \n"
				+ "  );                                                                 \n"
				+ "                                                                     \n";
		protected internal const String INIT_KEY_TABLE2 = "CREATE INDEX key_index ON Key(identity_name);                        \n";
	
		protected internal const String INIT_CERT_TABLE1 = "CREATE TABLE IF NOT EXISTS                                           \n"
				+ "  Certificate(                                                       \n"
				+ "      cert_name         BLOB NOT NULL,                               \n"
				+ "      cert_issuer       BLOB NOT NULL,                               \n"
				+ "      identity_name     BLOB NOT NULL,                               \n"
				+ "      key_identifier    BLOB NOT NULL,                               \n"
				+ "      not_before        TIMESTAMP,                                   \n"
				+ "      not_after         TIMESTAMP,                                   \n"
				+ "      certificate_data  BLOB NOT NULL,                               \n"
				+ "      valid_flag        INTEGER DEFAULT 1,                           \n"
				+ "      default_cert      INTEGER DEFAULT 0,                           \n"
				+ "                                                                     \n"
				+ "      PRIMARY KEY (cert_name)                                        \n"
				+ "  );                                                                 \n"
				+ "                                                                     \n";
		protected internal const String INIT_CERT_TABLE2 = "CREATE INDEX cert_index ON Certificate(cert_name);           \n";
		protected internal const String INIT_CERT_TABLE3 = "CREATE INDEX subject ON Certificate(identity_name);          \n";
	
		protected internal const String SELECT_doesIdentityExist = "SELECT count(*) FROM Identity WHERE identity_name=?";
		protected internal const String SELECT_doesKeyExist = "SELECT count(*) FROM Key WHERE identity_name=? AND key_identifier=?";
		protected internal const String SELECT_getKey = "SELECT public_key FROM Key WHERE identity_name=? AND key_identifier=?";
		protected internal const String SELECT_doesCertificateExist = "SELECT count(*) FROM Certificate WHERE cert_name=?";
		protected internal const String SELECT_getCertificate = "SELECT certificate_data FROM Certificate WHERE cert_name=?";
		protected internal const String SELECT_getDefaultIdentity = "SELECT identity_name FROM Identity WHERE default_identity=1";
		protected internal const String SELECT_getDefaultKeyNameForIdentity = "SELECT key_identifier FROM Key WHERE identity_name=? AND default_key=1";
		protected internal const String SELECT_getDefaultCertificateNameForKey = "SELECT cert_name FROM Certificate WHERE identity_name=? AND key_identifier=? AND default_cert=1";
		protected internal const String SELECT_getAllIdentities_default_true = "SELECT identity_name FROM Identity WHERE default_identity=1";
		protected internal const String SELECT_getAllIdentities_default_false = "SELECT identity_name FROM Identity WHERE default_identity=0";
		protected internal const String SELECT_getAllKeyNamesOfIdentity_default_true = "SELECT key_identifier FROM Key WHERE default_key=1 and identity_name=?";
		protected internal const String SELECT_getAllKeyNamesOfIdentity_default_false = "SELECT key_identifier FROM Key WHERE default_key=0 and identity_name=?";
		protected internal const String SELECT_getAllCertificateNamesOfKey_default_true = "SELECT cert_name FROM Certificate"
				+ "  WHERE default_cert=1 and identity_name=? and key_identifier=?";
		protected internal const String SELECT_getAllCertificateNamesOfKey_default_false = "SELECT cert_name FROM Certificate"
				+ "  WHERE default_cert=0 and identity_name=? and key_identifier=?";
		protected internal const String SELECT_getTpmLocator = "SELECT tpm_locator FROM TpmInfo";
	
		protected internal const String WHERE_updateKeyStatus = "identity_name=? AND key_identifier=?";
		protected internal const String WHERE_setDefaultIdentity_reset = "default_identity=1";
		protected internal const String WHERE_setDefaultIdentity_set = "identity_name=?";
		protected internal const String WHERE_setDefaultKeyNameForIdentity_reset = "default_key=1 and identity_name=?";
		protected internal const String WHERE_setDefaultKeyNameForIdentity_set = "identity_name=? AND key_identifier=?";
		protected internal const String WHERE_setDefaultCertificateNameForKey_reset = "default_cert=1 AND identity_name=? AND key_identifier=?";
		protected internal const String WHERE_setDefaultCertificateNameForKey_set = "identity_name=? AND key_identifier=? AND cert_name=?";
		protected internal const String WHERE_deleteCertificateInfo = "cert_name=?";
		protected internal const String WHERE_deletePublicKeyInfo = "identity_name=? and key_identifier=?";
		protected internal const String WHERE_deleteIdentityInfo = "identity_name=?";
	}
}
