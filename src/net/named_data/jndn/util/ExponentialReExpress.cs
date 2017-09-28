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
namespace net.named_data.jndn.util {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	
	/// <summary>
	/// An ExponentialReExpress uses an internal onTimeout to express the interest
	/// again with double the interestLifetime. See
	/// ExponentialReExpress.makeOnTimeout.
	/// </summary>
	///
	public class ExponentialReExpress : OnTimeout {
		/// <summary>
		/// Return an OnTimeout object to use in expressInterest for onTimeout which
		/// will express the interest again with double the interestLifetime. If the
		/// interesLifetime goes over maxInterestLifetime (see below), then call the
		/// provided onTimeout. If a Data packet is received, this calls the provided
		/// onData.
		/// </summary>
		///
		/// <param name="face">This calls face.expressInterest.</param>
		/// <param name="onData">expressInterest and data is the received Data object. This is normally the same onData you initially passed to expressInterest. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onTimeout">does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="maxInterestLifetime"></param>
		/// <returns>The OnTimeout object to pass to expressInterest.</returns>
		public static OnTimeout makeOnTimeout(Face face, OnData onData,
				OnTimeout onTimeout, double maxInterestLifetime) {
			return new ExponentialReExpress(face, onData, onTimeout,
					maxInterestLifetime);
		}
	
		/// <summary>
		/// Return an OnTimeout object to use in expressInterest for onTimeout which
		/// will express the interest again with double the interestLifetime. If the
		/// interesLifetime goes over 16000 milliseconds, then call the provided
		/// onTimeout. If a Data packet is received, this calls the provided onData.
		/// </summary>
		///
		/// <param name="face">This calls face.expressInterest.</param>
		/// <param name="onData">expressInterest and data is the received Data object. This is normally the same onData you initially passed to expressInterest. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onTimeout">does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <returns>The OnTimeout object to pass to expressInterest.</returns>
		public static OnTimeout makeOnTimeout(Face face, OnData onData,
				OnTimeout onTimeout) {
			return makeOnTimeout(face, onData, onTimeout, 16000.0d);
		}
	
		/// <summary>
		/// Create a new ExponentialReExpress where onTimeout expresses the interest
		/// again with double the interestLifetime. If the interesLifetime goes over
		/// maxInterestLifetime, then call the given onTimeout. If this internally
		/// gets onData, just call the given onData.
		/// </summary>
		///
		private ExponentialReExpress(Face face, OnData onData, OnTimeout onTimeout,
				double maxInterestLifetime) {
			face_ = face;
			callerOnData_ = onData;
			callerOnTimeout_ = onTimeout;
			maxInterestLifetime_ = maxInterestLifetime;
		}
	
		public virtual void onTimeout(Interest interest) {
			double interestLifetime = interest.getInterestLifetimeMilliseconds();
			if (interestLifetime < 0) {
				// Can't re-express.
				if (callerOnTimeout_ != null) {
					try {
						callerOnTimeout_.onTimeout(interest);
					} catch (Exception ex) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onTimeout", ex);
					}
				}
				return;
			}
	
			double nextInterestLifetime = interestLifetime * 2;
			if (nextInterestLifetime > maxInterestLifetime_) {
				if (callerOnTimeout_ != null) {
					try {
						callerOnTimeout_.onTimeout(interest);
					} catch (Exception ex_0) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onTimeout", ex_0);
					}
				}
				return;
			}
	
			Interest nextInterest = new Interest(interest);
			nextInterest.setInterestLifetimeMilliseconds(nextInterestLifetime);
			logger_.log(
					ILOG.J2CsMapping.Util.Logging.Level.FINE,
					"ExponentialReExpress: Increasing interest lifetime from {0} to {1} ms. Re-express interest {2}",
					new Object[] { interestLifetime, nextInterestLifetime,
							nextInterest.getName().toUri() });
			try {
				face_.expressInterest(nextInterest, callerOnData_, this);
			} catch (IOException ex_1) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_1);
			}
		}
	
		private readonly Face face_;
		private readonly OnData callerOnData_;
		private readonly OnTimeout callerOnTimeout_;
		private readonly double maxInterestLifetime_;
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(ExponentialReExpress).FullName);
	}
}
