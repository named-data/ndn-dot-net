// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2017 Regents of the University of California.
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
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	
	/// <summary>
	/// A MemoryContentCache holds a set of Data packets and answers an Interest to
	/// return the correct Data packet. The cache is periodically cleaned up to
	/// remove each stale Data packet based on its FreshnessPeriod (if it has one).
	/// </summary>
	///
	/// @note This class is an experimental feature.  See the API docs for more detail at
	/// http://named-data.net/doc/ndn-ccl-api/memory-content-cache.html .
	public class MemoryContentCache : OnInterestCallback {
		/// <summary>
		/// Create a new MemoryContentCache to use the given Face.
		/// </summary>
		///
		/// <param name="face"></param>
		/// <param name="cleanupIntervalMilliseconds">large number, then effectively the stale content will not be removed from the cache.</param>
		public MemoryContentCache(Face face, double cleanupIntervalMilliseconds) {
			this.onDataNotFoundForPrefix_ = new Hashtable();
			this.interestFilterIdList_ = new ArrayList<Int64>();
			this.registeredPrefixIdList_ = new ArrayList<Int64>();
			this.noStaleTimeCache_ = new ArrayList<Content>();
			this.staleTimeCache_ = new ArrayList<StaleTimeContent>();
			this.emptyComponent_ = new Name.Component();
			this.pendingInterestTable_ = new ArrayList<PendingInterest>();
			face_ = face;
			cleanupIntervalMilliseconds_ = cleanupIntervalMilliseconds;
			construct();
		}
	
		/// <summary>
		/// Create a new MemoryContentCache to use the given Face, with a default
		/// cleanupIntervalMilliseconds of 1000.0 milliseconds.
		/// </summary>
		///
		/// <param name="face"></param>
		public MemoryContentCache(Face face) {
			this.onDataNotFoundForPrefix_ = new Hashtable();
			this.interestFilterIdList_ = new ArrayList<Int64>();
			this.registeredPrefixIdList_ = new ArrayList<Int64>();
			this.noStaleTimeCache_ = new ArrayList<Content>();
			this.staleTimeCache_ = new ArrayList<StaleTimeContent>();
			this.emptyComponent_ = new Name.Component();
			this.pendingInterestTable_ = new ArrayList<PendingInterest>();
			face_ = face;
			cleanupIntervalMilliseconds_ = 1000.0d;
			construct();
		}
	
		private void construct() {
			nextCleanupTime_ = net.named_data.jndn.util.Common.getNowMilliseconds()
					+ cleanupIntervalMilliseconds_;
	
			storePendingInterestCallback_ = new MemoryContentCache.Anonymous_C0 (this);
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onRegisterSuccess">receives a success message from the forwarder. If onRegisterSuccess is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onDataNotFound">onDataNotFound.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="flags">See Face.registerPrefix.</param>
		/// <param name="wireFormat">See Face.registerPrefix.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed,
				OnRegisterSuccess onRegisterSuccess,
				OnInterestCallback onDataNotFound, ForwardingFlags flags,
				WireFormat wireFormat) {
			if (onDataNotFound != null)
				ILOG.J2CsMapping.Collections.Collections.Put(onDataNotFoundForPrefix_,prefix.toUri(),onDataNotFound);
			long registeredPrefixId = face_.registerPrefix(prefix, this,
					onRegisterFailed, onRegisterSuccess, flags, wireFormat);
			ILOG.J2CsMapping.Collections.Collections.Add(registeredPrefixIdList_,registeredPrefixId);
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// This uses the default WireFormat.getDefaultWireFormat().
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onRegisterSuccess">receives a success message from the forwarder. If onRegisterSuccess is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onDataNotFound">onInterest.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it.</param>
		/// <param name="flags">See Face.registerPrefix.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed,
				OnRegisterSuccess onRegisterSuccess,
				OnInterestCallback onDataNotFound, ForwardingFlags flags) {
			registerPrefix(prefix, onRegisterFailed, onRegisterSuccess,
					onDataNotFound, flags, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// This uses the default WireFormat.getDefaultWireFormat().
		/// Use default ForwardingFlags.
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onRegisterSuccess">receives a success message from the forwarder. If onRegisterSuccess is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onDataNotFound">onDataNotFound.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed,
				OnRegisterSuccess onRegisterSuccess,
				OnInterestCallback onDataNotFound) {
			registerPrefix(prefix, onRegisterFailed, onRegisterSuccess,
					onDataNotFound, new ForwardingFlags(),
					net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// Do not call a callback if a data packet is not found in the cache.
		/// This uses the default WireFormat.getDefaultWireFormat().
		/// Use default ForwardingFlags.
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onRegisterSuccess">receives a success message from the forwarder. If onRegisterSuccess is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed,
				OnRegisterSuccess onRegisterSuccess) {
			registerPrefix(prefix, onRegisterFailed, onRegisterSuccess, null,
					new ForwardingFlags(), net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onDataNotFound">onDataNotFound.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="flags">See Face.registerPrefix.</param>
		/// <param name="wireFormat">See Face.registerPrefix.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed,
				OnInterestCallback onDataNotFound, ForwardingFlags flags,
				WireFormat wireFormat) {
			registerPrefix(prefix, onRegisterFailed, null, onDataNotFound, flags,
					wireFormat);
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// This uses the default WireFormat.getDefaultWireFormat().
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onDataNotFound">onDataNotFound.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it.</param>
		/// <param name="flags">See Face.registerPrefix.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed,
				OnInterestCallback onDataNotFound, ForwardingFlags flags) {
			registerPrefix(prefix, onRegisterFailed, onDataNotFound, flags,
					net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// This uses the default WireFormat.getDefaultWireFormat().
		/// Use default ForwardingFlags.
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onDataNotFound">onDataNotFound.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed, OnInterestCallback onDataNotFound) {
			registerPrefix(prefix, onRegisterFailed, onDataNotFound,
					new ForwardingFlags(), net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Call registerPrefix on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name matches the filter.
		/// Alternatively, if the Face's registerPrefix has already been called, then
		/// you can call this object's setInterestFilter.
		/// Do not call a callback if a data packet is not found in the cache.
		/// This uses the default WireFormat.getDefaultWireFormat().
		/// Use default ForwardingFlags.
		/// </summary>
		///
		/// <param name="prefix">The Name for the prefix to register. This copies the Name.</param>
		/// <param name="onRegisterFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <exception cref="IOException">For I/O error in sending the registration request.</exception>
		/// <exception cref="System.Security.SecurityException">If signing a command interest for NFD and cannotfind the private key for the certificateName.</exception>
		public void registerPrefix(Name prefix,
				OnRegisterFailed onRegisterFailed) {
			registerPrefix(prefix, onRegisterFailed, null, new ForwardingFlags(),
					net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Call setInterestFilter on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name matches the filter.
		/// </summary>
		///
		/// <param name="filter"></param>
		/// <param name="onDataNotFound">onDataNotFound.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  Note: If you call setInterestFilter multiple times where filter.getPrefix() is the same, it is undetermined which onDataNotFound will be called. If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		public void setInterestFilter(InterestFilter filter,
				OnInterestCallback onDataNotFound) {
			if (onDataNotFound != null)
				ILOG.J2CsMapping.Collections.Collections.Put(onDataNotFoundForPrefix_,filter.getPrefix().toUri(),onDataNotFound);
			long interestFilterId = face_.setInterestFilter(filter, this);
			ILOG.J2CsMapping.Collections.Collections.Add(interestFilterIdList_,interestFilterId);
		}
	
		/// <summary>
		/// Call setInterestFilter on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Do not call a callback if a data packet is not found in the cache.
		/// </summary>
		///
		/// <param name="filter"></param>
		public void setInterestFilter(InterestFilter filter) {
			setInterestFilter(filter, null);
		}
	
		/// <summary>
		/// Call setInterestFilter on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// </summary>
		///
		/// <param name="prefix"></param>
		/// <param name="onDataNotFound">onDataNotFound.onInterest(prefix, interest, face, interestFilterId, filter). Your callback can find the Data packet for the interest and call face.putData(data).  Note: If you call setInterestFilter multiple times where filter.getPrefix() is the same, it is undetermined which onDataNotFound will be called. If your callback cannot find the Data packet, it can optionally call storePendingInterest(interest, face) to store the pending interest in this object to be satisfied by a later call to add(data). If you want to automatically store all pending interests, you can simply use getStorePendingInterest() for onDataNotFound. If onDataNotFound is null, this does not use it. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		public void setInterestFilter(Name prefix,
				OnInterestCallback onDataNotFound) {
			if (onDataNotFound != null)
				ILOG.J2CsMapping.Collections.Collections.Put(onDataNotFoundForPrefix_,prefix.toUri(),onDataNotFound);
			long interestFilterId = face_.setInterestFilter(prefix, this);
			ILOG.J2CsMapping.Collections.Collections.Add(interestFilterIdList_,interestFilterId);
		}
	
		/// <summary>
		/// Call setInterestFilter on the Face given to the constructor so that this
		/// MemoryContentCache will answer interests whose name has the prefix.
		/// Do not call a callback if a data packet is not found in the cache.
		/// </summary>
		///
		/// <param name="prefix"></param>
		public void setInterestFilter(Name prefix) {
			setInterestFilter(prefix, null);
		}
	
		/// <summary>
		/// Call Face.unsetInterestFilter and Face.removeRegisteredPrefix for all the
		/// prefixes given to the setInterestFilter and registerPrefix method on this
		/// MemoryContentCache object so that it will not receive interests any more.
		/// You can call this if you want to "shut down" this MemoryContentCache while
		/// your application is still running.
		/// </summary>
		///
		public void unregisterAll() {
			for (int i = 0; i < interestFilterIdList_.Count; ++i)
				face_.unsetInterestFilter((long) interestFilterIdList_[i]);
			ILOG.J2CsMapping.Collections.Collections.Clear(interestFilterIdList_);
	
			for (int i_0 = 0; i_0 < registeredPrefixIdList_.Count; ++i_0)
				face_.removeRegisteredPrefix((long) registeredPrefixIdList_[i_0]);
			ILOG.J2CsMapping.Collections.Collections.Clear(registeredPrefixIdList_);
	
			// Also clear each onDataNotFoundForPrefix given to registerPrefix.
			onDataNotFoundForPrefix_.clear();
		}
	
		/// <summary>
		/// Add the Data packet to the cache so that it is available to use to
		/// answer interests. If data.getMetaInfo().getFreshnessPeriod() is not
		/// negative, set the staleness time to now plus
		/// data.getMetaInfo().getFreshnessPeriod(), which is checked during cleanup to
		/// remove stale content. This also checks if cleanupIntervalMilliseconds
		/// milliseconds have passed and removes stale content from the cache. After
		/// removing stale content, remove timed-out pending interests from
		/// storePendingInterest(), then if the added Data packet satisfies any
		/// interest, send it through the face and remove the interest from the pending
		/// interest table.
		/// </summary>
		///
		/// <param name="data"></param>
		public void add(Data data) {
			doCleanup();
	
			if (data.getMetaInfo().getFreshnessPeriod() >= 0.0d) {
				// The content will go stale, so use staleTimeCache_.
				MemoryContentCache.StaleTimeContent  content = new MemoryContentCache.StaleTimeContent (data);
				// Insert into staleTimeCache, sorted on content.staleTimeMilliseconds.
				// Search from the back since we expect it to go there.
				int i = staleTimeCache_.Count - 1;
				while (i >= 0) {
					if (staleTimeCache_[i].getStaleTimeMilliseconds() <= content
							.getStaleTimeMilliseconds())
						break;
					--i;
				}
				// Element i is the greatest less than or equal to
				// content.staleTimeMilliseconds, so insert after it.
				staleTimeCache_.Insert(i + 1, content);
			} else
				// The data does not go stale, so use noStaleTimeCache_.
				ILOG.J2CsMapping.Collections.Collections.Add(noStaleTimeCache_,new MemoryContentCache.Content (data));
	
			// Remove timed-out interests and check if the data packet matches any
			// pending interest.
			// Go backwards through the list so we can erase entries.
			double nowMilliseconds = net.named_data.jndn.util.Common.getNowMilliseconds();
			for (int i_0 = pendingInterestTable_.Count - 1; i_0 >= 0; --i_0) {
				MemoryContentCache.PendingInterest  pendingInterest = pendingInterestTable_[i_0];
				if (pendingInterest.isTimedOut(nowMilliseconds)) {
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(pendingInterestTable_,i_0);
					continue;
				}
	
				if (pendingInterest.getInterest().matchesName(data.getName())) {
					try {
						// Send to the same face from the original call to onInterest.
						// wireEncode returns the cached encoding if available.
						pendingInterest.getFace().send(data.wireEncode());
					} catch (IOException ex) {
						ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(MemoryContentCache).FullName).log(
								ILOG.J2CsMapping.Util.Logging.Level.SEVERE, ex.Message);
						return;
					}
	
					// The pending interest is satisfied, so remove it.
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(pendingInterestTable_,i_0);
				}
			}
		}
	
		/// <summary>
		/// Store an interest from an OnInterest callback in the internal pending
		/// interest table (normally because there is no Data packet available yet to
		/// satisfy the interest). add(data) will check if the added Data packet
		/// satisfies any pending interest and send it through the face.
		/// </summary>
		///
		/// <param name="interest"></param>
		/// <param name="face"></param>
		public void storePendingInterest(Interest interest, Face face) {
			ILOG.J2CsMapping.Collections.Collections.Add(pendingInterestTable_,new MemoryContentCache.PendingInterest (interest, face));
		}
	
		/// <summary>
		/// Return a callback to use for onDataNotFound in registerPrefix which simply
		/// calls storePendingInterest() to store the interest that doesn't match a
		/// Data packet. add(data) will check if the added Data packet satisfies any
		/// pending interest and send it.
		/// </summary>
		///
		/// <returns>A callback to use for onDataNotFound in registerPrefix().</returns>
		public OnInterestCallback getStorePendingInterest() {
			return storePendingInterestCallback_;
		}
	
		public void onInterest(Name prefix, Interest interest, Face face,
				long interestFilterId, InterestFilter filter) {
			doCleanup();
	
			Name.Component selectedComponent = null;
			Blob selectedEncoding = null;
			// We need to iterate over both arrays.
			int totalSize = staleTimeCache_.Count + noStaleTimeCache_.Count;
			for (int i = 0; i < totalSize; ++i) {
				MemoryContentCache.Content  content;
				if (i < staleTimeCache_.Count)
					content = staleTimeCache_[i];
				else
					// We have iterated over the first array. Get from the second.
					content = noStaleTimeCache_[i - staleTimeCache_.Count];
	
				if (interest.matchesName(content.getName())) {
					if (interest.getChildSelector() < 0) {
						// No child selector, so send the first match that we have found.
						try {
							face.send(content.getDataEncoding());
						} catch (IOException ex) {
							ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(MemoryContentCache).FullName)
									.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex);
						}
						return;
					} else {
						// Update selectedEncoding based on the child selector.
						Name.Component component;
						if (content.getName().size() > interest.getName().size())
							component = content.getName().get(
									interest.getName().size());
						else
							component = emptyComponent_;
	
						bool gotBetterMatch = false;
						if (selectedEncoding == null)
							// Save the first match.
							gotBetterMatch = true;
						else {
							if (interest.getChildSelector() == 0) {
								// Leftmost child.
								if (component.compare(selectedComponent) < 0)
									gotBetterMatch = true;
							} else {
								// Rightmost child.
								if (component.compare(selectedComponent) > 0)
									gotBetterMatch = true;
							}
						}
	
						if (gotBetterMatch) {
							selectedComponent = component;
							selectedEncoding = content.getDataEncoding();
						}
					}
				}
			}
	
			if (selectedEncoding != null) {
				// We found the leftmost or rightmost child.
				try {
					face.send(selectedEncoding);
				} catch (IOException ex_0) {
					ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(MemoryContentCache).FullName).log(
							ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_0);
				}
			} else {
				// Call the onDataNotFound callback (if defined).
				Object onDataNotFound = ILOG.J2CsMapping.Collections.Collections.Get(onDataNotFoundForPrefix_,prefix.toUri());
				if (onDataNotFound != null) {
					try {
						((OnInterestCallback) onDataNotFound).onInterest(prefix,
								interest, face, interestFilterId, filter);
					} catch (Exception ex_1) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onDataNotFound", ex_1);
					}
				}
			}
		}
	
		public sealed class Anonymous_C0 : OnInterestCallback {
				private readonly MemoryContentCache outer_MemoryContentCache;
		
				public Anonymous_C0(MemoryContentCache paramouter_MemoryContentCache) {
					this.outer_MemoryContentCache = paramouter_MemoryContentCache;
				}
		
				public void onInterest(Name localPrefix, Interest localInterest,
						Face localFace, long localInterestFilterId,
						InterestFilter localFilter) {
					outer_MemoryContentCache.storePendingInterest(localInterest, localFace);
				}
			}
	
		/// <summary>
		/// Content is a private class to hold the name and encoding for each entry
		/// in the cache. This base class is for a Data packet without a
		/// FreshnessPeriod.
		/// </summary>
		///
		private class Content {
			/// <summary>
			/// Create a new Content entry to hold data's name and wire encoding.
			/// </summary>
			///
			/// <param name="data">The Data packet whose name and wire encoding are copied.</param>
			public Content(Data data) {
				// wireEncode returns the cached encoding if available.
				name_ = data.getName();
				dataEncoding_ = data.wireEncode();
			}
	
			public Name getName() {
				return name_;
			}
	
			public Blob getDataEncoding() {
				return dataEncoding_;
			}
	
			private readonly Name name_;
			private readonly Blob dataEncoding_;
		}
	
		/// <summary>
		/// StaleTimeContent extends Content to include the staleTimeMilliseconds
		/// for when this entry should be cleaned up from the cache.
		/// </summary>
		///
		private class StaleTimeContent : MemoryContentCache.Content  {
			/// <summary>
			/// Create a new StaleTimeContent to hold data's name and wire encoding
			/// as well as the staleTimeMilliseconds which is now plus
			/// data.getMetaInfo().getFreshnessPeriod().
			/// </summary>
			///
			/// <param name="data">The Data packet whose name and wire encoding are copied.</param>
			public StaleTimeContent(Data data) : base(data) {
				// Set up staleTimeMilliseconds_.
				staleTimeMilliseconds_ = net.named_data.jndn.util.Common.getNowMilliseconds()
						+ data.getMetaInfo().getFreshnessPeriod();
			}
	
			/// <summary>
			/// Check if this content is stale.
			/// </summary>
			///
			/// <param name="nowMilliseconds"></param>
			/// <returns>True if this content is stale, otherwise false.</returns>
			public bool isStale(double nowMilliseconds) {
				return staleTimeMilliseconds_ <= nowMilliseconds;
			}
	
			public double getStaleTimeMilliseconds() {
				return staleTimeMilliseconds_;
			}
	
			private readonly double staleTimeMilliseconds_;
			/**< The time when the content
			becomse stale in milliseconds according to Common.getNowMilliseconds() */
		}
	
		/// <summary>
		/// A PendingInterest holds an interest which onInterest received but could
		/// not satisfy. When we add a new data packet to the cache, we will also check
		/// if it satisfies a pending interest.
		/// </summary>
		///
		private class PendingInterest {
			/// <summary>
			/// Create a new PendingInterest and set the timeoutTime_ based on the current
			/// time and the interest lifetime.
			/// </summary>
			///
			/// <param name="interest">The interest.</param>
			/// <param name="face">packet to the face.</param>
			public PendingInterest(Interest interest, Face face) {
				interest_ = interest;
				face_ = face;
	
				// Set up timeoutTimeMilliseconds_.
				if (interest_.getInterestLifetimeMilliseconds() >= 0.0d)
					timeoutTimeMilliseconds_ = net.named_data.jndn.util.Common.getNowMilliseconds()
							+ interest_.getInterestLifetimeMilliseconds();
				else
					// No timeout.
					timeoutTimeMilliseconds_ = -1.0d;
			}
	
			/// <summary>
			/// Return the interest given to the constructor.
			/// </summary>
			///
			public Interest getInterest() {
				return interest_;
			}
	
			/// <summary>
			/// Return the face given to the constructor.
			/// </summary>
			///
			public Face getFace() {
				return face_;
			}
	
			/// <summary>
			/// Check if this interest is timed out.
			/// </summary>
			///
			/// <param name="nowMilliseconds"></param>
			/// <returns>True if this interest timed out, otherwise false.</returns>
			public bool isTimedOut(double nowMilliseconds) {
				return timeoutTimeMilliseconds_ >= 0.0d
						&& nowMilliseconds >= timeoutTimeMilliseconds_;
			}
	
			private readonly Interest interest_;
			private readonly Face face_;
			private readonly double timeoutTimeMilliseconds_;
			/**< The time when the
			* interest times out in milliseconds according to ndn_getNowMilliseconds,
			* or -1 for no timeout. */
		}
	
		/// <summary>
		/// Check if now is greater than nextCleanupTime_ and, if so, remove stale
		/// content from staleTimeCache_ and reset nextCleanupTime_ based on
		/// cleanupIntervalMilliseconds_. Since add(Data) does a sorted insert into
		/// staleTimeCache_, the check for stale data is quick and does not require
		/// searching the entire staleTimeCache_.
		/// </summary>
		///
		private void doCleanup() {
			double now = net.named_data.jndn.util.Common.getNowMilliseconds();
			if (now >= nextCleanupTime_) {
				// staleTimeCache_ is sorted on staleTimeMilliseconds_, so we only need to
				// erase the stale entries at the front, then quit.
				while (staleTimeCache_.Count > 0
						&& staleTimeCache_[0].isStale(now))
					ILOG.J2CsMapping.Collections.Collections.RemoveAt(staleTimeCache_,0);
	
				nextCleanupTime_ = now + cleanupIntervalMilliseconds_;
			}
		}
	
		private readonly Face face_;
		private readonly double cleanupIntervalMilliseconds_;
		private double nextCleanupTime_;
		// Use HashMap without generics so it works with older Java compilers.
		private readonly Hashtable onDataNotFoundForPrefix_;
		/// <summary>
		/// < The map key is the prefix.toUri().
		/// The value is the OnInterest callback. 
		/// </summary>
		///
		// Use ArrayList without generics so it works with older Java compilers.
		private readonly ArrayList<Int64> interestFilterIdList_;
		private readonly ArrayList<Int64> registeredPrefixIdList_;
		private readonly ArrayList<Content> noStaleTimeCache_;
		private readonly ArrayList<StaleTimeContent> staleTimeCache_;
		private readonly Name.Component emptyComponent_;
		private readonly ArrayList<PendingInterest> pendingInterestTable_;
		private OnInterestCallback storePendingInterestCallback_;
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(MemoryContentCache).FullName);
	}
}
