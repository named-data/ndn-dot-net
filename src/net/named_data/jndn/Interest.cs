// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2016 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Text;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// An Interest holds a Name and other fields for an interest.
	/// </summary>
	///
	public class Interest : ChangeCountable {
		/// <summary>
		/// Create a new Interest with the given name and interest lifetime and "none"
		/// for other values.
		/// </summary>
		///
		/// <param name="name">The name for the interest.</param>
		/// <param name="interestLifetimeMilliseconds"></param>
		public Interest(Name name, double interestLifetimeMilliseconds) {
			this.name_ = new ChangeCounter(new Name());
			this.minSuffixComponents_ = -1;
			this.maxSuffixComponents_ = -1;
			this.keyLocator_ = new ChangeCounter(
					new KeyLocator());
			this.exclude_ = new ChangeCounter(new Exclude());
			this.childSelector_ = -1;
			this.mustBeFresh_ = true;
			this.interestLifetimeMilliseconds_ = -1;
			this.nonce_ = new Blob();
			this.getNonceChangeCount_ = 0;
			this.localControlHeader_ = new LocalControlHeader();
			this.linkWireEncoding_ = new Blob();
			this.linkWireEncodingFormat_ = null;
			this.link_ = new ChangeCounter(null);
			this.selectedDelegationIndex_ = -1;
			this.defaultWireEncoding_ = new SignedBlob();
			this.getDefaultWireEncodingChangeCount_ = 0;
			this.changeCount_ = 0;
			if (name != null)
				name_.set(new Name(name));
			interestLifetimeMilliseconds_ = interestLifetimeMilliseconds;
		}
	
		/// <summary>
		/// Create a new Interest with the given name and "none" for other values.
		/// </summary>
		///
		/// <param name="name">The name for the interest.</param>
		public Interest(Name name) {
			this.name_ = new ChangeCounter(new Name());
			this.minSuffixComponents_ = -1;
			this.maxSuffixComponents_ = -1;
			this.keyLocator_ = new ChangeCounter(
					new KeyLocator());
			this.exclude_ = new ChangeCounter(new Exclude());
			this.childSelector_ = -1;
			this.mustBeFresh_ = true;
			this.interestLifetimeMilliseconds_ = -1;
			this.nonce_ = new Blob();
			this.getNonceChangeCount_ = 0;
			this.localControlHeader_ = new LocalControlHeader();
			this.linkWireEncoding_ = new Blob();
			this.linkWireEncodingFormat_ = null;
			this.link_ = new ChangeCounter(null);
			this.selectedDelegationIndex_ = -1;
			this.defaultWireEncoding_ = new SignedBlob();
			this.getDefaultWireEncodingChangeCount_ = 0;
			this.changeCount_ = 0;
			if (name != null)
				name_.set(new Name(name));
		}
	
		/// <summary>
		/// Create a new interest as a deep copy of the given interest.
		/// </summary>
		///
		/// <param name="interest">The interest to copy.</param>
		public Interest(Interest interest) {
			this.name_ = new ChangeCounter(new Name());
			this.minSuffixComponents_ = -1;
			this.maxSuffixComponents_ = -1;
			this.keyLocator_ = new ChangeCounter(
					new KeyLocator());
			this.exclude_ = new ChangeCounter(new Exclude());
			this.childSelector_ = -1;
			this.mustBeFresh_ = true;
			this.interestLifetimeMilliseconds_ = -1;
			this.nonce_ = new Blob();
			this.getNonceChangeCount_ = 0;
			this.localControlHeader_ = new LocalControlHeader();
			this.linkWireEncoding_ = new Blob();
			this.linkWireEncodingFormat_ = null;
			this.link_ = new ChangeCounter(null);
			this.selectedDelegationIndex_ = -1;
			this.defaultWireEncoding_ = new SignedBlob();
			this.getDefaultWireEncodingChangeCount_ = 0;
			this.changeCount_ = 0;
			name_.set(new Name(interest.getName()));
			minSuffixComponents_ = interest.minSuffixComponents_;
			maxSuffixComponents_ = interest.maxSuffixComponents_;
			keyLocator_.set(new KeyLocator(interest.getKeyLocator()));
			exclude_.set(new Exclude(interest.getExclude()));
			childSelector_ = interest.childSelector_;
			mustBeFresh_ = interest.mustBeFresh_;
	
			interestLifetimeMilliseconds_ = interest.interestLifetimeMilliseconds_;
			nonce_ = interest.getNonce();
	
			linkWireEncoding_ = interest.linkWireEncoding_;
			linkWireEncodingFormat_ = interest.linkWireEncodingFormat_;
			if (interest.link_.get() != null)
				link_.set(new Link((Link) interest.link_.get()));
			selectedDelegationIndex_ = interest.selectedDelegationIndex_;
	
			setDefaultWireEncoding(interest.getDefaultWireEncoding(),
					interest.defaultWireEncodingFormat_);
		}
	
		/// <summary>
		/// Create a new Interest with an empty name and "none" for all values.
		/// </summary>
		///
		public Interest() {
			this.name_ = new ChangeCounter(new Name());
			this.minSuffixComponents_ = -1;
			this.maxSuffixComponents_ = -1;
			this.keyLocator_ = new ChangeCounter(
					new KeyLocator());
			this.exclude_ = new ChangeCounter(new Exclude());
			this.childSelector_ = -1;
			this.mustBeFresh_ = true;
			this.interestLifetimeMilliseconds_ = -1;
			this.nonce_ = new Blob();
			this.getNonceChangeCount_ = 0;
			this.localControlHeader_ = new LocalControlHeader();
			this.linkWireEncoding_ = new Blob();
			this.linkWireEncodingFormat_ = null;
			this.link_ = new ChangeCounter(null);
			this.selectedDelegationIndex_ = -1;
			this.defaultWireEncoding_ = new SignedBlob();
			this.getDefaultWireEncodingChangeCount_ = 0;
			this.changeCount_ = 0;
		}
	
		public const int CHILD_SELECTOR_LEFT = 0;
		public const int CHILD_SELECTOR_RIGHT = 1;
	
		/// <summary>
		/// Encode this Interest for a particular wire format. If wireFormat is the
		/// default wire format, also set the defaultWireEncoding field to the encoded
		/// result.
		/// </summary>
		///
		/// <param name="wireFormat">A WireFormat object used to encode this Interest.</param>
		/// <returns>The encoded buffer.</returns>
		public SignedBlob wireEncode(WireFormat wireFormat) {
			if (!getDefaultWireEncoding().isNull()
					&& getDefaultWireEncodingFormat() == wireFormat)
				// We already have an encoding in the desired format.
				return getDefaultWireEncoding();
	
			int[] signedPortionBeginOffset = new int[1];
			int[] signedPortionEndOffset = new int[1];
			Blob encoding = wireFormat.encodeInterest(this,
					signedPortionBeginOffset, signedPortionEndOffset);
			SignedBlob wireEncoding = new SignedBlob(encoding,
					signedPortionBeginOffset[0], signedPortionEndOffset[0]);
	
			if (wireFormat == net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat())
				// This is the default wire encoding.
				setDefaultWireEncoding(wireEncoding,
						net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
	
			return wireEncoding;
		}
	
		/// <summary>
		/// Encode this Interest for the default wire format
		/// WireFormat.getDefaultWireFormat().
		/// </summary>
		///
		/// <returns>The encoded buffer.</returns>
		public SignedBlob wireEncode() {
			return wireEncode(net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input using a particular wire format and update this Interest.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(ByteBuffer input, WireFormat wireFormat) {
			int[] signedPortionBeginOffset = new int[1];
			int[] signedPortionEndOffset = new int[1];
			wireFormat.decodeInterest(this, input, signedPortionBeginOffset,
					signedPortionEndOffset);
	
			if (wireFormat == net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat())
				// This is the default wire encoding.
				setDefaultWireEncoding(new SignedBlob(input, true,
						signedPortionBeginOffset[0], signedPortionEndOffset[0]),
						net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
			else
				setDefaultWireEncoding(new SignedBlob(), null);
		}
	
		/// <summary>
		/// Decode the input using the default wire format
		/// WireFormat.getDefaultWireFormat() and update this Interest.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(ByteBuffer input) {
			wireDecode(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input using a particular wire format and update this Interest. If
		/// wireFormat is the default wire format, also set the defaultWireEncoding
		/// field another pointer to the input Blob.
		/// </summary>
		///
		/// <param name="input">The input blob to decode.</param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(Blob input, WireFormat wireFormat) {
			int[] signedPortionBeginOffset = new int[1];
			int[] signedPortionEndOffset = new int[1];
			wireFormat.decodeInterest(this, input.buf(), signedPortionBeginOffset,
					signedPortionEndOffset);
	
			if (wireFormat == net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat())
				// This is the default wire encoding.
				setDefaultWireEncoding(new SignedBlob(input,
						signedPortionBeginOffset[0], signedPortionEndOffset[0]),
						net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
			else
				setDefaultWireEncoding(new SignedBlob(), null);
		}
	
		/// <summary>
		/// Decode the input using the default wire format
		/// WireFormat.getDefaultWireFormat() and update this Interest.
		/// </summary>
		///
		/// <param name="input">The input blob to decode.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(Blob input) {
			wireDecode(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Encode the name according to the "NDN URI Scheme".  If there are interest
		/// selectors, append "?" and added the selectors as a query string.  For
		/// example "/test/name?ndn.ChildSelector=1".
		/// </summary>
		///
		/// <returns>The URI string.</returns>
		/// @note This is an experimental feature.  See the API docs for more detail at
		/// http://named-data.net/doc/ndn-ccl-api/interest.html#interest-touri-method .
		public String toUri() {
			StringBuilder selectors = new StringBuilder();
	
			if (minSuffixComponents_ >= 0)
				selectors.append("&ndn.MinSuffixComponents=").append(
						minSuffixComponents_);
			if (maxSuffixComponents_ >= 0)
				selectors.append("&ndn.MaxSuffixComponents=").append(
						maxSuffixComponents_);
			if (childSelector_ >= 0)
				selectors.append("&ndn.ChildSelector=").append(childSelector_);
			selectors.append("&ndn.MustBeFresh=").append((mustBeFresh_) ? 1 : 0);
			if (interestLifetimeMilliseconds_ >= 0)
				selectors.append("&ndn.InterestLifetime=").append(
						(long) Math.Round(interestLifetimeMilliseconds_,MidpointRounding.AwayFromZero));
			if (nonce_.size() > 0) {
				selectors.append("&ndn.Nonce=");
				net.named_data.jndn.Name.toEscapedString(nonce_.buf(), selectors);
			}
			if (getExclude().size() > 0)
				selectors.append("&ndn.Exclude=").append(getExclude().toUri());
	
			StringBuilder result = new StringBuilder();
	
			result.append(getName().toUri());
			String selectorsString = selectors.toString();
			if (selectorsString.Length > 0)
				// Replace the first & with ?.
				result.append("?").append(selectorsString.Substring(1));
	
			return result.toString();
		}
	
		public Name getName() {
			return (Name) name_.get();
		}
	
		public int getMinSuffixComponents() {
			return minSuffixComponents_;
		}
	
		public int getMaxSuffixComponents() {
			return maxSuffixComponents_;
		}
	
		public KeyLocator getKeyLocator() {
			return (KeyLocator) keyLocator_.get();
		}
	
		public Exclude getExclude() {
			return (Exclude) exclude_.get();
		}
	
		public int getChildSelector() {
			return childSelector_;
		}
	
		/// <summary>
		/// Get the must be fresh flag. If not specified, the default is true.
		/// </summary>
		///
		/// <returns>The must be fresh flag.</returns>
		public bool getMustBeFresh() {
			return mustBeFresh_;
		}
	
		public double getInterestLifetimeMilliseconds() {
			return interestLifetimeMilliseconds_;
		}
	
		/// <summary>
		/// Return the nonce value from the incoming interest.  If you change any of
		/// the fields in this Interest object, then the nonce value is cleared.
		/// </summary>
		///
		/// <returns>The nonce.</returns>
		public Blob getNonce() {
			if (getNonceChangeCount_ != getChangeCount()) {
				// The values have changed, so the existing nonce is invalidated.
				nonce_ = new Blob();
				getNonceChangeCount_ = getChangeCount();
			}
	
			return nonce_;
		}
	
		/// <summary>
		/// Check if this interest has a link object (or a link wire encoding which
		/// can be decoded to make the link object).
		/// </summary>
		///
		/// <returns>True if this interest has a link object, false if not.</returns>
		public bool hasLink() {
			return link_.get() != null || !linkWireEncoding_.isNull();
		}
	
		/// <summary>
		/// Get the link object. If necessary, decode it from the link wire encoding.
		/// </summary>
		///
		/// <returns>The link object, or null if not specified.</returns>
		/// <exception cref="EncodingException">For error decoding the link wire encoding (ifnecessary).</exception>
		public Link getLink() {
			if (link_.get() != null)
				return (Link) link_.get();
			else if (!linkWireEncoding_.isNull()) {
				// Decode the link object from linkWireEncoding_.
				Link link = new Link();
				link.wireDecode(linkWireEncoding_, linkWireEncodingFormat_);
				link_.set(link);
	
				// Clear linkWireEncoding_ since it is now managed by the link object.
				linkWireEncoding_ = new Blob();
				linkWireEncodingFormat_ = null;
	
				return link;
			} else
				return null;
		}
	
		/// <summary>
		/// Get the wire encoding of the link object. If there is already a wire
		/// encoding then return it. Otherwise encode from the link object (if
		/// available).
		/// </summary>
		///
		/// <param name="wireFormat">The desired wire format for the encoding.</param>
		/// <returns>The wire encoding, or an isNull Blob if the link is not specified.</returns>
		/// <exception cref="EncodingException">for error encoding the link object.</exception>
		public Blob getLinkWireEncoding(WireFormat wireFormat) {
			if (!linkWireEncoding_.isNull()
					&& linkWireEncodingFormat_ == wireFormat)
				return linkWireEncoding_;
	
			Link link = getLink();
			if (link != null)
				return link.wireEncode(wireFormat);
			else
				return new Blob();
		}
	
		/// <summary>
		/// Get the wire encoding of the link object. If there is already a wire
		/// encoding then return it. Otherwise encode from the link object (if
		/// available).
		/// </summary>
		///
		/// <returns>The wire encoding, or an isNull Blob if the link is not specified.</returns>
		/// <exception cref="EncodingException">for error encoding the link object.</exception>
		public Blob getLinkWireEncoding() {
			return getLinkWireEncoding(net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Get the selected delegation index.
		/// </summary>
		///
		/// <returns>The selected delegation index. If not specified, return -1.</returns>
		public int getSelectedDelegationIndex() {
			return selectedDelegationIndex_;
		}
	
		/// <summary>
		/// Get the incoming face ID of the local control header.
		/// </summary>
		///
		/// <returns>The incoming face ID. If not specified, return -1.</returns>
		/// @note This is an experimental feature. This API may change in the future.
		public long getIncomingFaceId() {
			return localControlHeader_.getIncomingFaceId();
		}
	
		/// <summary>
		/// Get the next hop face ID.
		/// </summary>
		///
		/// <returns>The next hop face ID. If not specified, return -1.</returns>
		/// @note This is an experimental feature. This API may change in the future.
		public long getNextHopFaceId() {
			return localControlHeader_.getNextHopFaceId();
		}
	
		/// <summary>
		/// Set the interest name.
		/// </summary>
		///
		/// @note You can also call getName and change the name values directly.
		/// <param name="name">The interest name. This makes a copy of the name.</param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setName(Name name) {
			name_.set((name == null) ? new Name() : new Name(name));
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set the min suffix components count.
		/// </summary>
		///
		/// <param name="minSuffixComponents"></param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setMinSuffixComponents(int minSuffixComponents) {
			minSuffixComponents_ = minSuffixComponents;
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set the max suffix components count.
		/// </summary>
		///
		/// <param name="maxSuffixComponents"></param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setMaxSuffixComponents(int maxSuffixComponents) {
			maxSuffixComponents_ = maxSuffixComponents;
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set the child selector.
		/// </summary>
		///
		/// <param name="childSelector">The child selector. If not specified, set to -1.</param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setChildSelector(int childSelector) {
			childSelector_ = childSelector;
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set the MustBeFresh flag.
		/// </summary>
		///
		/// <param name="mustBeFresh"></param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setMustBeFresh(bool mustBeFresh) {
			mustBeFresh_ = mustBeFresh;
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set the interest lifetime.
		/// </summary>
		///
		/// <param name="interestLifetimeMilliseconds"></param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setInterestLifetimeMilliseconds(
				double interestLifetimeMilliseconds) {
			interestLifetimeMilliseconds_ = interestLifetimeMilliseconds;
			++changeCount_;
			return this;
		}
	
		public Interest setNonce(Blob nonce) {
			nonce_ = ((nonce == null) ? new Blob() : nonce);
			// Set getNonceChangeCount_ so that the next call to getNonce() won't
			//   clear nonce_.
			++changeCount_;
			getNonceChangeCount_ = getChangeCount();
			return this;
		}
	
		/// <summary>
		/// Set this interest to use a copy of the given KeyLocator object.
		/// </summary>
		///
		/// @note ou can also call getKeyLocator and change the key locator directly.
		/// <param name="keyLocator">KeyLocator with an unspecified type.</param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setKeyLocator(KeyLocator keyLocator) {
			keyLocator_.set((keyLocator == null) ? new KeyLocator() : new KeyLocator(
					keyLocator));
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set this interest to use a copy of the given Exclude object.
		/// </summary>
		///
		/// @note You can also call getExclude and change the exclude entries directly.
		/// <param name="exclude">size() 0.</param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setExclude(Exclude exclude) {
			exclude_.set((exclude == null) ? new Exclude() : new Exclude(exclude));
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set the link wire encoding bytes, without decoding them. If there is
		/// a link object, set it to null. If you later call getLink(), it will
		/// decode the wireEncoding to create the link object.
		/// </summary>
		///
		/// <param name="encoding"></param>
		/// <param name="wireFormat"></param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setLinkWireEncoding(Blob encoding,
				WireFormat wireFormat) {
			linkWireEncoding_ = encoding;
			linkWireEncodingFormat_ = wireFormat;
	
			// Clear the link object, assuming that it has a different encoding.
			link_.set(null);
	
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set the link wire encoding bytes, without decoding them. If there is
		/// a link object, set it to null. IF you later call getLink(), it will
		/// decode the wireEncoding to create the link object.
		/// </summary>
		///
		/// <param name="encoding"></param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setLinkWireEncoding(Blob encoding) {
			return setLinkWireEncoding(encoding, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Clear the link wire encoding and link object so that getLink() returns null.
		/// </summary>
		///
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest unsetLink() {
			return setLinkWireEncoding(new Blob(), null);
		}
	
		/// <summary>
		/// Set the selected delegation index.
		/// </summary>
		///
		/// <param name="selectedDelegationIndex"></param>
		/// <returns>This Interest so that you can chain calls to update values.</returns>
		public Interest setSelectedDelegationIndex(int selectedDelegationIndex) {
			selectedDelegationIndex_ = selectedDelegationIndex;
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// An internal library method to set localControlHeader to a copy of the given
		/// LocalControlHeader for an incoming packet. The application should not call
		/// this.
		/// </summary>
		///
		/// <param name="localControlHeader">The LocalControlHeader which is copied.</param>
		/// @note This is an experimental feature. This API may change in the future.
		internal void setLocalControlHeader(LocalControlHeader localControlHeader) {
			localControlHeader_ = ((localControlHeader == null) ? new LocalControlHeader()
					: new LocalControlHeader(localControlHeader));
			// Don't update changeCount_ since this doesn't affect the wire encoding.
		}
	
		/// <summary>
		/// Check if this Interest's name matches the given name (using Name.match)
		/// and the given name also conforms to the interest selectors.
		/// </summary>
		///
		/// <param name="name">The name to check.</param>
		/// <returns>True if the name and interest selectors match, otherwise false.</returns>
		public bool matchesName(Name name) {
			if (!getName().match(name))
				return false;
	
			if (minSuffixComponents_ >= 0 &&
			// Add 1 for the implicit digest.
					!(name.size() + 1 - getName().size() >= minSuffixComponents_))
				return false;
			if (maxSuffixComponents_ >= 0 &&
			// Add 1 for the implicit digest.
					!(name.size() + 1 - getName().size() <= maxSuffixComponents_))
				return false;
			if (getExclude().size() > 0 && name.size() > getName().size()
					&& getExclude().matches(name.get(getName().size())))
				return false;
	
			return true;
		}
	
		/// <summary>
		/// Return a pointer to the defaultWireEncoding, which was encoded with
		/// getDefaultWireEncodingFormat().
		/// </summary>
		///
		/// <returns>The default wire encoding. Its pointer may be null.</returns>
		public SignedBlob getDefaultWireEncoding() {
			if (getDefaultWireEncodingChangeCount_ != getChangeCount()) {
				// The values have changed, so the default wire encoding is invalidated.
				defaultWireEncoding_ = new SignedBlob();
				defaultWireEncodingFormat_ = null;
				getDefaultWireEncodingChangeCount_ = getChangeCount();
			}
	
			return defaultWireEncoding_;
		}
	
		/// <summary>
		/// Get the WireFormat which is used by getDefaultWireEncoding().
		/// </summary>
		///
		/// <returns>The WireFormat, which is only meaningful if the
		/// getDefaultWireEncoding() does not have a null pointer.</returns>
		internal WireFormat getDefaultWireEncodingFormat() {
			return defaultWireEncodingFormat_;
		}
	
		/// <summary>
		/// Get the change count, which is incremented each time this object
		/// (or a child object) is changed.
		/// </summary>
		///
		/// <returns>The change count.</returns>
		public long getChangeCount() {
			// Make sure each of the checkChanged is called.
			bool changed = name_.checkChanged();
			changed = keyLocator_.checkChanged() || changed;
			changed = exclude_.checkChanged() || changed;
			changed = link_.checkChanged() || changed;
			if (changed)
				// A child object has changed, so update the change count.
				++changeCount_;
	
			return changeCount_;
		}
	
		private void setDefaultWireEncoding(SignedBlob defaultWireEncoding,
				WireFormat defaultWireEncodingFormat) {
			defaultWireEncoding_ = defaultWireEncoding;
			defaultWireEncodingFormat_ = defaultWireEncodingFormat;
			// Set getDefaultWireEncodingChangeCount_ so that the next call to
			//   getDefaultWireEncoding() won't clear defaultWireEncoding_.
			getDefaultWireEncodingChangeCount_ = getChangeCount();
		}
	
		private readonly ChangeCounter name_;
		private int minSuffixComponents_;
		private int maxSuffixComponents_;
		private readonly ChangeCounter keyLocator_;
		private readonly ChangeCounter exclude_;
		private int childSelector_;
		private bool mustBeFresh_;
		private double interestLifetimeMilliseconds_;
		private Blob nonce_;
		private long getNonceChangeCount_;
		private LocalControlHeader localControlHeader_;
		private Blob linkWireEncoding_;
		private WireFormat linkWireEncodingFormat_;
		private readonly ChangeCounter link_;
		private int selectedDelegationIndex_;
		private SignedBlob defaultWireEncoding_;
		private WireFormat defaultWireEncodingFormat_;
		private long getDefaultWireEncodingChangeCount_;
		private long changeCount_;
	}
}
