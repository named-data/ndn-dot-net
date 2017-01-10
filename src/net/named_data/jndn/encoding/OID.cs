// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encoding {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.util;
	
	public class OID {
		public OID(String oid) {
			this.oid_ = new int[0];
			String[] splitString = ILOG.J2CsMapping.Text.RegExUtil.Split(oid, "\\.");
			oid_ = new int[splitString.Length];
			for (int i = 0; i < oid_.Length; ++i)
				oid_[i] = Int32.Parse(splitString[i]);
		}
	
		public OID(int[] oid) {
			this.oid_ = new int[0];
			setIntegerList(oid);
		}
	
		public int[] getIntegerList() {
			return oid_;
		}
	
		public void setIntegerList(int[] oid) {
			oid_ = new int[oid.Length];
			for (int i = 0; i < oid_.Length; ++i)
				oid_[i] = oid[i];
		}
	
		public override String ToString() {
			String result = "";
			for (int i = 0; i < oid_.Length; ++i) {
				if (i != 0)
					result += ".";
				result += oid_[i];
			}
	
			return result;
		}
	
		public bool equals(OID other) {
			if (other == null || oid_.Length != other.oid_.Length)
				return false;
	
			for (int i = 0; i < oid_.Length; ++i) {
				if (oid_[i] != other.oid_[i])
					return false;
			}
			return true;
		}
	
		public override bool Equals(Object other) {
			if (!(other  is  OID))
				return false;
	
			return equals((OID) other);
		}
	
		private int[] oid_;
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
