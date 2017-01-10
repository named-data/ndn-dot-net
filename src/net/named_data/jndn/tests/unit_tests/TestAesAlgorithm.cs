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
namespace net.named_data.jndn.tests.unit_tests {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using javax.crypto;
	using net.named_data.jndn.encrypt;
	using net.named_data.jndn.encrypt.algo;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	public class TestAesAlgorithm {
		// Convert the int array to a ByteBuffer.
		private static ByteBuffer toBuffer(int[] array) {
			ByteBuffer result = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(array.Length);
			for (int i = 0; i < array.Length; ++i)
				result.put((byte) (array[i] & 0xff));
	
			result.flip();
			return result;
		}
	
		private static readonly ByteBuffer KEY = toBuffer(new int[] { 0xdd, 0x60,
				0x77, 0xec, 0xa9, 0x6b, 0x23, 0x1b, 0x40, 0x6b, 0x5a, 0xf8, 0x7d,
				0x3d, 0x55, 0x32 });
	
		// plaintext: AES-Encrypt-Test
		private static readonly ByteBuffer PLAINTEXT = toBuffer(new int[] { 0x41,
				0x45, 0x53, 0x2d, 0x45, 0x6e, 0x63, 0x72, 0x79, 0x70, 0x74, 0x2d,
				0x54, 0x65, 0x73, 0x74 });
	
		private static readonly ByteBuffer CIPHERTEXT_ECB = toBuffer(new int[] { 0xcb,
				0xe5, 0x6a, 0x80, 0x41, 0x24, 0x58, 0x23, 0x84, 0x14, 0x15, 0x61,
				0x80, 0xb9, 0x5e, 0xbd, 0xce, 0x32, 0xb4, 0xbe, 0xbc, 0x91, 0x31,
				0xd6, 0x19, 0x00, 0x80, 0x8b, 0xfa, 0x00, 0x05, 0x9c });
	
		private static readonly ByteBuffer INITIAL_VECTOR = toBuffer(new int[] { 0x6f,
				0x53, 0x7a, 0x65, 0x58, 0x6c, 0x65, 0x75, 0x44, 0x4c, 0x77, 0x35,
				0x58, 0x63, 0x78, 0x6e });
	
		private static readonly ByteBuffer CIPHERTEXT_CBC_IV = toBuffer(new int[] {
				0xb7, 0x19, 0x5a, 0xbb, 0x23, 0xbf, 0x92, 0xb0, 0x95, 0xae, 0x74,
				0xe9, 0xad, 0x72, 0x7c, 0x28, 0x6e, 0xc6, 0x73, 0xb5, 0x0b, 0x1a,
				0x9e, 0xb9, 0x4d, 0xc5, 0xbd, 0x8b, 0x47, 0x1f, 0x43, 0x00 });
	
		public void testEncryptionDecryption() {
			EncryptParams encryptParams = new EncryptParams(
					net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesEcb, 16);
	
			Blob key = new Blob(KEY, false);
			DecryptKey decryptKey = new DecryptKey(key);
			EncryptKey encryptKey = net.named_data.jndn.encrypt.algo.AesAlgorithm.deriveEncryptKey(decryptKey
					.getKeyBits());
	
			// Check key loading and key derivation.
			Assert.AssertTrue(encryptKey.getKeyBits().equals(key));
			Assert.AssertTrue(decryptKey.getKeyBits().equals(key));
	
			Blob plainBlob = new Blob(PLAINTEXT, false);
	
			// Encrypt data in AES_ECB.
			Blob cipherBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.encrypt(encryptKey.getKeyBits(),
					plainBlob, encryptParams);
			Assert.AssertTrue(cipherBlob.equals(new Blob(CIPHERTEXT_ECB, false)));
	
			// Decrypt data in AES_ECB.
			Blob receivedBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(decryptKey.getKeyBits(),
					cipherBlob, encryptParams);
			Assert.AssertTrue(receivedBlob.equals(plainBlob));
	
			// Encrypt/decrypt data in AES_CBC with auto-generated IV.
			encryptParams.setAlgorithmType(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc);
			cipherBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.encrypt(encryptKey.getKeyBits(), plainBlob,
					encryptParams);
			receivedBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(decryptKey.getKeyBits(),
					cipherBlob, encryptParams);
			Assert.AssertTrue(receivedBlob.equals(plainBlob));
	
			// Encrypt data in AES_CBC with specified IV.
			Blob initialVector = new Blob(INITIAL_VECTOR, false);
			encryptParams.setInitialVector(initialVector);
			cipherBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.encrypt(encryptKey.getKeyBits(), plainBlob,
					encryptParams);
			Assert.AssertTrue(cipherBlob.equals(new Blob(CIPHERTEXT_CBC_IV, false)));
	
			// Decrypt data in AES_CBC with specified IV.
			receivedBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(decryptKey.getKeyBits(),
					cipherBlob, encryptParams);
			Assert.AssertTrue(receivedBlob.equals(plainBlob));
		}
	
		public void testKeyGeneration() {
			AesKeyParams keyParams = new AesKeyParams(128);
			DecryptKey decryptKey = net.named_data.jndn.encrypt.algo.AesAlgorithm.generateKey(keyParams);
			EncryptKey encryptKey = net.named_data.jndn.encrypt.algo.AesAlgorithm.deriveEncryptKey(decryptKey
					.getKeyBits());
	
			Blob plainBlob = new Blob(PLAINTEXT, false);
	
			// Encrypt/decrypt data in AES_CBC with auto-generated IV.
			EncryptParams encryptParams = new EncryptParams(
					net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesEcb, 16);
			Blob cipherBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.encrypt(encryptKey.getKeyBits(),
					plainBlob, encryptParams);
			Blob receivedBlob = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(decryptKey.getKeyBits(),
					cipherBlob, encryptParams);
			Assert.AssertTrue(receivedBlob.equals(plainBlob));
		}
	}}
