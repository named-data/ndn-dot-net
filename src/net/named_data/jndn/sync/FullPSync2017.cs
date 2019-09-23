// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.sync {
	
	using ILOG.J2CsMapping.Collections;
	using ILOG.J2CsMapping.Util;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.tpm;
	using net.named_data.jndn.sync.detail;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// FullPSync2017 implements the full sync logic of PSync to synchronize with
	/// other nodes, where all nodes want to sync all the names. The application
	/// should call publishName whenever it wants to let consumers know that a new name
	/// is available. Currently, fetching and publishing the data given by the
	/// announced name needs to be handled by the application. The Full PSync
	/// protocol is described in Section G "Full-Data Synchronization" of:
	/// https://named-data.net/wp-content/uploads/2017/05/scalable_name-based_data_synchronization.pdf
	/// (Note: In the PSync library, this class is called FullProducerArbitrary. But
	/// because the class actually handles both producing and consuming, we omit
	/// "producer" in the name to avoid confusion.)
	/// </summary>
	///
	public class FullPSync2017 : PSyncProducerBase, 		SegmentFetcher.OnError {
		public sealed class Anonymous_C4 : OnInterestCallback {
				private readonly FullPSync2017 outer_FullPSync2017;
		
				public Anonymous_C4(FullPSync2017 paramouter_FullPSync2017) {
					this.outer_FullPSync2017 = paramouter_FullPSync2017;
				}
		
				public void onInterest(Name prefix, Interest interest,
						Face face, long interestFilterId,
						InterestFilter filter) {
					outer_FullPSync2017.onSyncInterest(prefix, interest, face,
							interestFilterId, filter);
				}
			}
	
		public sealed class Anonymous_C3 : OnRegisterFailed {
			public void onRegisterFailed(Name prefix) {
				net.named_data.jndn.sync.PSyncProducerBase.onRegisterFailed(prefix);
			}
		}
	
		public sealed class Anonymous_C2 : IRunnable {
				private readonly FullPSync2017 outer_FullPSync2017;
		
				public Anonymous_C2(FullPSync2017 paramouter_FullPSync2017) {
					this.outer_FullPSync2017 = paramouter_FullPSync2017;
				}
		
				public void run() {
					outer_FullPSync2017.sendSyncInterest();
				}
			}
	
		public sealed class Anonymous_C1 : SegmentFetcher.OnComplete {
				private readonly FullPSync2017 outer_FullPSync2017;
				private readonly Interest syncInterest;
		
				public Anonymous_C1(FullPSync2017 paramouter_FullPSync2017,
						Interest syncInterest_0) {
					this.syncInterest = syncInterest_0;
					this.outer_FullPSync2017 = paramouter_FullPSync2017;
				}
		
				public void onComplete(Blob content) {
					outer_FullPSync2017.onSyncData(content, syncInterest);
				}
			}
	
		public sealed class Anonymous_C0 : IRunnable {
				private readonly FullPSync2017 outer_FullPSync2017;
				private readonly Interest interest;
				private readonly FullPSync2017.PendingEntryInfoFull  entry;
		
				public Anonymous_C0(FullPSync2017 paramouter_FullPSync2017,
						Interest interest_0, FullPSync2017.PendingEntryInfoFull  entry_1) {
					this.interest = interest_0;
					this.entry = entry_1;
					this.outer_FullPSync2017 = paramouter_FullPSync2017;
				}
		
				public void run() {
					outer_FullPSync2017.delayedRemovePendingEntry(interest.getName(), entry,
							interest.getNonce());
				}
			}
	
		public interface OnNamesUpdate {
			void onNamesUpdate(ArrayList<Name> updates);
		}
	
		public interface CanAddToSyncData {
			bool canAddToSyncData(Name name, HashedSet<Int64> negative);
		}
	
		public interface CanAddReceivedName {
			bool canAddReceivedName(Name name);
		}
	
		/// <summary>
		/// Create a FullPSync2017.
		/// </summary>
		///
		/// <param name="expectedNEntries">The expected number of entries in the IBLT.</param>
		/// <param name="face">The application's Face.</param>
		/// <param name="syncPrefix">The prefix Name of the sync group, which is copied.</param>
		/// <param name="onNamesUpdate">see the canAddReceivedName callback which can control which names are added. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="keyChain">The KeyChain for signing Data packets.</param>
		/// <param name="syncInterestLifetime"></param>
		/// <param name="syncReplyFreshnessPeriod"></param>
		/// <param name="signingInfo"></param>
		/// <param name="canAddToSyncData">where Name is the candidate Name to add to the response Data packet of Names, and negative is the set of names that the other's user's Name set, but not in our own Name set. If the callback returns false, then this does not report the Name to the other user. However, if canAddToSyncData is null, then each name is reported.</param>
		/// <param name="canAddReceivedName">returns false, then this does not add to the IBLT or report to the application with onNamesUpdate. However, if canAddReceivedName is null, then each name is added.</param>
		public FullPSync2017(int expectedNEntries, Face face, Name syncPrefix,
				FullPSync2017.OnNamesUpdate  onNamesUpdate, KeyChain keyChain,
				double syncInterestLifetime, double syncReplyFreshnessPeriod,
				SigningInfo signingInfo, FullPSync2017.CanAddToSyncData  canAddToSyncData,
				FullPSync2017.CanAddReceivedName  canAddReceivedName) : base(expectedNEntries, syncPrefix, syncReplyFreshnessPeriod) {
			this.pendingEntries_ = new Hashtable<Name, PendingEntryInfoFull>();
			this.outstandingInterestName_ = new Name();
			construct(face, onNamesUpdate, keyChain, syncInterestLifetime,
					signingInfo, canAddToSyncData, canAddReceivedName);
		}
	
		/// <summary>
		/// Create a FullPSync2017.
		/// </summary>
		///
		/// <param name="expectedNEntries">The expected number of entries in the IBLT.</param>
		/// <param name="face">The application's Face.</param>
		/// <param name="syncPrefix">The prefix Name of the sync group, which is copied.</param>
		/// <param name="onNamesUpdate">see the canAddReceivedName callback which can control which names are added. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="keyChain">The KeyChain for signing Data packets.</param>
		/// <param name="syncInterestLifetime"></param>
		/// <param name="syncReplyFreshnessPeriod"></param>
		/// <param name="signingInfo"></param>
		public FullPSync2017(int expectedNEntries, Face face, Name syncPrefix,
				FullPSync2017.OnNamesUpdate  onNamesUpdate, KeyChain keyChain,
				double syncInterestLifetime, double syncReplyFreshnessPeriod,
				SigningInfo signingInfo) : base(expectedNEntries, syncPrefix, syncReplyFreshnessPeriod) {
			this.pendingEntries_ = new Hashtable<Name, PendingEntryInfoFull>();
			this.outstandingInterestName_ = new Name();
			construct(face, onNamesUpdate, keyChain, syncInterestLifetime,
					signingInfo, null, null);
		}
	
		/// <summary>
		/// Create a FullPSync2017, where signingInfo is the default SigningInfo().
		/// </summary>
		///
		/// <param name="expectedNEntries">The expected number of entries in the IBLT.</param>
		/// <param name="face">The application's Face.</param>
		/// <param name="syncPrefix">The prefix Name of the sync group, which is copied.</param>
		/// <param name="onNamesUpdate">see the canAddReceivedName callback which can control which names are added. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="keyChain">The KeyChain for signing Data packets.</param>
		/// <param name="syncInterestLifetime"></param>
		/// <param name="syncReplyFreshnessPeriod"></param>
		public FullPSync2017(int expectedNEntries, Face face, Name syncPrefix,
				FullPSync2017.OnNamesUpdate  onNamesUpdate, KeyChain keyChain,
				double syncInterestLifetime, double syncReplyFreshnessPeriod) : base(expectedNEntries, syncPrefix, syncReplyFreshnessPeriod) {
			this.pendingEntries_ = new Hashtable<Name, PendingEntryInfoFull>();
			this.outstandingInterestName_ = new Name();
			construct(face, onNamesUpdate, keyChain, syncInterestLifetime,
					new SigningInfo(), null, null);
		}
	
		/// <summary>
		/// Create a FullPSync2017, where syncInterestLifetime is
		/// DEFAULT_SYNC_INTEREST_LIFETIME, syncReplyFreshnessPeriod is
		/// DEFAULT_SYNC_REPLY_FRESHNESS_PERIOD and signingInfo is the default
		/// SigningInfo().
		/// </summary>
		///
		/// <param name="expectedNEntries">The expected number of entries in the IBLT.</param>
		/// <param name="face">The application's Face.</param>
		/// <param name="syncPrefix">The prefix Name of the sync group, which is copied.</param>
		/// <param name="onNamesUpdate">see the canAddReceivedName callback which can control which names are added. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="keyChain">The KeyChain for signing Data packets.</param>
		public FullPSync2017(int expectedNEntries, Face face, Name syncPrefix,
				FullPSync2017.OnNamesUpdate  onNamesUpdate, KeyChain keyChain) : base(expectedNEntries, syncPrefix, DEFAULT_SYNC_REPLY_FRESHNESS_PERIOD) {
			this.pendingEntries_ = new Hashtable<Name, PendingEntryInfoFull>();
			this.outstandingInterestName_ = new Name();
			construct(face, onNamesUpdate, keyChain,
					DEFAULT_SYNC_INTEREST_LIFETIME, new SigningInfo(), null, null);
		}
	
		private void construct(Face face, FullPSync2017.OnNamesUpdate  onNamesUpdate,
				KeyChain keyChain, double syncInterestLifetime,
				SigningInfo signingInfo, FullPSync2017.CanAddToSyncData  canAddToSyncData,
				FullPSync2017.CanAddReceivedName  canAddReceivedName) {
			face_ = face;
			keyChain_ = keyChain;
			syncInterestLifetime_ = syncInterestLifetime;
			signingInfo_ = new SigningInfo(signingInfo);
			onNamesUpdate_ = onNamesUpdate;
			canAddToSyncData_ = canAddToSyncData;
			canAddReceivedName_ = canAddReceivedName;
			segmentPublisher_ = new PSyncSegmentPublisher(face_, keyChain_);
	
			registeredPrefix_ = face_.registerPrefix(syncPrefix_,
					new FullPSync2017.Anonymous_C4 (this), new FullPSync2017.Anonymous_C3 ());
	
			// TODO: Should we do this after the registerPrefix onSuccess callback?
			sendSyncInterest();
		}
	
		/// <summary>
		/// Publish the Name to inform the others. However, if the Name has already
		/// been published, do nothing.
		/// </summary>
		///
		/// <param name="name">The Name to publish.</param>
		public void publishName(Name name) {
			if (nameToHash_.Contains(name)) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Already published, ignoring: {0}", name);
				return;
			}
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Publish: {0}", name);
			insertIntoIblt(name);
			satisfyPendingInterests();
		}
	
		/// <summary>
		/// Remove the Name from the IBLT so that it won't be announced to other users.
		/// </summary>
		///
		/// <param name="name">The Name to remove.</param>
		internal void removeName(Name name) {
			removeFromIblt(name);
		}
	
		public const double DEFAULT_SYNC_INTEREST_LIFETIME = 1000;
		public const double DEFAULT_SYNC_REPLY_FRESHNESS_PERIOD = 1000;
	
		public class PendingEntryInfoFull {
			public PendingEntryInfoFull(InvertibleBloomLookupTable iblt) {
				this.isRemoved_ = false;
				iblt_ = iblt;
			}
	
			public readonly InvertibleBloomLookupTable iblt_;
			public bool isRemoved_;
		} 
	
		/// <summary>
		/// Send the sync interest for full synchronization. This forms the interest
		/// name: /<sync-prefix>/<own-IBLT>. This cancels any pending sync interest
		/// we sent earlier on the face.
		/// </summary>
		///
		internal void sendSyncInterest() {
			/** Debug: Implement stopping an ongoing fetch.
			    // If we send two sync interest one after the other
			    // since there is no new data in the network yet,
			    // when data is available it may satisfy both of them
			    if (fetcher_) {
			      fetcher_->stop();
			    }
			*/
	
			// Sync Interest format for full sync: /<sync-prefix>/<ourLatestIBF>
			Name syncInterestName = new Name(syncPrefix_);
	
			// Append our latest IBLT.
			Blob encodedIblt;
			try {
				encodedIblt = iblt_.encode();
			} catch (IOException ex) {
				// We don't expect this error.
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "sendSyncInterest: Error in IBLT encode",
						ex);
				return;
			}
			syncInterestName.append(encodedIblt);
	
			outstandingInterestName_ = syncInterestName;
	
			// random1 is from 0.0 to 1.0.
			// random1 is from 0.0 to 1.0.
			double random1 = random_.nextDouble();
			// Get a jitter of +/- syncInterestLifetime_ * 0.2 .
			double jitter = (random1 - 0.5d) * (syncInterestLifetime_ * 0.2d);
	
			face_.callLater(syncInterestLifetime_ / 2 + jitter, new FullPSync2017.Anonymous_C2 (this));
	
			Interest syncInterest_0 = new Interest(syncInterestName);
			syncInterest_0.setInterestLifetimeMilliseconds(syncInterestLifetime_);
			syncInterest_0.setNonce(new Blob(new byte[4], false));
			syncInterest_0.refreshNonce();
	
			net.named_data.jndn.util.SegmentFetcher.fetch(face_, syncInterest_0,
					net.named_data.jndn.util.SegmentFetcher.DontVerifySegment,
					new FullPSync2017.Anonymous_C1 (this, syncInterest_0), this);
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "sendFullSyncInterest, nonce: "
					+ syncInterest_0.getNonce().toHex() + ", hash: "
					+ syncInterestName.GetHashCode());
		}
	
		/// <summary>
		/// Process a sync interest received from another party.
		/// This gets the difference between our IBLT and the IBLT in the other sync
		/// interest. If we cannot get the difference successfully, then send an
		/// application Nack. If we have some things in our IBLT that the other side
		/// does not have, then reply with the content. Or, if the number of
		/// different items is greater than threshold or equals zero, then send a
		/// Nack. Otherwise add the sync interest into the pendingEntries_ map with
		/// the interest name as the key and a PendingEntryInfoFull as the value.
		/// </summary>
		///
		/// <param name="prefixName">The prefix Name for the sync group which we registered.</param>
		/// <param name="interest_0">The the received Interest.</param>
		internal void onSyncInterest(Name prefixName, Interest interest_0,
				Face face, long interestFilterId, InterestFilter filter) {
			try {
				if (segmentPublisher_.replyFromStore(interest_0.getName()))
					return;
			} catch (IOException ex) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "onSyncInterest: Error in replyFromStore",
						ex);
				return;
			}
	
			Name nameWithoutSyncPrefix = interest_0.getName().getSubName(
					prefixName.size());
			Name interestName;
	
			if (nameWithoutSyncPrefix.size() == 1)
				// Get /<prefix>/IBLT from /<prefix>/IBLT
				interestName = interest_0.getName();
			else if (nameWithoutSyncPrefix.size() == 3)
				// Get /<prefix>/IBLT from /<prefix>/IBLT/<version>/<segment-no>
				interestName = interest_0.getName().getPrefix(-2);
			else
				return;
	
			Name.Component ibltName = interestName.get(-1);
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE,
					"Full Sync Interest received, nonce: "
							+ interest_0.getNonce().toHex() + ", hash:"
							+ interestName.GetHashCode());
	
			InvertibleBloomLookupTable iblt = new InvertibleBloomLookupTable(
					new InvertibleBloomLookupTable(expectedNEntries_));
			try {
				iblt.initialize(ibltName.getValue());
			} catch (Exception ex_1) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "onSyncInterest: Error in IBLT decode", ex_1);
				return;
			}
	
			InvertibleBloomLookupTable difference = iblt_.difference(iblt);
	
			HashedSet<Int64> positive = new HashedSet<Int64>();
			HashedSet<Int64> negative = new HashedSet<Int64>();
	
			if (!difference.listEntries(positive, negative)) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Cannot decode differences, positive: "
						+ positive.Count + " negative: " + negative.Count
						+ " threshold: " + threshold_);
	
				// Send all data if greater than the threshold, or if there are neither
				// positive nor negative differences. Otherwise, continue below and send
				// the positive as usual.
				if (positive.Count + negative.Count >= threshold_
						|| (positive.Count == 0 && negative.Count == 0)) {
					PSyncState state1 = new PSyncState();
					/* foreach */
					foreach (Name name  in  new ILOG.J2CsMapping.Collections.ListSet(nameToHash_.Keys))
						state1.addContent(name);
	
					if (state1.getContent().Count > 0) {
						try {
							segmentPublisher_.publish(interest_0.getName(),
									interest_0.getName(), state1.wireEncode(),
									syncReplyFreshnessPeriod_, signingInfo_);
						} catch (Exception ex_2) {
							logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
									"onSyncInterest: Error in publish", ex_2);
							return;
						}
					}
	
					return;
				}
			}
	
			PSyncState state = new PSyncState();
			/* foreach */
			foreach (Int64 hash  in  positive) {
				Name name_3 = ILOG.J2CsMapping.Collections.Collections.Get(hashToName_,hash);
	
				if (nameToHash_.Contains(name_3)) {
					if (canAddToSyncData_ == null
							|| canAddToSyncData_.canAddToSyncData(name_3, negative))
						state.addContent(name_3);
				}
			}
	
			if (state.getContent().Count > 0) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Sending sync content: " + state);
				try {
					sendSyncData(interestName, state.wireEncode());
				} catch (Exception ex_4) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
							"onSyncInterest: Error in sendSyncData", ex_4);
				}
	
				return;
			}
	
			FullPSync2017.PendingEntryInfoFull  entry_5 = new FullPSync2017.PendingEntryInfoFull (iblt);
			ILOG.J2CsMapping.Collections.Collections.Put(pendingEntries_,interestName,entry_5);
			face_.callLater(interest_0.getInterestLifetimeMilliseconds(),
					new FullPSync2017.Anonymous_C0 (this, interest_0, entry_5));
		}
	
		public void onError(SegmentFetcher.ErrorCode errorCode, String message) {
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Cannot fetch sync data, error: " + errorCode
					+ " message: " + message);
		}
	
		/// <summary>
		/// Send the sync Data. Check if the data will satisfy our own pending
		/// Interest. If it does, then remove it and then renew the sync interest.
		/// Otherwise, just send the Data.
		/// </summary>
		///
		/// <param name="name">The basis to use for the Data name.</param>
		/// <param name="content">The content of the Data.</param>
		private void sendSyncData(Name name, Blob content) {
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE,
					"Checking if the Data will satisfy our own pending interest");
	
			Name nameWithIblt = new Name();
			nameWithIblt.append(iblt_.encode());
	
			// Append the hash of our IBLT so that the Data name should be different for
			// each node.
			Name dataName = new Name(name).appendNumber(nameWithIblt.GetHashCode());
	
			// Check if our own Interest got satisfied.
			if (outstandingInterestName_.equals(name)) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Satisfies our own pending Interest");
				// remove outstanding interest
				/** Debug: Implement stopping an ongoing fetch.
				      if (fetcher_) {
				        _LOG_DEBUG("Removing our pending interest from the Face (stopping fetcher)");
				        fetcher_->stop();
				        outstandingInterestName_ = Name();
				      }
				**/
				outstandingInterestName_ = new Name();
	
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Sending sync Data");
	
				// Send Data after removing the pending sync interest on the Face.
				segmentPublisher_.publish(name, dataName, content,
						syncReplyFreshnessPeriod_, signingInfo_);
	
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "sendSyncData: Renewing sync interest");
				sendSyncInterest();
			} else {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE,
						"Sending Sync Data for not our own Interest");
				segmentPublisher_.publish(name, dataName, content,
						syncReplyFreshnessPeriod_, signingInfo_);
			}
		}
	
		/// <summary>
		/// Process the sync data after the content is assembled by the
		/// SegmentFetcher. Call deletePendingInterests to delete any pending sync
		/// Interest with the Interest name, which would have been satisfied by the
		/// forwarder once it got the data. For each name in the data content, check
		/// that we don't already have the name, and call _canAddReceivedName (which
		/// may process the name as a prefix/sequenceNo). Call onUpdate_ to notify
		/// the application about the updates. Call sendSyncInterest because the last
		/// one was satisfied by the incoming data.
		/// </summary>
		///
		/// <param name="encodedContent"></param>
		/// <param name="interest_0">The Interest for which we got the data.</param>
		internal void onSyncData(Blob encodedContent, Interest interest_0) {
			deletePendingInterests(interest_0.getName());
	
			PSyncState state;
			try {
				state = new PSyncState(encodedContent);
			} catch (EncodingException ex) {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "onSyncData: Error in PSyncState decode",
						ex);
				return;
			}
			ArrayList<Name> names = new ArrayList<Name>();
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Sync Data Received: {0}", state);
	
			ArrayList<Name> content = state.getContent();
			/* foreach */
			foreach (Name contentName  in  content) {
				if (!nameToHash_.Contains(contentName)) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Checking whether to add {0}",
							contentName);
					if (canAddReceivedName_ == null
							|| canAddReceivedName_.canAddReceivedName(contentName)) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Adding name {0}", contentName);
						ILOG.J2CsMapping.Collections.Collections.Add(names,contentName);
						insertIntoIblt(contentName);
					}
					// We should not call satisfyPendingSyncInterests here because we just
					// got data and deleted pending interests by calling deletePendingInterests.
					// But we might have interests which don't match this interest that might
					// not have been deleted from the pending sync interests.
				}
			}
	
			// We just got the data, so send a new sync Interest.
			if (names.Count > 0) {
				try {
					onNamesUpdate_.onNamesUpdate(names);
				} catch (Exception ex_1) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onNamesUpdate", ex_1);
				}
	
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "onSyncData: Renewing sync interest");
				sendSyncInterest();
			} else {
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "No new update, interest nonce: "
						+ interest_0.getNonce().toHex() + " , hash: "
						+ interest_0.getName().GetHashCode());
			}
		}
	
		/// <summary>
		/// Satisfy pending sync Interests. For a pending sync interests, if the
		/// IBLT of the sync Interest has any difference from our own IBLT, then
		/// send a Data back. If we can't decode the difference from the stored IBLT,
		/// then delete it.
		/// </summary>
		///
		private void satisfyPendingInterests() {
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Satisfying full sync Interest: "
					+ pendingEntries_.Count);
	
			// First copy the keys, to not change the HashMap while iterating.
			HashedSet<Name> keys = new HashedSet<Name>();
			/* foreach */
			foreach (Name key  in  new ILOG.J2CsMapping.Collections.ListSet(pendingEntries_.Keys))
				ILOG.J2CsMapping.Collections.Collections.Add(keys,key);
	
			/* foreach */
			foreach (Name keyName  in  keys) {
				FullPSync2017.PendingEntryInfoFull  pendingEntry = ILOG.J2CsMapping.Collections.Collections.Get(pendingEntries_,keyName);
	
				InvertibleBloomLookupTable entryIblt = pendingEntry.iblt_;
				InvertibleBloomLookupTable difference = iblt_.difference(entryIblt);
				HashedSet<Int64> positive = new HashedSet<Int64>();
				HashedSet<Int64> negative = new HashedSet<Int64>();
	
				if (!difference.listEntries(positive, negative)) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Decode failed for pending interest");
					if (positive.Count + negative.Count >= threshold_
							|| (positive.Count == 0 && negative.Count == 0)) {
						logger_.log(
								ILOG.J2CsMapping.Util.Logging.Level.INFO,
								"positive + negative > threshold or no difference can be found. Erase pending interest.");
						// Prevent delayedRemovePendingEntry from removing a new entry with the same Name.
						pendingEntry.isRemoved_ = true;
						ILOG.J2CsMapping.Collections.Collections.Remove(pendingEntries_,keyName);
						continue;
					}
				}
	
				PSyncState state = new PSyncState();
				/* foreach */
				foreach (Int64 hash  in  positive) {
					Name name = ILOG.J2CsMapping.Collections.Collections.Get(hashToName_,hash);
	
					if (nameToHash_.Contains(name))
						state.addContent(name);
				}
	
				if (state.getContent().Count > 0) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Satisfying sync content: {0}", state);
					try {
						sendSyncData(keyName, state.wireEncode());
					} catch (Exception ex) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
								"satisfyPendingInterests: Error in sendSyncData",
								ex);
					}
					// Prevent delayedRemovePendingEntry from removing a new entry with the same Name.
					pendingEntry.isRemoved_ = true;
					ILOG.J2CsMapping.Collections.Collections.Remove(pendingEntries_,keyName);
				}
			}
		}
	
		/// <summary>
		/// Delete pending sync Interests that match the given name.
		/// </summary>
		///
		private void deletePendingInterests(Name interestName) {
			FullPSync2017.PendingEntryInfoFull  entry_0 = ILOG.J2CsMapping.Collections.Collections.Get(pendingEntries_,interestName);
			if (entry_0 == null)
				return;
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Delete pending interest: {0}", interestName);
			// Prevent delayedRemovePendingEntry from removing a new entry with the same Name.
			entry_0.isRemoved_ = true;
			ILOG.J2CsMapping.Collections.Collections.Remove(pendingEntries_,interestName);
		}
	
		/// <summary>
		/// Remove the entry from pendingEntries_ which has the name. However, if
		/// entry.isRemoved_ is true, do nothing. Therefore, if an entry is
		/// directly removed from pendingEntries_, it should set isRemoved_.
		/// </summary>
		///
		/// <param name="name">The key in the pendingEntries_ map for the entry to remove.</param>
		/// <param name="entry_0"></param>
		/// <param name="nonce">This is only used for the log message.</param>
		internal void delayedRemovePendingEntry(Name name,
				FullPSync2017.PendingEntryInfoFull  entry_0, Blob nonce) {
			if (entry_0.isRemoved_)
				// A previous operation already removed this entry, so don't try again to
				// remove the entry with the Name in case it is a new entry.
				return;
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.FINE, "Remove Pending Interest {0}", nonce.toHex());
			entry_0.isRemoved_ = true;
			ILOG.J2CsMapping.Collections.Collections.Remove(pendingEntries_,name);
		}
	
		private Face face_;
		private KeyChain keyChain_;
		private SigningInfo signingInfo_;
		private PSyncSegmentPublisher segmentPublisher_;
		private readonly Hashtable<Name, PendingEntryInfoFull> pendingEntries_;
		private double syncInterestLifetime_;
		private FullPSync2017.OnNamesUpdate  onNamesUpdate_;
		private FullPSync2017.CanAddToSyncData  canAddToSyncData_;
		private FullPSync2017.CanAddReceivedName  canAddReceivedName_;
		private Name outstandingInterestName_;
		private long registeredPrefix_;
		private static readonly Random random_ = new Random();
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(FullPSync2017).FullName);
	}
}
