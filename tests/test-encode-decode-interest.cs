/**
 * Copyright (C) 2014 Regents of the University of California.
 * @author: Jeff Thompson <jefft0@remap.ucla.edu>
 * See COPYING for copyright and distribution information.
 */

using System;
using System.Collections.Generic;
using System.Text;

using net.named_data.jndn;
using net.named_data.jndn.util;

namespace TestNdnDotNet
{
    class TestEncodeDecodeInterest
    {
        static int[] BinaryXmlInterest = new int[] {
0x01, 0xd2,
  0xf2, 0xfa, 0x9d, 0x6e, 0x64, 0x6e, 0x00, 0xfa, 0x9d, 0x61, 0x62, 0x63, 0x00, 0x00, 
  0x05, 0x9a, 0x9e, 0x31, 0x32, 0x33, 0x00, 
  0x05, 0xa2, 0x8e, 0x34, 0x00,
  0x03, 0xe2, 
    0x02, 0x85, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 
                0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f, 
  0x00,
  0x02, 0xda, 0xfa, 0x9d, 0x61, 0x62, 0x63, 0x00, 0xea, 0x00, 0x00, 
  0x05, 0xaa, 0x8e, 0x31, 0x00, 
  0x02, 0xfa, 0x8e, 0x34, 0x00, 
  0x02, 0xd2, 0x8e, 0x32, 0x00, 
  0x03, 0x82, 0x9d, 0x01, 0xe0, 0x00, 0x00, 
  0x02, 0xca, 0xb5, 0x61, 0x62, 0x61, 0x62, 0x61, 0x62, 0x00, 
0x00, 
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
            Console.Out.WriteLine("scope: " +
                (interest.getScope() >= 0 ? "" + interest.getScope() : "<none>"));
            Console.Out.WriteLine("nonce: " +
                (interest.getNonce().size() >= 0 ? "" + interest.getNonce().toHex() : "<none>"));
        }

        static void Main(string[] args)
        {
            var interest = new Interest();
            interest.wireDecode(new Blob(BinaryXmlInterest));
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
            freshInterest.getPublisherPublicKeyDigest().setPublisherPublicKeyDigest
                (new Blob(new byte[] {
                 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 
                 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F }));
            freshInterest.getExclude().appendComponent(new Name("abc").get(0)).appendAny();
            freshInterest.setInterestLifetimeMilliseconds(30000);
            freshInterest.setChildSelector(1);
            freshInterest.setScope(2);

            Interest reDecodedFreshInterest = new Interest();
            reDecodedFreshInterest.wireDecode(freshInterest.wireEncode());

            Console.Out.WriteLine("");
            Console.Out.WriteLine("Re-decoded fresh Interest:");
            dumpInterest(reDecodedFreshInterest);
        }
    }
}
