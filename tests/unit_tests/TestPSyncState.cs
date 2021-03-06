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
namespace net.named_data.jndn.tests.unit_tests {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.sync.detail;
	using net.named_data.jndn.util;
	
	public class TestPSyncState {
		// Convert the int array to a ByteBuffer.
		private static ByteBuffer toBuffer(int[] array) {
			ByteBuffer result = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(array.Length);
			for (int i = 0; i < array.Length; ++i)
				result.put((byte) (array[i] & 0xff));
	
			result.flip();
			return result;
		}
	
		public void testEncodeDecode() {
			PSyncState state = new PSyncState();
			state.addContent(new Name("test1"));
			state.addContent(new Name("test2"));
	
			// Simulate getting a buffer of content from a segment fetcher.
			Data data = new Data();
			Blob encoding = state.wireEncode();
			ByteBuffer expectedEncoding = toBuffer(new int[] { 0x80, 0x12, // PSyncContent
					0x07, 0x07, 0x08, 0x05, 0x74, 0x65, 0x73, 0x74, 0x31, // Name = "/test1"
					0x07, 0x07, 0x08, 0x05, 0x74, 0x65, 0x73, 0x74, 0x32 // Name = "/test2"
			});
			Assert.AssertTrue(encoding.equals(new Blob(expectedEncoding, false)));
			data.setContent(encoding);
	
			PSyncState receivedState = new PSyncState();
			receivedState.wireDecode(data.getContent());
	
			Assert.AssertArrayEquals(ILOG.J2CsMapping.Collections.Collections.ToArray(state.getContent()), ILOG.J2CsMapping.Collections.Collections.ToArray(receivedState
							.getContent()));
		}
	
		public void testEmptyContent() {
			PSyncState state = new PSyncState();
	
			// Simulate getting a buffer of content from a segment fetcher.
			Data data = new Data();
			data.setContent(state.wireEncode());
	
			PSyncState state2 = new PSyncState(data.getContent());
			Assert.AssertEquals(0, state2.getContent().Count);
		}
	}
}
