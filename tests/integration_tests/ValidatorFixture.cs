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
namespace net.named_data.jndn.tests.integration_tests {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// ValidatorFixture extends IdentityManagementFixture to use the given policy
	/// and to set up a test face to answer Interests.
	/// </summary>
	///
	public class ValidatorFixture : IdentityManagementFixture {
		/// <summary>
		/// Create a ValidatorFixture to use the given policy. Set the default
		/// face_.processInterest_ to use the cache_ to respond to expressInterest. To
		/// change this behavior, you can set face_.processInterest_ to your callback,
		/// or to null to always time out.
		/// </summary>
		///
		/// <param name="policy">The ValidationPolicy used by validator_.</param>
		public ValidatorFixture(ValidationPolicy policy) {
			this.face_ = new ValidatorFixture.TestFace ();
					this.cache_ = new CertificateCacheV2(
							100 * 24 * 3600 * 1000.0d);
			validator_ = new Validator(policy, new CertificateFetcherFromNetwork(
					face_));
			policy_ = policy;
	
			face_.processInterest_ = new ValidatorFixture.Anonymous_C0 (this);
		}
	
		public sealed class Anonymous_C0 : TestFace.ProcessInterest {
				private readonly ValidatorFixture outer_ValidatorFixture;
		
				public Anonymous_C0(ValidatorFixture paramouter_ValidatorFixture) {
					this.outer_ValidatorFixture = paramouter_ValidatorFixture;
				}
		
				public void processInterest(Interest interest, OnData onData,
						OnTimeout onTimeout, OnNetworkNack onNetworkNack) {
					CertificateV2 certificate = outer_ValidatorFixture.cache_.find(interest);
					if (certificate != null)
						onData.onData(interest, certificate);
					else
						onTimeout.onTimeout(interest);
				}
			}
		/// <summary>
		/// TestFace extends Face to instantly simulate a call to expressInterest.
		/// See expressInterest for details.
		/// </summary>
		///
		public class TestFace : Face {
			public interface ProcessInterest {
				void processInterest(Interest interest, OnData onData,
						OnTimeout onTimeout, OnNetworkNack onNetworkNack);
			}
	
			public TestFace() : base("localhost") {
				this.processInterest_ = null;
				this.sentInterests_ = new ArrayList<Interest>();
			}
	
			/// <summary>
			/// If processInterest_ is not null, call
			/// processInterest_.processInterest(interest, onData, onTimeout, onNetworkNack)
			/// which must call one of the callbacks to simulate the response. Otherwise, 
			/// just call onTimeout(interest) to simulate a timeout. This adds a copy of
			/// the interest to sentInterests_ .
			/// </summary>
			///
			public override long expressInterest(Interest interest, OnData onData,
					OnTimeout onTimeout, OnNetworkNack onNetworkNack,
					WireFormat wireFormat) {
				// Makes a copy of the interest.
				ILOG.J2CsMapping.Collections.Collections.Add(sentInterests_,new Interest(interest));
	
				if (processInterest_ != null)
					processInterest_.processInterest(interest, onData, onTimeout,
							onNetworkNack);
				else
					onTimeout.onTimeout(interest);
	
				return 0;
			}
	
			public net.named_data.jndn.tests.integration_tests.ValidatorFixture.TestFace.ProcessInterest  processInterest_;
			public ArrayList<Interest> sentInterests_;
		} 
	
		public readonly ValidatorFixture.TestFace  face_;
		public readonly Validator validator_;
		public readonly ValidationPolicy policy_;
		// Set maxLifetime to 100 days.
		public readonly CertificateCacheV2 cache_;
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
