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
namespace net.named_data.jndn.util.regex {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	
	public class NdnRegexPseudoMatcher : NdnRegexMatcherBase {
		public NdnRegexPseudoMatcher() : base("", net.named_data.jndn.util.regex.NdnRegexMatcherBase.NdnRegexExprType.PSEUDO) {
		}
	
		protected internal override void compile() {
		}
	
		public void setMatchResult(String str) {
			ILOG.J2CsMapping.Collections.Collections.Add(matchResult_,new Name.Component(str));
		}
	
		public void resetMatchResult() {
			ILOG.J2CsMapping.Collections.Collections.Clear(matchResult_);
		}
	}
}
