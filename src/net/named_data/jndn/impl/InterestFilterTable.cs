// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2019 Regents of the University of California.
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
	using net.named_data.jndn.util;
	
	/// <summary>
	/// An InterestFilterTable is an internal class to hold a list of entries with
	/// an interest Filter and its OnInterestCallback.
	/// </summary>
	///
	public class InterestFilterTable {
		public InterestFilterTable() {
			this.table_ = new ArrayList<Entry>();
		}
		/// <summary>
		/// An Entry holds an interestFilterId, an InterestFilter and the
		/// OnInterestCallback with its related Face.
		/// </summary>
		///
		public class Entry {
			/// <summary>
			/// Create a new Entry with the given values.
			/// </summary>
			///
			/// <param name="interestFilterId">The ID from Node.getNextEntryId().</param>
			/// <param name="filter">The InterestFilter for this entry.</param>
			/// <param name="onInterest">The callback to call.</param>
			/// <param name="face"></param>
			public Entry(long interestFilterId, InterestFilter filter,
					OnInterestCallback onInterest, Face face) {
				interestFilterId_ = interestFilterId;
				filter_ = filter;
				onInterest_ = onInterest;
				face_ = face;
			}
	
			/// <summary>
			/// Get the interestFilterId given to the constructor.
			/// </summary>
			///
			/// <returns>The interestFilterId.</returns>
			public long getInterestFilterId() {
				return interestFilterId_;
			}
	
			/// <summary>
			/// Get the InterestFilter given to the constructor.
			/// </summary>
			///
			/// <returns>The InterestFilter.</returns>
			public InterestFilter getFilter() {
				return filter_;
			}
	
			/// <summary>
			/// Get the OnInterestCallback given to the constructor.
			/// </summary>
			///
			/// <returns>The OnInterestCallback.</returns>
			public OnInterestCallback getOnInterest() {
				return onInterest_;
			}
	
			/// <summary>
			/// Get the Face given to the constructor.
			/// </summary>
			///
			/// <returns>The Face.</returns>
			public Face getFace() {
				return face_;
			}
	
			private readonly long interestFilterId_;
			/// <summary>
			/// < A unique identifier for this entry so it can be deleted 
			/// </summary>
			///
			private readonly InterestFilter filter_;
			private readonly OnInterestCallback onInterest_;
			private readonly Face face_;
		}
	
		/// <summary>
		/// Add a new entry to the table.
		/// </summary>
		///
		/// <param name="interestFilterId">The ID from Node.getNextEntryId().</param>
		/// <param name="filter">The InterestFilter for this entry.</param>
		/// <param name="onInterest">The callback to call.</param>
		/// <param name="face"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void setInterestFilter(long interestFilterId,
				InterestFilter filter, OnInterestCallback onInterest, Face face) {
			ILOG.J2CsMapping.Collections.Collections.Add(table_,new InterestFilterTable.Entry (interestFilterId, filter, onInterest, face));
		}
	
		/// <summary>
		/// Find all entries from the interest filter table where the interest conforms
		/// to the entry's filter, and add to the matchedFilters list.
		/// </summary>
		///
		/// <param name="interest">The interest which may match the filter in multiple entries.</param>
		/// <param name="matchedFilters"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void getMatchedFilters(Interest interest,
				ArrayList matchedFilters) {
			for (int i = 0; i < table_.Count; ++i) {
				InterestFilterTable.Entry  entry = table_[i];
				if (entry.getFilter().doesMatch(interest.getName()))
					ILOG.J2CsMapping.Collections.Collections.Add(matchedFilters,entry);
			}
		}
	
		/// <summary>
		/// Remove the interest filter entry which has the interestFilterId from the
		/// interest filter table. This does not affect another interest filter with
		/// a different interestFilterId, even if it has the same prefix name.
		/// If there is no entry with the interestFilterId, do nothing.
		/// </summary>
		///
		/// <param name="interestFilterId">The ID returned from setInterestFilter.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void unsetInterestFilter(long interestFilterId) {
			int count = 0;
			// Go backwards through the list so we can remove entries.
			// Remove all entries even though interestFilterId should be unique.
			for (int i = table_.Count - 1; i >= 0; --i) {
				if ((table_[i]).getInterestFilterId() == interestFilterId) {
					++count;
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(table_,i);
				}
			}
	
			if (count == 0)
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.WARNING,
						"unsetInterestFilter: Didn't find interestFilterId {0}",
						interestFilterId);
		}
	
		private readonly ArrayList<Entry> table_;
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(InterestFilterTable).FullName);
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
