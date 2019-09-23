/**
 * Copyright (C) 2014-2019 Regents of the University of California.
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

package net.named_data.jndn.util;

import java.io.File;
import java.nio.ByteBuffer;
import java.security.InvalidKeyException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.security.spec.InvalidKeySpecException;
import java.util.Date;
import java.util.Random;
import javax.crypto.Mac;
import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.PBEKeySpec;
import javax.crypto.spec.SecretKeySpec;
import net.named_data.jndn.contrib.Murmur3;

/**
 * The Common class has static utility functions.
 */
public class Common {
  /**
   * Get the current time in milliseconds.
   * @return  The current time in milliseconds since 1/1/1970 UTC.
   */
  public static double
  getNowMilliseconds() { return (double)System.currentTimeMillis(); }

  /**
   * Get the library-wide random number generator. This method is synchronized so that multiple accesses to the
   * generator when a generator is not yet set will not throw an UnsupportedOperationException
   * @return the random number generator set in {@link #setRandom(Random)} or (by default) a SecureRandom
   */
  public static synchronized Random
  getRandom() {
    if(randomNumberGenerator_ == null){
      setRandom(new SecureRandom());
    }
    return randomNumberGenerator_;
  }

  /**
   * Set the library-wide random number generator; this method will only allow the generator to be set once.
   * Additionally, this method is thread-safe in that it guarantees that only the first caller will be able to set
   * the generator.
   * @param randomNumberGenerator the random number generator
   * @throws UnsupportedOperationException if a user attempts to set the generator a second time
   */
  public static synchronized void
  setRandom(Random randomNumberGenerator) {
    if(randomNumberGenerator_ == null) {
      randomNumberGenerator_ = randomNumberGenerator;
    }
    else{
      throw new UnsupportedOperationException("The random number generator may only be set once");
    }
  }

  /**
   * Compute the sha-256 digest of data.
   * @param data The input byte buffer. This does not change the position.
   * @return The digest.
   */
  public static byte[]
  digestSha256(ByteBuffer data)
  {
    MessageDigest sha256;
    try {
      sha256 = MessageDigest.getInstance("SHA-256");
    }
    catch (NoSuchAlgorithmException exception) {
      // Don't expect this to happen.
      throw new Error
        ("MessageDigest: SHA-256 is not supported: " + exception.getMessage());
    }
    int savePosition = data.position();
    sha256.update(data);
    data.position(savePosition);
    return sha256.digest();
  }

  /**
   * Compute the sha-256 digest of data.
   * @param data The input byte buffer.
   * @return The digest.
   */
  public static byte[]
  digestSha256(byte[] data)
  {
    MessageDigest sha256;
    try {
      sha256 = MessageDigest.getInstance("SHA-256");
    }
    catch (NoSuchAlgorithmException exception) {
      // Don't expect this to happen.
      throw new Error
        ("MessageDigest: SHA-256 is not supported: " + exception.getMessage());
    }
    sha256.update(data);
    return sha256.digest();
  }

  /**
   * Compute the HMAC with SHA-256 of data, as defined in
   * http://tools.ietf.org/html/rfc2104#section-2 .
   * @param key The key byte array.
   * @param data The input byte buffer.
   * @return The HMAC result.
   */
  public static byte[]
  computeHmacWithSha256(byte[] key, ByteBuffer data)
  {
    final String algorithm = "HmacSHA256";
    Mac mac;
    try {
      mac = Mac.getInstance(algorithm);
    }
    catch (NoSuchAlgorithmException ex) {
      // Don't expect this to happen.
      throw new Error
        ("computeHmac: " + algorithm + " is not supported: " + ex.getMessage());
    }

    try {
      mac.init(new SecretKeySpec(key, algorithm));
    } catch (InvalidKeyException ex) {
      // Don't expect this to happen.
      throw new Error
        ("computeHmac: Can't init " + algorithm + " with key: " + ex.getMessage());
    }
    int savePosition = data.position();
    mac.update(data);
    data.position(savePosition);
    return mac.doFinal();
  }

  /**
   * Compute the PBKDF2 with HMAC SHA1 of the password.
   * @param password The input password, which should have characters in the
   * range of 1 to 127.
   * @param salt The 8-byte salt.
   * @param nIterations The number of iterations of the hashing algorithm.
   * @param resultLength The number of bytes of the result array.
   * @return The result byte array.
   */
  public static byte[]
  computePbkdf2WithHmacSha1
    (char[] password, byte[] salt, int nIterations, int resultLength)
    throws NoSuchAlgorithmException, InvalidKeySpecException
  {
    SecretKeyFactory keyFactory = SecretKeyFactory.getInstance
      ("PBKDF2WithHmacSHA1");
    PBEKeySpec keySpec = new PBEKeySpec
      (password, salt, nIterations, resultLength * 8);
    return keyFactory.generateSecret(keySpec).getEncoded();
  }

  /**
   * Compute the PBKDF2 with HMAC SHA1 of the password.
   * @param password The input password, which should have values in the
   * range of 1 to 127.
   * @param salt The 8-byte salt.
   * @param nIterations The number of iterations of the hashing algorithm.
   * @param resultLength The number of bytes of the result array.
   * @return The result byte array.
   */
  public static byte[]
  computePbkdf2WithHmacSha1
    (byte[] password, byte[] salt, int nIterations, int resultLength)
    throws NoSuchAlgorithmException, InvalidKeySpecException
  {
    char[] charPassword = new char[password.length];
    for (int i = 0; i < password.length; ++i)
      charPassword[i] = (char)password[i];

    return computePbkdf2WithHmacSha1
      (charPassword, salt, nIterations, resultLength);
  }

  /**
   * Return a hex string of the contents of buffer.
   * @param buffer The buffer.
   * @return A string of hex bytes.
   */
  public static String
  toHex(byte[] buffer)
  {
    StringBuffer output = new StringBuffer(buffer.length * 2);
    for (int i = 0; i < buffer.length; ++i) {
      String hex = Integer.toHexString((int)buffer[i] & 0xff);
      if (hex.length() <= 1)
        // Append the leading zero.
        output.append("0");
      output.append(hex);
    }

    return output.toString();
  }

  /**
   * Encode the input as base64 using the appropriate base64Converter_ from
   * establishBase64Converter(), for ANDROID or Java 7+.
   * @param input The bytes to encode.
   * @return The base64 string.
   * @throws UnsupportedOperationException If can't establish a base64 converter for
   * this platform.
   */
  public static String
  base64Encode(byte[] input)
  {
    establishBase64Converter();

    try {
      if (base64ConverterType_ == Base64ConverterType.ANDROID)
        // Base64.NO_WRAP  is 2.
        return (String)base64Converter_.getDeclaredMethod
          ("encodeToString", byte[].class, int.class).invoke(null, input, 2);
      else if (base64ConverterType_ == Base64ConverterType.JAVA_UTIL) {
        Object encoder = base64Converter_.getDeclaredMethod
          ("getEncoder").invoke(null);
        return (String)encoder.getClass().getDeclaredMethod
          ("encodeToString", byte[].class).invoke(encoder, input);
      }
      else
        // Default to Base64ConverterType.JAVAX.
        return (String)base64Converter_.getDeclaredMethod
          ("printBase64Binary", byte[].class).invoke(null, input);
    } catch (Exception ex) {
      throw new UnsupportedOperationException("base64Encode: Error invoking method: " + ex);
    }
  }

  /**
   * Encode the input as base64 using the appropriate base64Converter_ from
   * establishBase64Converter(), for ANDROID or Java 7+.
   * @param input The bytes to encode.
   * @param addNewlines If true, add newlines to the output (good for writing to
   * a file).  If false, do not add newlines.
   * @return The base64 string.
   * @throws UnsupportedOperationException If can't establish a base64 converter for
   * this platform.
   */
  public static String
  base64Encode(byte[] input, boolean addNewlines)
  {
    String base64 = base64Encode(input);
    if (!addNewlines)
      return base64;

    StringBuffer result = new StringBuffer();
    for (int i = 0; i < base64.length(); i += 64) {
      int lineLength = 64;
      if (i + lineLength > base64.length())
        lineLength = base64.length() - i;

      result.append(base64, i, i + lineLength);
      result.append('\n');
    }

    return result.toString();
  }

  /**
   * Decode the input as base64 using the appropriate base64Converter_ from
   * establishBase64Converter(), for ANDROID or Java 7+.
   * @param encoding The base64 string.
   * @return The decoded bytes.
   * @throws UnsupportedOperationException If can't establish a base64 converter for
   * this platform.
   */
  public static byte[]
  base64Decode(String encoding) throws SecurityException
  {
    establishBase64Converter();

    try {
      if (base64ConverterType_ == Base64ConverterType.ANDROID)
        // Base64.DEFAULT is 0.
        return (byte[])base64Converter_.getDeclaredMethod
          ("decode", String.class, int.class).invoke(null, encoding, 0);
      else if (base64ConverterType_ == Base64ConverterType.JAVA_UTIL) {
        // java.util.Base64 doesn't like whitespace, so remove it.
        encoding = encoding.replaceAll("\\s", "");

        Object decoder = base64Converter_.getDeclaredMethod
          ("getDecoder").invoke(null);
        return (byte[])decoder.getClass().getDeclaredMethod
          ("decode", String.class).invoke(decoder, encoding);
      }
      else
        // Default to Base64ConverterType.JAVAX.
        return (byte[])base64Converter_.getDeclaredMethod
          ("parseBase64Binary", String.class).invoke(null, encoding);
    } catch (Exception ex) {
      throw new UnsupportedOperationException("base64Decode: Error invoking method: " + ex);
    }
  }

  /**
   * Convert the milliseconds to a Date object. This is a centralized utility
   * method to support portability.
   * @param millisecondsSince1970 The milliseconds since 1970.
   * @return A new Date object.
   */
  public static Date
  millisecondsSince1970ToDate(long millisecondsSince1970)
  {
    return new Date(millisecondsSince1970);
  }

  /**
   * Convert a Date object milliseconds. This is a centralized utility
   * method to support portability.
   * @param date the Date object.
   * @return The milliseconds since 1970.
   */
  public static long
  dateToMillisecondsSince1970(Date date)
  {
    return date.getTime();
  }

  /**
   * Return true if the platform is OS X.
   * @return True if OS X, false if not.
   */
  public static boolean
  platformIsOSX()
  {
    return System.getProperty("os.name").equals("Mac OS X");
  }

  /**
   * Get the user's home directory.
   * @return The home directory as a File object, or new File(".") if unknown.
   */
  public static File
  getHomeDirectory()
  {
    return new File(System.getProperty("user.home", "."));
  }

  /**
   * Get the current directory.
   * @return The current directory as a File object.
   */
  public static File
  getCurrentDirectory()
  {
    return new File(System.getProperty("user.dir", "."));
  }

  /**
   * Compute the MurmurHash3 of the data.
   * @param nHashSeed The hash seed.
   * @param dataToHash The input byte array to hash.
   * @return The hash value. This returns a long to make it easier to interpret
   * it as an unsigned 32-bit integer (instead of the Java int which is signed).
   */
  public static long
  murmurHash3(int nHashSeed, byte[] dataToHash)
  {
    int hash = Murmur3.hash32(dataToHash, dataToHash.length, nHashSeed);
    return hash >= 0 ? hash : 0x100000000L + hash;
  }

  /**
   * Compute the MurmurHash3 of the integer value.
   * @param nHashSeed The hash seed.
   * @param value The integer value, interpreted as a 4-byte little-endian array.
   * This ignores the upper 4 bytes of the long integer.
   * @return The hash value. This returns a Java int which is a 32-bit signed
   * integer, but it should be interpreted as a 32-bit unsigned integer.
   */
  public static long
  murmurHash3(int nHashSeed, long value)
  {
    byte[] dataToHash = new byte[] {
      (byte) (value        & 0xff),
      (byte)((value >> 8)  & 0xff),
      (byte)((value >> 16) & 0xff),
      (byte)((value >> 24) & 0xff)
    };
    return murmurHash3(nHashSeed, dataToHash);
  }

  /**
   * The practical limit of the size of a network-layer packet. If a packet is
   * larger than this, the library or application MAY drop it. This constant is
   * defined in this low-level class so that internal code can use it, but
   * applications should use the static API method
   * Face.getMaxNdnPacketSize() which is equivalent.
   */
  public static final int MAX_NDN_PACKET_SIZE = 8800;

  private enum Base64ConverterType {
    UNINITIALIZED, JAVAX, JAVA_UTIL, ANDROID, UNSUPPORTED
  }

  /**
   * If not already initialized, set base64Converter_ to the correct loaded
   * class and set base64ConverterType_ to the loaded type.
   * If base64ConverterType_ is UNINITIALIZED, set base64Converter_ to
   * the class for javax.xml.bind.DatatypeConverter and set
   * base64ConverterType_ to JAVAX.  Else try to set base64Converter_ to
   * the class for android.util.Base64 and set base64ConverterType_ to ANDROID.
   * If these fail, set base64ConverterType_ to UNSUPPORTED and throw an
   * UnsupportedOperationException from now on.
   */
  private static void
  establishBase64Converter()
  {
    if (base64ConverterType_ == Base64ConverterType.UNINITIALIZED) {
      try {
        // This is for Java 7, 8.
        base64Converter_ = Class.forName("javax.xml.bind.DatatypeConverter");
        base64ConverterType_ = Base64ConverterType.JAVAX;
        return;
      } catch (ClassNotFoundException ex) {}

      try {
        // This is for Java 9+.
        base64Converter_ = Class.forName("java.util.Base64");
        base64ConverterType_ = Base64ConverterType.JAVA_UTIL;
        return;
      } catch (ClassNotFoundException ex) {}

      try {
        base64Converter_ = Class.forName("android.util.Base64");
        base64ConverterType_ = Base64ConverterType.ANDROID;
        return;
      } catch (ClassNotFoundException ex) {}

      base64ConverterType_ = Base64ConverterType.UNSUPPORTED;
    }

   if (base64ConverterType_ == Base64ConverterType.UNSUPPORTED)
      throw new UnsupportedOperationException
        ("Common.establishBase64Converter: Cannot load a Base64 converter");
  }

  private static Base64ConverterType base64ConverterType_ = Base64ConverterType.UNINITIALIZED;
  private static Class base64Converter_ = null;
  private static Random randomNumberGenerator_;
}
