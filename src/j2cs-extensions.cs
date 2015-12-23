/**
 * Copyright (C) 2015 Regents of the University of California.
 * @author: Jeff Thompson <jefft0@remap.ucla.edu>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * A copy of the GNU Lesser General Public License is in the file COPYING.
 */

using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using net.named_data.jndn;
using net.named_data.jndn.encoding.der;
using net.named_data.jndn.encrypt.algo;

namespace net.named_data.jndn.util {
  /// <summary>
  /// We set the j2cstranslator to not capitalize method names, but it mistakenly doesn't
  /// capitalize method names for .NET classes, so this has extension methods for the
  /// uncapitalized methods to call the capitalized ones.
  /// </summary>
  public static class J2CsExtensions {
    // Enum extensions.
    public static int 
    getNumericType(this ContentType contentType) { return (int)contentType; }

    public static int 
    getNumericType(this DerNodeType nodeType) { return (int)nodeType; }

    public static int 
    getNumericType(this EncryptAlgorithmType algorithmType) { return (int)algorithmType; }

    // String extensions.
    public static int 
    indexOf(this String str, char value) { return str.IndexOf(value); }

    public static int 
    indexOf(this String str, char value, int startIndex) { return str.IndexOf(value, startIndex); }

    public static string[]
    split(this String str, string regex) { return Regex.Split(str, regex); }

    public static string 
    trim(this String str) { return str.Trim(); }

    // StringBuilder extensions.
    public static StringBuilder 
    append(this StringBuilder builder, int value) 
    { 
      builder.Append(value);
      return builder;
    }

    public static StringBuilder 
    append(this StringBuilder builder, long value) 
    { 
      builder.Append(value);
      return builder;
    }

    public static StringBuilder 
    append(this StringBuilder builder, string value) 
    { 
      builder.Append(value);
      return builder;
    }

    public static string 
    toString(this StringBuilder builder) { return builder.ToString(); }
  }

  public class BufferUnderflowException : Exception {
    public BufferUnderflowException(string message) : base(message) {}
  }

  public class ParseException : Exception {
    public ParseException(string message) : base(message) {}
  }

  /// <summary>
  /// This is a simplified implementation of java.text.SimpleDateFormat which 
  /// is hard-wired to UTC time.
  /// </summary>
  public class SimpleDateFormat {
    public SimpleDateFormat(string format) {
      format_ = format; 
    }

    public string
    format(DateTime dateTime) { return dateTime.ToUniversalTime().ToString(format_); }

    public DateTime
    parse(string value) 
    { 
      return DateTime.ParseExact(value, format_, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime(); 
    }

    public void
    setTimeZone(string timeZone)
    {
      // We always use UTC.
      if (timeZone != "UTC")
        throw new NotSupportedException
          ("SimpleDateFormat.setTimeZone does not support timeZone " + timeZone);
    }

    private string format_;
  }

  public class SecureRandom {
  }
}

namespace System.Collections {
  /// <summary>
  /// This is used with SimpleDateFormat.
  /// </summary>
  public class TimeZone {
    public static string
    getTimeZone(string timeZone)
    {
      if (timeZone == "UTC")
        return timeZone;
      else
        throw new NotSupportedException
          ("TimeZone.getTimeZone does not support timeZone " + timeZone);
    }
  }
}