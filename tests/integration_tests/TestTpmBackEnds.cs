// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.tests.integration_tests {
	
	using ILOG.J2CsMapping.Collections;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.spec;
	using javax.crypto;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encrypt.algo;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.tpm;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	public class TestTpmBackEnds {
		public TestTpmBackEnds() {
			this.backEndList = new TpmBackEnd[2];
		}
	
		internal TpmBackEndMemory backEndMemory;
		internal TpmBackEndFile backEndFile;
	
		internal TpmBackEnd[] backEndList;
	
		public void setUp() {
			backEndMemory = new TpmBackEndMemory();
	
			FileInfo locationPath = new FileInfo(System.IO.Path.Combine(net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon.getPolicyConfigDirectory().FullName,"ndnsec-key-file"));
			if (locationPath.Exists) {
				/* foreach */
				// Delete files from a previous test.
				foreach (FileInfo file  in  locationPath.listFiles())
					file.delete();
			}
			backEndFile = new TpmBackEndFile(locationPath.FullName);
	
			backEndList[0] = backEndMemory;
			backEndList[1] = backEndFile;
		}
	
		public void tearDown() {
		}
	
		public void testKeyManagement() {
			/* foreach */
			foreach (TpmBackEnd tpm  in  backEndList) {
				Name identityName = new Name("/Test/KeyName");
				Name.Component keyId = new Name.Component("1");
				Name keyName = net.named_data.jndn.security.pib.PibKey.constructKeyName(identityName, keyId);
	
				// The key should not exist.
				Assert.AssertEquals(false, tpm.hasKey(keyName));
				Assert.AssertTrue(tpm.getKeyHandle(keyName) == null);
	
				// Create a key, which should exist.
				Assert.AssertTrue(tpm.createKey(identityName, new RsaKeyParams(keyId)) != null);
				Assert.AssertTrue(tpm.hasKey(keyName));
				Assert.AssertTrue(tpm.getKeyHandle(keyName) != null);
	
				// Create a key with the same name, which should throw an error.
				try {
					tpm.createKey(identityName, new RsaKeyParams(keyId));
					Assert.Fail("Did not throw the expected exception");
				} catch (Tpm.Error ex) {
				} catch (Exception ex_0) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Delete the key, then it should not exist.
				tpm.deleteKey(keyName);
				Assert.AssertEquals(false, tpm.hasKey(keyName));
				Assert.AssertTrue(tpm.getKeyHandle(keyName) == null);
			}
		}
	
		public void testRsaSigning() {
			/* foreach */
			foreach (TpmBackEnd tpm  in  backEndList) {
				// Create an RSA key.
				Name identityName = new Name("/Test/KeyName");
	
				TpmKeyHandle key = tpm.createKey(identityName, new RsaKeyParams());
				Name keyName = key.getKeyName();
	
				Blob content = new Blob(new int[] { 0x01, 0x02, 0x03, 0x04 });
				Blob signature = key.sign(net.named_data.jndn.security.DigestAlgorithm.SHA256, content.buf());
	
				Blob publicKey = key.derivePublicKey();
	
				bool result = net.named_data.jndn.security.VerificationHelpers.verifySignature(content,
						signature, publicKey);
				Assert.AssertEquals(true, result);
	
				tpm.deleteKey(keyName);
				Assert.AssertEquals(false, tpm.hasKey(keyName));
			}
		}
	
		public void testRsaDecryption() {
			/* foreach */
			foreach (TpmBackEnd tpm  in  backEndList) {
				// Create an rsa key.
				Name identityName = new Name("/Test/KeyName");
	
				TpmKeyHandle key = tpm.createKey(identityName, new RsaKeyParams());
				Name keyName = key.getKeyName();
	
				Blob content = new Blob(new int[] { 0x01, 0x02, 0x03, 0x04 });
	
				Blob publicKey = key.derivePublicKey();
	
				// TODO: Move encrypt to PublicKey?
				Blob cipherText = net.named_data.jndn.encrypt.algo.RsaAlgorithm.encrypt(publicKey, content,
						new EncryptParams(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep));
	
				Blob plainText = key.decrypt(cipherText.buf());
	
				Assert.AssertTrue(plainText.equals(content));
	
				tpm.deleteKey(keyName);
				Assert.AssertEquals(false, tpm.hasKey(keyName));
			}
		}
	
		/* Debug: derivePublicKey for EC is not implemented.
		  @Test
		  public void
		  testEcdsaSigning() throws TpmBackEnd.Error, Tpm.Error, SecurityException
		  {
		    for (TpmBackEnd tpm : backEndList) {
		      // Create an EC key.
		      Name identityName = new Name("/Test/Ec/KeyName");
	
		      TpmKeyHandle key = tpm.createKey(identityName, new EcdsaKeyParams());
		      Name ecKeyName = key.getKeyName();
	
		      Blob content = new Blob(new int[] { 0x01, 0x02, 0x03, 0x04});
		      Blob signature = key.sign(DigestAlgorithm.SHA256, content.buf());
	
		      Blob publicKey = key.derivePublicKey();
	
		      // TODO: Move verify to PublicKey?
		      boolean result = VerificationHelpers.verifySignature
		        (content, signature, publicKey);
		      assertEquals(true, result);
	
		      tpm.deleteKey(ecKeyName);
		      assertEquals(false, tpm.hasKey(ecKeyName));
		    }
		  }
		*/
	
		// TODO: ImportExport
	
		public void testRandomKeyId() {
			TpmBackEnd tpm = backEndMemory;
	
			Name identityName = new Name("/Test/KeyName");
	
			HashedSet<Name> keyNames = new HashedSet<Name>();
			for (int i = 0; i < 100; i++) {
				TpmKeyHandle key = tpm.createKey(identityName, new RsaKeyParams());
				Name keyName = key.getKeyName();
				Assert.AssertTrue(ILOG.J2CsMapping.Collections.Collections.Add(keyNames,keyName));
			}
		}
	}
}
