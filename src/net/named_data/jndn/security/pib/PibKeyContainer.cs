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
namespace net.named_data.jndn.security.pib {
	
	using ILOG.J2CsMapping.Collections;
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security.pib.detail;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A PibKeyContainer is used to search/enumerate the keys of an identity.
	/// (A PibKeyContainer object can only be created by PibIdentity.)
	/// </summary>
	///
	public class PibKeyContainer {
		/// <summary>
		/// Get the number of keys in the container.
		/// </summary>
		///
		/// <returns>The number of keys.</returns>
		public int size() {
			return keyNames_.Count;
		}
	
		/// <summary>
		/// Add a key with name keyName into the container. If a key with the same name
		/// already exists, this replaces it.
		/// </summary>
		///
		/// <param name="key">The buffer of encoded key bytes.</param>
		/// <param name="keyName">The name of the key, which is copied.</param>
		/// <returns>The PibKey object.</returns>
		/// <exception cref="System.ArgumentException">if the name of the key does not match theidentity name.</exception>
		public PibKey add(ByteBuffer key, Name keyName) {
			if (!identityName_.equals(net.named_data.jndn.security.pib.PibKey.extractIdentityFromKeyName(keyName)))
				throw new ArgumentException("The key name `"
						+ keyName.toUri() + "` does not match the identity name `"
						+ identityName_.toUri() + "`");
	
			// Copy the Name.
			ILOG.J2CsMapping.Collections.Collections.Add(keyNames_,new Name(keyName));
			ILOG.J2CsMapping.Collections.Collections.Put(keys_,new Name(keyName),new PibKeyImpl(keyName, key, pibImpl_));
	
			return get(keyName);
		}
	
		/// <summary>
		/// Remove the key with name keyName from the container, and its related
		/// certificates. If the key does not exist, do nothing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <exception cref="System.ArgumentException">if keyName does not match the identity name.</exception>
		public void remove(Name keyName) {
			if (!identityName_.equals(net.named_data.jndn.security.pib.PibKey.extractIdentityFromKeyName(keyName)))
				throw new ArgumentException("Key name `" + keyName.toUri()
						+ "` does not match identity `" + identityName_.toUri()
						+ "`");
	
			ILOG.J2CsMapping.Collections.Collections.Remove(keyNames_,keyName);
			ILOG.J2CsMapping.Collections.Collections.Remove(keys_,keyName);
			pibImpl_.removeKey(keyName);
		}
	
		/// <summary>
		/// Get the key with name keyName from the container.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The PibKey object.</returns>
		/// <exception cref="System.ArgumentException">if keyName does not match the identity name.</exception>
		/// <exception cref="Pib.Error">if the key does not exist.</exception>
		public PibKey get(Name keyName) {
			if (!identityName_.equals(net.named_data.jndn.security.pib.PibKey.extractIdentityFromKeyName(keyName)))
				throw new ArgumentException("Key name `" + keyName.toUri()
						+ "` does not match identity `" + identityName_.toUri()
						+ "`");
	
			PibKeyImpl pibKeyImpl = ILOG.J2CsMapping.Collections.Collections.Get(keys_,keyName);
	
			if (pibKeyImpl == null) {
				pibKeyImpl = new PibKeyImpl(keyName, pibImpl_);
				// Copy the Name.
				ILOG.J2CsMapping.Collections.Collections.Put(keys_,new Name(keyName),pibKeyImpl);
			}
	
			return new PibKey(pibKeyImpl);
		}
	
		/// <summary>
		/// Get the names of all the keys in the container.
		/// </summary>
		///
		/// <returns>A new list of Name.</returns>
		public ArrayList<Name> getKeyNames() {
			ArrayList<Name> result = new ArrayList<Name>();
	
			/* foreach */
			foreach (Name name  in  new ILOG.J2CsMapping.Collections.ListSet(keys_.Keys))
				// Copy the Name.
				ILOG.J2CsMapping.Collections.Collections.Add(result,new Name(name));
	
			return result;
		}
	
		/// <summary>
		/// Check if the container is consistent with the backend storage.
		/// </summary>
		///
		/// <returns>True if the container is consistent, false otherwise.</returns>
		/// @note This method is heavy-weight and should be used in a debugging mode
		/// only.
		public bool isConsistent() {
			return keyNames_.equals(pibImpl_.getKeysOfIdentity(identityName_));
		}
	
		/// <summary>
		/// Create a PibKeyContainer for an identity with identityName. This
		/// constructor should only be called by PibIdentityImpl.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity, which is copied.</param>
		/// <param name="pibImpl">The PIB backend implementation.</param>
		public PibKeyContainer(Name identityName, PibImpl pibImpl) {
			this.keys_ = new Hashtable<Name, PibKeyImpl>();
			// Copy the Name.
			identityName_ = new Name(identityName);
			pibImpl_ = pibImpl;
	
			if (pibImpl == null)
				throw new AssertionError("The pibImpl is null");
	
			keyNames_ = pibImpl_.getKeysOfIdentity(identityName);
		}
	
		/// <summary>
		/// Get the keys_ map, which should only be used for testing.
		/// </summary>
		///
		public Hashtable<Name, PibKeyImpl> getKeys_() {
			return keys_;
		}
	
		private readonly Name identityName_;
		private readonly HashedSet<Name> keyNames_;
		// Cache of loaded PibKeyImpl objects.
		private readonly Hashtable<Name, PibKeyImpl> keys_;
	
		private readonly PibImpl pibImpl_;
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
