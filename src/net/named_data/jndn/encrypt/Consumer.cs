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
namespace net.named_data.jndn.encrypt {
	
	using ILOG.J2CsMapping.NIO;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encrypt.algo;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A Consumer manages fetched group keys used to decrypt a data packet in the
	/// group-based encryption protocol.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public class Consumer {
		/// <summary>
		/// Create a Consumer to use the given ConsumerDb, Face and other values.
		/// </summary>
		///
		/// <param name="face">The face used for data packet and key fetching.</param>
		/// <param name="keyChain">The keyChain used to verify data packets.</param>
		/// <param name="groupName"></param>
		/// <param name="consumerName"></param>
		/// <param name="database">The ConsumerDb database for storing decryption keys.</param>
		public Consumer(Face face, KeyChain keyChain, Name groupName,
				Name consumerName, ConsumerDb database) {
			this.cKeyMap_ = new Hashtable();
					this.dKeyMap_ = new Hashtable();
			database_ = database;
			keyChain_ = keyChain;
			face_ = face;
			groupName_ = new Name(groupName);
			consumerName_ = new Name(consumerName);
		}
	
		public sealed class Anonymous_C6 : OnData {
				public sealed class Anonymous_C17 : OnVerified {
								public sealed class Anonymous_C18 : Consumer.OnPlainText {
																private readonly net.named_data.jndn.encrypt.Consumer.Anonymous_C6.Anonymous_C17  outer_Anonymous_C17;
												
																public Anonymous_C18(net.named_data.jndn.encrypt.Consumer.Anonymous_C6.Anonymous_C17  paramouter_Anonymous_C17) {
																	this.outer_Anonymous_C17 = paramouter_Anonymous_C17;
																}
												
																public void onPlainText(Blob plainText) {
																	try {
																		outer_Anonymous_C17.outer_Anonymous_C6.onConsumeComplete.onConsumeComplete(
																				outer_Anonymous_C17.contentData, plainText);
																	} catch (Exception ex) {
																		net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
																				"Error in onConsumeComplete",
																				ex);
																	}
																}
															}
					
								internal readonly Consumer.Anonymous_C6  outer_Anonymous_C6;
								internal readonly Data contentData;
					
								public Anonymous_C17(Consumer.Anonymous_C6  paramouter_Anonymous_C6,
										Data contentData_0) {
									this.contentData = contentData_0;
									this.outer_Anonymous_C6 = paramouter_Anonymous_C6;
								}
					
								public void onVerified(Data validData) {
									// Decrypt the content.
									outer_Anonymous_C6.outer_Consumer.decryptContent(validData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C6.Anonymous_C17.Anonymous_C18 (this), outer_Anonymous_C6.onError);
								}
							}
		
				public sealed class Anonymous_C16 : OnVerifyFailed {
								private readonly Consumer.Anonymous_C6  outer_Anonymous_C6;
					
								public Anonymous_C16(Consumer.Anonymous_C6  paramouter_Anonymous_C6) {
									this.outer_Anonymous_C6 = paramouter_Anonymous_C6;
								}
					
								public void onVerifyFailed(Data d) {
									try {
										outer_Anonymous_C6.onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.Validation,
												"verifyData failed");
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError",
												ex);
									}
								}
							}
		
				internal readonly Consumer outer_Consumer;
				internal readonly Consumer.OnConsumeComplete  onConsumeComplete;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
		
				public Anonymous_C6(Consumer paramouter_Consumer,
						Consumer.OnConsumeComplete  onConsumeComplete_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
					this.onConsumeComplete = onConsumeComplete_0;
					this.onError = onError_1;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onData(Interest contentInterest, Data contentData_0) {
					// The Interest has no selectors, so assume the library correctly
					// matched with the Data name before calling onData.
		
					try {
						outer_Consumer.keyChain_.verifyData(contentData_0, new net.named_data.jndn.encrypt.Consumer.Anonymous_C6.Anonymous_C17 (this, contentData_0), new net.named_data.jndn.encrypt.Consumer.Anonymous_C6.Anonymous_C16 (this));
					} catch (SecurityException ex) {
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.SecurityException,
									"verifyData error: " + ex.Message);
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
						}
					}
				}
			}
	
		public sealed class Anonymous_C5 : OnTimeout {
				public sealed class Anonymous_C15 : OnTimeout {
								private readonly Consumer.Anonymous_C5  outer_Anonymous_C5;
					
								public Anonymous_C15(Consumer.Anonymous_C5  paramouter_Anonymous_C5) {
									this.outer_Anonymous_C5 = paramouter_Anonymous_C5;
								}
					
								public void onTimeout(Interest contentInterest) {
									try {
										outer_Anonymous_C5.onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.Timeout, outer_Anonymous_C5.interest
												.getName().toUri());
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError",
												ex);
									}
								}
							}
		
				private readonly Consumer outer_Consumer;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				internal readonly Interest interest;
				private readonly OnData onData;
		
				public Anonymous_C5(Consumer paramouter_Consumer, net.named_data.jndn.encrypt.EncryptError.OnError  onError_0,
						Interest interest_1, OnData onData_2) {
					this.onError = onError_0;
					this.interest = interest_1;
					this.onData = onData_2;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onTimeout(Interest contentInterest) {
					// We should re-try at least once.
					try {
						outer_Consumer.face_.expressInterest(interest, onData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C5.Anonymous_C15 (this));
					} catch (IOException ex) {
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.IOException,
									"expressInterest error: " + ex.Message);
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
						}
					}
				}
			}
	
		public sealed class Anonymous_C4 : OnData {
				public sealed class Anonymous_C13 : OnVerified {
								public sealed class Anonymous_C14 : Consumer.OnPlainText  {
									public void onPlainText(Blob cKeyBits) {
										// cKeyName is already a copy inside the local dataEncryptedContent.
										ILOG.J2CsMapping.Collections.Collections.Put(outer_Anonymous_C4.outer_Consumer.cKeyMap_,outer_Anonymous_C13.outer_Anonymous_C4.cKeyName,cKeyBits);
										Consumer.decrypt(outer_Anonymous_C13.outer_Anonymous_C4.dataEncryptedContent, cKeyBits,
												outer_Anonymous_C13.outer_Anonymous_C4.onPlainText, outer_Anonymous_C13.outer_Anonymous_C4.onError);
									}
								}
					
								private readonly Consumer.Anonymous_C4  outer_Anonymous_C4;
					
								public Anonymous_C13(Consumer.Anonymous_C4  paramouter_Anonymous_C4) {
									this.outer_Anonymous_C4 = paramouter_Anonymous_C4;
								}
					
								public void onVerified(Data validCKeyData) {
									outer_Anonymous_C4.outer_Consumer.decryptCKey(validCKeyData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C4.Anonymous_C13.Anonymous_C14 (), outer_Anonymous_C4.onError);
								}
							}
		
				public sealed class Anonymous_C12 : OnVerifyFailed {
								private readonly Consumer.Anonymous_C4  outer_Anonymous_C4;
					
								public Anonymous_C12(Consumer.Anonymous_C4  paramouter_Anonymous_C4) {
									this.outer_Anonymous_C4 = paramouter_Anonymous_C4;
								}
					
								public void onVerifyFailed(Data d) {
									try {
										outer_Anonymous_C4.onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.Validation,
												"verifyData failed");
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
												"Error in onError", ex);
									}
								}
							}
		
				internal readonly Consumer outer_Consumer;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				internal readonly Consumer.OnPlainText  onPlainText;
				internal readonly Name cKeyName;
				internal readonly EncryptedContent dataEncryptedContent;
		
				public Anonymous_C4(Consumer paramouter_Consumer, net.named_data.jndn.encrypt.EncryptError.OnError  onError_0,
						Consumer.OnPlainText  onPlainText_1, Name cKeyName_2,
						EncryptedContent dataEncryptedContent_3) {
					this.onError = onError_0;
					this.onPlainText = onPlainText_1;
					this.cKeyName = cKeyName_2;
					this.dataEncryptedContent = dataEncryptedContent_3;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onData(Interest cKeyInterest, Data cKeyData) {
					// The Interest has no selectors, so assume the library correctly
					// matched with the Data name before calling onData.
		
					try {
						outer_Consumer.keyChain_.verifyData(cKeyData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C4.Anonymous_C13 (this), new net.named_data.jndn.encrypt.Consumer.Anonymous_C4.Anonymous_C12 (this));
					} catch (SecurityException ex) {
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.SecurityException,
									"verifyData error: " + ex.Message);
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError",
									exception);
						}
					}
				}
			}
	
		public sealed class Anonymous_C3 : OnTimeout {
				public sealed class Anonymous_C11 : OnTimeout {
								private readonly Consumer.Anonymous_C3  outer_Anonymous_C3;
					
								public Anonymous_C11(Consumer.Anonymous_C3  paramouter_Anonymous_C3) {
									this.outer_Anonymous_C3 = paramouter_Anonymous_C3;
								}
					
								public void onTimeout(
										Interest contentInterest) {
									try {
										outer_Anonymous_C3.onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.Timeout,
												outer_Anonymous_C3.interest.getName().toUri());
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
												"Error in onError", ex);
									}
								}
							}
		
				private readonly Consumer outer_Consumer;
				private readonly OnData onData;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				internal readonly Interest interest;
		
				public Anonymous_C3(Consumer paramouter_Consumer, OnData onData_0,
						net.named_data.jndn.encrypt.EncryptError.OnError  onError_1, Interest interest_2) {
					this.onData = onData_0;
					this.onError = onError_1;
					this.interest = interest_2;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onTimeout(Interest dKeyInterest) {
					// We should re-try at least once.
					try {
						outer_Consumer.face_.expressInterest(interest, onData,
								new net.named_data.jndn.encrypt.Consumer.Anonymous_C3.Anonymous_C11 (this));
					} catch (IOException ex) {
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.IOException,
									"expressInterest error: " + ex.Message);
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError",
									exception);
						}
					}
				}
			}
	
		public sealed class Anonymous_C2 : OnData {
				public sealed class Anonymous_C9 : OnVerified {
								public sealed class Anonymous_C10 : Consumer.OnPlainText  {
									public void onPlainText(Blob dKeyBits) {
										// dKeyName is already a local copy.
										ILOG.J2CsMapping.Collections.Collections.Put(outer_Anonymous_C2.outer_Consumer.dKeyMap_,outer_Anonymous_C9.outer_Anonymous_C2.dKeyName,dKeyBits);
										Consumer.decrypt(outer_Anonymous_C9.outer_Anonymous_C2.cKeyEncryptedContent, dKeyBits,
												outer_Anonymous_C9.outer_Anonymous_C2.onPlainText, outer_Anonymous_C9.outer_Anonymous_C2.onError);
									}
								}
					
								private readonly Consumer.Anonymous_C2  outer_Anonymous_C2;
					
								public Anonymous_C9(Consumer.Anonymous_C2  paramouter_Anonymous_C2) {
									this.outer_Anonymous_C2 = paramouter_Anonymous_C2;
								}
					
								public void onVerified(Data validDKeyData) {
									outer_Anonymous_C2.outer_Consumer.decryptDKey(validDKeyData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C2.Anonymous_C9.Anonymous_C10 (), outer_Anonymous_C2.onError);
								}
							}
		
				public sealed class Anonymous_C8 : OnVerifyFailed {
								private readonly Consumer.Anonymous_C2  outer_Anonymous_C2;
					
								public Anonymous_C8(Consumer.Anonymous_C2  paramouter_Anonymous_C2) {
									this.outer_Anonymous_C2 = paramouter_Anonymous_C2;
								}
					
								public void onVerifyFailed(Data d) {
									try {
										outer_Anonymous_C2.onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.Validation,
												"verifyData failed");
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
												"Error in onError", ex);
									}
								}
							}
		
				internal readonly Consumer outer_Consumer;
				internal readonly EncryptedContent cKeyEncryptedContent;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				internal readonly Name dKeyName;
				internal readonly Consumer.OnPlainText  onPlainText;
		
				public Anonymous_C2(Consumer paramouter_Consumer,
						EncryptedContent cKeyEncryptedContent_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1,
						Name dKeyName_2, Consumer.OnPlainText  onPlainText_3) {
					this.cKeyEncryptedContent = cKeyEncryptedContent_0;
					this.onError = onError_1;
					this.dKeyName = dKeyName_2;
					this.onPlainText = onPlainText_3;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onData(Interest dKeyInterest, Data dKeyData) {
					// The Interest has no selectors, so assume the library correctly
					// matched with the Data name before calling onData.
		
					try {
						outer_Consumer.keyChain_.verifyData(dKeyData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C2.Anonymous_C9 (this), new net.named_data.jndn.encrypt.Consumer.Anonymous_C2.Anonymous_C8 (this));
					} catch (SecurityException ex) {
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.SecurityException,
									"verifyData error: " + ex.Message);
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError",
									exception);
						}
					}
				}
			}
	
		public sealed class Anonymous_C1 : OnTimeout {
				public sealed class Anonymous_C7 : OnTimeout {
								private readonly Consumer.Anonymous_C1  outer_Anonymous_C1;
					
								public Anonymous_C7(Consumer.Anonymous_C1  paramouter_Anonymous_C1) {
									this.outer_Anonymous_C1 = paramouter_Anonymous_C1;
								}
					
								public void onTimeout(
										Interest contentInterest) {
									try {
										outer_Anonymous_C1.onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.Timeout,
												outer_Anonymous_C1.interest.getName().toUri());
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
												"Error in onError", ex);
									}
								}
							}
		
				private readonly Consumer outer_Consumer;
				internal readonly Interest interest;
				private readonly OnData onData;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
		
				public Anonymous_C1(Consumer paramouter_Consumer, Interest interest_0,
						OnData onData_1, net.named_data.jndn.encrypt.EncryptError.OnError  onError_2) {
					this.interest = interest_0;
					this.onData = onData_1;
					this.onError = onError_2;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onTimeout(Interest dKeyInterest) {
					// We should re-try at least once.
					try {
						outer_Consumer.face_.expressInterest(interest, onData,
								new net.named_data.jndn.encrypt.Consumer.Anonymous_C1.Anonymous_C7 (this));
					} catch (IOException ex) {
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.IOException,
									"expressInterest error: " + ex.Message);
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError",
									exception);
						}
					}
				}
			}
	
		public sealed class Anonymous_C0 : Consumer.OnPlainText {
			private readonly Consumer.OnPlainText  callerOnPlainText;
			private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
			private readonly Blob encryptedPayloadBlob;
	
			public Anonymous_C0(Consumer.OnPlainText  callerOnPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1,
					Blob encryptedPayloadBlob_2) {
				this.callerOnPlainText = callerOnPlainText_0;
				this.onError = onError_1;
				this.encryptedPayloadBlob = encryptedPayloadBlob_2;
			}
	
			public void onPlainText(Blob nonceKeyBits) {
				decrypt(encryptedPayloadBlob, nonceKeyBits, callerOnPlainText,
						onError);
			}
		}
	
		public interface OnConsumeComplete {
			void onConsumeComplete(Data data, Blob result);
		}
	
		/// <summary>
		/// Express an Interest to fetch the content packet with contentName, and
		/// decrypt it, fetching keys as needed.
		/// </summary>
		///
		/// <param name="contentName">The name of the content packet.</param>
		/// <param name="onConsumeComplete_0">contentData is the fetched Data packet and result is the decrypted plain text Blob. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onError_1">better error handling the callback should catch and properly handle any exceptions.</param>
		public void consume(Name contentName,
				Consumer.OnConsumeComplete  onConsumeComplete_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			Interest interest_2 = new Interest(contentName);
	
			// Prepare the callback functions.
			OnData onData_3 = new Consumer.Anonymous_C6 (this, onConsumeComplete_0, onError_1);
	
			OnTimeout onTimeout = new Consumer.Anonymous_C5 (this, onError_1, interest_2, onData_3);
	
			// Express the Interest.
			try {
				face_.expressInterest(interest_2, onData_3, onTimeout);
			} catch (IOException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.IOException,
							"expressInterest error: " + ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
			}
		}
	
		/// <summary>
		/// Set the group name.
		/// </summary>
		///
		/// <param name="groupName"></param>
		public void setGroup(Name groupName) {
			groupName_ = new Name(groupName);
		}
	
		/// <summary>
		/// Add a new decryption key with keyName and keyBlob to the database.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <param name="keyBlob">The encoded key.</param>
		/// <exception cref="ConsumerDb.Error">if a key with the same keyName already exists inthe database, or other database error.</exception>
		/// <exception cref="System.Exception">if the consumer name is not a prefix of the key name.</exception>
		public void addDecryptionKey(Name keyName, Blob keyBlob) {
			if (!(consumerName_.match(keyName)))
				throw new Exception(
						"addDecryptionKey: The consumer name must be a prefix of the key name");
	
			database_.addKey(keyName, keyBlob);
		}
	
		public interface OnPlainText {
			void onPlainText(Blob plainText);
		}
	
		/// <summary>
		/// Decode encryptedBlob as an EncryptedContent and decrypt using keyBits.
		/// </summary>
		///
		/// <param name="encryptedBlob">The encoded EncryptedContent to decrypt.</param>
		/// <param name="keyBits">The key value.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		private static void decrypt(Blob encryptedBlob, Blob keyBits,
				Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			EncryptedContent encryptedContent = new EncryptedContent();
			try {
				encryptedContent.wireDecode(encryptedBlob);
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
	
			decrypt(encryptedContent, keyBits, onPlainText_0, onError_1);
		}
	
		/// <summary>
		/// Decrypt encryptedContent using keyBits.
		/// </summary>
		///
		/// <param name="encryptedContent">The EncryptedContent to decrypt.</param>
		/// <param name="keyBits">The key value.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		static internal void decrypt(EncryptedContent encryptedContent,
				Blob keyBits, Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			Blob payload = encryptedContent.getPayload();
	
			if (encryptedContent.getAlgorithmType() == net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc) {
				// Prepare the parameters.
				EncryptParams decryptParams = new EncryptParams(
						net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc);
				decryptParams.setInitialVector(encryptedContent.getInitialVector());
	
				// Decrypt the content.
				Blob content;
				try {
					content = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(keyBits, payload, decryptParams);
				} catch (Exception ex) {
					try {
						onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
								ex.Message);
					} catch (Exception exception) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
					}
					return;
				}
				try {
					onPlainText_0.onPlainText(content);
				} catch (Exception ex_2) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onPlainText", ex_2);
				}
			} else if (encryptedContent.getAlgorithmType() == net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep) {
				// Prepare the parameters.
				EncryptParams decryptParams_3 = new EncryptParams(
						net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep);
	
				// Decrypt the content.
				Blob content_4;
				try {
					content_4 = net.named_data.jndn.encrypt.algo.RsaAlgorithm.decrypt(keyBits, payload, decryptParams_3);
				} catch (Exception ex_5) {
					try {
						onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
								ex_5.Message);
					} catch (Exception exception_6) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_6);
					}
					return;
				}
				try {
					onPlainText_0.onPlainText(content_4);
				} catch (Exception ex_7) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onPlainText", ex_7);
				}
			} else {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.UnsupportedEncryptionScheme,
							encryptedContent.getAlgorithmType().toString());
				} catch (Exception ex_8) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", ex_8);
				}
			}
		}
	
		/// <summary>
		/// Decrypt the data packet.
		/// </summary>
		///
		/// <param name="data">The data packet. This does not verify the packet.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		internal void decryptContent(Data data, Consumer.OnPlainText  onPlainText_0,
				net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			// Get the encrypted content.
			EncryptedContent dataEncryptedContent_2 = new EncryptedContent();
			try {
				dataEncryptedContent_2.wireDecode(data.getContent());
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
			Name cKeyName_3 = dataEncryptedContent_2.getKeyLocator().getKeyName();
	
			// Check if the content key is already in the store.
			Blob cKey = (Blob) ILOG.J2CsMapping.Collections.Collections.Get(cKeyMap_,cKeyName_3);
			if (cKey != null)
				decrypt(dataEncryptedContent_2, cKey, onPlainText_0, onError_1);
			else {
				// Retrieve the C-KEY Data from the network.
				Name interestName = new Name(cKeyName_3);
				interestName.append(net.named_data.jndn.encrypt.algo.Encryptor.NAME_COMPONENT_FOR)
						.append(groupName_);
				Interest interest_4 = new Interest(interestName);
	
				// Prepare the callback functions.
				OnData onData_5 = new Consumer.Anonymous_C4 (this, onError_1, onPlainText_0, cKeyName_3,
						dataEncryptedContent_2);
	
				OnTimeout onTimeout = new Consumer.Anonymous_C3 (this, onData_5, onError_1, interest_4);
	
				// Express the Interest.
				try {
					face_.expressInterest(interest_4, onData_5, onTimeout);
				} catch (IOException ex_6) {
					try {
						onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.IOException,
								"expressInterest error: " + ex_6.Message);
					} catch (Exception exception_7) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_7);
					}
				}
			}
		}
	
		/// <summary>
		/// Decrypt cKeyData.
		/// </summary>
		///
		/// <param name="cKeyData">The C-KEY data packet.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		internal void decryptCKey(Data cKeyData, Consumer.OnPlainText  onPlainText_0,
				net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			// Get the encrypted content.
			Blob cKeyContent = cKeyData.getContent();
			EncryptedContent cKeyEncryptedContent_2 = new EncryptedContent();
			try {
				cKeyEncryptedContent_2.wireDecode(cKeyContent);
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
			Name eKeyName = cKeyEncryptedContent_2.getKeyLocator().getKeyName();
			Name dKeyName_3 = eKeyName.getPrefix(-3);
			dKeyName_3.append(net.named_data.jndn.encrypt.algo.Encryptor.NAME_COMPONENT_D_KEY).append(
					eKeyName.getSubName(-2));
	
			// Check if the decryption key is already in the store.
			Blob dKey = (Blob) ILOG.J2CsMapping.Collections.Collections.Get(dKeyMap_,dKeyName_3);
			if (dKey != null)
				decrypt(cKeyEncryptedContent_2, dKey, onPlainText_0, onError_1);
			else {
				// Get the D-Key Data.
				Name interestName = new Name(dKeyName_3);
				interestName.append(net.named_data.jndn.encrypt.algo.Encryptor.NAME_COMPONENT_FOR).append(
						consumerName_);
				Interest interest_4 = new Interest(interestName);
	
				// Prepare the callback functions.
				OnData onData_5 = new Consumer.Anonymous_C2 (this, cKeyEncryptedContent_2, onError_1, dKeyName_3,
						onPlainText_0);
	
				OnTimeout onTimeout = new Consumer.Anonymous_C1 (this, interest_4, onData_5, onError_1);
	
				// Express the Interest.
				try {
					face_.expressInterest(interest_4, onData_5, onTimeout);
				} catch (IOException ex_6) {
					try {
						onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.IOException,
								"expressInterest error: " + ex_6.Message);
					} catch (Exception exception_7) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_7);
					}
				}
			}
		}
	
		/// <summary>
		/// Decrypt dKeyData.
		/// </summary>
		///
		/// <param name="dKeyData">The D-KEY data packet.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		internal void decryptDKey(Data dKeyData, Consumer.OnPlainText  onPlainText_0,
				net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			// Get the encrypted content.
			Blob dataContent = dKeyData.getContent();
	
			// Process the nonce.
			// dataContent is a sequence of the two EncryptedContent.
			EncryptedContent encryptedNonce = new EncryptedContent();
			try {
				encryptedNonce.wireDecode(dataContent);
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
			Name consumerKeyName = encryptedNonce.getKeyLocator().getKeyName();
	
			// Get consumer decryption key.
			Blob consumerKeyBlob;
			try {
				consumerKeyBlob = getDecryptionKey(consumerKeyName);
			} catch (ConsumerDb.Error ex_2) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.NoDecryptKey,
							"Database error: " + ex_2.Message);
				} catch (Exception exception_3) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_3);
				}
				return;
			}
			if (consumerKeyBlob.size() == 0) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.NoDecryptKey,
							"The desired consumer decryption key in not in the database");
				} catch (Exception exception_4) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_4);
				}
				return;
			}
	
			// Process the D-KEY.
			// Use the size of encryptedNonce to find the start of encryptedPayload.
			ByteBuffer encryptedPayloadBuffer = dataContent.buf().duplicate();
			encryptedPayloadBuffer.position(encryptedNonce.wireEncode().size());
			Blob encryptedPayloadBlob_5 = new Blob(encryptedPayloadBuffer,
					false);
			if (encryptedPayloadBlob_5.size() == 0) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							"The data packet does not satisfy the D-KEY packet format");
				} catch (Exception ex_6) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", ex_6);
				}
				return;
			}
	
			// Decrypt the D-KEY.
			Consumer.OnPlainText  callerOnPlainText_7 = onPlainText_0;
			decrypt(encryptedNonce, consumerKeyBlob, new Consumer.Anonymous_C0 (callerOnPlainText_7, onError_1, encryptedPayloadBlob_5), onError_1);
		}
	
		/// <summary>
		/// Get the encoded blob of the decryption key with decryptionKeyName from the
		/// database.
		/// </summary>
		///
		/// <param name="decryptionKeyName">The key name.</param>
		/// <returns>A Blob with the encoded key, or an isNull Blob if cannot find the
		/// key with decryptionKeyName.</returns>
		/// <exception cref="ConsumerDb.Error">for a database error.</exception>
		private Blob getDecryptionKey(Name decryptionKeyName) {
			return database_.getKey(decryptionKeyName);
		}
	
		/// <summary>
		/// A class implements Friend if it has a method setConsumerFriendAccess
		/// which setFriendAccess calls to set the FriendAccess object.
		/// </summary>
		///
		public interface Friend {
			void setConsumerFriendAccess(Consumer.FriendAccess  friendAccess);
		}
	
		/// <summary>
		/// Call friend.setConsumerFriendAccess to pass an instance of
		/// a FriendAccess class to allow a friend class to call private methods.
		/// </summary>
		///
		/// <param name="friend">Therefore, only a friend class gets an implementation of FriendAccess.</param>
		public static void setFriendAccess(Consumer.Friend  friend) {
			if (friend
							.GetType().FullName
					.equals("src.net.named_data.jndn.tests.integration_tests.TestGroupConsumer")) {
				friend.setConsumerFriendAccess(new Consumer.FriendAccessImpl ());
			}
		}
	
		/// <summary>
		/// A friend class can call the methods of FriendAccess to access private
		/// methods.  This abstract class is public, but setFriendAccess passes an
		/// instance of a private class which implements the methods.
		/// </summary>
		///
		public abstract class FriendAccess {
			public abstract void decrypt(Blob encryptedBlob, Blob keyBits,
					Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1);
		}
	
		/// <summary>
		/// setFriendAccess passes an instance of this private class which implements
		/// the FriendAccess methods.
		/// </summary>
		///
		private class FriendAccessImpl : Consumer.FriendAccess  {
			public override void decrypt(Blob encryptedBlob, Blob keyBits,
					Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
				net.named_data.jndn.encrypt.Consumer.decrypt(encryptedBlob, keyBits, onPlainText_0, onError_1);
			}
		}
	
		private readonly ConsumerDb database_;
		internal readonly KeyChain keyChain_;
		internal readonly Face face_;
		private Name groupName_;
		private readonly Name consumerName_;
		// Use HashMap without generics so it works with older Java compilers.
		internal readonly Hashtable cKeyMap_;
		/// <summary>
		/// < The map key is the C-KEY name. The value is the encoded key Blob. 
		/// </summary>
		///
		internal readonly Hashtable dKeyMap_;
		/// <summary>
		/// < The map key is the D-KEY name. The value is the encoded key Blob. 
		/// </summary>
		///
		static internal readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(Consumer).FullName);
	}
}
