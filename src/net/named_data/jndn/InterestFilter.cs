// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.util;
	using net.named_data.jndn.util.regex;
	
	/// <summary>
	/// An InterestFilter holds a Name prefix and optional regex match expression for
	/// use in Face.setInterestFilter.
	/// </summary>
	///
	public class InterestFilter {
		/// <summary>
		/// Create an InterestFilter to match any Interest whose name starts with the
		/// given prefix.
		/// </summary>
		///
		/// <param name="prefix">The prefix Name. This makes a copy of the Name.</param>
		public InterestFilter(Name prefix) {
			prefix_ = new Name(prefix);
			regexFilter_ = null;
			regexFilterPattern_ = null;
		}
	
		/// <summary>
		/// Create an InterestFilter to match any Interest whose name starts with the
		/// given prefix.
		/// </summary>
		///
		/// <param name="prefixUri">The URI of the prefix Name.</param>
		public InterestFilter(String prefixUri) {
			prefix_ = new Name(prefixUri);
			regexFilter_ = null;
			regexFilterPattern_ = null;
		}
	
		/// <summary>
		/// Create an InterestFilter to match any Interest whose name starts with the
		/// given prefix and the remaining components match the regexFilter regular
		/// expression as described in doesMatch.
		/// </summary>
		///
		/// <param name="prefix">The prefix Name. This makes a copy of the Name.</param>
		/// <param name="regexFilter"></param>
		public InterestFilter(Name prefix, String regexFilter) {
			prefix_ = new Name(prefix);
			regexFilter_ = regexFilter;
			regexFilterPattern_ = makePattern(regexFilter);
		}
	
		/// <summary>
		/// Create an InterestFilter to match any Interest whose name starts with the
		/// given prefix URI and the remaining components match the regexFilter regular
		/// expression as described in doesMatch.
		/// </summary>
		///
		/// <param name="prefixUri">The URI of the prefix Name.</param>
		/// <param name="regexFilter"></param>
		public InterestFilter(String prefixUri, String regexFilter) {
			prefix_ = new Name(prefixUri);
			regexFilter_ = regexFilter;
			regexFilterPattern_ = makePattern(regexFilter);
		}
	
		/// <summary>
		/// Create an InterestFilter which is a copy of the given interestFilter.
		/// </summary>
		///
		/// <param name="interestFilter">The InterestFilter with values to copy from.</param>
		public InterestFilter(InterestFilter interestFilter) {
			// Make a deep copy of the Name.
			prefix_ = new Name(interestFilter.prefix_);
			regexFilter_ = interestFilter.regexFilter_;
			regexFilterPattern_ = interestFilter.regexFilterPattern_;
		}
	
		/// <summary>
		/// Check if the given name matches this filter. Match if name starts with this
		/// filter's prefix. If this filter has the optional regexFilter then the
		/// remaining components match the regexFilter regular expression.
		/// For example, the following InterestFilter:
		/// InterestFilter("/hello", "&lt;world&gt;&lt;&gt;+")
		/// will match all Interests, whose name has the prefix `/hello` which is
		/// followed by a component `world` and has at least one more component after it.
		/// Examples:
		/// /hello/world/!
		/// /hello/world/x/y/z
		/// Note that the regular expression will need to match all remaining components
		/// (e.g., there are implicit heading `^` and trailing `$` symbols in the
		/// regular expression).
		/// </summary>
		///
		/// <param name="name">The name to check against this filter.</param>
		/// <returns>True if name matches this filter, otherwise false.</returns>
		public bool doesMatch(Name name) {
			if (name.size() < prefix_.size())
				return false;
	
			if (hasRegexFilter()) {
				// Perform a prefix match and regular expression match for the remaining
				// components.
				if (!prefix_.match(name))
					return false;
	
				try {
					return new NdnRegexTopMatcher(regexFilterPattern_).match(name
							.getSubName(prefix_.size()));
				} catch (NdnRegexMatcherBase.Error ex) {
					ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(InterestFilter).FullName).log(
							ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex);
					return false;
				}
			} else
				// Just perform a prefix match.
				return prefix_.match(name);
		}
	
		/// <summary>
		/// Get the prefix given to the constructor.
		/// </summary>
		///
		/// <returns>The prefix Name which you should not modify.</returns>
		public Name getPrefix() {
			return prefix_;
		}
	
		/// <summary>
		/// Check if a regexFilter was supplied to the constructor.
		/// </summary>
		///
		/// <returns>True if a regexFilter was supplied to the constructor.</returns>
		public bool hasRegexFilter() {
			return regexFilter_ != null;
		}
	
		/// <summary>
		/// Get the regex filter. This is only valid if hasRegexFilter() is true.
		/// </summary>
		///
		/// <returns>The regular expression for matching the remaining name components.</returns>
		public String getRegexFilter() {
			return regexFilter_;
		}
	
		/// <summary>
		/// If regexFilter doesn't already have them, add ^ to the beginning and $ to
		/// the end since these are required by NdnRegexTopMatcher.
		/// </summary>
		///
		/// <param name="regexFilter">The regex filter.</param>
		/// <returns>The regex pattern with ^ and $.</returns>
		private static String makePattern(String regexFilter) {
			String pattern = regexFilter;
			if (!pattern.StartsWith("^"))
				pattern = "^" + pattern;
			if (!pattern.endsWith("$"))
				pattern = pattern + "$";
	
			return pattern;
		}
	
		private readonly Name prefix_;
		private readonly String regexFilter_;
		private readonly String regexFilterPattern_;
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
