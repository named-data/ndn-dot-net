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
	
	using ILOG.J2CsMapping.Util.Logging;
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
	
	internal class ValidationPolicyCommandInterestFixture : 		HierarchicalValidatorFixture {
		public ValidationPolicyCommandInterestFixture(
				ValidationPolicyCommandInterest.Options options) : base(new ValidationPolicyCommandInterest(new ValidationPolicySimpleHierarchy(),options)) {
			signer_ = new CommandInterestSigner(keyChain_);
		}
	
		public ValidationPolicyCommandInterestFixture() : base(new ValidationPolicyCommandInterest(new ValidationPolicySimpleHierarchy())) {
			signer_ = new CommandInterestSigner(keyChain_);
		}
	
		internal Interest makeCommandInterest(PibIdentity identity) {
			return signer_.makeCommandInterest(new Name(identity.getName())
					.append("CMD"), new SigningInfo(identity));
		}
	
		/// <summary>
		/// Set the offset for the validation policy and signer.
		/// </summary>
		///
		/// <param name="nowOffsetMilliseconds">The offset in milliseconds.</param>
		internal void setNowOffsetMilliseconds(double nowOffsetMilliseconds) {
			((ValidationPolicyCommandInterest) validator_.getPolicy())
					.setNowOffsetMilliseconds_(nowOffsetMilliseconds);
			validator_.setCacheNowOffsetMilliseconds_(nowOffsetMilliseconds);
			signer_.setNowOffsetMilliseconds_(nowOffsetMilliseconds);
		}
	
		public CommandInterestSigner signer_;
	}
	
	public class TestValidationPolicyCommandInterest {
		public sealed class Anonymous_C7 : DataValidationSuccessCallback {
			public void successCallback(Data data) {
			}
		}
	
		public sealed class Anonymous_C6 : DataValidationFailureCallback {
			private readonly String message;
	
			public Anonymous_C6(String message_0) {
				this.message = message_0;
			}
	
			public void failureCallback(Data data, ValidationError error) {
				Assert.Fail(message);
			}
		}
	
		public sealed class Anonymous_C5 : 			InterestValidationSuccessCallback {
			public void successCallback(Interest interest) {
			}
		}
	
		public sealed class Anonymous_C4 : 			InterestValidationFailureCallback {
			private readonly String message;
	
			public Anonymous_C4(String message_0) {
				this.message = message_0;
			}
	
			public void failureCallback(Interest interest,
					ValidationError error) {
				Assert.Fail(message);
			}
		}
	
		public sealed class Anonymous_C3 : DataValidationSuccessCallback {
			private readonly String message;
	
			public Anonymous_C3(String message_0) {
				this.message = message_0;
			}
	
			public void successCallback(Data data) {
				Assert.Fail(message);
			}
		}
	
		public sealed class Anonymous_C2 : DataValidationFailureCallback {
			public void failureCallback(Data data, ValidationError error) {
			}
		}
	
		public sealed class Anonymous_C1 : 			InterestValidationSuccessCallback {
			private readonly String message;
	
			public Anonymous_C1(String message_0) {
				this.message = message_0;
			}
	
			public void successCallback(Interest interest) {
				Assert.Fail(message);
			}
		}
	
		public sealed class Anonymous_C0 : 			InterestValidationFailureCallback {
			public void failureCallback(Interest interest,
					ValidationError error) {
			}
		}
	
		internal ValidationPolicyCommandInterestFixture fixture_;
	
		public void setUp() {
			// Turn off INFO log messages.
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger("").setLevel(ILOG.J2CsMapping.Util.Logging.Level.SEVERE);
	
			fixture_ = new ValidationPolicyCommandInterestFixture();
		}
	
		/// <summary>
		/// Call fixture_.validator_.validate and if it calls the failureCallback then
		/// fail the test with the given message.
		/// </summary>
		///
		/// <param name="data">The Data to validate.</param>
		/// <param name="message_0">The message to show if the test fails.</param>
		internal void validateExpectSuccess(Data data, String message_0) {
			fixture_.validator_.validate(data, new TestValidationPolicyCommandInterest.Anonymous_C7 (), new TestValidationPolicyCommandInterest.Anonymous_C6 (message_0));
		}
	
		/// <summary>
		/// Call fixture_.validator_.validate and if it calls the failureCallback then
		/// fail the test with the given message.
		/// </summary>
		///
		/// <param name="interest">The Interest to validate.</param>
		/// <param name="message_0">The message to show if the test fails.</param>
		internal void validateExpectSuccess(Interest interest, String message_0) {
			fixture_.validator_.validate(interest,
					new TestValidationPolicyCommandInterest.Anonymous_C5 (), new TestValidationPolicyCommandInterest.Anonymous_C4 (message_0));
		}
	
		/// <summary>
		/// Call fixture_.validator_.validate and if it calls the successCallback then
		/// fail the test with the given message.
		/// </summary>
		///
		/// <param name="data">The Data to validate.</param>
		/// <param name="message_0">The message to show if the test succeeds.</param>
		internal void validateExpectFailure(Data data, String message_0) {
			fixture_.validator_.validate(data, new TestValidationPolicyCommandInterest.Anonymous_C3 (message_0), new TestValidationPolicyCommandInterest.Anonymous_C2 ());
		}
	
		/// <summary>
		/// Call fixture_.validator_.validate and if it calls the successCallback then
		/// fail the test with the given message.
		/// </summary>
		///
		/// <param name="interest">The Interest to validate.</param>
		/// <param name="message_0">The message to show if the test succeeds.</param>
		internal void validateExpectFailure(Interest interest, String message_0) {
			fixture_.validator_.validate(interest,
					new TestValidationPolicyCommandInterest.Anonymous_C1 (message_0), new TestValidationPolicyCommandInterest.Anonymous_C0 ());
		}
	
		static internal void setNameComponent(Interest interest, int index,
				Name.Component component) {
			Name name = interest.getName().getPrefix(index);
			name.append(component);
			name.append(interest.getName().getSubName(name.size()));
			interest.setName(name);
		}
	
		static internal void setNameComponent(Interest interest, int index, String component) {
			setNameComponent(interest, index, new Name.Component(component));
		}
	
		static internal void setNameComponent(Interest interest, int index, Blob component) {
			setNameComponent(interest, index, new Name.Component(component));
		}
	
		public void testBasic() {
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			validateExpectSuccess(interest1, "Should succeed (within grace period)");
	
			fixture_.setNowOffsetMilliseconds(5 * 1000.0d);
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			validateExpectSuccess(interest2,
					"Should succeed (timestamp larger than previous)");
		}
	
		public void testDataPassthrough() {
			Data data1 = new Data(new Name("/Security/V2/ValidatorFixture/Sub1"));
			fixture_.keyChain_.sign(data1);
			validateExpectSuccess(data1,
					"Should succeed (fallback on inner validation policy for data)");
		}
	
		public void testNameTooShort() {
			Interest interest1 = new Interest(new Name("/name/too/short"));
			validateExpectFailure(interest1, "Should fail (name is too short)");
		}
	
		public void testBadSignatureInfo() {
			Interest interest1;
			interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			setNameComponent(interest1, net.named_data.jndn.security.CommandInterestSigner.POS_SIGNATURE_INFO,
					"not-SignatureInfo");
			validateExpectFailure(interest1, "Should fail (missing signature info)");
		}
	
		public void testMissingKeyLocator() {
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			Sha256WithRsaSignature signatureInfo = new Sha256WithRsaSignature();
			setNameComponent(interest1, net.named_data.jndn.security.CommandInterestSigner.POS_SIGNATURE_INFO,
					net.named_data.jndn.encoding.TlvWireFormat.get().encodeSignatureInfo(signatureInfo));
			validateExpectFailure(interest1, "Should fail (missing KeyLocator)");
		}
	
		public void testBadKeyLocatorType() {
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			KeyLocator keyLocator = new KeyLocator();
			keyLocator.setType(net.named_data.jndn.KeyLocatorType.KEY_LOCATOR_DIGEST);
			keyLocator.setKeyData(new Blob(new int[] { 0xdd, 0xdd, 0xdd, 0xdd,
					0xdd, 0xdd, 0xdd, 0xdd }));
			Sha256WithRsaSignature signatureInfo = new Sha256WithRsaSignature();
			signatureInfo.setKeyLocator(keyLocator);
	
			setNameComponent(interest1, net.named_data.jndn.security.CommandInterestSigner.POS_SIGNATURE_INFO,
					net.named_data.jndn.encoding.TlvWireFormat.get().encodeSignatureInfo(signatureInfo));
			validateExpectFailure(interest1, "Should fail (bad KeyLocator type)");
		}
	
		public void testBadCertificateName() {
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			KeyLocator keyLocator = new KeyLocator();
			keyLocator.setType(net.named_data.jndn.KeyLocatorType.KEYNAME);
			keyLocator.setKeyName(new Name("/bad/cert/name"));
			Sha256WithRsaSignature signatureInfo = new Sha256WithRsaSignature();
			signatureInfo.setKeyLocator(keyLocator);
	
			setNameComponent(interest1, net.named_data.jndn.security.CommandInterestSigner.POS_SIGNATURE_INFO,
					net.named_data.jndn.encoding.TlvWireFormat.get().encodeSignatureInfo(signatureInfo));
			validateExpectFailure(interest1, "Should fail (bad certificate name)");
		}
	
		public void testInnerPolicyReject() {
			Interest interest1 = fixture_
					.makeCommandInterest(fixture_.otherIdentity_);
			validateExpectFailure(interest1,
					"Should fail (inner policy should reject)");
		}
	
		public void testTimestampOutOfGracePositive() {
			fixture_ = new ValidationPolicyCommandInterestFixture(
					new ValidationPolicyCommandInterest.Options(15 * 1000.0d));
	
			// Signed at 0 seconds.
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			// Verifying at +16 seconds.
			fixture_.setNowOffsetMilliseconds(16 * 1000.0d);
			validateExpectFailure(interest1,
					"Should fail (timestamp outside the grace period)");
	
			// Signed at +16 seconds.
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			validateExpectSuccess(interest2, "Should succeed");
		}
	
		public void testTimestampOutOfGraceNegative() {
			fixture_ = new ValidationPolicyCommandInterestFixture(
					new ValidationPolicyCommandInterest.Options(15 * 1000.0d));
	
			// Signed at 0 seconds.
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +1 seconds.
			fixture_.setNowOffsetMilliseconds(1 * 1000.0d);
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +2 seconds.
			fixture_.setNowOffsetMilliseconds(2 * 1000.0d);
			Interest interest3 = fixture_.makeCommandInterest(fixture_.identity_);
	
			// Verifying at -16 seconds.
			fixture_.setNowOffsetMilliseconds(-16 * 1000.0d);
			validateExpectFailure(interest1,
					"Should fail (timestamp outside the grace period)");
	
			// The CommandInterestValidator should not remember interest1's timestamp.
			validateExpectFailure(interest2,
					"Should fail (timestamp outside the grace period)");
	
			// The CommandInterestValidator should not remember interest2's timestamp, and
			// should treat interest3 as initial.
			// Verifying at +2 seconds.
			fixture_.setNowOffsetMilliseconds(2 * 1000.0d);
			validateExpectSuccess(interest3, "Should succeed");
		}
	
		public void testTimestampReorderEqual() {
			// Signed at 0 seconds.
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			validateExpectSuccess(interest1, "Should succeed");
	
			// Signed at 0 seconds.
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			setNameComponent(interest2, net.named_data.jndn.security.CommandInterestSigner.POS_TIMESTAMP,
					interest1.getName().get(net.named_data.jndn.security.CommandInterestSigner.POS_TIMESTAMP));
			validateExpectFailure(interest2, "Should fail (timestamp reordered)");
	
			// Signed at +2 seconds.
			fixture_.setNowOffsetMilliseconds(2 * 1000.0d);
			Interest interest3 = fixture_.makeCommandInterest(fixture_.identity_);
			validateExpectSuccess(interest3, "Should succeed");
		}
	
		public void testTimestampReorderNegative() {
			// Signed at 0 seconds.
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +200 milliseconds.
			fixture_.setNowOffsetMilliseconds(200.0d);
			Interest interest3 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +1100 milliseconds.
			fixture_.setNowOffsetMilliseconds(1100.0d);
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +1400 milliseconds.
			fixture_.setNowOffsetMilliseconds(1400.0d);
			Interest interest4 = fixture_.makeCommandInterest(fixture_.identity_);
	
			// Verifying at +1100 milliseconds.
			fixture_.setNowOffsetMilliseconds(1100.0d);
			validateExpectSuccess(interest1, "Should succeed");
	
			// Verifying at 0 milliseconds.
			fixture_.setNowOffsetMilliseconds(0.0d);
			validateExpectFailure(interest2, "Should fail (timestamp reordered)");
	
			// The CommandInterestValidator should not remember interest2's timestamp.
			// Verifying at +200 milliseconds.
			fixture_.setNowOffsetMilliseconds(200.0d);
			validateExpectFailure(interest3, "Should fail (timestamp reordered)");
	
			// Verifying at +1400 milliseconds.
			fixture_.setNowOffsetMilliseconds(1400.0d);
			validateExpectSuccess(interest4, "Should succeed");
		}
	
		public void testLimitedRecords() {
			fixture_ = new ValidationPolicyCommandInterestFixture(
					new ValidationPolicyCommandInterest.Options(15 * 1000.0d, 3));
	
			PibIdentity identity1 = fixture_.addSubCertificate(new Name(
					"/Security/V2/ValidatorFixture/Sub1"), fixture_.identity_);
			fixture_.cache_.insert(identity1.getDefaultKey()
					.getDefaultCertificate());
			PibIdentity identity2 = fixture_.addSubCertificate(new Name(
					"/Security/V2/ValidatorFixture/Sub2"), fixture_.identity_);
			fixture_.cache_.insert(identity2.getDefaultKey()
					.getDefaultCertificate());
			PibIdentity identity3 = fixture_.addSubCertificate(new Name(
					"/Security/V2/ValidatorFixture/Sub3"), fixture_.identity_);
			fixture_.cache_.insert(identity3.getDefaultKey()
					.getDefaultCertificate());
			PibIdentity identity4 = fixture_.addSubCertificate(new Name(
					"/Security/V2/ValidatorFixture/Sub4"), fixture_.identity_);
			fixture_.cache_.insert(identity4.getDefaultKey()
					.getDefaultCertificate());
	
			Interest interest1 = fixture_.makeCommandInterest(identity2);
			Interest interest2 = fixture_.makeCommandInterest(identity3);
			Interest interest3 = fixture_.makeCommandInterest(identity4);
			// Signed at 0 seconds.
			Interest interest00 = fixture_.makeCommandInterest(identity1);
			// Signed at +1 seconds.
			fixture_.setNowOffsetMilliseconds(1 * 1000.0d);
			Interest interest01 = fixture_.makeCommandInterest(identity1);
			// Signed at +2 seconds.
			fixture_.setNowOffsetMilliseconds(2 * 1000.0d);
			Interest interest02 = fixture_.makeCommandInterest(identity1);
	
			validateExpectSuccess(interest00, "Should succeed");
	
			validateExpectSuccess(interest02, "Should succeed");
	
			validateExpectSuccess(interest1, "Should succeed");
	
			validateExpectSuccess(interest2, "Should succeed");
	
			validateExpectSuccess(interest3, "Should succeed, forgets identity1");
	
			validateExpectSuccess(
					interest01,
					"Should succeed despite timestamp is reordered, because the record has been evicted");
		}
	
		public void testUnlimitedRecords() {
			fixture_ = new ValidationPolicyCommandInterestFixture(
					new ValidationPolicyCommandInterest.Options(15 * 1000.0d, -1));
	
			ArrayList<PibIdentity> identities = new ArrayList<PibIdentity>();
			for (int i = 0; i < 20; ++i) {
				PibIdentity identity = fixture_.addSubCertificate(new Name(
						"/Security/V2/ValidatorFixture/Sub" + i),
						fixture_.identity_);
				fixture_.cache_.insert(identity.getDefaultKey()
						.getDefaultCertificate());
				ILOG.J2CsMapping.Collections.Collections.Add(identities,identity);
			}
	
			// Signed at 0 seconds.
			Interest interest1 = fixture_.makeCommandInterest(identities[0]);
			fixture_.setNowOffsetMilliseconds(1 * 1000.0d);
			for (int i_0 = 0; i_0 < 20; ++i_0) {
				// Signed at +1 seconds.
				Interest interest2 = fixture_
						.makeCommandInterest(identities[i_0]);
	
				validateExpectSuccess(interest2, "Should succeed");
			}
	
			validateExpectFailure(interest1, "Should fail (timestamp reorder)");
		}
	
		public void testZeroRecords() {
			fixture_ = new ValidationPolicyCommandInterestFixture(
					new ValidationPolicyCommandInterest.Options(15 * 1000.0d, 0));
	
			// Signed at 0 seconds.
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +1 seconds.
			fixture_.setNowOffsetMilliseconds(1 * 1000.0d);
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			validateExpectSuccess(interest2, "Should succeed");
	
			validateExpectSuccess(
					interest1,
					"Should succeed despite the timestamp being reordered, because the record isn't kept");
		}
	
		public void testLimitedRecordLifetime() {
			fixture_ = new ValidationPolicyCommandInterestFixture(
					new ValidationPolicyCommandInterest.Options(400 * 1000.0d, 1000,
							300 * 1000.0d));
	
			// Signed at 0 seconds.
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +240 seconds.
			fixture_.setNowOffsetMilliseconds(240 * 1000.0d);
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +360 seconds.
			fixture_.setNowOffsetMilliseconds(360 * 1000.0d);
			Interest interest3 = fixture_.makeCommandInterest(fixture_.identity_);
	
			// Validate at 0 seconds.
			fixture_.setNowOffsetMilliseconds(0.0d);
			validateExpectSuccess(interest1, "Should succeed");
	
			validateExpectSuccess(interest3, "Should succeed");
	
			// Validate at +301 seconds.
			fixture_.setNowOffsetMilliseconds(301 * 1000.0d);
			validateExpectSuccess(
					interest2,
					"Should succeed despite the timestamp being reordered, because the record has expired");
		}
	
		public void testZeroRecordLifetime() {
			fixture_ = new ValidationPolicyCommandInterestFixture(
					new ValidationPolicyCommandInterest.Options(15 * 1000.0d, 1000,
							0.0d));
	
			// Signed at 0 seconds.
			Interest interest1 = fixture_.makeCommandInterest(fixture_.identity_);
			// Signed at +1 second.
			fixture_.setNowOffsetMilliseconds(1 * 1000.0d);
			Interest interest2 = fixture_.makeCommandInterest(fixture_.identity_);
			validateExpectSuccess(interest2, "Should succeed");
	
			validateExpectSuccess(
					interest1,
					"Should succeed despite the timestamp being reordered, because the record has expired");
		}
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
