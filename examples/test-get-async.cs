/**
 * Copyright (C) 2014 Regents of the University of California.
 * @author: Jeff Thompson <jefft0@remap.ucla.edu>
 * See COPYING for copyright and distribution information.
 */

using System;
using System.Collections.Generic;
using System.Text;

using java.nio;
using net.named_data.jndn;
using net.named_data.jndn.encoding;
using net.named_data.jndn.transport;
using net.named_data.jndn.util;

class Counter : ElementListener {
  public void 
  onReceivedElement(ByteBuffer element) 
  {
    if (element.get(0) == 0x04) {
      ++callbackCount_;
      try {
        Data data = new Data();
        data.wireDecode(element);

        Console.Out.WriteLine
          ("Got data packet with name " + data.getName().toUri());
        ByteBuffer content = data.getContent().buf();
		string contentString = "";
        for (int i = content.position(); i < content.limit(); ++i)
		  contentString += (char)content.get(i);
		Console.Out.WriteLine(contentString);
      } 
      catch (EncodingException e) 
      {
        Console.Out.WriteLine("EncodingException " + e.getMessage());
      }
    }
  }
  
  public int callbackCount_ = 0;
}

namespace TestNdnDotNet
{
  class TestGetAsync {
	static void Main(string[] args)
	{
	  try {
        Counter counter = new Counter();

        TcpTransport transport = new TcpTransport();
        // Connect to port 9695 until the testbed hubs use NDNx and are 
        //   connected to the test repo.
        transport.connect(new TcpTransport.ConnectionInfo
          ("ndn-remap-p02.it.ucla.edu", 9695), counter);

        Name name1 = new Name("/");
        Console.Out.WriteLine("Express name " + name1.toUri());
        Interest interest = new Interest(name1, 4000.0);
        Blob encoding = interest.wireEncode();
        transport.send(encoding.buf());

        // The main event loop.
        while (counter.callbackCount_ < 1) {
          transport.processEvents();
          // We need to sleep for a few milliseconds so we don't use 100% of the CPU.
		  System.Threading.Thread.Sleep(5);
        }
	  }
	  catch (Exception e) {
		Console.Out.WriteLine("exception: " + e.Message);
      }
    }
  }
}
