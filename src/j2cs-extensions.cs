/**
 * Copyright (C) 2015-2017 Regents of the University of California.
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
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using ILOG.J2CsMapping.NIO;
using net.named_data.jndn;
using net.named_data.jndn.encoding;
using net.named_data.jndn.encoding.der;
using net.named_data.jndn.encrypt.algo;
using net.named_data.jndn.security;
using net.named_data.jndn.util;
using Mono.Data.Sqlite;

namespace net.named_data.jndn.util {
  /// <summary>
  /// We set j2cstranslator to not capitalize method names, but it mistakenly doesn't
  /// capitalize method names for .NET classes, so this has extension methods for the
  /// uncapitalized methods to call the capitalized ones.
  /// </summary>
  public static class J2CsExtensions {
    // ArrayList<T> extensions.
    public static int 
    indexOf<T>(this ArrayList<T> array, T value) { return array.IndexOf(value); }

    // Blob extensions.
    public static string
    toString(this Blob blob) { return blob.ToString(); }

    // Enum extensions.
    public static int 
    getNumericType(this ContentType contentType) 
    { 
      return contentType == ContentType.OTHER_CODE ? 0x7fff : (int)contentType; 
    }

    public static int 
    getNumericType(this EncryptAlgorithmType algorithmType)
    {
      // The C# enum values are automatically assigned 0, 1, 2, etc. We must be explicit.
      if (algorithmType == EncryptAlgorithmType.NONE)
        return -1;
      else if (algorithmType ==  EncryptAlgorithmType.AesEcb)
        return 0;
      else if (algorithmType ==  EncryptAlgorithmType.AesCbc)
        return 1;
      else if (algorithmType ==  EncryptAlgorithmType.RsaPkcs)
        return 2;
      else if (algorithmType ==  EncryptAlgorithmType.RsaOaep)
        return 3;
      else
        throw new NotImplementedException
        ("getNumericType: Unrecognized EncryptAlgorithmType: " + algorithmType);
    }

    public static int 
    getNumericType(this Name.Component.ComponentType componentType) 
    {
      // The C# enum values are automatically assigned 0, 1, 2, etc. We must be explicit.
      if (componentType == Name.Component.ComponentType.IMPLICIT_SHA256_DIGEST)
        return 1;
      else if (componentType ==  Name.Component.ComponentType.GENERIC)
        return 8;
      else
        throw new NotImplementedException
        ("getNumericType: Unrecognized Name.Component.ComponentType: " + componentType);
    }

    public static int 
    getNumericType(this KeyType keyType) 
    {
      // The C# enum values are automatically assigned 0, 1, 2, etc. We must be explicit.
      if (keyType == KeyType.RSA)
        return 0;
      else if (keyType ==  KeyType.ECDSA)
        return 1;
      else if (keyType ==  KeyType.AES)
        return 128;
      else
        throw new NotImplementedException
          ("getNumericType: Unrecognized KeyType: " + keyType);
    }

    public static int 
    getNumericType(this NetworkNack.Reason reason) 
    {
      // The C# enum values are automatically assigned 0, 1, 2, etc. We must be explicit.
      if (reason == NetworkNack.Reason.NONE)
        return 0;
      else if (reason == NetworkNack.Reason.CONGESTION)
        return 50;
      else if (reason == NetworkNack.Reason.DUPLICATE)
        return 100;
      else if (reason == NetworkNack.Reason.NO_ROUTE)
        return 150;
      else if (reason == NetworkNack.Reason.OTHER_CODE)
        return 0x7fff;
      else
        throw new NotImplementedException
          ("getNumericType: Unrecognized NetworkNack.Reason: " + reason);
    }

    // FileInfo extensions.
    public static void 
    delete(this FileInfo fileInfo) { fileInfo.Delete(); }

    public static FileInfo[] 
    listFiles(this FileInfo fileInfo) { return new DirectoryInfo(fileInfo.FullName).GetFiles(); }

    // Hashtable extensions.
    public static void 
    clear(this Hashtable map) { map.Clear(); }

    // IDictionary extensions.
    public static void
    clear(this IDictionary dictionary) { dictionary.Clear(); }

    // IDictionary<TKey, TValue> extensions.
    public static bool
    Contains<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) 
    { 
      return dictionary.ContainsKey(key); 
    }

    // Random extensions.
    public static void
    nextBytes(this Random random, byte[] array) { random.NextBytes(array); }

    // StreamWriter extensions.
    public static void 
    close(this StreamWriter writer) { writer.Close(); }

    public static void 
    flush(this StreamWriter writer) { writer.Flush(); }

    public static void 
    write(this StreamWriter writer, string value) { writer.Write(value); }

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

    public static bool
    regionMatches(this String str, int thisOffset, string other, int otherOffset, int length)
    { 
      if (thisOffset + length > str.Length || otherOffset + length > other.Length)
        // The length runs past the end of one or the other string.
        return false;
 
      return str.Substring(thisOffset, length) == other.Substring(otherOffset, length);
    }

    public static string
    replace(this String str, string oldValue, string newValue) { return str.Replace(oldValue, newValue); }

    public static string
    replace(this String str, char oldValue, char newValue) { return str.Replace(oldValue, newValue); }

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

    // TextReader extensions.
    public static void 
    close(this TextReader reader) { reader.Close(); }

    public static string 
    readLine(this TextReader reader) { return reader.ReadLine(); }
  }

  // We need a generic version of ArrayList.
  public class ArrayList<T> : System.Collections.Generic.List<T> {
    public ArrayList() {}

    public ArrayList(IList list)
    {
      foreach (T item in list)
        Add(item);
    }
  }

  public class Assert {
    public static void
    AssertArrayEquals(string message, object[] expected, object[] actual)
    {
      if (expected.Length != actual.Length)
        throw new Exception("Array lengths not equal: " + message);
      
      for (int i = 0; i < expected.Length; ++i) {
        if (!expected[i].Equals(actual[i]))
          throw new Exception("Not equal at index " + i + ": " + message);
      }
    }

    public static void
    AssertEquals(string message, object expected, object actual)
    {
      if (expected == null) {
        if (actual != null)
          throw new Exception("Not equal: " + message);
      } 
      else {
        if (!expected.Equals(actual))
          throw new Exception("Not equal: " + message);
      }
    }

    public static void
    AssertEquals(object expected, object actual)
    {
      AssertEquals("The expected value is not equal to the actual. Expected: " +
        expected + ", actual: " + actual, expected, actual);
    }

    public static void
    AssertFalse(string message, bool condition)
    {
      if (condition)
        throw new Exception("Not false: " + message);
    }

    public static void
    AssertFalse(bool condition)
    {
      AssertFalse("The value is not false as expected.", condition);
    }

    public static void
    AssertTrue(string message, bool condition)
    {
      if (!condition)
        throw new Exception("Not true: " + message);
    }

    public static void
    AssertTrue(bool condition)
    {
      AssertTrue("The value is not true as expected.", condition);
    }

    public static void
    AssertNotNull(string message, object obj)
    {
      if (obj == null)
        throw new Exception("Not non-null: " + message);
    }

    public static void
    AssertNotNull(object obj)
    {
      AssertNotNull("The value is not non-null as expected.", obj);
    }

    public static void
    AssertNotSame(string message, object obj1, object obj2)
    {
      if (Object.ReferenceEquals(obj1, obj2))
        throw new Exception("Not non-same: " + message);
    }

    public static void
    AssertNotSame(object obj1, object obj2)
    {
      AssertNotSame("The value is not non-same as expected.", obj1, obj2);
    }

    public static void
    AssertNull(string message, object obj)
    {
      if (obj != null)
        throw new Exception("Not null: " + message);
    }

    public static void
    AssertNull(object obj)
    {
      AssertNull("The value is not null as expected.", obj);
    }

    public static void
    AssertSame(string message, object obj1, object obj2)
    {
      if (!Object.ReferenceEquals(obj1, obj2))
        throw new Exception("Not same: " + message);
    }

    public static void
    AssertSame(object obj1, object obj2)
    {
      AssertSame("The value is not same as expected.", obj1, obj2);
    }

    public static void
    Fail(string message)
    {
      throw new Exception("Fail: " + message);
    }

    public static void
    Fail()
    {
      Fail("The assertion has failed.");
    }
  }

  public class BufferOverflowException : Exception {
    public BufferOverflowException(string message) : base(message) {}
  }

  public class BufferUnderflowException : Exception {
    public BufferUnderflowException(string message) : base(message) {}
  }

  public class FileReader : StreamReader {
    public FileReader(string filename) : base(filename) {}
  }

  public class HashedSet<T> : System.Collections.Generic.HashSet<T> {
  }

  // We need a generic version of Hashtable, which is j2cstranslator's conversion of HashMap.
  public class Hashtable<TKey, TValue> 
    : System.Collections.Generic.Dictionary<TKey, TValue>, IDictionary<TKey, TValue> {
  }

  // We need a generic version of IDictionary, which is j2cstranslator's conversion of Map.
  public interface IDictionary<TKey, TValue> : System.Collections.Generic.IDictionary<TKey, TValue> {
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
  public interface
  AlgorithmParameterSpec {
  }

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
        var modulus = getIntegerArrayWithoutLeadingZero(((DerNode)rsaPrivateKeyChildren[1]).getPayload());
        parameters.Modulus = modulus;
        parameters.Exponent = getIntegerArrayWithoutLeadingZero(((DerNode)rsaPrivateKeyChildren[2]).getPayload());
        // RSAParameters expects the integer array of the correct length.
        parameters.D = getIntegerArrayOfSize(((DerNode)rsaPrivateKeyChildren[3]).getPayload(), modulus.Length);
        parameters.P = getIntegerArrayOfSize(((DerNode)rsaPrivateKeyChildren[4]).getPayload(), modulus.Length / 2);
        parameters.Q = getIntegerArrayOfSize(((DerNode)rsaPrivateKeyChildren[5]).getPayload(), modulus.Length / 2);
        parameters.DP = getIntegerArrayOfSize(((DerNode)rsaPrivateKeyChildren[6]).getPayload(), modulus.Length / 2);
        parameters.DQ = getIntegerArrayOfSize(((DerNode)rsaPrivateKeyChildren[7]).getPayload(), modulus.Length / 2);
        parameters.InverseQ = getIntegerArrayOfSize(((DerNode)rsaPrivateKeyChildren[8]).getPayload(), modulus.Length / 2);

        return new RsaSecurityPrivateKey(parameters);
      } catch (DerDecodingException ex) {
        throw new net.named_data.jndn.util.InvalidKeySpecException
          ("RsaKeyFactory.generatePrivate error decoding the private key DER: " + ex);
      }
    }

    public override SecurityPublicKey
    generatePublic(KeySpec keySpec)
    {
      if (keySpec is X509EncodedKeySpec) {
        try {
          // Decode the X.509 public key.
          var parsedNode = DerNode.parse(new ByteBuffer(((X509EncodedKeySpec)keySpec).KeyDer), 0);
          var rootChildren = parsedNode.getChildren();
          var algorithmIdChildren = DerNode.getSequence(rootChildren, 0).getChildren();
          var oidString = ((DerNode.DerOid)algorithmIdChildren[0]).toVal().ToString();
          var rsaPublicKeyDerBitString = ((DerNode)rootChildren[1]).getPayload();

          if (oidString != RSA_ENCRYPTION_OID)
            throw new net.named_data.jndn.util.InvalidKeySpecException("The PKCS #8 private key is not RSA_ENCRYPTION");

          // Decode the PKCS #1 RSAPublicKey.
          // Skip the leading 0 byte in the DER BitString.
          parsedNode = DerNode.parse(rsaPublicKeyDerBitString.buf(), 1);
          var rsaPublicKeyChildren = parsedNode.getChildren();

          // Copy the parameters.
          var parameters = new RSAParameters();
          parameters.Modulus = getIntegerArrayWithoutLeadingZero(((DerNode)rsaPublicKeyChildren[0]).getPayload());
          parameters.Exponent = getIntegerArrayWithoutLeadingZero(((DerNode)rsaPublicKeyChildren[1]).getPayload());

          return new RsaSecurityPublicKey(parameters);
        }
        catch (DerDecodingException ex) {
          throw new net.named_data.jndn.util.InvalidKeySpecException("RsaKeyFactory.generatePublic error decoding the public key DER: " + ex);
        }
      }
      else if (keySpec is RSAPublicKeySpec) {
        var parameters = new RSAParameters();
        parameters.Modulus = ((RSAPublicKeySpec)keySpec).Modulus;
        parameters.Exponent = ((RSAPublicKeySpec)keySpec).Exponent;

        return new RsaSecurityPublicKey(parameters);
      }
      else
        throw new net.named_data.jndn.util.InvalidKeySpecException
          ("RsaKeyFactory.generatePublic unrecognized KeySpec");
    }

    /// <summary>
    /// A Der Integer is signed which means it can have a leading zero, but we need
    /// to strip the leading zero to use it in an RSAParameters.
    /// </summary>
    /// <returns>The array without leading a zero.</returns>
    /// <param name="integer">The DER Integer payload.</param>
    public static byte[]
    getIntegerArrayWithoutLeadingZero(Blob integer)
    {
      var buffer = integer.buf();
      if (buffer.get(buffer.position()) == 0)
        return getIntegerArrayOfSize(integer, buffer.remaining() - 1);
      else
        return integer.getImmutableArray();
    }

    /// <summary>
    /// Strip leading zeros until the integer Blob has the given size. This is
    /// necessary because RSAParameters expects integer byte arrays of a given
    /// size based on the size of the modulus.
    /// </summary>
    /// <returns>The array of the given size.</returns>
    /// <param name="integer">The DER Integer payload.</param>
    /// <param name="size">The number of bytes.</param>
    public static byte[]
    getIntegerArrayOfSize(Blob integer, int size)
    {
      var buffer = integer.buf();
      while (buffer.remaining() > size) {
        if (buffer.get(buffer.position()) != 0)
          throw new Exception("getIntegerArrayOfSize: The leading byte to strip is not zero");
        buffer.position(buffer.position() + 1);
      }

      // If position was unchanged, this does not copy.
      return new Blob(buffer, false).getImmutableArray();
    }

    /// <summary>
    /// Return the integer byte array as a ByteBuffer, prepending a zero byte if
    /// the first byte of the integer is >= 0x80.
    /// </summary>
    /// <returns>The positive integer buffer.</returns>
    /// <param name="integer">The integer byte array. If this doesn't prepend a zero,
    /// then this just returns ByteBuffer.wrap(integer).</param>
    public static ByteBuffer
    getPositiveIntegerBuffer(byte[] integer)
    {
      if (integer.Length == 0 || integer[0] < 0x80)
        return ByteBuffer.wrap(integer);

      var result = ByteBuffer.allocate(integer.Length + 1);
      result.put((byte)0);
      result.put(integer);
      result.flip();
      return result;
    }

    public static string RSA_ENCRYPTION_OID = "1.2.840.113549.1.1.1";
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.KeyPair to System.KeyPair.
  /// </summary>
  public class KeyPair {
    public KeyPair(SecurityPublicKey publicKey, PrivateKey privateKey)
    {
      publicKey_ = publicKey;
      privateKey_ = privateKey;
    }

    public SecurityPublicKey
    getPublic() { return publicKey_; }

    public PrivateKey
    getPrivate() { return privateKey_; }

    private SecurityPublicKey publicKey_;
    private PrivateKey privateKey_;
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.KeyPairGenerator to System.KeyPairGenerator.
  /// </summary>
  public abstract class KeyPairGenerator {
    public static KeyPairGenerator 
    getInstance(string type)
    {
      if (type == "RSA")
        return new RsaKeyPairGenerator();
      else
        throw new NotImplementedException("KeyPairGenerator type is not implemented: " + type);
    }

    public abstract void
    initialize(int keySize);

    public abstract KeyPair
    generateKeyPair();
  }

  public class RsaKeyPairGenerator : KeyPairGenerator {
    public override void
    initialize(int keySize)
    {
      keySize_ = keySize;
    }

    public override KeyPair
    generateKeyPair()
    {
      var parameters = new RSACryptoServiceProvider(keySize_).ExportParameters(true);
      return new KeyPair
        (new RsaSecurityPublicKey(parameters), new RsaSecurityPrivateKey(parameters));
    }

    private int keySize_;
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.KeySpec to System.KeySpec.
  /// </summary>
  public interface KeySpec {
  }

  public class RSAPublicKeySpec : KeySpec {
    public RSAPublicKeySpec(byte[] modulus, byte[] exponent)
    {
      Modulus = modulus;
      Exponent = exponent;
    }

    public readonly byte[] Modulus;
    public readonly byte[] Exponent;
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
  public abstract class PrivateKey : javax.crypto.Key {
    public abstract byte[]
    getEncoded();
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

    public override byte[]
    getEncoded()
    {
      // First encode an PKCS #1 RSAPrivateKey.
      var rsaPrivateKey = new DerNode.DerSequence();
      rsaPrivateKey.addChild(new DerNode.DerInteger(0));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.Modulus)));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.Exponent)));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.D)));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.P)));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.Q)));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.DP)));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.DQ)));
      rsaPrivateKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.InverseQ)));

      // Encode rsaPrivateKey as a PKCS #8 private key.
      var algorithmIdentifier = new DerNode.DerSequence();
      algorithmIdentifier.addChild(new DerNode.DerOid(new OID(RsaKeyFactory.RSA_ENCRYPTION_OID)));
      algorithmIdentifier.addChild(new DerNode.DerNull());

      var privateKey = new DerNode.DerSequence();
      privateKey.addChild(new DerNode.DerInteger(0));
      privateKey.addChild(algorithmIdentifier);
      privateKey.addChild(new DerNode.DerOctetString(rsaPrivateKey.encode().buf()));

      return privateKey.encode().getImmutableArray();
    }

    public readonly RSAParameters Parameters;
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.PublicKey to System.SecurityPublicKey.
  /// We also globally rename System.SecurityPublicKey to System.SecurityPublicKey to not
  /// conclict with PublicKey when using net.named_data.security.certificate.
  /// </summary>
  public abstract class SecurityPublicKey : javax.crypto.Key {
    public abstract byte[]
    getEncoded();
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

    public override byte[]
    getEncoded()
    {
      // First encode an PKCS #1 RSAPublicKey.
      var rsaPublicKey = new DerNode.DerSequence();
      rsaPublicKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.Modulus)));
      rsaPublicKey.addChild(new DerNode.DerInteger
        (RsaKeyFactory.getPositiveIntegerBuffer(Parameters.Exponent)));

      // Encode rsaPublicKey as an X.509 public key.
      var algorithmIdentifier = new DerNode.DerSequence();
      algorithmIdentifier.addChild(new DerNode.DerOid(new OID(RsaKeyFactory.RSA_ENCRYPTION_OID)));
      algorithmIdentifier.addChild(new DerNode.DerNull());

      var publicKey = new DerNode.DerSequence();
      publicKey.addChild(algorithmIdentifier);
      publicKey.addChild(new DerNode.DerBitString(rsaPublicKey.encode().buf(), 0));

      return publicKey.encode().getImmutableArray();
    }

    public readonly RSAParameters Parameters;
  }

  /// <summary>
  /// j2cstranslator naively converts java.security.Signature to System.SecuritySignature.
  /// We also globally rename System.SecuritySignature to System.SecuritySignature to not
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
      var result = provider_.SignData(memoryStream_.ToArray(), "SHA256");

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
      var result = provider_.VerifyData(memoryStream_.ToArray(), "SHA256", signature);

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

namespace System.Data.SqlClient {
  public class DriverManager {
    /// <summary>
    /// Check the url and return the appropriate connection type.
    /// </summary>
    /// <returns>The SqlConnection.</returns>
    /// <param name="url">If url starts with "jdbc:sqlite:", return an
    /// SQLiteSqlConnection. Otherwise throw an exception</param>
    /// <exception cref="NotSupportedException">for an unrecognized URL.</exception>
    public static SqlConnection 
    getConnection(string url)
    {
      if (url.StartsWith("jdbc:sqlite:"))
        return new SQLiteSqlConnection(url.Substring("jdbc:sqlite:".Length));
      else
        throw new NotSupportedException
          ("System.Data.SqlClient: Unrecognized URL: " + url);
    }
  }

  public interface Statement {
    void
    close();

    SqlDataReader
    executeQuery(string sql);

    void
    executeUpdate(string sql);
  }

  public interface PreparedStatement : Statement {
    SqlDataReader
    executeQuery();

    void
    executeUpdate();

    void
    setBytes(int index, byte[] value);

    void
    setInt(int index, int value);

    void
    setLong(int index, long value);

    void
    setString(int index, string value);
  }

  public interface SqlConnection {
    Statement
    CreateCommand();

    PreparedStatement
    prepareStatement(string sql);
  }

  public interface SqlDataReader {
    void
    close();

    byte[]
    getBytes(int index);

    byte[]
    getBytes(string name);

    int
    getInt(int index);

    string
    getString(int index);

    string
    getString(string name);

    bool
    NextResult();

    string this [string name] { get; }
  }

  /// <summary>
  /// The converted Java code expects SQLException.
  /// </summary>
  class SQLException : Exception {
    public SQLException(string message) : base(message) {}
  }

  /// <summary>
  /// SQLiteStatement extends Statement and works with a Mono SqliteCommand.
  /// In all methods, a thrown DbException is converted to an SQLException.
  /// </summary>
  public class SQLiteStatement : Statement {
    public SQLiteStatement(SqliteConnection connection)
    {
      connection_ = connection;
    }

    public void
    close()
    { 
      if (connectionIsOpen_) {
        connectionIsOpen_ = false;
        connection_.Close();
      }
    }

    public SqlDataReader
    executeQuery(string sql)
    {
      try {
        connection_.Open();
        connectionIsOpen_ = true;
        return new SQLiteSqlDataReader(makeCommand(sql).ExecuteReader(), this);
      } catch (DbException ex) {
        throw new SQLException("Error in executeQuery: " + ex);
      }
    }

    public void
    executeUpdate(string sql)
    {
      try {
        connection_.Open();
        connectionIsOpen_ = true;
        makeCommand(sql).ExecuteNonQuery();
        close();
      } catch (DbException ex) {
        throw new SQLException("Error in executeQuery: " + ex);
      }
    }

    /// <summary>
    /// Replace each "?" in sql with @1, @2, etc. and return a new SqliteCommand.
    /// </summary>
    /// <returns>The SqliteCommand.</returns>
    /// <param name="sql">The SQL query with "?"</param>
    protected SqliteCommand 
    makeCommand(string sql)
    {
      if (sql.IndexOf('@') >= 0)
        throw new NotSupportedException
        ("makeCommand: @ is not allowed in the sql " + sql);
      string[] splitSql = sql.Split(new char[] { '?' });
      string parameterizedSql = splitSql[0];
      for (int i = 1; i < splitSql.Length; ++i)
        parameterizedSql += "@" + i + splitSql[i];

      try {
        SqliteCommand command = connection_.CreateCommand();
        command.CommandText = parameterizedSql;
        return command;
      } catch (DbException ex) {
        throw new SQLException("Error in makeCommand: " + ex);
      }
    }

    protected SqliteConnection connection_;
    protected bool connectionIsOpen_;
  }

  /// <summary>
  /// SQLitePreparedStatement extends PreparedStatement and wraps a Mono SqliteCommand.
  /// In all methods, a thrown DbException is converted to an SQLException.
  /// </summary>
  public class SQLitePreparedStatement : SQLiteStatement, PreparedStatement {
    public SQLitePreparedStatement(SqliteConnection connection, string sql)
      : base(connection)
    {
      command_ = makeCommand(sql);
    }

    public SqlDataReader
    executeQuery()
    {
      try {
        connection_.Open();
        connectionIsOpen_ = true;
        return new SQLiteSqlDataReader(command_.ExecuteReader(), this);
      } catch (DbException ex) {
        throw new SQLException("Error in executeQuery: " + ex);
      }
    }

    public void
    executeUpdate()
    {
      try {
        connection_.Open();
        connectionIsOpen_ = true;
        command_.ExecuteNonQuery();
        close();
      } catch (DbException ex) {
        throw new SQLException("Error in executeUpdate: " + ex);
      }
    }

    public void
    setBytes(int index, byte[] value)
    {
      try {
        command_.Parameters.AddWithValue("@" + index, value);
      } catch (DbException ex) {
        throw new SQLException("Error in setBytes: " + ex);
      }
    }

    public void
    setInt(int index, int value)
    {
      try {
        command_.Parameters.AddWithValue("@" + index, value);
      } catch (DbException ex) {
        throw new SQLException("Error in setInt: " + ex);
      }
    }

    public void
    setLong(int index, long value)
    {
      try {
        command_.Parameters.AddWithValue("@" + index, value);
      } catch (DbException ex) {
        throw new SQLException("Error in setLong: " + ex);
      }
    }

    public void
    setString(int index, string value)
    {
      try {
        command_.Parameters.AddWithValue("@" + index, value);
      } catch (DbException ex) {
        throw new SQLException("Error in setString: " + ex);
      }
    }

    private SqliteCommand command_;
  }

  /// <summary>
  /// SQLiteSqlConnection extends SqlConnection and wraps a Mono SqliteConnection.
  /// In all methods, a thrown DbException is converted to an SQLException.
  /// </summary>
  public class SQLiteSqlConnection : SqlConnection {
    public SQLiteSqlConnection(string databaseFilePath) {
      connection_ = new SqliteConnection("URI=file:" + databaseFilePath);
    }

    public Statement
    CreateCommand() { return new SQLiteStatement(connection_); }

    public PreparedStatement
    prepareStatement(string sql)
    {
      return new SQLitePreparedStatement(connection_, sql);
    }

    private SqliteConnection connection_;
  }

  /// <summary>
  /// SQLiteSqlDataReader extends SqlDataReader and wraps a Mono SqliteDataReader.
  /// In all methods, a thrown DbException is converted to an SQLException.
  /// </summary>
  public class SQLiteSqlDataReader : SqlDataReader {
    public SQLiteSqlDataReader(SqliteDataReader reader, SQLiteStatement statement)
    {
      reader_ = reader;
      isReaderOpen_ = true;
      statement_ = statement;
    }

    public void
    close()
    {
      try {
        if (isReaderOpen_) {
          isReaderOpen_ = false;
          reader_.Close();
        }

        // This closes the connection if it is still open.
        statement_.close();
      } catch (DbException ex) {
        throw new SQLException("Error in close: " + ex);
      }
    }

    public byte[]
    getBytes(int index)
    {
      try {
        int nBytes = (int)reader_.GetBytes(index - 1, 0, null, 0, 0);
        byte[] result = new byte[nBytes];

        reader_.GetBytes(index - 1, 0, result, 0, nBytes);
        return result;
      } catch (DbException ex) {
        throw new SQLException("Error in getBytes: " + ex);
      }
    }

    public byte[]
    getBytes(string name)
    {
      try {
        // Add 1 to the index to make it 1-based as expected.
        return getBytes(reader_.GetOrdinal(name) + 1);
      } catch (DbException ex) {
        throw new SQLException("Error in getBytes: " + ex);
      }
    }

    public int
    getInt(int index)
    { 
      try {
        return reader_.GetInt32(index - 1); 
      } catch (DbException ex) {
        throw new SQLException("Error in getInt: " + ex);
      }
    }

    public string
    getString(int index)
    {
      try {
        return reader_.GetString(index - 1); 
      } catch (DbException ex) {
        throw new SQLException("Error in getString: " + ex);
      }
    }

    public string
    getString(string name)
    {
      try {
        // Add 1 to the index to make it 1-based as expected.
        return getString(reader_.GetOrdinal(name) + 1); 
      } catch (DbException ex) {
        throw new SQLException("Error in getString: " + ex);
      }
    }

    public bool
    NextResult() 
    {
      try {
        return reader_.Read(); 
      } catch (DbException ex) {
        throw new SQLException("Error in NextResult: " + ex);
      }
    }

    public string this [string name] { get { return getString(name); } }

    private SqliteDataReader reader_;
    private bool isReaderOpen_;
    private SQLiteStatement statement_;
  }
}

namespace System.spec {
  class SystemSpecStubClass {}
}

namespace javax.crypto {
  public abstract class Cipher {
    public const int DECRYPT_MODE = 1;
    public const int ENCRYPT_MODE = 2;

    public static Cipher 
    getInstance(string type)
    {
      if (type == "AES/ECB/PKCS5PADDING")
        return new AesEcbPkcs5PaddingCipher();
      else if (type == "AES/CBC/PKCS5PADDING")
        return new AesCbcPkcs5PaddingCipher();
      else if (type == "RSA/ECB/PKCS1Padding")
        return new RsaCipher(false);
      else if (type == "RSA/ECB/OAEPWithSHA-1AndMGF1Padding")
        return new RsaCipher(true);
      else
        throw new NotImplementedException("Cipher type is not implemented: " + type);
    }

    public virtual void
    init(int mode, Key key)
    {
      throw new NotImplementedException("Cipher.init(int mode, Key key) is not implemented");
    }

    public virtual void
    init(int mode, Key key, AlgorithmParameterSpec parameters)
    {
      throw new NotImplementedException
        ("Cipher.init(int mode, Key key, AlgorithmParameterSpec parameters) is not implemented");
    }

    public abstract byte[]
    doFinal(byte[] data);
  }

  // An abstract base class for AesEcbPkcs5PaddingCipher, etc.
  public abstract class CryptoTransformCipher : Cipher {
    public override byte[]
    doFinal(byte[] data)
    {
      if (mode_ == DECRYPT_MODE) {
        using (MemoryStream inStream = new MemoryStream(data)) {
          using (CryptoStream decrypt = new CryptoStream
                 (inStream, transform_, CryptoStreamMode.Read)) {
            using (MemoryStream result = new MemoryStream()) {
              // Copy the decrypt stream to the result stream.
              int value;
              while ((value = decrypt.ReadByte()) >= 0)
                result.WriteByte((byte)value);
              
              return result.ToArray();
            }
          }
        }
      } else if (mode_ == ENCRYPT_MODE) {
        using (MemoryStream result = new MemoryStream()) {
          using (CryptoStream encrypt = new CryptoStream
                 (result, transform_, CryptoStreamMode.Write)) {
            encrypt.Write(data, 0, data.Length);
            encrypt.FlushFinalBlock();
            return result.ToArray();
          }
        }
      }
      else
        // We don't expect this.
        throw new Exception("Unrecognized Cipher mode " + mode_);
    }

    protected int mode_;
    protected ICryptoTransform transform_;
  }

  public class AesEcbPkcs5PaddingCipher : CryptoTransformCipher {
    public override void
    init(int mode, Key key)
    {
      mode_ = mode;

      var aes = new AesManaged();
      aes.Mode = CipherMode.ECB;
      aes.Padding = PaddingMode.PKCS7;
      if (!(key is javax.crypto.spec.SecretKeySpec))
        throw new net.named_data.jndn.util.InvalidKeyException
          ("AesEcbPkcs5PaddingCipher expects a SecretKeySpec");
      aes.Key = ((javax.crypto.spec.SecretKeySpec)key).Key;

      if (mode == DECRYPT_MODE)
        // CreateDecryptor wants an IV, even though ECB doesn't use it.
        transform_ = aes.CreateDecryptor(aes.Key, new byte[16]);
      else if (mode == ENCRYPT_MODE)
        transform_ = aes.CreateEncryptor(aes.Key, new byte[16]);
      else
        // We don't expect this.
        throw new Exception("Unrecognized Cipher mode " + mode);
    }
  }

  public class AesCbcPkcs5PaddingCipher : CryptoTransformCipher {
    public override void
    init(int mode, Key key, AlgorithmParameterSpec parameters)
    {
      mode_ = mode;

      var aes = new AesManaged();
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;
      if (!(key is javax.crypto.spec.SecretKeySpec))
        throw new net.named_data.jndn.util.InvalidKeyException
          ("AesCbcPkcs5PaddingCipher expects a SecretKeySpec");
      aes.Key = ((javax.crypto.spec.SecretKeySpec)key).Key;

      if (mode == DECRYPT_MODE)
        transform_ = aes.CreateDecryptor
          (aes.Key, ((javax.crypto.spec.IvParameterSpec)parameters).IV);
      else if (mode == ENCRYPT_MODE)
        transform_ = aes.CreateEncryptor
          (aes.Key, ((javax.crypto.spec.IvParameterSpec)parameters).IV);
      else
        // We don't expect this.
        throw new Exception("Unrecognized Cipher mode " + mode);
    }
  }
      
  public class RsaCipher : Cipher {
    /// <summary>
    /// Create an RsaCipher to do PKCS1 or OAEP padding based on useOAEP.
    /// </summary>
    /// <param name="useOAEP">True to use OAEP padding, false to use PKCS1.</param>
    public RsaCipher(bool useOAEP) { useOAEP_ = useOAEP; }

    public override byte[]
    doFinal(byte[] data)
    {
      using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
      {
        if (mode_ == DECRYPT_MODE) {
          if (!(key_ is RsaSecurityPrivateKey))
            throw new net.named_data.jndn.util.InvalidKeyException
              ("RsaCipher expects a RsaSecurityPrivateKey");

          rsa.ImportParameters(((RsaSecurityPrivateKey)key_).Parameters);
          return rsa.Decrypt(data, useOAEP_);
        } 
        else if (mode_ == ENCRYPT_MODE) {
          if (!(key_ is RsaSecurityPublicKey))
            throw new net.named_data.jndn.util.InvalidKeyException
            ("RsaCipher expects an RsaSecurityPublicKey");

          rsa.ImportParameters(((RsaSecurityPublicKey)key_).Parameters);
          return rsa.Encrypt(data, useOAEP_);
        }
        else
          // We don't expect this.
          throw new Exception("Unrecognized Cipher mode " + mode_);
      }
    }

    public override void
    init(int mode, Key key)
    {
      mode_ = mode;
      key_ = key;
    }

    private bool useOAEP_;
    private int mode_;
    private Key key_;
  }

  public interface Key {
  }

  public interface SecretKey : Key {
  }
}

namespace javax.crypto.spec {
  public class IvParameterSpec : AlgorithmParameterSpec {
    public IvParameterSpec(byte[] iv)
    {
      IV = iv;
    }

    public readonly byte[] IV;
  }

  public class SecretKeySpec : KeySpec, SecretKey {
    public SecretKeySpec(byte[] key, string algorithm)
    {
      Key = key;
    }

    public readonly byte[] Key;
  }
}
