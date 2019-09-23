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
namespace net.named_data.jndn.sync.detail {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Text;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encoding.tlv;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// The PSyncState class represents a sequence of Names as the state of PSync.
	/// It has methods to encode and decode for the wire.
	/// </summary>
	///
	public class PSyncState {
		/// <summary>
		/// Create a PSyncState with empty content.
		/// </summary>
		///
		public PSyncState() {
			this.content_ = new ArrayList<Name>();
		}
	
		/// <summary>
		/// Create a PSyncState by decoding the input as an NDN-TLV PSyncContent.
		/// </summary>
		///
		/// <param name="input"></param>
		public PSyncState(ByteBuffer input) {
			this.content_ = new ArrayList<Name>();
			wireDecode(input);
		}
	
		/// <summary>
		/// Create a PSyncState by decoding the input as an NDN-TLV PSyncContent.
		/// </summary>
		///
		/// <param name="input"></param>
		public PSyncState(Blob input) {
			this.content_ = new ArrayList<Name>();
			wireDecode(input);
		}
	
		/// <summary>
		/// Append the name to the content.
		/// </summary>
		///
		/// <param name="name">The Name to add, which is copied.</param>
		public void addContent(Name name) {
			ILOG.J2CsMapping.Collections.Collections.Add(content_,new Name(name));
		}
	
		/// <summary>
		/// Get the sequence of Names in the content.
		/// </summary>
		///
		/// <returns>The array of Names, which should not be modified.</returns>
		public ArrayList<Name> getContent() {
			return content_;
		}
	
		/// <summary>
		/// Remove the content.
		/// </summary>
		///
		public void clear() {
			ILOG.J2CsMapping.Collections.Collections.Clear(content_);
		}
	
		/// <summary>
		/// Encode this as an NDN-TLV PSyncContent.
		/// </summary>
		///
		/// <returns>The encoding as a Blob.</returns>
		public Blob wireEncode() {
			TlvEncoder encoder = new TlvEncoder(256);
			int saveLength = encoder.getLength();
	
			// Encode backwards.
			for (int i = content_.Count - 1; i >= 0; --i)
				net.named_data.jndn.encoding.Tlv0_2WireFormat.encodeName(content_[i], new int[1],
						new int[1], encoder);
	
			encoder.writeTypeAndLength(Tlv_PSyncContent, encoder.getLength()
					- saveLength);
	
			return new Blob(encoder.getOutput(), false);
		}
	
		/// <summary>
		/// Decode the input as an NDN-TLV PSyncContent and update this object.
		/// </summary>
		///
		/// <param name="input"></param>
		public void wireDecode(ByteBuffer input) {
			clear();
	
			// Decode directly as TLV. We don't support the WireFormat abstraction
			// because this isn't meant to go directly on the wire.
			TlvDecoder decoder = new TlvDecoder(input);
			int endOffset = decoder.readNestedTlvsStart(Tlv_PSyncContent);
	
			// Decode a sequence of Name.
			while (decoder.getOffset() < endOffset) {
				Name name = new Name();
				net.named_data.jndn.encoding.Tlv0_2WireFormat.decodeName(name, new int[1], new int[1], decoder,
						true);
				ILOG.J2CsMapping.Collections.Collections.Add(content_,name);
			}
	
			decoder.finishNestedTlvs(endOffset);
		}
	
		/// <summary>
		/// Decode the input as an NDN-TLV PSyncContent and update this object.
		/// </summary>
		///
		/// <param name="input"></param>
		public void wireDecode(Blob input) {
			wireDecode(input.buf());
		}
	
		/// <summary>
		/// Get the string representation of this PSyncState.
		/// </summary>
		///
		/// <returns>The string representation.</returns>
		public override String ToString() {
			StringBuilder result = new StringBuilder();
	
			result.append("[");
	
			for (int i = 0; i < content_.Count; ++i) {
				result.append(content_[i].toUri());
				if (i < content_.Count - 1)
					result.append(", ");
			}
	
			result.append("]");
	
			return result.toString();
		}
	
		public const int Tlv_PSyncContent = 128;
	
		private ArrayList<Name> content_;
	}
}