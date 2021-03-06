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
namespace net.named_data.jndn.tests.integration_tests {
	
	using ILOG.J2CsMapping.NIO;
	using ILOG.J2CsMapping.Threading;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using javax.crypto;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encrypt;
	using net.named_data.jndn.in_memory_storage;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.util;
	
	public class TestEncryptorV2 {
		internal TestEncryptorV2.EncryptorFixture  fixture_;
	
		public sealed class Anonymous_C1 : net.named_data.jndn.encrypt.EncryptError.OnError  {
			public void onError(EncryptError.ErrorCode errorCode, String message) {
				Assert.Fail("onError: " + message);
			}
		}
	
		public sealed class Anonymous_C0 : net.named_data.jndn.encrypt.EncryptError.OnError  {
			private readonly int[] nErrors;
	
			public Anonymous_C0(int[] nErrors_0) {
				this.nErrors = nErrors_0;
			}
	
			public void onError(EncryptError.ErrorCode errorCode, String message) {
				++nErrors[0];
			}
		}
	
		internal class EncryptorFixture : IdentityManagementFixture {
			public EncryptorFixture(bool shouldPublishData, net.named_data.jndn.encrypt.EncryptError.OnError  onError) {
				this.storage_ = new InMemoryStorageRetaining();
				// Include the code here from the NAC unit-tests class
				// EncryptorStaticDataEnvironment instead of making it a base class.
				if (shouldPublishData)
					publishData();
	
				face_ = new InMemoryStorageFace(storage_);
				validator_ = new ValidatorNull();
				encryptor_ = new EncryptorV2(new Name(
						"/access/policy/identity/NAC/dataset"), new Name(
						"/some/ck/prefix"), new SigningInfo(
						net.named_data.jndn.security.SigningInfo.SignerType.SHA256), onError, validator_,
						keyChain_, face_);
			}
	
			public void publishData() {
				/* foreach */
				foreach (ByteBuffer buffer  in  net.named_data.jndn.tests.integration_tests.EncryptStaticData.managerPackets) {
					Data data = new Data();
					data.wireDecode(buffer);
					storage_.insert(data);
				}
			}
	
			public readonly InMemoryStorageRetaining storage_;
			public readonly InMemoryStorageFace face_;
			public readonly ValidatorNull validator_;
			public readonly EncryptorV2 encryptor_;
		}
	
		public void setUp() {
			// Turn off INFO log messages.
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger("").setLevel(ILOG.J2CsMapping.Util.Logging.Level.SEVERE);
	
			fixture_ = new TestEncryptorV2.EncryptorFixture (true, new TestEncryptorV2.Anonymous_C1 ());
		}
	
		public void testEncryptAndPublishCk() {
			fixture_.encryptor_.clearKekData_();
			Assert.AssertEquals(false, fixture_.encryptor_.getIsKekRetrievalInProgress_());
			fixture_.encryptor_.regenerateCk();
			// Unlike the ndn-group-encrypt unit tests, we don't check
			// isKekRetrievalInProgress_ true because we use a synchronous face which
			// finishes immediately.
	
			Blob plainText = new Blob("Data to encrypt");
			EncryptedContent encryptedContent = fixture_.encryptor_
					.encrypt(plainText.getImmutableArray());
	
			Name ckPrefix = encryptedContent.getKeyLocatorName();
			Assert.AssertTrue(new Name("/some/ck/prefix/CK")
					.equals(ckPrefix.getPrefix(-1)));
	
			Assert.AssertTrue(encryptedContent.hasInitialVector());
			Assert.AssertTrue(!encryptedContent.getPayload().equals(plainText));
	
			// Check that the KEK Interest has been sent.
			Assert.AssertTrue(fixture_.face_.sentInterests_[0].getName().getPrefix(6)
					.equals(new Name("/access/policy/identity/NAC/dataset/KEK")));
	
			Data kekData = fixture_.face_.sentData_[0];
			Assert.AssertTrue(kekData.getName().getPrefix(6)
					.equals(new Name("/access/policy/identity/NAC/dataset/KEK")));
			Assert.AssertEquals(7, kekData.getName().size());
	
			ILOG.J2CsMapping.Collections.Collections.Clear(fixture_.face_.sentData_);
			ILOG.J2CsMapping.Collections.Collections.Clear(fixture_.face_.sentInterests_);
	
			fixture_.face_.receive(new Interest(ckPrefix).setCanBePrefix(true)
					.setMustBeFresh(true));
	
			Name ckName = fixture_.face_.sentData_[0].getName();
			Assert.AssertTrue(ckName.getPrefix(4).equals(new Name("/some/ck/prefix/CK")));
			Assert.AssertTrue(ckName.get(5).equals(new Name.Component("ENCRYPTED-BY")));
	
			Name extractedKek = ckName.getSubName(6);
			Assert.AssertTrue(extractedKek.equals(kekData.getName()));
	
			Assert.AssertEquals(false, fixture_.encryptor_.getIsKekRetrievalInProgress_());
		}
	
		public void testKekRetrievalFailure() {
			int[] nErrors_0 = new int[] { 0 };
			fixture_ = new TestEncryptorV2.EncryptorFixture (false, new TestEncryptorV2.Anonymous_C0 (nErrors_0));
	
			Blob plainText = new Blob("Data to encrypt");
			EncryptedContent encryptedContent = fixture_.encryptor_
					.encrypt(plainText.getImmutableArray());
	
			// Check that KEK interests has been sent.
			Assert.AssertTrue(fixture_.face_.sentInterests_[0].getName().getPrefix(6)
					.equals(new Name("/access/policy/identity/NAC/dataset/KEK")));
	
			// ... and failed to retrieve.
			Assert.AssertEquals(0, fixture_.face_.sentData_.Count);
	
			Assert.AssertEquals(1, nErrors_0[0]);
			Assert.AssertEquals(0, fixture_.face_.sentData_.Count);
	
			// Check recovery.
			fixture_.publishData();
	
			fixture_.face_.delayedCallTable_.setNowOffsetMilliseconds_(73000);
			fixture_.face_.processEvents();
	
			Data kekData = fixture_.face_.sentData_[0];
			Assert.AssertTrue(kekData.getName().getPrefix(6)
					.equals(new Name("/access/policy/identity/NAC/dataset/KEK")));
			Assert.AssertEquals(7, kekData.getName().size());
		}
	
		public void testEnumerateDataFromInMemoryStorage() {
			ILOG.J2CsMapping.Threading.ThreadWrapper.sleep(200);
			fixture_.encryptor_.regenerateCk();
			ILOG.J2CsMapping.Threading.ThreadWrapper.sleep(200);
			fixture_.encryptor_.regenerateCk();
	
			Assert.AssertEquals(3, fixture_.encryptor_.size());
			int nCk = 0;
			/* foreach */
			foreach (Object data  in  fixture_.encryptor_.getCache_().Values) {
				if (((Data) data).getName().getPrefix(4)
						.equals(new Name("/some/ck/prefix/CK")))
					++nCk;
			}
			Assert.AssertEquals(3, nCk);
		}
	}
}
