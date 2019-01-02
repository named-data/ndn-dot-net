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
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	public class TestPibImpl {
		public TestPibImpl() {
			this.pibImpls = new PibDataFixture2[2];
		}
	
		internal class PibMemoryFixture : PibDataFixture2 {
			public PibMemoryFixture() {
				this.myPib_ = new PibMemory();
				pib = myPib_;
			}
	
			private readonly PibMemory myPib_;
		} 
	
		public class PibSqlite3Fixture : PibDataFixture2 {
				private TestPibImpl outer_TestPibImpl;
		
				public PibSqlite3Fixture(TestPibImpl impl) {
					outer_TestPibImpl = impl;
					FileInfo databaseDirectoryPath = net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon
							.getPolicyConfigDirectory();
					String databaseFilename = "test-pib.db";
					outer_TestPibImpl.databaseFilePath = new FileInfo(System.IO.Path.Combine(databaseDirectoryPath.FullName,databaseFilename));
					outer_TestPibImpl.databaseFilePath.delete();
		
					myPib_ = new PibSqlite3(databaseDirectoryPath.FullName,
							databaseFilename);
		
					pib = myPib_;
				}
		
				private readonly PibSqlite3 myPib_;
			} 
	
		internal TestPibImpl.PibMemoryFixture  pibMemoryFixture;
		internal TestPibImpl.PibSqlite3Fixture  pibSqlite3Fixture;
	
		internal PibDataFixture2[] pibImpls;
	
		public void setUp() {
			pibMemoryFixture = new TestPibImpl.PibMemoryFixture ();
			pibSqlite3Fixture = new TestPibImpl.PibSqlite3Fixture (this);
	
			pibImpls[0] = pibMemoryFixture;
			pibImpls[1] = pibSqlite3Fixture;
		}
	
		public void tearDown() {
			databaseFilePath.delete();
		}
	
		public void testCertificateDecoding() {
			// Use pibMemoryFixture to test.
			PibDataFixture2 fixture = pibMemoryFixture;
	
			Assert.AssertTrue(fixture.id1Key1Cert1.getPublicKey().equals(
					fixture.id1Key1Cert2.getPublicKey()));
			Assert.AssertTrue(fixture.id1Key2Cert1.getPublicKey().equals(
					fixture.id1Key2Cert2.getPublicKey()));
			Assert.AssertTrue(fixture.id2Key1Cert1.getPublicKey().equals(
					fixture.id2Key1Cert2.getPublicKey()));
			Assert.AssertTrue(fixture.id2Key2Cert1.getPublicKey().equals(
					fixture.id2Key2Cert2.getPublicKey()));
	
			Assert.AssertTrue(fixture.id1Key1Cert1.getPublicKey().equals(fixture.id1Key1));
			Assert.AssertTrue(fixture.id1Key1Cert2.getPublicKey().equals(fixture.id1Key1));
			Assert.AssertTrue(fixture.id1Key2Cert1.getPublicKey().equals(fixture.id1Key2));
			Assert.AssertTrue(fixture.id1Key2Cert2.getPublicKey().equals(fixture.id1Key2));
	
			Assert.AssertTrue(fixture.id2Key1Cert1.getPublicKey().equals(fixture.id2Key1));
			Assert.AssertTrue(fixture.id2Key1Cert2.getPublicKey().equals(fixture.id2Key1));
			Assert.AssertTrue(fixture.id2Key2Cert1.getPublicKey().equals(fixture.id2Key2));
			Assert.AssertTrue(fixture.id2Key2Cert2.getPublicKey().equals(fixture.id2Key2));
	
			Assert.AssertTrue(fixture.id1Key1Cert2.getIdentity().equals(fixture.id1));
			Assert.AssertTrue(fixture.id1Key2Cert1.getIdentity().equals(fixture.id1));
			Assert.AssertTrue(fixture.id1Key2Cert2.getIdentity().equals(fixture.id1));
	
			Assert.AssertTrue(fixture.id2Key1Cert2.getIdentity().equals(fixture.id2));
			Assert.AssertTrue(fixture.id2Key2Cert1.getIdentity().equals(fixture.id2));
			Assert.AssertTrue(fixture.id2Key2Cert2.getIdentity().equals(fixture.id2));
	
			Assert.AssertTrue(fixture.id1Key1Cert2.getKeyName()
					.equals(fixture.id1Key1Name));
			Assert.AssertTrue(fixture.id1Key2Cert2.getKeyName()
					.equals(fixture.id1Key2Name));
	
			Assert.AssertTrue(fixture.id2Key1Cert2.getKeyName()
					.equals(fixture.id2Key1Name));
			Assert.AssertTrue(fixture.id2Key2Cert2.getKeyName()
					.equals(fixture.id2Key2Name));
		}
	
		public void testTpmLocator() {
			/* foreach */
			foreach (PibDataFixture2 fixture  in  pibImpls) {
				PibImpl pib = fixture.pib;
	
				// Basic getting and setting
				try {
					pib.getTpmLocator();
				} catch (Exception ex) {
					Assert.Fail("Unexpected exception: " + ex.Message);
				}
	
				try {
					pib.setTpmLocator("tpmLocator");
				} catch (Exception ex_0) {
					Assert.Fail("Unexpected exception: " + ex_0.Message);
				}
				Assert.AssertEquals(pib.getTpmLocator(), "tpmLocator");
	
				// Add a certificate, and do not change the TPM locator.
				pib.addCertificate(fixture.id1Key1Cert1);
				Assert.AssertTrue(pib.hasIdentity(fixture.id1));
				Assert.AssertTrue(pib.hasKey(fixture.id1Key1Name));
				Assert.AssertTrue(pib.hasCertificate(fixture.id1Key1Cert1.getName()));
	
				// Set the TPM locator to the same value. Nothing should change.
				pib.setTpmLocator("tpmLocator");
				Assert.AssertTrue(pib.hasIdentity(fixture.id1));
				Assert.AssertTrue(pib.hasKey(fixture.id1Key1Name));
				Assert.AssertTrue(pib.hasCertificate(fixture.id1Key1Cert1.getName()));
	
				// Change the TPM locator. (The contents of the PIB should not change.)
				pib.setTpmLocator("newTpmLocator");
				Assert.AssertTrue(pib.hasIdentity(fixture.id1));
				Assert.AssertTrue(pib.hasKey(fixture.id1Key1Name));
				Assert.AssertTrue(pib.hasCertificate(fixture.id1Key1Cert1.getName()));
			}
		}
	
		public void testIdentityManagement() {
			/* foreach */
			foreach (PibDataFixture2 fixture  in  pibImpls) {
				PibImpl pib = fixture.pib;
	
				// No default identity is set. This should throw an Error.
				try {
					pib.getDefaultIdentity();
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex) {
				} catch (Exception ex_0) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Check for id1, which should not exist.
				Assert.AssertEquals(false, pib.hasIdentity(fixture.id1));
	
				// Add id1, which should be the default.
				pib.addIdentity(fixture.id1);
				Assert.AssertEquals(true, pib.hasIdentity(fixture.id1));
				try {
					pib.getDefaultIdentity();
				} catch (Exception ex_1) {
					Assert.Fail("Unexpected exception: " + ex_1.Message);
				}
				Assert.AssertEquals(fixture.id1, pib.getDefaultIdentity());
	
				// Add id2, which should not be the default.
				pib.addIdentity(fixture.id2);
				Assert.AssertEquals(true, pib.hasIdentity(fixture.id2));
				Assert.AssertEquals(fixture.id1, pib.getDefaultIdentity());
	
				// Explicitly set id2 as the default.
				pib.setDefaultIdentity(fixture.id2);
				Assert.AssertEquals(fixture.id2, pib.getDefaultIdentity());
	
				// Remove id2. The PIB should not have a default identity.
				pib.removeIdentity(fixture.id2);
				Assert.AssertEquals(false, pib.hasIdentity(fixture.id2));
				try {
					pib.getDefaultIdentity();
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_2) {
				} catch (Exception ex_3) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Set id2 as the default. This should add id2 again.
				pib.setDefaultIdentity(fixture.id2);
				Assert.AssertEquals(fixture.id2, pib.getDefaultIdentity());
	
				// Get all the identities, which should have id1 and id2.
				HashedSet<Name> idNames = pib.getIdentities();
				Assert.AssertEquals(2, idNames.Count);
				Assert.AssertTrue(ILOG.J2CsMapping.Collections.Collections.Contains(fixture.id1,idNames));
				Assert.AssertTrue(ILOG.J2CsMapping.Collections.Collections.Contains(fixture.id2,idNames));
			}
		}
	
		public void testClearIdentities() {
			/* foreach */
			foreach (PibDataFixture2 fixture  in  pibImpls) {
				PibImpl pib = fixture.pib;
	
				pib.setTpmLocator("tpmLocator");
	
				// Add id, key, and cert.
				pib.addCertificate(fixture.id1Key1Cert1);
				Assert.AssertTrue(pib.hasIdentity(fixture.id1));
				Assert.AssertTrue(pib.hasKey(fixture.id1Key1Name));
				Assert.AssertTrue(pib.hasCertificate(fixture.id1Key1Cert1.getName()));
	
				// Clear identities.
				pib.clearIdentities();
				Assert.AssertEquals(0, pib.getIdentities().Count);
				Assert.AssertEquals(0, pib.getKeysOfIdentity(fixture.id1).Count);
				Assert.AssertEquals(0, pib.getCertificatesOfKey(fixture.id1Key1Name).Count);
				Assert.AssertEquals("tpmLocator", pib.getTpmLocator());
			}
		}
	
		public void testKeyManagement() {
			/* foreach */
			foreach (PibDataFixture2 fixture  in  pibImpls) {
				PibImpl pib = fixture.pib;
	
				// There is no default setting. This should throw an Error.
				Assert.AssertEquals(false, pib.hasIdentity(fixture.id2));
				try {
					pib.getDefaultKeyOfIdentity(fixture.id1);
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex) {
				} catch (Exception ex_0) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Check for id1Key1, which should not exist. Neither should id1.
				Assert.AssertEquals(false, pib.hasKey(fixture.id1Key1Name));
				Assert.AssertEquals(false, pib.hasIdentity(fixture.id1));
	
				// Add id1Key1, which should be the default. id1 should be added implicitly.
				pib.addKey(fixture.id1, fixture.id1Key1Name, fixture.id1Key1.buf());
				Assert.AssertEquals(true, pib.hasKey(fixture.id1Key1Name));
				Assert.AssertEquals(true, pib.hasIdentity(fixture.id1));
				Blob keyBits = pib.getKeyBits(fixture.id1Key1Name);
				Assert.AssertTrue(keyBits.equals(fixture.id1Key1));
				try {
					pib.getDefaultKeyOfIdentity(fixture.id1);
				} catch (Exception ex_1) {
					Assert.Fail("Unexpected exception: " + ex_1.Message);
				}
				Assert.AssertEquals(fixture.id1Key1Name,
						pib.getDefaultKeyOfIdentity(fixture.id1));
	
				// Add id1Key2, which should not be the default.
				pib.addKey(fixture.id1, fixture.id1Key2Name, fixture.id1Key2.buf());
				Assert.AssertEquals(true, pib.hasKey(fixture.id1Key2Name));
				Assert.AssertEquals(fixture.id1Key1Name,
						pib.getDefaultKeyOfIdentity(fixture.id1));
	
				// Explicitly Set id1Key2 as the default.
				pib.setDefaultKeyOfIdentity(fixture.id1, fixture.id1Key2Name);
				Assert.AssertEquals(fixture.id1Key2Name,
						pib.getDefaultKeyOfIdentity(fixture.id1));
	
				// Set a non-existing key as the default. This should throw an Error.
				try {
					pib.setDefaultKeyOfIdentity(fixture.id1, new Name(
							"/non-existing"));
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_2) {
				} catch (Exception ex_3) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Remove id1Key2. The PIB should not have a default key.
				pib.removeKey(fixture.id1Key2Name);
				Assert.AssertEquals(false, pib.hasKey(fixture.id1Key2Name));
				try {
					pib.getKeyBits(fixture.id1Key2Name);
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_4) {
				} catch (Exception ex_5) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				try {
					pib.getDefaultKeyOfIdentity(fixture.id1);
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_6) {
				} catch (Exception ex_7) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Add id1Key2 back, which should be the default.
				pib.addKey(fixture.id1, fixture.id1Key2Name, fixture.id1Key2.buf());
				try {
					pib.getKeyBits(fixture.id1Key2Name);
				} catch (Exception ex_8) {
					Assert.Fail("Unexpected exception: " + ex_8.Message);
				}
				Assert.AssertEquals(fixture.id1Key2Name,
						pib.getDefaultKeyOfIdentity(fixture.id1));
	
				// Get all the keys, which should have id1Key1 and id1Key2.
				HashedSet<Name> keyNames = pib.getKeysOfIdentity(fixture.id1);
				Assert.AssertEquals(2, keyNames.Count);
				Assert.AssertTrue(ILOG.J2CsMapping.Collections.Collections.Contains(fixture.id1Key1Name,keyNames));
				Assert.AssertTrue(ILOG.J2CsMapping.Collections.Collections.Contains(fixture.id1Key2Name,keyNames));
	
				// Remove id1, which should remove all the keys.
				pib.removeIdentity(fixture.id1);
				keyNames = pib.getKeysOfIdentity(fixture.id1);
				Assert.AssertEquals(0, keyNames.Count);
			}
		}
	
		public void testCertificateManagement() {
			/* foreach */
			foreach (PibDataFixture2 fixture  in  pibImpls) {
				PibImpl pib = fixture.pib;
	
				// There is no default setting. This should throw an Error.
				try {
					pib.getDefaultCertificateOfKey(fixture.id1Key1Name);
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex) {
				} catch (Exception ex_0) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Check for id1Key1Cert1, which should not exist. Neither should id1 or id1Key1.
				Assert.AssertEquals(false,
						pib.hasCertificate(fixture.id1Key1Cert1.getName()));
				Assert.AssertEquals(false, pib.hasIdentity(fixture.id1));
				Assert.AssertEquals(false, pib.hasKey(fixture.id1Key1Name));
	
				// Add id1Key1Cert1, which should be the default.
				// id1 and id1Key1 should be added implicitly.
				pib.addCertificate(fixture.id1Key1Cert1);
				Assert.AssertEquals(true,
						pib.hasCertificate(fixture.id1Key1Cert1.getName()));
				Assert.AssertEquals(true, pib.hasIdentity(fixture.id1));
				Assert.AssertEquals(true, pib.hasKey(fixture.id1Key1Name));
				Assert.AssertTrue(pib.getCertificate(fixture.id1Key1Cert1.getName())
						.wireEncode().equals(fixture.id1Key1Cert1.wireEncode()));
				try {
					pib.getDefaultCertificateOfKey(fixture.id1Key1Name);
				} catch (Exception ex_1) {
					Assert.Fail("Unexpected exception: " + ex_1.Message);
				}
				// Use the wire encoding to check equivalence.
				Assert.AssertTrue(fixture.id1Key1Cert1.wireEncode().equals(
						pib.getDefaultCertificateOfKey(fixture.id1Key1Name)
								.wireEncode()));
	
				// Add id1Key1Cert2, which should not be the default.
				pib.addCertificate(fixture.id1Key1Cert2);
				Assert.AssertEquals(true,
						pib.hasCertificate(fixture.id1Key1Cert2.getName()));
				Assert.AssertTrue(fixture.id1Key1Cert1.wireEncode().equals(
						pib.getDefaultCertificateOfKey(fixture.id1Key1Name)
								.wireEncode()));
	
				// Explicitly set id1Key1Cert2 as the default.
				pib.setDefaultCertificateOfKey(fixture.id1Key1Name,
						fixture.id1Key1Cert2.getName());
				Assert.AssertTrue(fixture.id1Key1Cert2.wireEncode().equals(
						pib.getDefaultCertificateOfKey(fixture.id1Key1Name)
								.wireEncode()));
	
				// Set a non-existing certificate as the default. This should throw an Error.
				try {
					pib.setDefaultCertificateOfKey(fixture.id1Key1Name, new Name(
							"/non-existing"));
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_2) {
				} catch (Exception ex_3) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Remove id1Key1Cert2, which should not have a default certificate.
				pib.removeCertificate(fixture.id1Key1Cert2.getName());
				Assert.AssertEquals(false,
						pib.hasCertificate(fixture.id1Key1Cert2.getName()));
				try {
					pib.getCertificate(fixture.id1Key1Cert2.getName());
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_4) {
				} catch (Exception ex_5) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				try {
					pib.getDefaultCertificateOfKey(fixture.id1Key1Name);
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_6) {
				} catch (Exception ex_7) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				// Add id1Key1Cert2, which should be the default.
				pib.addCertificate(fixture.id1Key1Cert2);
				try {
					pib.getCertificate(fixture.id1Key1Cert1.getName());
				} catch (Exception ex_8) {
					Assert.Fail("Unexpected exception: " + ex_8.Message);
				}
				Assert.AssertTrue(fixture.id1Key1Cert2.wireEncode().equals(
						pib.getDefaultCertificateOfKey(fixture.id1Key1Name)
								.wireEncode()));
	
				// Get all certificates, which should have id1Key1Cert1 and id1Key1Cert2.
				HashedSet<Name> certNames = pib
						.getCertificatesOfKey(fixture.id1Key1Name);
				Assert.AssertEquals(2, certNames.Count);
				Assert.AssertTrue(ILOG.J2CsMapping.Collections.Collections.Contains(fixture.id1Key1Cert1.getName(),certNames));
				Assert.AssertTrue(ILOG.J2CsMapping.Collections.Collections.Contains(fixture.id1Key1Cert2.getName(),certNames));
	
				// Remove id1Key1, which should remove all the certificates.
				pib.removeKey(fixture.id1Key1Name);
				certNames = pib.getCertificatesOfKey(fixture.id1Key1Name);
				Assert.AssertEquals(0, certNames.Count);
			}
		}
	
		public void testDefaultsManagement() {
			/* foreach */
			foreach (PibDataFixture2 fixture  in  pibImpls) {
				PibImpl pib = fixture.pib;
	
				pib.addIdentity(fixture.id1);
				Assert.AssertEquals(fixture.id1, pib.getDefaultIdentity());
	
				pib.addIdentity(fixture.id2);
				Assert.AssertEquals(fixture.id1, pib.getDefaultIdentity());
	
				pib.removeIdentity(fixture.id1);
				try {
					pib.getDefaultIdentity();
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex) {
				} catch (Exception ex_0) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				pib.addKey(fixture.id2, fixture.id2Key1Name, fixture.id2Key1.buf());
				Assert.AssertEquals(fixture.id2, pib.getDefaultIdentity());
				Assert.AssertEquals(fixture.id2Key1Name,
						pib.getDefaultKeyOfIdentity(fixture.id2));
	
				pib.addKey(fixture.id2, fixture.id2Key2Name, fixture.id2Key2.buf());
				Assert.AssertEquals(fixture.id2Key1Name,
						pib.getDefaultKeyOfIdentity(fixture.id2));
	
				pib.removeKey(fixture.id2Key1Name);
				try {
					pib.getDefaultKeyOfIdentity(fixture.id2);
					Assert.Fail("Did not throw the expected exception");
				} catch (Pib.Error ex_1) {
				} catch (Exception ex_2) {
					Assert.Fail("Did not throw the expected exception");
				}
	
				pib.addCertificate(fixture.id2Key2Cert1);
				Assert.AssertEquals(fixture.id2Key2Name,
						pib.getDefaultKeyOfIdentity(fixture.id2));
				Assert.AssertEquals(fixture.id2Key2Cert1.getName(), pib
						.getDefaultCertificateOfKey(fixture.id2Key2Name).getName());
	
				pib.addCertificate(fixture.id2Key2Cert2);
				Assert.AssertEquals(fixture.id2Key2Cert1.getName(), pib
						.getDefaultCertificateOfKey(fixture.id2Key2Name).getName());
	
				pib.removeCertificate(fixture.id2Key2Cert2.getName());
				Assert.AssertEquals(fixture.id2Key2Cert1.getName(), pib
						.getDefaultCertificateOfKey(fixture.id2Key2Name).getName());
			}
		}
	
		public void testOverwrite() {
			/* foreach */
			foreach (PibDataFixture2 fixture  in  pibImpls) {
				PibImpl pib = fixture.pib;
	
				// Check for id1Key1, which should not exist.
				pib.removeIdentity(fixture.id1);
				Assert.AssertEquals(false, pib.hasKey(fixture.id1Key1Name));
	
				// Add id1Key1.
				pib.addKey(fixture.id1, fixture.id1Key1Name, fixture.id1Key1.buf());
				Assert.AssertEquals(true, pib.hasKey(fixture.id1Key1Name));
				Blob keyBits = pib.getKeyBits(fixture.id1Key1Name);
				Assert.AssertTrue(keyBits.equals(fixture.id1Key1));
	
				// To check overwrite, add a key with the same name.
				pib.addKey(fixture.id1, fixture.id1Key1Name, fixture.id1Key2.buf());
				Blob keyBits2 = pib.getKeyBits(fixture.id1Key1Name);
				Assert.AssertTrue(keyBits2.equals(fixture.id1Key2));
	
				// Check for id1Key1Cert1, which should not exist.
				pib.removeIdentity(fixture.id1);
				Assert.AssertEquals(false,
						pib.hasCertificate(fixture.id1Key1Cert1.getName()));
	
				// Add id1Key1Cert1.
				pib.addKey(fixture.id1, fixture.id1Key1Name, fixture.id1Key1.buf());
				pib.addCertificate(fixture.id1Key1Cert1);
				Assert.AssertEquals(true,
						pib.hasCertificate(fixture.id1Key1Cert1.getName()));
	
				CertificateV2 cert = pib.getCertificate(fixture.id1Key1Cert1
						.getName());
				Assert.AssertTrue(cert.wireEncode().equals(
						fixture.id1Key1Cert1.wireEncode()));
	
				// Create a fake certificate with the same name.
				CertificateV2 cert2 = fixture.id1Key2Cert1;
				cert2.setName(fixture.id1Key1Cert1.getName());
				cert2.setSignature(fixture.id1Key2Cert1.getSignature());
				pib.addCertificate(cert2);
	
				CertificateV2 cert3 = pib.getCertificate(fixture.id1Key1Cert1
						.getName());
				Assert.AssertTrue(cert3.wireEncode().equals(cert2.wireEncode()));
	
				// Check that both the key and certificate are overwritten.
				Blob keyBits3 = pib.getKeyBits(fixture.id1Key1Name);
				Assert.AssertTrue(keyBits3.equals(fixture.id1Key2));
			}
		}
	
		internal FileInfo databaseFilePath;
	}
}
