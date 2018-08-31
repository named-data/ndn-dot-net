// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	
	/// <summary>
	/// InMemoryStoragePersistent provides an application cache with persistent
	/// in-memory storage, of which no replacement policy will be employed. Entries
	/// will only be deleted by explicit application control.
	/// </summary>
	///
	public class InMemoryStoragePersistent {
		public InMemoryStoragePersistent() {
			this.cache_ = new Hashtable();
		}
	
		/// <summary>
		/// Insert a Data packet. If a Data packet with the same name, including the
		/// implicit digest, already exists, replace it. 
		/// </summary>
		///
		/// <param name="data">The packet to insert, which is copied.</param>
		/// <exception cref="EncodingException">for error encoding the Data packet to get theimplicit digest.</exception>
		public void insert(Data data) {
			ILOG.J2CsMapping.Collections.Collections.Put(cache_,data.getFullName(),new Data(data));
		}
	
		/// <summary>
		/// Find the best match Data for an Interest.
		/// </summary>
		///
		/// <param name="interest">The Interest with the Name of the Data packet to find.</param>
		/// <returns>The best match if any, otherwise null. You should not modify the
		/// returned object. If you need to modify it then you must make a copy.</returns>
		public Data find(Interest interest) {
			/* foreach */
			foreach (Object entry  in  cache_) {
				// Debug: Check selectors, especially CanBePrefix.
				if (interest.getName().isPrefixOf(
						(Name) ((DictionaryEntry) ((DictionaryEntry) entry)).Key))
					return (Data) ((DictionaryEntry) ((DictionaryEntry) entry)).Value;
			}
	
			return null;
		}
	
		/// <summary>
		/// Get the number of packets stored in the in-memory storage.
		/// </summary>
		///
		/// <returns>The number of packets.</returns>
		public int size() {
			return cache_.Count;
		}
	
		/// <summary>
		/// Get the the storage cache, which should only be used for testing.
		/// </summary>
		///
		/// <returns>The storage cache.</returns>
		public Hashtable getCache_() {
			return cache_;
		}
	
		// Use HashMap without generics so it works with older Java compilers.
		private readonly Hashtable cache_;
		/**< The map key is the Data packet Name. The value is a Data. */
	}
}