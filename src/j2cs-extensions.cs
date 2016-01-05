/**
 * Copyright (C) 2015-2016 Regents of the University of California.
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
using System.IO;
using System.Collections;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using ILOG.J2CsMapping.NIO;
using net.named_data.jndn;
using net.named_data.jndn.encoding.der;
using net.named_data.jndn.encrypt.algo;

namespace net.named_data.jndn.util {
  /// <summary>
  /// We set j2cstranslator to not capitalize method names, but it mistakenly doesn't
  /// capitalize method names for .NET classes, so this has extension methods for the
  /// uncapitalized methods to call the capitalized ones.
  /// </summary>
  public static class J2CsExtensions {
    // Enum extensions.
    public static int 
    getNumericType(this ContentType contentType) { return (int)contentType; }

    public static int 
    getNumericType(this EncryptAlgorithmType algorithmType) { return (int)algorithmType; }

    // Hashtable extensions.
    public static void 
    clear(this Hashtable map) { map.Clear(); }

    // String extensions.
    public static bool 
    contains(this String str, string value) { return str.Contains(value); }

    public static bool 
    endsWith(this String str, string value) { return str.EndsWith(value); }

    public static bool 
    equals(this String str, string value) { return str.Equals(value); }

    public static int 
    indexOf(this String str, char value) { return str.IndexOf(value); }

    public static int 
    indexOf(this String str, char value, int startIndex) { return str.IndexOf(value, startIndex); }

    public static string
    replace(this String str, string oldValue, string newValue) { return str.Replace(oldValue, newValue); }

    public static string[]
    split(this String str, string regex) { return Regex.Split(str, regex); }

    public static string 
    trim(this String str) { return str.Trim(); }

    // StringBuilder extensions.
    public static StringBuilder 
    append(this StringBuilder builder, char value) 
    { 
      builder.Append(value);
      return builder;
    }

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

  public class InvalidKeyException : Exception {
    public InvalidKeyException(string message) : base(message) {}
  }

  public class InvalidKeySpecException : Exception {
    public InvalidKeySpecException(string message) : base(message) {}
  }

  public interface IRunnable {
    void run();
  }

  public class ParseException : Exception {
    public ParseException(string message) : base(message) {}
  }

  public class SignatureException : Exception {
    public SignatureException(string message) : base(message) {}
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
    public void nextBytes(byte[] array) { generator_.GetBytes(array); }

    /// <summary>
    /// When when the code calls Next(), it always casts to a byte, so for
    /// simplicity just return a byte.
    /// </summary>
    public byte Next() 
    {
      var result = new byte[1];
      generator_.GetBytes(result);
      return result[0];
    }

    private static RNGCryptoServiceProvider generator_ = new RNGCryptoServiceProvider();
  }
}

/// <summary>
/// j2cstranslator makes naive assumptions and puts some Java classes into the System
/// name space, so we have to pollute the System name space with them.
/// </summary>
namespace System {
  /// <summary>
  /// j2cstranslator naively converts java.security.KeyFactory to System.KeyFactory.
  /// </summary>
  public abstract class KeyFactory {
    public static KeyFactory
    getInstance(string type)
    {
      if (type == "RSA")
        return new RsaKeyFactory();
      else
        throw new NotImplementedException("KeyFactory type is not implemented: " + type);
    }

    public abstract PrivateKey
    generatePrivate(KeySpec keySpec);

    public abstract SecurityPublicKey
    generatePublic(KeySpec keySpec);
  }

  public class RsaKeyFactory : KeyFactory {
    public override PrivateKey
    generatePrivate(KeySpec keySpec)
    {
      if (!(keySpec is PKCS8EncodedKeySpec))
        throw new net.named_data.jndn.util.InvalidKeySpecException
          ("RsaKeyFactory.generatePrivate expects a PKCS8EncodedKeySpec");

      try {
        // Decode the PKCS #8 private key.
        var parsedNode = DerNode.parse(new ByteBuffer(((PKCS8EncodedKeySpec)keySpec).KeyDer), 0);
        var pkcs8Children = parsedNode.getChildren();
        var algorithmIdChildren = DerNode.getSequence(pkcs8Children, 1).getChildren();
        var oidString = ((DerNode.DerOid)algorithmIdChildren[0]).toVal().ToString();
        var rsaPrivateKeyDer = ((DerNode)pkcs8Children[2]).getPayload();

        if (oidString != RSA_ENCRYPTION_OID)
          throw new net.named_data.jndn.util.InvalidKeySpecException
            ("The PKCS #8 private key is not RSA_ENCRYPTION");

        // Decode the PKCS #1 RSAPrivateKey.
        parsedNode = DerNode.parse(rsaPrivateKeyDer.buf(), 0);
        var rsaPrivateKeyChildren = parsedNode.getChildren();

        // Copy the parameters.
        RSAParameters parameters = new RSAParameters();
        parameters.Modulus = ((DerNode)rsaPrivateKeyChildren[1]).getPayload().getImmutableArray();
        parameters.Exponent = ((DerNode)rsaPrivateKeyChildren[2]).getPayload().getImmutableArray();
        parameters.D = ((DerNode)rsaPrivateKeyChildren[3]).getPayload().getImmutableArray();
        parameters.P = ((DerNode)rsaPrivateKeyChildren[4]).getPayload().getImmutableArray();
        parameters.Q = ((DerNode)rsaPrivateKeyChildren[5]).getPayload().getImmutableArray();
        parameters.DP = ((DerNode)rsaPrivateKeyChildren[6]).getPayload().getImmutableArray();
        parameters.DQ = ((DerNode)rsaPrivateKeyChildren[7]).getPayload().getImmutableArray();
        parameters.InverseQ = ((DerNode)rsaPrivateKeyChildren[8]).getPayload().getImmutableArray();

        return new RsaSecurityPrivateKey(parameters);
      } catch (DerDecodingException ex) {
        throw new net.named_data.jndn.util.InvalidKeySpecException
          ("RsaKeyFactory.generatePrivate error decoding the private key DER: " + ex);
      }
    }

    public override SecurityPublicKey
    generatePublic(KeySpec keySpec)
    {
      if (!(keySpec is X509EncodedKeySpec))
        throw new net.named_data.jndn.util.InvalidKeySpecException
        ("RsaKeyFactory.generatePublic expects a X509EncodedKeySpec");

      try {
        // Decode the X.509 public key.
        var parsedNode = DerNode.parse(new ByteBuffer(((X509EncodedKeySpec)keySpec).KeyDer), 0);
        var rootChildren = parsedNode.getChildren();
        var algorithmIdChildren = DerNode.getSequence(rootChildren, 0).getChildren();
        var oidString = ((DerNode.DerOid)algorithmIdChildren[0]).toVal().ToString();
        var rsaPublicKeyDerBitString = ((DerNode)rootChildren[1]).getPayload();

        if (oidString != RSA_ENCRYPTION_OID)
          throw new net.named_data.jndn.util.InvalidKeySpecException
          ("The PKCS #8 private key is not RSA_ENCRYPTION");

        // Decode the PKCS #1 RSAPublicKey.
        // Skip the leading 0 byte in the DER BitString.
        parsedNode = DerNode.parse(rsaPublicKeyDerBitString.buf(), 1);
        var rsaPublicKeyChildren = parsedNode.getChildren();

        // Copy the parameters.
        RSAParameters parameters = new RSAParameters();
        parameters.Modulus = ((DerNode)rsaPublicKeyChildren[0]).getPayload().getImmutableArray();
        parameters.Exponent = ((DerNode)rsaPublicKeyChildren[1]).getPayload().getImmutableArray();

        return new RsaSecurityPublicKey(parameters);
        } catch (DerDecodingException ex) {
          throw new net.named_data.jndn.util.InvalidKeySpecException
          ("RsaKeyFactory.generatePublic error decoding the public key DER: " + ex);
      }
    }

    private static string RSA_ENCRYPTION_OID = "1.2.840.113549.1.1.1";
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.KeySpec to System.KeySpec.
  /// </summary>
  public abstract class KeySpec {
  }

  public class PKCS8EncodedKeySpec : KeySpec {
    public PKCS8EncodedKeySpec(byte[] keyDer)
    {
      KeyDer = keyDer;
    }

    public readonly byte[] KeyDer;
  }

  public class X509EncodedKeySpec : KeySpec {
    public X509EncodedKeySpec(byte[] keyDer)
    {
      KeyDer = keyDer;
    }

    public readonly byte[] KeyDer;
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.PrivateKey to System.PrivateKey.
  /// </summary>
  public class PrivateKey {
  }

  public class RsaSecurityPrivateKey : PrivateKey {
    /// <summary>
    /// Create an RsaPrivateKey with the RSAParameters used by RSACryptoServiceProvider.
    /// </summary>
    /// <param name="parameters">Parameters.</param>
    public RsaSecurityPrivateKey(RSAParameters parameters)
    {
      Parameters = parameters;
    }

    public readonly RSAParameters Parameters;
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.PublicKey to System.PublicKey.
  /// We also globally rename System.PublicKey to System.SecurityPublicKey to not
  /// conclict with PublicKey when using net.named_data.security.certificate.
  /// </summary>
  public class SecurityPublicKey {
  }

  public class RsaSecurityPublicKey : SecurityPublicKey {
    /// <summary>
    /// Create an RsaSecurityPublicKey with the RSAParameters used by RSACryptoServiceProvider.
    /// </summary>
    /// <param name="parameters">Parameters.</param>
    public RsaSecurityPublicKey(RSAParameters parameters)
    {
      Parameters = parameters;
    }

    public readonly RSAParameters Parameters;
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.Signature to System.Signature.
  /// We also globally rename System.Signature to System.SecuritySignature to not
  /// conclict with Signature when using net.named_data.jndn.
  /// </summary>
  public abstract class SecuritySignature {
    public static SecuritySignature 
    getInstance(string type)
    {
      if (type == "SHA256withRSA")
        return new Sha256withRsaSecuritySignature();
      else
        throw new NotImplementedException("SecuritySignature type is not implemented: " + type);
    }

    public abstract void
    initSign(PrivateKey privateKey);

    public abstract void
    initVerify(SecurityPublicKey publicKey);

    public abstract byte[]
    sign();

    public abstract void
    update(ByteBuffer data);

    public abstract bool
    verify(byte[] signature);
  }

  public class Sha256withRsaSecuritySignature : SecuritySignature {
    public override void
    initSign(PrivateKey privateKey)
    {
      if (!(privateKey is RsaSecurityPrivateKey))
        throw new net.named_data.jndn.util.InvalidKeyException
        ("Sha256withRsaSecuritySignature.initSign expects an RsaSecurityPrivateKey");

      provider_ = new RSACryptoServiceProvider();
      provider_.ImportParameters(((RsaSecurityPrivateKey)privateKey).Parameters);

      memoryStream_ = new MemoryStream();
    }

    public override void
    initVerify(SecurityPublicKey publicKey)
    {
      if (!(publicKey is RsaSecurityPublicKey))
        throw new net.named_data.jndn.util.InvalidKeyException
        ("Sha256withRsaSecuritySignature.initVerify expects an RsaSecurityPublicKey");

      provider_ = new RSACryptoServiceProvider();
      provider_.ImportParameters(((RsaSecurityPublicKey)publicKey).Parameters);

      memoryStream_ = new MemoryStream();
    }

    public override byte[]
    sign()
    {
      memoryStream_.Flush();
      var result = provider_.SignData(memoryStream_.ToArray(), new SHA256CryptoServiceProvider());

      // We don't need the data in the stream any more.
      memoryStream_.Dispose();
      memoryStream_ = null;

      return result;
    }
      
    public override void
    update(ByteBuffer data)
    {
      memoryStream_.Write(data.array(), data.arrayOffset(), data.remaining());
    }

    public override bool
    verify(byte[] signature)
    {
      memoryStream_.Flush();
      var result = provider_.VerifyData
        (memoryStream_.ToArray(), new SHA256CryptoServiceProvider(), signature);

      // We don't need the data in the stream any more.
      memoryStream_.Dispose();
      memoryStream_ = null;

      return result;
    }

    private RSACryptoServiceProvider provider_;
    private MemoryStream memoryStream_;
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

namespace System.spec {
}
