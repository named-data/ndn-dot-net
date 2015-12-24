/**
 * Copyright (C) 2014-2015 Regents of the University of California.
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
using System.Collections.Generic;
using System.Text;

using net.named_data.jndn;
using net.named_data.jndn.util;

namespace TestNdnDotNet {
  class TestEncodeDecodeInterest {
    static int[] TlvInterest = new int[] {
0x05, 0x50, // Interest
  0x07, 0x0A, 0x08, 0x03, 0x6E, 0x64, 0x6E, 0x08, 0x03, 0x61, 0x62, 0x63, // Name
  0x09, 0x38, // Selectors
    0x0D, 0x01, 0x04, // MinSuffixComponents
    0x0E, 0x01, 0x06, // MaxSuffixComponents
    0x0F, 0x22, // KeyLocator
      0x1D, 0x20, // KeyLocatorDigest
                  0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                  0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
    0x10, 0x07, // Exclude
      0x08, 0x03, 0x61, 0x62, 0x63, // NameComponent
      0x13, 0x00, // Any
    0x11, 0x01, 0x01, // ChildSelector
    0x12, 0x00, // MustBeFesh
  0x0A, 0x04, 0x61, 0x62, 0x61, 0x62,	// Nonce
  0x0C, 0x02, 0x75, 0x30, // InterestLifetime
1
    };
   
    static void dumpInterest(Interest interest)
    {
      Console.Out.WriteLine("name: " + interest.getName().toUri());
      Console.Out.WriteLine("minSuffixComponents: " + 
        (interest.getMinSuffixComponents() >= 0 ? 
         "" + interest.getMinSuffixComponents() : "<none>"));
      Console.Out.WriteLine("maxSuffixComponents: " +
        (interest.getMaxSuffixComponents() >= 0 ?
         "" + interest.getMaxSuffixComponents() : "<none>"));
      Console.Out.WriteLine
        ("exclude: " + (interest.getExclude().size() > 0 ? 
                interest.getExclude().toUri() : "<none>"));
      Console.Out.WriteLine("lifetimeMilliseconds: " +
        (interest.getInterestLifetimeMilliseconds() >= 0 ?
         "" + interest.getInterestLifetimeMilliseconds() : "<none>"));
      Console.Out.WriteLine("childSelector: " +
        (interest.getChildSelector() >= 0 ?
         "" + interest.getChildSelector() : "<none>"));
      Console.Out.WriteLine("nonce: " +
        (interest.getNonce().size() > 0 ? "" + interest.getNonce().toHex() : "<none>"));
    }

    static void Main(string[] args)
    {
      var interest = new Interest();
      interest.wireDecode(new Blob(TlvInterest));
      Console.Out.WriteLine("Interest:");
      dumpInterest(interest);

      var encoding = interest.wireEncode();
      Console.Out.WriteLine("Re-encoded interest " + encoding.toHex());

      var reDecodedInterest = new Interest();
      reDecodedInterest.wireDecode(encoding);
      Console.Out.WriteLine("");
      Console.Out.WriteLine("Re-decoded Interest:");
      dumpInterest(reDecodedInterest);

      var freshInterest = new Interest(new Name("/ndn/abc"));
      freshInterest.setMinSuffixComponents(4);
      freshInterest.setMaxSuffixComponents(6);
      freshInterest.getExclude().appendComponent(new Name("abc").get(0)).appendAny();
      freshInterest.setInterestLifetimeMilliseconds(30000);
      freshInterest.setChildSelector(1);

      Interest reDecodedFreshInterest = new Interest();
      reDecodedFreshInterest.wireDecode(freshInterest.wireEncode());

      Console.Out.WriteLine("");
      Console.Out.WriteLine("Re-decoded fresh Interest:");
      dumpInterest(reDecodedFreshInterest);
    }
  }
}
