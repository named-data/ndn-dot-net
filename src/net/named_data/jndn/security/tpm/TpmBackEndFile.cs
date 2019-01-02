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
namespace net.named_data.jndn.security.tpm {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Text;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// TpmBackEndFile extends TpmBackEnd to implement a TPM back-end using
	/// on-disk file storage. In this TPM, each private key is stored in a separate
	/// file with permission 0400, i.e., owner read-only.  The key is stored in
	/// PKCS #1 format in base64 encoding.
	/// </summary>
	///
	public class TpmBackEndFile : TpmBackEnd {
		/// <summary>
		/// A TpmBackEndFile.Error extends TpmBackEnd.Error and represents a
		/// non-semantic error in backend TPM file processing.
		/// Note that even though this is called "Error" to be consistent with the
		/// other libraries, it extends the Java Exception class, not Error.
		/// </summary>
		///
		[Serializable]
		public class Error : TpmBackEnd.Error {
			public Error(String message) : base(message) {
			}
		}
	
		/// <summary>
		/// Create a TpmBackEndFile to store files in the default location
		/// HOME/.ndn/ndnsec-key-file where HOME is System.getProperty("user.home").
		/// This creates the directory if it doesn't exist.
		/// </summary>
		///
		public TpmBackEndFile() {
			keyStorePath_ = new FileInfo(
					getDefaultDirecoryPath(net.named_data.jndn.util.Common.getHomeDirectory()));
			System.IO.Directory.CreateDirectory(keyStorePath_.FullName);
		}
	
		/// <summary>
		/// Create a TpmBackEndFile to use the given path to store files.
		/// </summary>
		///
		/// <param name="locationPath">the default directory path from an Android files directory with getDefaultDirecoryPath(context.getFilesDir()) .</param>
		public TpmBackEndFile(String locationPath) {
			keyStorePath_ = new FileInfo(locationPath);
			System.IO.Directory.CreateDirectory(keyStorePath_.FullName);
		}
	
		/// <summary>
		/// Get the default directory path for private keys based on the files root.
		/// For example if filesRoot is "/data/data/org.example/files", this returns
		/// "/data/data/org.example/files/.ndn/ndnsec-tpm-file".
		/// </summary>
		///
		/// <param name="filesRoot"></param>
		/// <returns>The default directory path.</returns>
		public static String getDefaultDirecoryPath(FileInfo filesRoot) {
			return getDefaultDirecoryPath(filesRoot.FullName);
		}
	
		/// <summary>
		/// Get the default directory path for private keys based on the files root.
		/// </summary>
		///
		/// <param name="filesRoot">The root file directory.</param>
		/// <returns>The default directory path.</returns>
		public static String getDefaultDirecoryPath(String filesRoot) {
			// NOTE: Use File because java.nio.file.Path is not available before Java 7.
			return new FileInfo(System.IO.Path.Combine(new FileInfo(System.IO.Path.Combine(new FileInfo(filesRoot).FullName,".ndn")).FullName,"ndnsec-key-file")).FullName;
		}
	
		public static String getScheme() {
			return "tpm-file";
		}
	
		/// <summary>
		/// Check if the key with name keyName exists in the TPM.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>True if the key exists.</returns>
		protected internal override bool doHasKey(Name keyName) {
			if (!toFilePath(keyName).Exists)
				return false;
	
			try {
				loadKey(keyName);
				return true;
			} catch (TpmBackEnd.Error ex) {
				return false;
			}
		}
	
		/// <summary>
		/// Get the handle of the key with name keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The handle of the key, or null if the key does not exist.</returns>
		protected internal override TpmKeyHandle doGetKeyHandle(Name keyName) {
			if (!doHasKey(keyName))
				return null;
	
			return new TpmKeyHandleMemory(loadKey(keyName));
		}
	
		/// <summary>
		/// Create a key for identityName according to params. The created key is
		/// named as: /{identityName}/[keyId]/KEY . The key name is set in the returned
		/// TpmKeyHandle.
		/// </summary>
		///
		/// <param name="identityName">The name if the identity.</param>
		/// <param name="params">The KeyParams for creating the key.</param>
		/// <returns>The handle of the created key.</returns>
		/// <exception cref="TpmBackEnd.Error">if the key cannot be created.</exception>
		protected internal override TpmKeyHandle doCreateKey(Name identityName, KeyParams paras) {
			TpmPrivateKey key;
			try {
				key = net.named_data.jndn.security.tpm.TpmPrivateKey.generatePrivateKey(paras);
			} catch (TpmPrivateKey.Error ex) {
				throw new TpmBackEnd.Error(
						"Error in TpmPrivateKey.generatePrivateKey: " + ex);
			}
			TpmKeyHandle keyHandle = new TpmKeyHandleMemory(key);
	
			net.named_data.jndn.security.tpm.TpmBackEnd.setKeyName(keyHandle, identityName, paras);
	
			saveKey(keyHandle.getKeyName(), key);
			return keyHandle;
		}
	
		/// <summary>
		/// Delete the key with name keyName. If the key doesn't exist, do nothing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key to delete.</param>
		/// <exception cref="TpmBackEnd.Error">if the deletion fails.</exception>
		protected internal override void doDeleteKey(Name keyName) {
			toFilePath(keyName).delete();
		}
	
		/// <summary>
		/// Get the encoded private key with name keyName in PKCS #8 format, possibly
		/// password-encrypted.
		/// </summary>
		///
		/// <param name="keyName">The name of the key in the TPM.</param>
		/// <param name="password">it to return a PKCS #8 EncryptedPrivateKeyInfo. If the password is null, return an unencrypted PKCS #8 PrivateKeyInfo.</param>
		/// <returns>The encoded private key.</returns>
		/// <exception cref="TpmBackEnd.Error">if the key does not exist or if the key cannot beexported, e.g., insufficient privileges.</exception>
		protected internal override Blob doExportKey(Name keyName, ByteBuffer password) {
			TpmPrivateKey key;
			try {
				key = loadKey(keyName);
			} catch (TpmBackEnd.Error ex) {
				throw new TpmBackEnd.Error("Cannot export private key: " + ex);
			}
	
			try {
				if (password != null)
					return key.toEncryptedPkcs8(password);
				else
					return key.toPkcs8();
			} catch (TpmPrivateKey.Error ex_0) {
				// We don't expect this since we just decoded it.
				throw new TpmBackEnd.Error("Error PKCS#8 encoding private key: "
						+ ex_0);
			}
		}
	
		/// <summary>
		/// Import an encoded private key with name keyName in PKCS #8 format, possibly
		/// password-encrypted.
		/// </summary>
		///
		/// <param name="keyName">The name of the key to use in the TPM.</param>
		/// <param name="pkcs8">unencrypted PKCS #8 PrivateKeyInfo.</param>
		/// <param name="password">it to decrypt the PKCS #8 EncryptedPrivateKeyInfo. If the password is null, import an unencrypted PKCS #8 PrivateKeyInfo.</param>
		/// <exception cref="TpmBackEnd.Error">for an error importing the key.</exception>
		protected internal override void doImportKey(Name keyName, ByteBuffer pkcs8,
				ByteBuffer password) {
			TpmPrivateKey key = new TpmPrivateKey();
			try {
				if (password != null)
					key.loadEncryptedPkcs8(pkcs8, password);
				else
					key.loadPkcs8(pkcs8);
			} catch (TpmPrivateKey.Error ex) {
				throw new TpmBackEnd.Error("Cannot import private key: " + ex);
			}
	
			saveKey(keyName, key);
		}
	
		/// <summary>
		/// Load the private key with name keyName from the key file directory.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The key loaded into a TpmPrivateKey.</returns>
		internal TpmPrivateKey loadKey(Name keyName) {
			TpmPrivateKey key = new TpmPrivateKey();
			StringBuilder base64 = new StringBuilder();
			try {
				TextReader reader = new FileReader(
									toFilePath(keyName).FullName);
				// Use "try/finally instead of "try-with-resources" or "using"
				// which are not supported before Java 7.
				try {
					String line = null;
					while ((line = reader.readLine()) != null)
						base64.append(line);
				} finally {
					reader.close();
				}
			} catch (FileNotFoundException ex) {
				throw new TpmBackEnd.Error("Error reading private key file: " + ex);
			} catch (IOException ex_0) {
				throw new TpmBackEnd.Error("Error reading private key file: " + ex_0);
			}
	
			byte[] pkcs = net.named_data.jndn.util.Common.base64Decode(base64.toString());
	
			try {
				key.loadPkcs1(ILOG.J2CsMapping.NIO.ByteBuffer.wrap(pkcs),  default(KeyType)/* was: null */);
			} catch (TpmPrivateKey.Error ex_1) {
				throw new TpmBackEnd.Error("Error decoding private key file: " + ex_1);
			}
			return key;
		}
	
		/// <summary>
		/// Save the private key using keyName into the key file directory.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="key">The private key to save.</param>
		private void saveKey(Name keyName, TpmPrivateKey key) {
			String filePath = toFilePath(keyName).FullName;
			String base64;
			try {
				base64 = net.named_data.jndn.util.Common.base64Encode(key.toPkcs1().getImmutableArray(),
						true);
			} catch (TpmPrivateKey.Error ex) {
				throw new TpmBackEnd.Error("Error encoding private key file: " + ex);
			}
	
			try {
				var writer = (new StreamWriter(filePath));
				// Use "try/finally instead of "try-with-resources" or "using"
				// which are not supported before Java 7.
				try {
					writer.Write(base64,0,base64.Substring(0,base64.Length));
					writer.flush();
				} finally {
					writer.close();
				}
			} catch (IOException ex_0) {
				throw new TpmBackEnd.Error("Error writing private key file: " + ex_0);
			}
		}
	
		/// <summary>
		/// Get the file path for the keyName, which is keyStorePath_ + "/" +
		/// hex(sha256(keyName-wire-encoding)) + ".privkey" .
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The file path for the key.</returns>
		private FileInfo toFilePath(Name keyName) {
			Blob keyEncoding = keyName.wireEncode();
			byte[] digest = net.named_data.jndn.util.Common.digestSha256(keyEncoding.buf());
	
			return new FileInfo(System.IO.Path.Combine(keyStorePath_.FullName,new Blob(digest, false).toHex()
							+ ".privkey"));
		}
	
		private FileInfo keyStorePath_;
	}
}
