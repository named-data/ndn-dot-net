// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.impl {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A PendingInterestTable is an internal class to hold a list of pending
	/// interests with their callbacks.
	/// </summary>
	///
	public class PendingInterestTable {
		public PendingInterestTable() {
			this.table_ = new ArrayList<Entry>();
			this.removeRequests_ = new ArrayList<Int64>();
		}
		/// <summary>
		/// Entry holds the callbacks and other fields for an entry in the pending
		/// interest table.
		/// </summary>
		///
		public class Entry {
			/// <summary>
			/// Create a new Entry with the given fields. Note: You should not call this
			/// directly but call PendingInterestTable.add.
			/// </summary>
			///
			public Entry(long pendingInterestId, Interest interest, OnData onData,
					OnTimeout onTimeout, OnNetworkNack onNetworkNack) {
				this.isRemoved_ = false;
				pendingInterestId_ = pendingInterestId;
				interest_ = interest;
				onData_ = onData;
				onTimeout_ = onTimeout;
				onNetworkNack_ = onNetworkNack;
			}
	
			/// <summary>
			/// Get the pendingInterestId given to the constructor.
			/// </summary>
			///
			/// <returns>The pendingInterestId.</returns>
			public long getPendingInterestId() {
				return pendingInterestId_;
			}
	
			/// <summary>
			/// Get the interest given to the constructor (from Face.expressInterest).
			/// </summary>
			///
			/// <returns>The interest. NOTE: You must not change the interest object - if
			/// you need to change it then make a copy.</returns>
			public Interest getInterest() {
				return interest_;
			}
	
			/// <summary>
			/// Get the OnData callback given to the constructor.
			/// </summary>
			///
			/// <returns>The OnData callback.</returns>
			public OnData getOnData() {
				return onData_;
			}
	
			/// <summary>
			/// Get the OnNetworkNack callback given to the constructor.
			/// </summary>
			///
			/// <returns>The OnNetworkNack callback.</returns>
			public OnNetworkNack getOnNetworkNack() {
				return onNetworkNack_;
			}
	
			/// <summary>
			/// Set the isRemoved flag which is returned by getIsRemoved().
			/// </summary>
			///
			public void setIsRemoved() {
				isRemoved_ = true;
			}
	
			/// <summary>
			/// Check if setIsRemoved() was called.
			/// </summary>
			///
			/// <returns>True if setIsRemoved() was called.</returns>
			public bool getIsRemoved() {
				return isRemoved_;
			}
	
			/// <summary>
			/// Call onTimeout_ (if defined). This ignores exceptions from the call to
			/// onTimeout_.
			/// </summary>
			///
			public void callTimeout() {
				if (onTimeout_ != null) {
					try {
						onTimeout_.onTimeout(interest_);
					} catch (Exception ex) {
						net.named_data.jndn.impl.PendingInterestTable.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onTimeout", ex);
					}
				}
			}
	
			private readonly Interest interest_;
			private readonly long pendingInterestId_;
			/// <summary>
			/// < A unique identifier for this entry so it can be deleted 
			/// </summary>
			///
			private readonly OnData onData_;
			private readonly OnTimeout onTimeout_;
			private readonly OnNetworkNack onNetworkNack_;
			private bool isRemoved_;
		}
	
		/// <summary>
		/// Add a new entry to the pending interest table. However, if
		/// removePendingInterest was already called with the pendingInterestId, don't
		/// add an entry and return null.
		/// </summary>
		///
		/// <param name="pendingInterestId"></param>
		/// <param name="interestCopy"></param>
		/// <param name="onData"></param>
		/// <param name="onTimeout"></param>
		/// <param name="onNetworkNack"></param>
		/// <returns>The new PendingInterestTable.Entry, or null if
		/// removePendingInterest was already called with the pendingInterestId.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public PendingInterestTable.Entry  add(long pendingInterestId,
				Interest interestCopy, OnData onData, OnTimeout onTimeout,
				OnNetworkNack onNetworkNack) {
			int removeRequestIndex = removeRequests_.indexOf(pendingInterestId);
			if (removeRequestIndex >= 0) {
				// removePendingInterest was called with the pendingInterestId returned by
				//   expressInterest before we got here, so don't add a PIT entry.
				ILOG.J2CsMapping.Collections.Collections.RemoveAt(removeRequests_,removeRequestIndex);
				return null;
			}
	
			PendingInterestTable.Entry  entry = new PendingInterestTable.Entry (pendingInterestId, interestCopy, onData,
					onTimeout, onNetworkNack);
			ILOG.J2CsMapping.Collections.Collections.Add(table_,entry);
			return entry;
		}
	
		/// <summary>
		/// Find all entries from the pending interest table where data conforms to
		/// the entry's interest selectors, remove the entries from the table, set each
		/// entry's isRemoved flag, and add to the entries list.
		/// </summary>
		///
		/// <param name="data">The incoming Data packet to find the interest for.</param>
		/// <param name="entries"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void extractEntriesForExpressedInterest(
				Data data, ArrayList<Entry> entries) {
			// Go backwards through the list so we can remove entries.
			for (int i = table_.Count - 1; i >= 0; --i) {
				PendingInterestTable.Entry  pendingInterest = table_[i];
	
				if (pendingInterest.getInterest().matchesData(data)) {
					ILOG.J2CsMapping.Collections.Collections.Add(entries,table_[i]);
					// We let the callback from callLater call _processInterestTimeout, but
					// for efficiency, mark this as removed so that it returns right away.
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(table_,i);
					pendingInterest.setIsRemoved();
				}
			}
		}
	
		/// <summary>
		/// Find all entries from the pending interest table where the OnNetworkNack
		/// callback is not null and the entry's interest is the same as the given
		/// interest, remove the entries from the table, set each entry's isRemoved
		/// flag, and add to the entries list. (We don't remove the entry if the
		/// OnNetworkNack callback is null so that OnTimeout will be called later.) The
		/// interests are the same if their default wire encoding is the same (which
		/// has everything including the name, nonce, link object and selectors).
		/// </summary>
		///
		/// <param name="interest">The Interest to search for (typically from a Nack packet).</param>
		/// <param name="entries"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void extractEntriesForNackInterest(
				Interest interest, ArrayList<Entry> entries) {
			SignedBlob encoding = interest.wireEncode();
	
			// Go backwards through the list so we can remove entries.
			for (int i = table_.Count - 1; i >= 0; --i) {
				PendingInterestTable.Entry  pendingInterest = table_[i];
				if (pendingInterest.getOnNetworkNack() == null)
					continue;
	
				// wireEncode returns the encoding cached when the interest was sent (if
				// it was the default wire encoding).
				if (pendingInterest.getInterest().wireEncode().equals(encoding)) {
					ILOG.J2CsMapping.Collections.Collections.Add(entries,table_[i]);
					// We let the callback from callLater call _processInterestTimeout, but
					// for efficiency, mark this as removed so that it returns right away.
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(table_,i);
					pendingInterest.setIsRemoved();
				}
			}
		}
	
		/// <summary>
		/// Remove the pending interest entry with the pendingInterestId from the
		/// pending interest table and set its isRemoved flag. This does not affect
		/// another pending interest with a different pendingInterestId, even if it has
		/// the same interest name. If there is no entry with the pendingInterestId, do
		/// nothing.
		/// </summary>
		///
		/// <param name="pendingInterestId">The ID returned from expressInterest.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void removePendingInterest(long pendingInterestId) {
			int count = 0;
			// Go backwards through the list so we can remove entries.
			// Remove all entries even though pendingInterestId should be unique.
			for (int i = table_.Count - 1; i >= 0; --i) {
				if ((table_[i]).getPendingInterestId() == pendingInterestId) {
					++count;
					// For efficiency, mark this as removed so that
					// processInterestTimeout doesn't look for it.
					(table_[i]).setIsRemoved();
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(table_,i);
				}
			}
	
			if (count == 0)
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.WARNING,
						"removePendingInterest: Didn't find pendingInterestId {0}",
						pendingInterestId);
	
			if (count == 0) {
				// The pendingInterestId was not found. Perhaps this has been called before
				//   the callback in expressInterest can add to the PIT. Add this
				//   removal request which will be checked before adding to the PIT.
				if (removeRequests_.indexOf(pendingInterestId) < 0)
					// Not already requested, so add the request.
					ILOG.J2CsMapping.Collections.Collections.Add(removeRequests_,pendingInterestId);
			}
		}
	
		/// <summary>
		/// Remove the specific pendingInterest entry from the table and set its
		/// isRemoved flag. However, if the pendingInterest isRemoved flag is already
		/// true or the entry is not in the pending interest table then do nothing.
		/// </summary>
		///
		/// <param name="pendingInterest">The Entry from the pending interest table.</param>
		/// <returns>True if the entry was removed, false if not.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool removeEntry(PendingInterestTable.Entry  pendingInterest) {
			if (pendingInterest.getIsRemoved())
				// extractEntriesForExpressedInterest or removePendingInterest has
				// removed pendingInterest from the table, so we don't need to look for it.
				// Do nothing.
				return false;
	
			if (ILOG.J2CsMapping.Collections.Collections.Remove(table_,pendingInterest)) {
				pendingInterest.setIsRemoved();
				return true;
			} else
				return false;
		}
	
		private readonly ArrayList<Entry> table_;
		private readonly ArrayList<Int64> removeRequests_;
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(PendingInterestTable).FullName);
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
