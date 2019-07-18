// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.util;
	
	public class KeyLocator : ChangeCountable {
		/// <summary>
		/// Create a new KeyLocator with default values.
		/// </summary>
		///
		public KeyLocator() {
			this.type_ = net.named_data.jndn.KeyLocatorType.NONE;
			this.keyData_ = new Blob();
			this.keyName_ = new ChangeCounter(new Name());
			this.changeCount_ = 0;
		}
	
		/// <summary>
		/// Create a new KeyLocator with a copy of the fields in keyLocator.
		/// </summary>
		///
		/// <param name="keyLocator">The KeyLocator to copy.</param>
		public KeyLocator(KeyLocator keyLocator) {
			this.type_ = net.named_data.jndn.KeyLocatorType.NONE;
			this.keyData_ = new Blob();
			this.keyName_ = new ChangeCounter(new Name());
			this.changeCount_ = 0;
			type_ = keyLocator.type_;
			keyData_ = keyLocator.keyData_;
			keyName_.set(new Name(keyLocator.getKeyName()));
		}
	
		public KeyLocatorType getType() {
			return type_;
		}
	
		public Blob getKeyData() {
			return keyData_;
		}
	
		public Name getKeyName() {
			return (Name) keyName_.get();
		}
	
		public void setType(KeyLocatorType type) {
			type_ = type;
			++changeCount_;
		}
	
		public void setKeyData(Blob keyData) {
			keyData_ = ((keyData == null) ? new Blob() : keyData);
			++changeCount_;
		}
	
		public void setKeyName(Name keyName) {
			keyName_.set((keyName == null) ? new Name() : new Name(keyName));
			++changeCount_;
		}
	
		/// <summary>
		/// Clear fields and reset to default values.
		/// </summary>
		///
		public void clear() {
			type_ = net.named_data.jndn.KeyLocatorType.NONE;
			keyData_ = new Blob();
			keyName_.set(new Name());
			++changeCount_;
		}
	
		/// <summary>
		/// Check if this key locator has the same values as the given key locator.
		/// </summary>
		///
		/// <param name="other">The other key locator to check.</param>
		/// <returns>true if the key locators are equal, otherwise false.</returns>
		public bool equals(KeyLocator other) {
			if (type_ != other.type_)
				return false;
	
			if (type_ == net.named_data.jndn.KeyLocatorType.KEYNAME) {
				if (!getKeyName().equals(other.getKeyName()))
					return false;
			} else if (type_ == net.named_data.jndn.KeyLocatorType.KEY_LOCATOR_DIGEST) {
				if (!getKeyData().equals(other.getKeyData()))
					return false;
			}
	
			return true;
		}
	
		public override bool Equals(Object other) {
			if (!(other  is  KeyLocator))
				return false;
	
			return equals((KeyLocator) other);
		}
	
		/// <summary>
		/// If the signature is a type that has a KeyLocator (so that
		/// getFromSignature will succeed), return true.
		/// Note: This is a static method of KeyLocator instead of a method of
		/// Signature so that the Signature base class does not need to be overloaded
		/// with all the different kinds of information that various signature
		/// algorithms may use.
		/// </summary>
		///
		/// <param name="signature">An object of a subclass of Signature.</param>
		/// <returns>True if the signature is a type that has a KeyLocator, otherwise
		/// false.</returns>
		public static bool canGetFromSignature(Signature signature) {
			return signature  is  Sha256WithRsaSignature
					|| signature  is  Sha256WithEcdsaSignature
					|| signature  is  HmacWithSha256Signature;
		}
	
		/// <summary>
		/// If the signature is a type that has a KeyLocator, then return it. Otherwise
		/// throw an error. To check if the signature has a KeyLocator without throwing
		/// an error, you can use canGetFromSignature().
		/// </summary>
		///
		/// <param name="signature">An object of a subclass of Signature.</param>
		/// <returns>The signature's KeyLocator. It is an error if signature doesn't
		/// have a KeyLocator.</returns>
		public static KeyLocator getFromSignature(Signature signature) {
			if (signature  is  Sha256WithRsaSignature)
				return ((Sha256WithRsaSignature) signature).getKeyLocator();
			else if (signature  is  Sha256WithEcdsaSignature)
				return ((Sha256WithEcdsaSignature) signature).getKeyLocator();
			else if (signature  is  HmacWithSha256Signature)
				return ((HmacWithSha256Signature) signature).getKeyLocator();
			else
				throw new Exception(
						"KeyLocator.getFromSignature: Signature type does not have a KeyLocator");
		}
	
		/// <summary>
		/// Get the change count, which is incremented each time this object
		/// (or a child object) is changed.
		/// </summary>
		///
		/// <returns>The change count.</returns>
		public long getChangeCount() {
			if (keyName_.checkChanged())
				// A child object has changed, so update the change count.
				++changeCount_;
	
			return changeCount_;
		}
	
		private KeyLocatorType type_;
		private Blob keyData_;
		/// <summary>
		/// < A Blob for the key data as follows:
		/// If type_ is KeyLocatorType.KEY_LOCATOR_DIGEST, the digest data.
		/// </summary>
		///
		private readonly ChangeCounter keyName_;
		/// <summary>
		/// < The key name (only used if
		/// type_ KeyLocatorType.KEYNAME.) 
		/// </summary>
		///
		private long changeCount_;
	}
}
