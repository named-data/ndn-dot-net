/**
 * Copyright (C) 2014-2017 Regents of the University of California.
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
using net.named_data.jndn.encoding;
using net.named_data.jndn.transport;
using net.named_data.jndn.util;

class Counter1 : OnData, OnTimeout {
  public void 
  onData(Interest interest, Data data)
  {
    ++callbackCount_;
    Console.Out.WriteLine("Got data packet with name " + data.getName().toUri());
    var content = data.getContent().buf();
    var contentString = "";
    for (int i = content.position(); i < content.limit(); ++i)
      contentString += (char)content.get(i);
    Console.Out.WriteLine(contentString);
  }
  
  public void 
  onTimeout(Interest interest)
  {
    ++callbackCount_;
    Console.Out.WriteLine("Time out for interest " + interest.getName().toUri());
  }

  public int callbackCount_ = 0;
}

namespace TestNdnDotNet
{
  class TestEchoConsumer {
    static void Main(string[] args)
    {
      try {
        var face = new Face
          (new TcpTransport(), new TcpTransport.ConnectionInfo("localhost"));

        var counter = new Counter1();

        Console.Out.WriteLine("Enter a word to echo:");
        var word = Console.In.ReadLine();

        var name = new Name("/testecho");
        name.append(word);
        Console.Out.WriteLine("Express name " + name.toUri());
        face.expressInterest(name, counter, counter);

        // The main event loop.
        while (counter.callbackCount_ < 1) {
          face.processEvents();

          // We need to sleep for a few milliseconds so we don't use 100% of the CPU.
          System.Threading.Thread.Sleep(5);
        }
      } catch (Exception e) {
        Console.Out.WriteLine("exception: " + e.Message);
      }
    }
  }
}
