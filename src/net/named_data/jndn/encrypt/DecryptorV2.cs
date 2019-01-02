// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2018-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using javax.crypto;
	using javax.crypto.spec;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// DecryptorV2 decrypts the supplied EncryptedContent element, using
	/// asynchronous operations, contingent on the retrieval of the CK Data packet,
	/// the KDK, and the successful decryption of both of these. For the meaning of
	/// "KDK", etc. see:
	/// https://github.com/named-data/name-based-access-control/blob/new/docs/spec.rst
	/// </summary>
	///
	public class DecryptorV2 {
		public sealed class Anonymous_C5 : OnData {
				private readonly DecryptorV2 outer_DecryptorV2;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				private readonly DecryptorV2.ContentKey  contentKey;
		
				public Anonymous_C5(DecryptorV2 paramouter_DecryptorV2,
						net.named_data.jndn.encrypt.EncryptError.OnError  onError_0, DecryptorV2.ContentKey  contentKey_1) {
					this.onError = onError_0;
					this.contentKey = contentKey_1;
					this.outer_DecryptorV2 = paramouter_DecryptorV2;
				}
		
				public void onData(Interest ckInterest, Data ckData) {
					try {
						contentKey.pendingInterest = 0;
						// TODO: Verify that the key is legitimate.
						Name[] kdkPrefix = new Name[1];
						Name[] kdkIdentityName = new Name[1];
						Name[] kdkKeyName = new Name[1];
						if (!DecryptorV2.extractKdkInfoFromCkName(ckData.getName(),
								ckInterest.getName(), onError,
								kdkPrefix, kdkIdentityName, kdkKeyName))
							// The error has already been reported.
							return;
		
						// Check if the KDK already exists.
						PibIdentity kdkIdentity = null;
						try {
							kdkIdentity = outer_DecryptorV2.internalKeyChain_.getPib()
									.getIdentity(kdkIdentityName[0]);
						} catch (Pib.Error ex) {
						}
						if (kdkIdentity != null) {
							PibKey kdkKey = null;
							try {
								kdkKey = kdkIdentity
										.getKey(kdkKeyName[0]);
							} catch (Pib.Error ex_0) {
							}
							if (kdkKey != null) {
								// The KDK was already fetched and imported.
								net.named_data.jndn.encrypt.DecryptorV2.logger_.log(
										ILOG.J2CsMapping.Util.Logging.Level.INFO,
										"KDK {0} already exists, so directly using it to decrypt the CK",
										kdkKeyName);
								outer_DecryptorV2.decryptCkAndProcessPendingDecrypts(
										contentKey, ckData,
										kdkKeyName[0], onError);
								return;
							}
						}
		
						outer_DecryptorV2.fetchKdk(contentKey, kdkPrefix[0], ckData,
								onError, net.named_data.jndn.encrypt.EncryptorV2.N_RETRIES);
					} catch (Exception ex_1) {
						onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.General,
								"Error in fetchCk onData: " + ex_1);
					}
				}
			}
	
		public sealed class Anonymous_C4 : OnTimeout {
				private readonly DecryptorV2 outer_DecryptorV2;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				private readonly int nTriesLeft;
				private readonly DecryptorV2.ContentKey  contentKey;
				private readonly Name ckName;
		
				public Anonymous_C4(DecryptorV2 paramouter_DecryptorV2,
						net.named_data.jndn.encrypt.EncryptError.OnError  onError_0, int nTriesLeft_1, DecryptorV2.ContentKey  contentKey_2,
						Name ckName_3) {
					this.onError = onError_0;
					this.nTriesLeft = nTriesLeft_1;
					this.contentKey = contentKey_2;
					this.ckName = ckName_3;
					this.outer_DecryptorV2 = paramouter_DecryptorV2;
				}
		
				public void onTimeout(Interest interest) {
					contentKey.pendingInterest = 0;
					if (nTriesLeft > 1)
						outer_DecryptorV2.fetchCk(ckName, contentKey, onError,
								nTriesLeft - 1);
					else
						onError.onError(
								net.named_data.jndn.encrypt.EncryptError.ErrorCode.CkRetrievalTimeout,
								"Retrieval of CK ["
										+ interest.getName().toUri()
										+ "] timed out");
				}
			}
	
		public sealed class Anonymous_C3 : OnNetworkNack {
			private readonly DecryptorV2.ContentKey  contentKey;
			private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
	
			public Anonymous_C3(DecryptorV2.ContentKey  contentKey_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
				this.contentKey = contentKey_0;
				this.onError = onError_1;
			}
	
			public void onNetworkNack(Interest interest,
					NetworkNack networkNack) {
				contentKey.pendingInterest = 0;
				onError.onError(
						net.named_data.jndn.encrypt.EncryptError.ErrorCode.CkRetrievalFailure,
						"Retrieval of CK ["
								+ interest.getName().toUri()
								+ "] failed. Got NACK ("
								+ networkNack.getReason() + ")");
			}
		}
	
		public sealed class Anonymous_C2 : OnData {
				private readonly DecryptorV2 outer_DecryptorV2;
				private readonly Name kdkPrefix;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				private readonly DecryptorV2.ContentKey  contentKey;
				private readonly Data ckData;
		
				public Anonymous_C2(DecryptorV2 paramouter_DecryptorV2, Name kdkPrefix_0,
						net.named_data.jndn.encrypt.EncryptError.OnError  onError_1, DecryptorV2.ContentKey  contentKey_2, Data ckData_3) {
					this.kdkPrefix = kdkPrefix_0;
					this.onError = onError_1;
					this.contentKey = contentKey_2;
					this.ckData = ckData_3;
					this.outer_DecryptorV2 = paramouter_DecryptorV2;
				}
		
				public void onData(Interest kdkInterest, Data kdkData) {
					contentKey.pendingInterest = 0;
					// TODO: Verify that the key is legitimate.
		
					bool isOk = outer_DecryptorV2.decryptAndImportKdk(kdkData, onError);
					if (!isOk)
						return;
					// This way of getting the kdkKeyName is a bit hacky.
					Name kdkKeyName = kdkPrefix.getPrefix(-2)
							.append("KEY").append(kdkPrefix.get(-1));
					outer_DecryptorV2.decryptCkAndProcessPendingDecrypts(contentKey,
							ckData, kdkKeyName, onError);
				}
			}
	
		public sealed class Anonymous_C1 : OnTimeout {
				private readonly DecryptorV2 outer_DecryptorV2;
				private readonly DecryptorV2.ContentKey  contentKey;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				private readonly Name kdkPrefix;
				private readonly Data ckData;
				private readonly int nTriesLeft;
		
				public Anonymous_C1(DecryptorV2 paramouter_DecryptorV2,
						DecryptorV2.ContentKey  contentKey_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1, Name kdkPrefix_2,
						Data ckData_3, int nTriesLeft_4) {
					this.contentKey = contentKey_0;
					this.onError = onError_1;
					this.kdkPrefix = kdkPrefix_2;
					this.ckData = ckData_3;
					this.nTriesLeft = nTriesLeft_4;
					this.outer_DecryptorV2 = paramouter_DecryptorV2;
				}
		
				public void onTimeout(Interest interest) {
					contentKey.pendingInterest = 0;
					if (nTriesLeft > 1)
						outer_DecryptorV2.fetchKdk(contentKey, kdkPrefix, ckData,
								onError, nTriesLeft - 1);
					else
						onError.onError(
								net.named_data.jndn.encrypt.EncryptError.ErrorCode.KdkRetrievalTimeout,
								"Retrieval of KDK ["
										+ interest.getName().toUri()
										+ "] timed out");
				}
			}
	
		public sealed class Anonymous_C0 : OnNetworkNack {
			private readonly DecryptorV2.ContentKey  contentKey;
			private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
	
			public Anonymous_C0(DecryptorV2.ContentKey  contentKey_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
				this.contentKey = contentKey_0;
				this.onError = onError_1;
			}
	
			public void onNetworkNack(Interest interest,
					NetworkNack networkNack) {
				contentKey.pendingInterest = 0;
				onError.onError(
						net.named_data.jndn.encrypt.EncryptError.ErrorCode.KdkRetrievalFailure,
						"Retrieval of KDK ["
								+ interest.getName().toUri()
								+ "] failed. Got NACK ("
								+ networkNack.getReason() + ")");
			}
		}
	
		public interface DecryptSuccessCallback {
			void onSuccess(Blob plainData);
		}
	
		/// <summary>
		/// Create a DecryptorV2 with the given parameters.
		/// </summary>
		///
		/// <param name="credentialsKey"></param>
		/// <param name="validator"></param>
		/// <param name="keyChain">The KeyChain that will be used to decrypt the KDK.</param>
		/// <param name="face">The Face that will be used to fetch the CK and KDK.</param>
		public DecryptorV2(PibKey credentialsKey, Validator validator,
				KeyChain keyChain, Face face) {
			this.contentKeys_ = new Hashtable<Name, ContentKey>();
			credentialsKey_ = credentialsKey;
			// validator_ = validator;
			face_ = face;
			keyChain_ = keyChain;
			try {
				internalKeyChain_ = new KeyChain("pib-memory:", "tpm-memory:");
			} catch (Exception ex) {
				// We are just creating an in-memory store, so we don't expect an error.
				throw new Exception("Error creating in-memory KeyChain: " + ex);
			}
		}
	
		public void shutdown() {
			/* foreach */
			foreach (DecryptorV2.ContentKey  contentKey_0  in  contentKeys_.Values) {
				if (contentKey_0.pendingInterest > 0) {
					face_.removePendingInterest(contentKey_0.pendingInterest);
					contentKey_0.pendingInterest = 0;
	
					/* foreach */
					foreach (ContentKey.PendingDecrypt pendingDecrypt  in  contentKey_0.pendingDecrypts)
						pendingDecrypt.onError
								.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.CkRetrievalFailure,
										"Canceling pending decrypt as ContentKey is being destroyed");
	
					// Clear is not really necessary, but just in case.
					ILOG.J2CsMapping.Collections.Collections.Clear(contentKey_0.pendingDecrypts);
				}
			}
		}
	
		/// <summary>
		/// Asynchronously decrypt the encryptedContent.
		/// </summary>
		///
		/// <param name="encryptedContent">the EncryptedContent object. If you may change it later, then pass in a copy of the object.</param>
		/// <param name="onSuccess">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onError_0">error string. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		public void decrypt(EncryptedContent encryptedContent,
				DecryptorV2.DecryptSuccessCallback  onSuccess, EncryptError.OnError onError_0) {
			if (encryptedContent.getKeyLocator().getType() != net.named_data.jndn.KeyLocatorType.KEYNAME) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
						"Missing required KeyLocator in the supplied EncryptedContent block");
				onError_0.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.MissingRequiredKeyLocator,
						"Missing required KeyLocator in the supplied EncryptedContent block");
				return;
			}
	
			if (!encryptedContent.hasInitialVector()) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
						"Missing required initial vector in the supplied EncryptedContent block");
				onError_0.onError(
						net.named_data.jndn.encrypt.EncryptError.ErrorCode.MissingRequiredInitialVector,
						"Missing required initial vector in the supplied EncryptedContent block");
				return;
			}
	
			Name ckName_1 = encryptedContent.getKeyLocatorName();
			DecryptorV2.ContentKey  contentKey_2 = ILOG.J2CsMapping.Collections.Collections.Get(contentKeys_,ckName_1);
			bool isNew = (contentKey_2 == null);
			if (isNew) {
				contentKey_2 = new DecryptorV2.ContentKey ();
				ILOG.J2CsMapping.Collections.Collections.Put(contentKeys_,ckName_1,contentKey_2);
			}
	
			if (contentKey_2.isRetrieved)
				doDecrypt(encryptedContent, contentKey_2.bits, onSuccess, onError_0);
			else {
				logger_.log(
						ILOG.J2CsMapping.Util.Logging.Level.INFO,
						"CK {0} not yet available, so adding to the pending decrypt queue",
						ckName_1);
				ILOG.J2CsMapping.Collections.Collections.Add(contentKey_2.pendingDecrypts,new ContentKey.PendingDecrypt(
									encryptedContent, onSuccess, onError_0));
			}
	
			if (isNew)
				fetchCk(ckName_1, contentKey_2, onError_0, net.named_data.jndn.encrypt.EncryptorV2.N_RETRIES);
		}
	
		public class ContentKey {
			public ContentKey() {
				this.isRetrieved = false;
				this.pendingInterest = 0;
				this.pendingDecrypts = new ArrayList<PendingDecrypt>();
			}
			public class PendingDecrypt {
				public PendingDecrypt(EncryptedContent encryptedContent,
						DecryptorV2.DecryptSuccessCallback  onSuccess,
						EncryptError.OnError onError_0) {
					this.encryptedContent = encryptedContent;
					this.onSuccess = onSuccess;
					this.onError = onError_0;
				}
	
				public EncryptedContent encryptedContent;
				public DecryptorV2.DecryptSuccessCallback  onSuccess;
				public EncryptError.OnError onError;
			} 
	
			public bool isRetrieved;
			public Blob bits;
			public long pendingInterest;
			public ArrayList<PendingDecrypt> pendingDecrypts;
		}
	
		internal void fetchCk(Name ckName_0, DecryptorV2.ContentKey  contentKey_1,
				EncryptError.OnError onError_2, int nTriesLeft_3) {
			// The full name of the CK is
			//
			// <whatever-prefix>/CK/<ck-id>  /ENCRYPTED-BY /<kek-prefix>/KEK/<key-id>
			// \                          /                \                        /
			//  -----------  -------------                  -----------  -----------
			//             \/                                          \/
			//   from the encrypted data          unknown (name in retrieved CK is used to determine KDK)
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Fetching CK {0}", ckName_0);
	
			try {
				contentKey_1.pendingInterest = face_.expressInterest(new Interest(
						ckName_0).setMustBeFresh(false).setCanBePrefix(true),
						new DecryptorV2.Anonymous_C5 (this, onError_2, contentKey_1), new DecryptorV2.Anonymous_C4 (this, onError_2, nTriesLeft_3, contentKey_1,
								ckName_0), new DecryptorV2.Anonymous_C3 (contentKey_1, onError_2));
			} catch (Exception ex) {
				onError_2.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.General,
						"expressInterest error: " + ex);
			}
		}
	
		internal void fetchKdk(DecryptorV2.ContentKey  contentKey_0, Name kdkPrefix_1,
				Data ckData_2, EncryptError.OnError onError_3,
				int nTriesLeft_4) {
			// <kdk-prefix>/KDK/<kdk-id>    /ENCRYPTED-BY  /<credential-identity>/KEY/<key-id>
			// \                          /                \                                /
			//  -----------  -------------                  ---------------  ---------------
			//             \/                                              \/
			//     from the CK data                                from configuration
	
			Name kdkName = new Name(kdkPrefix_1);
			kdkName.append(net.named_data.jndn.encrypt.EncryptorV2.NAME_COMPONENT_ENCRYPTED_BY).append(
					credentialsKey_.getName());
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Fetching KDK {0}", kdkName);
	
			try {
				contentKey_0.pendingInterest = face_.expressInterest(new Interest(
						kdkName).setMustBeFresh(true).setCanBePrefix(false),
						new DecryptorV2.Anonymous_C2 (this, kdkPrefix_1, onError_3, contentKey_0,
								ckData_2), new DecryptorV2.Anonymous_C1 (this, contentKey_0, onError_3, kdkPrefix_1,
								ckData_2, nTriesLeft_4), new DecryptorV2.Anonymous_C0 (contentKey_0, onError_3));
			} catch (Exception ex) {
				onError_3.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.General,
						"expressInterest error: " + ex);
			}
		}
	
		
		/// <returns>True for success, false for error (where this has called onError).</returns>
		internal bool decryptAndImportKdk(Data kdkData,
				EncryptError.OnError onError_0) {
			try {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Decrypting and importing KDK {0}",
						kdkData.getName());
				EncryptedContent encryptedContent_1 = new EncryptedContent();
				encryptedContent_1.wireDecodeV2(kdkData.getContent());
	
				SafeBag safeBag = new SafeBag(encryptedContent_1.getPayload());
				Blob secret = keyChain_.getTpm().decrypt(
						encryptedContent_1.getPayloadKey().buf(),
						credentialsKey_.getName());
				if (secret.isNull()) {
					onError_0.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.TpmKeyNotFound,
							"Could not decrypt secret, "
									+ credentialsKey_.getName().toUri()
									+ " not found in TPM");
					return false;
				}
	
				internalKeyChain_.importSafeBag(safeBag, secret.buf());
				return true;
			} catch (Exception ex) {
				// This can be EncodingException, Pib.Error, Tpm.Error, or a bunch of
				// other runtime-derived errors.
				onError_0.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.DecryptionFailure,
						"Failed to decrypt KDK [" + kdkData.getName().toUri()
								+ "]: " + ex);
				return false;
			}
		}
	
		internal void decryptCkAndProcessPendingDecrypts(DecryptorV2.ContentKey  contentKey_0,
				Data ckData_1, Name kdkKeyName, EncryptError.OnError onError_2) {
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Decrypting CK data {0}", ckData_1.getName());
	
			EncryptedContent content = new EncryptedContent();
			try {
				content.wireDecodeV2(ckData_1.getContent());
			} catch (Exception ex) {
				onError_2.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
						"Error decrypting EncryptedContent: " + ex);
				return;
			}
	
			Blob ckBits;
			try {
				ckBits = internalKeyChain_.getTpm().decrypt(
						content.getPayload().buf(), kdkKeyName);
			} catch (Exception ex_3) {
				// We don't expect this from the in-memory KeyChain.
				onError_2.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.DecryptionFailure,
						"Error decrypting the CK EncryptedContent " + ex_3);
				return;
			}
	
			if (ckBits.isNull()) {
				onError_2.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.TpmKeyNotFound,
						"Could not decrypt secret, " + kdkKeyName.toUri()
								+ " not found in TPM");
				return;
			}
	
			contentKey_0.bits = ckBits;
			contentKey_0.isRetrieved = true;
	
			/* foreach */
			foreach (ContentKey.PendingDecrypt pendingDecrypt  in  contentKey_0.pendingDecrypts)
				// TODO: If this calls onError, should we quit?
				doDecrypt(pendingDecrypt.encryptedContent, contentKey_0.bits,
						pendingDecrypt.onSuccess, pendingDecrypt.onError);
	
			ILOG.J2CsMapping.Collections.Collections.Clear(contentKey_0.pendingDecrypts);
		}
	
		private static void doDecrypt(EncryptedContent content, Blob ckBits,
				DecryptorV2.DecryptSuccessCallback  onSuccess_0, EncryptError.OnError onError_1) {
			if (!content.hasInitialVector()) {
				onError_1.onError(
						net.named_data.jndn.encrypt.EncryptError.ErrorCode.MissingRequiredInitialVector,
						"Expecting Initial Vector in the encrypted content, but it is not present");
				return;
			}
	
			Blob plainData;
			try {
				Cipher cipher = javax.crypto.Cipher.getInstance("AES/CBC/PKCS5PADDING");
				cipher.init(javax.crypto.Cipher.DECRYPT_MODE,
						new SecretKeySpec(ckBits.getImmutableArray(), "AES"),
						new IvParameterSpec(content.getInitialVector()
								.getImmutableArray()));
				plainData = new Blob(cipher.doFinal(content.getPayload()
						.getImmutableArray()), false);
			} catch (Exception ex) {
				onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.DecryptionFailure,
						"Decryption error in doDecrypt: " + ex);
				return;
			}
	
			try {
				onSuccess_0.onSuccess(plainData);
			} catch (Exception exception) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onSuccess", exception);
			}
		}
	
		/// <summary>
		/// Convert the KEK name to the KDK prefix:
		/// <access-namespace>/KEK/<key-id> ==> <access-namespace>/KDK/<key-id>.
		/// </summary>
		///
		/// <param name="kekName">The KEK name.</param>
		/// <param name="onError_0">This calls onError.onError(errorCode, message) for an error.</param>
		/// <returns>The KDK prefix, or null if an error was reported to onError.</returns>
		private static Name convertKekNameToKdkPrefix(Name kekName,
				EncryptError.OnError onError_0) {
			if (kekName.size() < 2
					|| !kekName.get(-2).equals(net.named_data.jndn.encrypt.EncryptorV2.NAME_COMPONENT_KEK)) {
				onError_0.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.KekInvalidName,
						"Invalid KEK name [" + kekName.toUri() + "]");
				return null;
			}
	
			return kekName.getPrefix(-2).append(net.named_data.jndn.encrypt.EncryptorV2.NAME_COMPONENT_KDK)
					.append(kekName.get(-1));
		}
	
		/// <summary>
		/// Extract the KDK information from the CK Data packet name. The KDK identity name
		/// plus the KDK key ID together identify the KDK private key in the KeyChain.
		/// </summary>
		///
		/// <param name="ckDataName">The name of the CK Data packet.</param>
		/// <param name="ckName_0">The CK name from the Interest used to fetch the CK Data packet.</param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		/// <param name="kdkPrefix_2">This sets kdkPrefix[0] to the KDK prefix.</param>
		/// <param name="kdkIdentityName">This sets kdkIdentityName[0] to the KDK identity name.</param>
		/// <param name="kdkKeyId">This sets kdkKeyId[0] to the KDK key ID.</param>
		/// <returns>True for success or false if an error was reported to onError.</returns>
		static internal bool extractKdkInfoFromCkName(Name ckDataName,
				Name ckName_0, EncryptError.OnError onError_1, Name[] kdkPrefix_2,
				Name[] kdkIdentityName, Name[] kdkKeyId) {
			// <full-ck-name-with-id> | /ENCRYPTED-BY/<kek-prefix>/NAC/KEK/<key-id>
	
			if (ckDataName.size() < ckName_0.size() + 1
					|| !ckDataName.getPrefix(ckName_0.size()).equals(ckName_0)
					|| !ckDataName.get(ckName_0.size()).equals(
							net.named_data.jndn.encrypt.EncryptorV2.NAME_COMPONENT_ENCRYPTED_BY)) {
				onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.CkInvalidName,
						"Invalid CK name [" + ckDataName.toUri() + "]");
				return false;
			}
	
			Name kekName = ckDataName.getSubName(ckName_0.size() + 1);
			kdkPrefix_2[0] = convertKekNameToKdkPrefix(kekName, onError_1);
			if (kdkPrefix_2[0] == null)
				// The error has already been reported.
				return false;
	
			kdkIdentityName[0] = kekName.getPrefix(-2);
			kdkKeyId[0] = kekName.getPrefix(-2).append("KEY")
					.append(kekName.get(-1));
			return true;
		}
	
		private readonly PibKey credentialsKey_;
		// private final Validator validator_;
		private readonly Face face_;
		// The external keychain with access credentials.
		private readonly KeyChain keyChain_;
		// The internal in-memory keychain for temporarily storing KDKs.
		internal readonly KeyChain internalKeyChain_;
	
		// TODO: add some expiration, so they are not stored forever.
		private readonly Hashtable<Name, ContentKey> contentKeys_;
	
		static internal readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(DecryptorV2).FullName);
	}
}
