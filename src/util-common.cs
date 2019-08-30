/**
 * Copyright (C) 2016-2019 Regents of the University of California.
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

using ILOG.J2CsMapping.NIO;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace net.named_data.jndn.util
{
  /// <summary>
  /// The Common class has static utility functions. This is ported by hand.
  /// </summary>
  ///
  public class Common {
    /// <summary>
    /// Get the current time in milliseconds.
    /// </summary>
    ///
    /// <returns>The current time in milliseconds since 1/1/1970, including
    /// fractions of a millisecond.</returns>
    public static double
    getNowMilliseconds()
    {
      return (double)(DateTime.UtcNow - unixEpoch_).TotalMilliseconds;
    }

    /// <summary>
    /// Get the library-wide random number generator. This method is synchronized so that multiple accesses to the
    /// generator when a generator is not yet set will not throw an UnsupportedOperationException
    /// </summary>
    ///
    /// <returns>the random number generator set in
    /// <see cref="M:Net.Named_data.Jndn.Util.Common.SetRandom(System.Random)"/>
    ///  or (by default) a SecureRandom</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static SecureRandom
    getRandom()
    {
      if (randomNumberGenerator_ == null)
        setRandom(new SecureRandom());

      return randomNumberGenerator_;
    }

    /// <summary>
    /// Set the library-wide random number generator; this method will only allow the generator to be set once.
    /// Additionally, this method is thread-safe in that it guarantees that only the first caller will be able to set
    /// the generator.
    /// </summary>
    ///
    /// <param name="randomNumberGenerator">the random number generator</param>
    /// <exception cref="System.NotSupportedException">if a user attempts to set the generator a second time</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void
    setRandom(SecureRandom randomNumberGenerator)
    {
      if (randomNumberGenerator_ == null)
        randomNumberGenerator_ = randomNumberGenerator;
      else
        throw new NotSupportedException("The random number generator may only be set once");
    }

    /// <summary>
    /// Compute the sha-256 digest of data.
    /// </summary>
    ///
    /// <param name="data">The input byte buffer. This does not change the position.</param>
    /// <returns>The digest.</returns>
    public static byte[]
    digestSha256(ByteBuffer data)
    {
      // Copy the buffer to an array.
      var array = new byte[data.remaining()];
      var savePosition = data.position();
      data.get(array);
      data.position(savePosition);

      return sha256_.ComputeHash(array);
    }

    /// <summary>
    /// Compute the sha-256 digest of data.
    /// </summary>
    ///
    /// <param name="data">The input byte buffer.</param>
    /// <returns>The digest.</returns>
    public static byte[]
    digestSha256(byte[] data) { return sha256_.ComputeHash(data); }

    /// <summary>
    /// Compute the HMAC with SHA-256 of data, as defined in
    /// http://tools.ietf.org/html/rfc2104#section-2 .
    /// </summary>
    ///
    /// <param name="key">The key byte array.</param>
    /// <param name="data">The input byte buffer. This does not change the position.</param>
    /// <returns>The HMAC result.</returns>
    public static byte[]
    computeHmacWithSha256(byte[] key, ByteBuffer data)
    {
      using (var hmac = new HMACSHA256(key)) {
        // Copy the buffer to an array.
        var array = new byte[data.remaining()];
        var savePosition = data.position();
        data.get(array);
        data.position(savePosition);

        return hmac.ComputeHash(array);
      }
    }

    /// <summary>
    /// Compute the PBKDF2 with HMAC SHA1 of the password.
    /// </summary>
    ///
    /// <param name="password">The input password, which should have characters
    /// in the range of 1 to 127.</param>
    /// <param name="salt">The 8-byte salt.</param>
    /// <param name="nIterations">The number of iterations of the hashing
    /// algorithm.</param>
    /// <param name="resultLength">The number of bytes of the result array.</param>
    /// <returns>The result byte array.</returns>
    public static byte[]
    computePbkdf2WithHmacSha1
      (byte[] password, byte[] salt, int nIterations, int resultLength)
    {
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, nIterations);
      return pbkdf2.GetBytes(resultLength);
    }

    /// <summary>
    /// Return a hex string of the contents of buffer.
    /// </summary>
    ///
    /// <param name="buffer">The buffer.</param>
    /// <returns>A string of hex bytes.</returns>
    public static string
    toHex(byte[] buffer)
    {
      var output = new StringBuilder(buffer.Length * 2);
      for (int i = 0; i < buffer.Length; ++i) {
        var hex = ((int)buffer[i] & 0xff).ToString("x");
        if (hex.Length <= 1)
          // Append the leading zero.
          output.append("0");
        output.append(hex);
      }

      return output.toString();
    }

    /// <summary>
    /// Encode the input as base64.
    /// </summary>
    ///
    /// <param name="input">The bytes to encode.</param>
    /// <returns>The base64 string.</returns>
    public static string
    base64Encode(byte[] input) { return Convert.ToBase64String(input); }

    /// <summary>
    /// Encode the input as base64.
    /// </summary>
    ///
    /// <param name="input">The bytes to encode.</param>
    /// <param name="addNewlines">If true, add newlines to the output (good for
    /// writing to a file). If false, do not add newlines.</param>
    /// <returns>The base64 string.</returns>
    public static string
    base64Encode(byte[] input, bool addNewlines) 
    { 
      var base64 = base64Encode(input);
      if (!addNewlines)
        return base64;

      var result = new StringBuilder();
      for (var i = 0; i < base64.Length; i += 64) {
        int lineLength = 64;
        if (i + lineLength > base64.Length)
          lineLength = base64.Length - i;

        result.Append(base64, i, lineLength);
        result.Append('\n');
      }

      return result.toString();
    }

    /// <summary>
    /// Decode the input as base64.
    /// </summary>
    ///
    /// <param name="encoding">The base64 string.</param>
    /// <returns>The decoded bytes.</returns>
    public static byte[]
    base64Decode(string encoding) { return Convert.FromBase64String(encoding); }

    /// <summary>
    /// The practical limit of the size of a network-layer packet. If a packet is
    /// larger than this, the library or application MAY drop it. This constant is
    /// defined in this low-level class so that internal code can use it, but
    /// applications should use the static API method
    /// Face.getMaxNdnPacketSize() which is equivalent.
    /// </summary>
    ///
    public const int MAX_NDN_PACKET_SIZE = 8800;

    /// <summary>
    /// Convert the milliseconds to a DateTime object. This is a centralized utility
    /// method to support portability.
    /// </summary>
    ///
    /// <param name="millisecondsSince1970">The milliseconds since 1970.</param>
    /// <returns>A new DateTime object.</returns>
    public static DateTime 
    millisecondsSince1970ToDate(long millisecondsSince1970) 
    {
      return unixEpoch_.AddMilliseconds(millisecondsSince1970);
    }

    /// <summary>
    /// Convert a Date object milliseconds. This is a centralized utility method to 
    /// support portability.
    /// </summary>
    ///
    /// <param name="date">The Date object.</param>
    /// <returnsThe milliseconds since 1970.</returns>
    public static long 
    dateToMillisecondsSince1970(DateTime date) 
    {
      return (long)(date - unixEpoch_).TotalMilliseconds;
    }

    /// <summary>
    /// Return true if the platform is OS X.
    /// </summary>
    /// <returns>True if OS X, false if not.</returns>
    public static bool 
    platformIsOSX()
    {
      // This is kind of a hack, but on OS X, Environment.OSVersion.Platform is Unix!
      return Directory.Exists("/Applications") &&
             Directory.Exists("/System") &&
             Directory.Exists("/Users") &&
             Directory.Exists("/Volumes");
    }

    /// <summary>
    /// Get the user's home directory.
    /// </summary>
    /// <returns>The home directory, or "." if unknown.</returns>
    public static FileInfo
    getHomeDirectory()
    {
      if (Environment.OSVersion.Platform == PlatformID.Unix ||
          Environment.OSVersion.Platform == PlatformID.MacOSX) {
        var result = Environment.GetEnvironmentVariable("HOME");
        if (result == null)
          result = ".";
        return new FileInfo(result);
      }
      else
        // Windows.
        return new FileInfo(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"));
    }

    /// <summary>
    /// Get the current directory.
    /// </summary>
    /// <returns>The current directory as a FileInfo object.</returns>
    public static FileInfo
    getCurrentDirectory()
    {
      return new FileInfo(Directory.GetCurrentDirectory());
    }

    /// <summary>
    /// Compute the MurmurHash3 of the data.
    /// </summary>
    /// <param name="nHashSeed">The hash seed.</param>
    /// <param name="dataToHash">The input byte array to hash.</param>
    /// <returns>The hash value. This returns a long to make it easier to interpret
    /// it as an unsigned 32-bit integer (instead of the Java int which is signed).</returns>
    public static long
    MurmurHash3(int nHashSeed, byte[] dataToHash)
    {
      using (MemoryStream stream = new MemoryStream(dataToHash)) {
        int hash = MurMurHash3.Hash((uint)nHashSeed, stream);
        return hash >= 0 ? hash : 0x100000000L + hash;
      }
    }

    /// <summary>
    /// Compute the MurmurHash3 of the integer value.
    /// </summary>
    /// <param name="nHashSeed">The hash seed.</param>
    /// <param name="value">The integer value, interpreted as a 4-byte little-endian array.
    /// This ignores the upper 4 bytes of the long integer.</param>
    /// <returns>The hash value. This returns a Java int which is a 32-bit signed
    /// integer, but it should be interpreted as a 32-bit unsigned integer.</returns>
    public static long
    MurmurHash3(int nHashSeed, long value)
    {
      byte[] dataToHash = new byte[] {
        (byte) (value        & 0xff),
        (byte)((value >> 8)  & 0xff),
        (byte)((value >> 16) & 0xff),
        (byte)((value >> 24) & 0xff)
      };
      return MurmurHash3(nHashSeed, dataToHash);
    }

    // From https://gist.github.com/automatonic/3725443
    public static class MurMurHash3
    {
      public static int Hash(uint seed, Stream stream)
      {
        const uint c1 = 0xcc9e2d51;
        const uint c2 = 0x1b873593;

        uint h1 = seed;
        uint k1 = 0;
        uint streamLength = 0;

        using (BinaryReader reader = new BinaryReader(stream))
        {
          byte[] chunk = reader.ReadBytes(4);
          while (chunk.Length > 0)
          {
            streamLength += (uint)chunk.Length;
            switch(chunk.Length)
            {
            case 4:
              /* Get four bytes from the input into an uint */
              k1 = (uint)
                (chunk[0]
                  | chunk[1] << 8
                  | chunk[2] << 16
                  | chunk[3] << 24);

              /* bitmagic hash */
              k1 *= c1;
              k1 = rotl32(k1, 15);
              k1 *= c2;

              h1 ^= k1;
              h1 = rotl32(h1, 13);
              h1 = h1 * 5 + 0xe6546b64;
              break;
            case 3:
              k1 = (uint)
                (chunk[0]
                  | chunk[1] << 8
                  | chunk[2] << 16);
              k1 *= c1;
              k1 = rotl32(k1, 15);
              k1 *= c2;
              h1 ^= k1;
              break;
            case 2:
              k1 = (uint)
                (chunk[0]
                  | chunk[1] << 8);
              k1 *= c1;
              k1 = rotl32(k1, 15);
              k1 *= c2;
              h1 ^= k1;
              break;
            case 1:
              k1 = (uint)(chunk[0]);
              k1 *= c1;
              k1 = rotl32(k1, 15);
              k1 *= c2;
              h1 ^= k1;
              break;

            }
            chunk = reader.ReadBytes(4);
          }
        }

        // finalization, magic chants to wrap it all up
        h1 ^= streamLength;
        h1 = fmix(h1);

        unchecked //ignore overflow
        {
          return (int)h1;
        }
      }

      private static uint rotl32(uint x, byte r)
      {
        return (x << r) | (x >> (32 - r));
      }

      private static uint fmix(uint h)
      {
        h ^= h >> 16;
        h *= 0x85ebca6b;
        h ^= h >> 13;
        h *= 0xc2b2ae35;
        h ^= h >> 16;
        return h;
      }
    }

    private static SecureRandom randomNumberGenerator_;
    private static SHA256 sha256_ = SHA256Managed.Create();
    private static DateTime unixEpoch_ = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
  }
}
