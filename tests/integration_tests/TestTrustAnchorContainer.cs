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
	
	using ILOG.J2CsMapping.Threading;
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
	
	public class TestTrustAnchorContainer {
		internal TrustAnchorContainer anchorContainer;
	
		internal FileInfo certificateDirectoryPath;
		internal FileInfo certificatePath1;
		internal FileInfo certificatePath2;
	
		internal PibIdentity identity1;
		internal PibIdentity identity2;
	
		internal CertificateV2 certificate1;
		internal CertificateV2 certificate2;
		internal IdentityManagementFixture fixture;
	
		public void setUp() {
			anchorContainer = new TrustAnchorContainer();
			fixture = new IdentityManagementFixture();
	
			// Create a directory and prepares two certificates.
			certificateDirectoryPath = new FileInfo(System.IO.Path.Combine(net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon.getPolicyConfigDirectory().FullName,"test-cert-dir"));
			System.IO.Directory.CreateDirectory(certificateDirectoryPath.FullName);
	
			certificatePath1 = new FileInfo(System.IO.Path.Combine(certificateDirectoryPath.FullName,"trust-anchor-1.cert"));
			certificatePath2 = new FileInfo(System.IO.Path.Combine(certificateDirectoryPath.FullName,"trust-anchor-2.cert"));
	
			identity1 = fixture.addIdentity(new Name("/TestAnchorContainer/First"));
			certificate1 = identity1.getDefaultKey().getDefaultCertificate();
			fixture.saveCertificateToFile(certificate1,
					certificatePath1.FullName);
	
			identity2 = fixture
					.addIdentity(new Name("/TestAnchorContainer/Second"));
			certificate2 = identity2.getDefaultKey().getDefaultCertificate();
			fixture.saveCertificateToFile(certificate2,
					certificatePath2.FullName);
		}
	
		public void tearDown() {
			certificatePath1.delete();
			certificatePath2.delete();
		}
	
		public void testInsert() {
			// Static
			anchorContainer.insert("group1", certificate1);
			Assert.AssertTrue(anchorContainer.find(certificate1.getName()) != null);
			Assert.AssertTrue(anchorContainer.find(identity1.getName()) != null);
			CertificateV2 certificate = anchorContainer
					.find(certificate1.getName());
			try {
				// Re-inserting the same certificate should do nothing.
				anchorContainer.insert("group1", certificate1);
			} catch (Exception ex) {
				Assert.Fail("Unexpected exception: " + ex.Message);
			}
			// It should still be the same instance of the certificate.
			Assert.AssertTrue(certificate == anchorContainer.find(certificate1.getName()));
			// Cannot add a dynamic group when the static already exists.
			try {
				anchorContainer.insert("group1",
						certificatePath1.FullName, 400.0d);
				Assert.Fail("Did not throw the expected exception");
			} catch (TrustAnchorContainer.Error ex_0) {
			} catch (Exception ex_1) {
				Assert.Fail("Did not throw the expected exception");
			}
	
			Assert.AssertEquals(1, anchorContainer.getGroup("group1").size());
			Assert.AssertEquals(1, anchorContainer.size());
	
			// From file
			anchorContainer.insert("group2", certificatePath2.FullName,
					400.0d);
			Assert.AssertTrue(anchorContainer.find(certificate2.getName()) != null);
			Assert.AssertTrue(anchorContainer.find(identity2.getName()) != null);
			try {
				anchorContainer.insert("group2", certificate2);
				Assert.Fail("Did not throw the expected exception");
			} catch (TrustAnchorContainer.Error ex_2) {
			} catch (Exception ex_3) {
				Assert.Fail("Did not throw the expected exception");
			}
	
			try {
				anchorContainer.insert("group2",
						certificatePath2.FullName, 400.0d);
				Assert.Fail("Did not throw the expected exception");
			} catch (TrustAnchorContainer.Error ex_4) {
			} catch (Exception ex_5) {
				Assert.Fail("Did not throw the expected exception");
			}
	
			Assert.AssertEquals(1, anchorContainer.getGroup("group2").size());
			Assert.AssertEquals(2, anchorContainer.size());
	
			certificatePath2.delete();
			// Wait for the refresh period to expire.
			ILOG.J2CsMapping.Threading.ThreadWrapper.sleep(500);
	
			Assert.AssertTrue(anchorContainer.find(identity2.getName()) == null);
			Assert.AssertTrue(anchorContainer.find(certificate2.getName()) == null);
			Assert.AssertEquals(0, anchorContainer.getGroup("group2").size());
			Assert.AssertEquals(1, anchorContainer.size());
	
			TrustAnchorGroup group = anchorContainer.getGroup("group1");
			Assert.AssertTrue(group  is  StaticTrustAnchorGroup);
			StaticTrustAnchorGroup staticGroup = (StaticTrustAnchorGroup) group;
			Assert.AssertEquals(1, staticGroup.size());
			staticGroup.remove(certificate1.getName());
			Assert.AssertEquals(0, staticGroup.size());
			Assert.AssertEquals(0, anchorContainer.size());
	
			try {
				anchorContainer.getGroup("non-existing-group");
				Assert.Fail("Did not throw the expected exception");
			} catch (TrustAnchorContainer.Error ex_6) {
			} catch (Exception ex_7) {
				Assert.Fail("Did not throw the expected exception");
			}
		}
	
		public void testDynamicAnchorFromDirectory() {
			certificatePath2.delete();
	
			anchorContainer.insert("group",
					certificateDirectoryPath.FullName, 400.0d, true);
	
			Assert.AssertTrue(anchorContainer.find(identity1.getName()) != null);
			Assert.AssertTrue(anchorContainer.find(identity2.getName()) == null);
			Assert.AssertEquals(1, anchorContainer.getGroup("group").size());
	
			fixture.saveCertificateToFile(certificate2,
					certificatePath2.FullName);
	
			// Wait for the refresh period to expire. The dynamic anchors should remain.
			ILOG.J2CsMapping.Threading.ThreadWrapper.sleep(500);
	
			Assert.AssertTrue(anchorContainer.find(identity1.getName()) != null);
			Assert.AssertTrue(anchorContainer.find(identity2.getName()) != null);
			Assert.AssertEquals(2, anchorContainer.getGroup("group").size());
	
			if (certificateDirectoryPath.Exists) {
				/* foreach */
				// Delete files from a previous test.
				foreach (FileInfo file  in  certificateDirectoryPath.listFiles())
					file.delete();
			}
	
			// Wait for the refresh period to expire. The dynamic anchors should be gone.
			ILOG.J2CsMapping.Threading.ThreadWrapper.sleep(500);
	
			Assert.AssertTrue(anchorContainer.find(identity1.getName()) == null);
			Assert.AssertTrue(anchorContainer.find(identity2.getName()) == null);
			Assert.AssertEquals(0, anchorContainer.getGroup("group").size());
		}
	
		public void testFindByInterest() {
			anchorContainer.insert("group1", certificatePath1.FullName,
					400.0d);
			Interest interest = new Interest(identity1.getName());
			Assert.AssertTrue(anchorContainer.find(interest) != null);
			Interest interest1 = new Interest(identity1.getName().getPrefix(-1));
			Assert.AssertTrue(anchorContainer.find(interest1) != null);
			Interest interest2 = new Interest(
					new Name(identity1.getName()).appendVersion(1));
			Assert.AssertTrue(anchorContainer.find(interest2) == null);
	
			CertificateV2 certificate3 = fixture.addCertificate(
					identity1.getDefaultKey(), "3");
			CertificateV2 certificate4 = fixture.addCertificate(
					identity1.getDefaultKey(), "4");
			CertificateV2 certificate5 = fixture.addCertificate(
					identity1.getDefaultKey(), "5");
	
			CertificateV2 certificate3Copy = new CertificateV2(certificate3);
			anchorContainer.insert("group2", certificate3Copy);
			anchorContainer.insert("group3", certificate4);
			anchorContainer.insert("group4", certificate5);
	
			Interest interest3 = new Interest(certificate3.getKeyName());
			CertificateV2 foundCertificate = anchorContainer.find(interest3);
			Assert.AssertTrue(foundCertificate != null);
			Assert.AssertTrue(interest3.getName().isPrefixOf(foundCertificate.getName()));
			Assert.AssertTrue(certificate3.getName().equals(foundCertificate.getName()));
	
			interest3.getExclude().appendComponent(
					certificate3.getName().get(net.named_data.jndn.security.v2.CertificateV2.ISSUER_ID_OFFSET));
			foundCertificate = anchorContainer.find(interest3);
			Assert.AssertTrue(foundCertificate != null);
			Assert.AssertTrue(interest3.getName().isPrefixOf(foundCertificate.getName()));
			Assert.AssertTrue(!foundCertificate.getName().equals(certificate3.getName()));
		}
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
