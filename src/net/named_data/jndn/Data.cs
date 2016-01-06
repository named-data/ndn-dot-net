// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
// 12/23/15 3:55 PM    
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2015 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.util;
	
	public class Data : ChangeCountable, SignatureHolder {
		/// <summary>
		/// Create a new Data object with default values and where the signature is a
		/// blank Sha256WithRsaSignature.
		/// </summary>
		///
		public Data() {
			this.signature_ = new ChangeCounter(
					new Sha256WithRsaSignature());
			this.name_ = new ChangeCounter(new Name());
			this.metaInfo_ = new ChangeCounter(new MetaInfo());
			this.content_ = new Blob();
			this.localControlHeader_ = new LocalControlHeader();
			this.defaultWireEncoding_ = new SignedBlob();
			this.getDefaultWireEncodingChangeCount_ = 0;
			this.changeCount_ = 0;
		}
	
		/// <summary>
		/// Create a new Data object with the given name and default values and where
		/// the signature is a blank Sha256WithRsaSignature.
		/// </summary>
		///
		/// <param name="name">The name which is copied.</param>
		public Data(Name name) {
			this.signature_ = new ChangeCounter(
					new Sha256WithRsaSignature());
			this.name_ = new ChangeCounter(new Name());
			this.metaInfo_ = new ChangeCounter(new MetaInfo());
			this.content_ = new Blob();
			this.localControlHeader_ = new LocalControlHeader();
			this.defaultWireEncoding_ = new SignedBlob();
			this.getDefaultWireEncodingChangeCount_ = 0;
			this.changeCount_ = 0;
			name_.set(new Name(name));
		}
	
		/// <summary>
		/// Create a deep copy of the given data object, including a clone of the
		/// signature object.
		/// </summary>
		///
		/// <param name="data">The data object to copy.</param>
		public Data(Data data) {
			this.signature_ = new ChangeCounter(
					new Sha256WithRsaSignature());
			this.name_ = new ChangeCounter(new Name());
			this.metaInfo_ = new ChangeCounter(new MetaInfo());
			this.content_ = new Blob();
			this.localControlHeader_ = new LocalControlHeader();
			this.defaultWireEncoding_ = new SignedBlob();
			this.getDefaultWireEncodingChangeCount_ = 0;
			this.changeCount_ = 0;
			try {
				signature_
						.set((data.signature_ == null) ? (net.named_data.jndn.util.ChangeCountable) (new Sha256WithRsaSignature())
								: (net.named_data.jndn.util.ChangeCountable) ((Signature) data.getSignature().Clone()));
			} catch (Exception e) {
				// We don't expect this to happen, so just treat it as if we got a null pointer.
				throw new NullReferenceException(
						"Data.setSignature: unexpected exception in clone(): "
								+ e.Message);
			}
	
			name_.set(new Name(data.getName()));
			metaInfo_.set(new MetaInfo(data.getMetaInfo()));
			content_ = data.content_;
			setDefaultWireEncoding(data.defaultWireEncoding_, null);
		}
	
		/// <summary>
		/// Encode this Data for a particular wire format. If wireFormat is the default
		/// wire format, also set the defaultWireEncoding field to the encoded result.
		/// </summary>
		///
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <returns>The encoded buffer.</returns>
		public SignedBlob wireEncode(WireFormat wireFormat) {
			if (!getDefaultWireEncoding().isNull()
					&& getDefaultWireEncodingFormat() == wireFormat)
				// We already have an encoding in the desired format.
				return getDefaultWireEncoding();
	
			int[] signedPortionBeginOffset = new int[1];
			int[] signedPortionEndOffset = new int[1];
			Blob encoding = wireFormat.encodeData(this, signedPortionBeginOffset,
					signedPortionEndOffset);
			SignedBlob wireEncoding = new SignedBlob(encoding,
					signedPortionBeginOffset[0], signedPortionEndOffset[0]);
	
			if (wireFormat == net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat())
				// This is the default wire encoding.
				setDefaultWireEncoding(wireEncoding,
						net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
	
			return wireEncoding;
		}
	
		/// <summary>
		/// Encode this Data for the default wire format WireFormat.getDefaultWireFormat().
		/// Also set the defaultWireEncoding field to the encoded result.
		/// </summary>
		///
		/// <returns>The encoded buffer.</returns>
		public SignedBlob wireEncode() {
			return wireEncode(net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input using a particular wire format and update this Data. If
		/// wireFormat is the default wire format, also set the defaultWireEncoding
		/// field another pointer to the input Blob.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public virtual void wireDecode(Blob input, WireFormat wireFormat) {
			int[] signedPortionBeginOffset = new int[1];
			int[] signedPortionEndOffset = new int[1];
			wireFormat.decodeData(this, input.buf(), signedPortionBeginOffset,
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
		/// WireFormat.getDefaultWireFormat() and update this Data. Also set the
		/// defaultWireEncoding field another pointer to the input Blob.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(Blob input) {
			wireDecode(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input using a particular wire format and update this Data. If
		/// wireFormat is the default wire format, also set the defaultWireEncoding
		/// field to a copy of the input. (To not copy the input, see
		/// wireDecode(Blob).)
		/// </summary>
		///
		/// <param name="input"></param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(ByteBuffer input, WireFormat wireFormat) {
			wireDecode(new Blob(input, true), wireFormat);
		}
	
		/// <summary>
		/// Decode the input using the default wire format
		/// WireFormat.getDefaultWireFormat() and update this Data. Also set the
		/// defaultWireEncoding field to the input.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(ByteBuffer input) {
			wireDecode(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		public Signature getSignature() {
			return (Signature) signature_.get();
		}
	
		public Name getName() {
			return (Name) name_.get();
		}
	
		public MetaInfo getMetaInfo() {
			return (MetaInfo) metaInfo_.get();
		}
	
		public Blob getContent() {
			return content_;
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
		/// Set the signature to a copy of the given signature.
		/// </summary>
		///
		/// <param name="signature">The signature object which is cloned.</param>
		/// <returns>This Data so that you can chain calls to update values.</returns>
		public Data setSignature(Signature signature) {
			try {
				signature_.set((signature == null) ? (net.named_data.jndn.util.ChangeCountable) (new Sha256WithRsaSignature())
						: (net.named_data.jndn.util.ChangeCountable) ((Signature) signature.Clone()));
			} catch (Exception e) {
				// We don't expect this to happen, so just treat it as if we got a null
				//   pointer.
				throw new NullReferenceException(
						"Data.setSignature: unexpected exception in clone(): "
								+ e.Message);
			}
	
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set name to a copy of the given Name.  This is not final so that a subclass
		/// can override to validate the name.
		/// </summary>
		///
		/// <param name="name">The Name which is copied.</param>
		/// <returns>This Data so that you can chain calls to update values.</returns>
		public virtual Data setName(Name name) {
			name_.set((name == null) ? new Name() : new Name(name));
			++changeCount_;
			return this;
		}
	
		/// <summary>
		/// Set metaInfo to a copy of the given MetaInfo.
		/// </summary>
		///
		/// <param name="metaInfo">The MetaInfo which is copied.</param>
		/// <returns>This Data so that you can chain calls to update values.</returns>
		public Data setMetaInfo(MetaInfo metaInfo) {
			metaInfo_.set((metaInfo == null) ? new MetaInfo()
					: new MetaInfo(metaInfo));
			++changeCount_;
			return this;
		}
	
		public Data setContent(Blob content) {
			content_ = ((content == null) ? new Blob() : content);
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
		/// <returns>This Data so that you can chain calls to update values.</returns>
		/// @note This is an experimental feature. This API may change in the future.
		internal Data setLocalControlHeader(LocalControlHeader localControlHeader) {
			localControlHeader_ = ((localControlHeader == null) ? new LocalControlHeader()
					: new LocalControlHeader(localControlHeader));
			// Don't update changeCount_ since this doesn't affect the wire encoding.
			return this;
		}
	
		/// <summary>
		/// Get the change count, which is incremented each time this object
		/// (or a child object) is changed.
		/// </summary>
		///
		/// <returns>The change count.</returns>
		public long getChangeCount() {
			// Make sure each of the checkChanged is called.
			bool changed = signature_.checkChanged();
			changed = name_.checkChanged() || changed;
			changed = metaInfo_.checkChanged() || changed;
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
	
		private readonly ChangeCounter signature_;
		private readonly ChangeCounter name_;
		private readonly ChangeCounter metaInfo_;
		private Blob content_;
		private LocalControlHeader localControlHeader_;
		private SignedBlob defaultWireEncoding_;
		private WireFormat defaultWireEncodingFormat_;
		private long getDefaultWireEncodingChangeCount_;
		private long changeCount_;
	}
}