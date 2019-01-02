// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.sync {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Security.Cryptography;
	using net.named_data.jndn.util;
	
	public class DigestTree {
		public DigestTree() {
			this.digestNode_ = new ArrayList<DigestTree.Node>();
			root_ = "00";
		}
	
		public class Node {
			/// <summary>
			/// Create a new DigestTree.Node with the given fields and compute the digest.
			/// </summary>
			///
			/// <param name="dataPrefix">The data prefix. This is encoded as UTF-8 to digest.</param>
			/// <param name="sessionNo">The session number.</param>
			/// <param name="sequenceNo">The sequence number.</param>
			public Node(String dataPrefix, long sessionNo, long sequenceNo) {
				dataPrefix_ = dataPrefix;
				sessionNo_ = sessionNo;
				sequenceNo_ = sequenceNo;
				recomputeDigest();
			}
	
			public String getDataPrefix() {
				return dataPrefix_;
			}
	
			public long getSessionNo() {
				return sessionNo_;
			}
	
			public long getSequenceNo() {
				return sequenceNo_;
			}
	
			/// <summary>
			/// Get the digest.
			/// </summary>
			///
			/// <returns>The digest as a hex string.</returns>
			public String getDigest() {
				return digest_;
			}
	
			/// <summary>
			/// Set the sequence number and recompute the digest.
			/// </summary>
			///
			/// <param name="sequenceNo">The new sequence number.</param>
			public void setSequenceNo(long sequenceNo) {
				sequenceNo_ = sequenceNo;
				recomputeDigest();
			}
	
			/// <summary>
			/// Compare this Node with node2 first comparing dataPrefix_ then sessionNo_.
			/// </summary>
			///
			/// <param name="node2">The other Node to compare.</param>
			/// <returns>True if this node is less than node2.</returns>
			public bool lessThan(DigestTree.Node  node2) {
				// We compare the Unicode strings which is OK because it has the same sort
				// order as the UTF-8 encoding: http://en.wikipedia.org/wiki/UTF-8#Advantages
				// "Sorting a set of UTF-8 encoded strings as strings of unsigned bytes
				// yields the same order as sorting the corresponding Unicode strings
				// lexicographically by codepoint."
				int prefixComparison = String.CompareOrdinal(dataPrefix_,node2.dataPrefix_);
				if (prefixComparison != 0)
					return prefixComparison < 0;
	
				return sessionNo_ < node2.sessionNo_;
			}
	
			/// <summary>
			/// Digest the fields and set digest_ to the hex digest.
			/// </summary>
			///
			public void recomputeDigest() {
				MD5 sha256;
				try {
					sha256 = System.Security.Cryptography.MD5.Create();
				} catch (Exception exception) {
					// Don't expect this to happen.
					throw new Exception("MessageDigest: SHA-256 is not supported: "
							+ exception.Message);
				}
	
				byte[] number = new byte[4];
				// Debug: sync-state-proto.proto defines seq and session as uint64, but
				//   the original ChronoChat-js only digests 32 bits.
				int32ToLittleEndian((int) sessionNo_, number);
				sha256.ComputeHash(number);
				int32ToLittleEndian((int) sequenceNo_, number);
				sha256.ComputeHash(number);
				byte[] sequenceDigest = sha256.Hash;
	
				sha256.Initialize();
				try {
					sha256.ComputeHash(ILOG.J2CsMapping.Util.StringUtil.GetBytes(dataPrefix_,"UTF-8"));
				} catch (IOException ex) {
					// We don't expect this to happen.
					throw new Exception("UTF-8 encoder not supported: "
							+ ex.Message);
				}
				byte[] nameDigest = sha256.Hash;
	
				sha256.Initialize();
				sha256.ComputeHash(nameDigest);
				sha256.ComputeHash(sequenceDigest);
				byte[] nodeDigest = sha256.Hash;
				digest_ = net.named_data.jndn.util.Common.toHex(nodeDigest);
			}
	
			public static void int32ToLittleEndian(int value_ren, byte[] result) {
				for (int i = 0; i < 4; i++) {
					result[i] = (byte) (value_ren & 0xff);
					value_ren >>= 8;
				}
			}
	
			private readonly String dataPrefix_;
			private readonly long sessionNo_;
			private long sequenceNo_;
			private String digest_;
		}
	
		/// <summary>
		/// Update the digest tree and recompute the root digest.  If the combination
		/// of dataPrefix and sessionNo already exists in the tree then update its
		/// sequenceNo (only if the given sequenceNo is newer), otherwise add a new node.
		/// </summary>
		///
		/// <param name="dataPrefix">The name prefix. This is encoded as UTF-8 to digest.</param>
		/// <param name="sessionNo">The session number.</param>
		/// <param name="sequenceNo">The new sequence number.</param>
		/// <returns>True if the digest tree is updated, false if not (because the
		/// given sequenceNo is not newer than the existing sequence number).</returns>
		public bool update(String dataPrefix, long sessionNo,
				long sequenceNo) {
			int nodeIndex = find(dataPrefix, sessionNo);
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(DigestTree).FullName).log(ILOG.J2CsMapping.Util.Logging.Level.FINE,
					"{0}, {1}", new Object[] { dataPrefix, sessionNo });
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(DigestTree).FullName).log(ILOG.J2CsMapping.Util.Logging.Level.FINE,
					"DigestTree.update session {0}, nodeIndex {1}",
					new Object[] { sessionNo, nodeIndex });
			if (nodeIndex >= 0) {
				// Only update to a  newer status.
				if (digestNode_[nodeIndex].getSequenceNo() < sequenceNo)
					digestNode_[nodeIndex].setSequenceNo(sequenceNo);
				else
					return false;
			} else {
				ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(DigestTree).FullName).log(ILOG.J2CsMapping.Util.Logging.Level.FINE,
						"new comer {0}, session {1}, sequence {2}",
						new Object[] { dataPrefix, sessionNo, sequenceNo });
				// Insert into digestnode_ sorted.
				DigestTree.Node  temp = new DigestTree.Node (dataPrefix, sessionNo, sequenceNo);
				// Find the index of the first node where it is not less than temp.
				int i = 0;
				while (i < digestNode_.Count) {
					if (!digestNode_[i].lessThan(temp))
						break;
					++i;
				}
				digestNode_.Insert(i, temp);
			}
	
			recomputeRoot();
			return true;
		}
	
		public int find(String dataPrefix, long sessionNo) {
			for (int i = 0; i < digestNode_.Count; ++i) {
				if (digestNode_[i].getDataPrefix().equals(dataPrefix)
						&& digestNode_[i].getSessionNo() == sessionNo)
					return i;
			}
	
			return -1;
		}
	
		public int size() {
			return digestNode_.Count;
		}
	
		public DigestTree.Node  get(int i) {
			return digestNode_[i];
		}
	
		/// <summary>
		/// Get the root digest.
		/// </summary>
		///
		/// <returns>The root digest as a hex string.</returns>
		public String getRoot() {
			return root_;
		}
	
		/// <summary>
		/// Convert the hex character to an integer from 0 to 15, or -1 if not a hex character.
		/// </summary>
		///
		private static int fromHexChar(char c) {
			if (c >= '0' && c <= '9')
				return (int) c - (int) '0';
			else if (c >= 'A' && c <= 'F')
				return (int) c - (int) 'A' + 10;
			else if (c >= 'a' && c <= 'f')
				return (int) c - (int) 'a' + 10;
			else
				return -1;
		}
	
		/// <summary>
		/// Convert the hex string to bytes and call messageDigest.update.
		/// </summary>
		///
		/// <param name="messageDigest">The MessageDigest to update.</param>
		/// <param name="hex">The hex string.</param>
		private static void updateHex(MD5 messageDigest, String hex) {
			byte[] data = new byte[hex.Length / 2];
			for (int i = 0; i < data.Length; ++i)
				data[i] = (byte) ((16 * fromHexChar(hex[2 * i]) + fromHexChar(hex[2 * i + 1])) & 0xff);
	
			messageDigest.ComputeHash(data);
		}
	
		/// <summary>
		/// Set root_ to the digest of all digests in digestnode_. This sets root_
		/// to the hex value of the digest.
		/// </summary>
		///
		private void recomputeRoot() {
			MD5 sha256;
			try {
				sha256 = System.Security.Cryptography.MD5.Create();
			} catch (Exception exception) {
				// Don't expect this to happen.
				throw new Exception("MessageDigest: SHA-256 is not supported: "
						+ exception.Message);
			}
	
			for (int i = 0; i < digestNode_.Count; ++i)
				updateHex(sha256, digestNode_[i].getDigest());
			byte[] digestRoot = sha256.Hash;
			root_ = net.named_data.jndn.util.Common.toHex(digestRoot);
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(DigestTree).FullName).log(ILOG.J2CsMapping.Util.Logging.Level.FINE,
					"update root to: {0}", root_);
		}
	
		private readonly ArrayList<DigestTree.Node> digestNode_;
		private String root_;
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
