/**
 * Copyright (C) 2014 Regents of the University of California.
 * @author: Jeff Thompson <jefft0@remap.ucla.edu>
 * See COPYING for copyright and distribution information.
 */

using System;
using System.Net;
using System.Net.Sockets;
using java.nio;
using net.named_data.jndn;
using net.named_data.jndn.encoding;

namespace net.named_data.jndn.transport
{
  /// <summary>
  /// A DotNetTcpTransport extends Transport to connect using a .NET 
  /// System.Net.Sockets.Socket over TCP.
  /// </summary>
  public class DotNetTcpTransport : Transport
  {
    /// <summary>
    /// A TcpTransport::ConnectionInfo extends Transport::ConnectionInfo to hold 
    /// the host and port info for the TCP connection.
    /// </summary>
    public new class ConnectionInfo : Transport.ConnectionInfo
    {
      /// <summary>
      /// Create a ConnectionInfo with the given host and port.
      /// </summary>
      /// <param name="host">The host for the connection.</param>
      /// <param name="port">The port number for the connection.</param>
      public
      ConnectionInfo(String host, int port)
      {
        host_ = host;
        port_ = port;
      }

      /// <summary>
      /// Create a ConnectionInfo with the given host and default port 6363.
      /// </summary>
      /// <param name="host">The host for the connection.</param>
      public
      ConnectionInfo(String host)
      {
        host_ = host;
        port_ = 6363;
      }

      /// <summary>
      ///  Get the host given to the constructor.
      /// </summary>
      /// <returns>The host.</returns>
      public String
      getHost() { return host_; }

      /// <summary>
      /// Get the port given to the constructor.
      /// </summary>
      /// <returns>The port number.</returns>
      public int
      getPort() { return port_; }

      private String host_;
      private int port_;
    }

    /// <summary>
    /// This is from the code sample at
    /// http://msdn.microsoft.com/en-us/library/system.net.sockets.socket(v=vs.110).aspx
    /// </summary>
    /// <param name="server"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    private static Socket
    connectSocket(string server, int port)
    {
      Socket s = null;

      // Get host related information.
      var hostEntry = Dns.GetHostEntry(server);

      // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid 
      // an exception that occurs when the host IP Address is not compatible with the address family 
      // (typical in the IPv6 case). 
      foreach (IPAddress address in hostEntry.AddressList)
      {
        var ipe = new IPEndPoint(address, port);
        var tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        tempSocket.Connect(ipe);

        if (tempSocket.Connected)
        {
          s = tempSocket;
          break;
        }
      }

      return s;
    }

    /// <summary>
    /// Connect according to the info in ConnectionInfo, and use elementListener.
    /// </summary>
    /// <param name="connectionInfo">A DotNetTcpTransport.ConnectionInfo.</param>
    /// <param name="elementListener">The ElementListener must remain valid during the 
    /// life of this object.</param>
    public override void
    connect(Transport.ConnectionInfo connectionInfo, ElementListener elementListener)
    {
      close();

      socket_ = connectSocket
        (((ConnectionInfo)connectionInfo).getHost(),
         ((ConnectionInfo)connectionInfo).getPort());

      elementReader_ = new BinaryXmlElementReader(elementListener);
    }

    /// <summary>
    /// Set data to the host.
    /// </summary>
    /// <param name="data">The buffer of data to send.  This reads from position() to 
    /// limit(), but does not change the position.</param>
    public override void
    send(ByteBuffer data)
    {
      if (socket_ == null)
        throw new System.IO.IOException
          ("Cannot send because the socket is not open.  Use connect.");

      // ByteBuffer is readonly so we can't call array(), and we can't do a low-level
      //   operation on the array, so we have to copy.
      var buffer = new byte[data.remaining()];
      int savePosition = data.position();
      data.get(buffer);
      data.position(savePosition);

      socket_.Send(buffer);
    }

    /// <summary>
    ///  Process any data to receive.  For each element received, call 
    /// elementListener.onReceivedElement.
    /// This is non-blocking and will return immediately if there is no data to 
    /// receive. You should normally not call this directly since it is called by 
    /// Face.processEvents.
    /// If you call this from an main event loop, you may want to catch and 
    /// log/disregard all exceptions.
    /// </summary>
    public override void
    processEvents()
    {
      if (!getIsConnected())
        return;

      while (true)
      {
        // Set the timeout to 0 to return immediately.
        if (!socket_.Poll(0, SelectMode.SelectRead))
          // Nothing to read.
          return;

        int bytesRead = socket_.Receive(inputBuffer_.array(), inputBuffer_.capacity(), 0);
        if (bytesRead <= 0)
          return;

        inputBuffer_.limit(bytesRead);
        inputBuffer_.position(0);
        elementReader_.onReceivedData(inputBuffer_);
      }
    }

    /// <summary>
    ///  Check if the transport is connected.
    /// </summary>
    /// <returns>True if connected.</returns>
    public override bool
    getIsConnected()
    {
      if (socket_ == null)
        return false;

      return socket_.Connected;
    }

    /// <summary>
    /// Close the connection.  If not connected, this does nothing.
    /// </summary>
    public override void
    close()
    {
      if (socket_ != null)
      {
        if (socket_.Connected)
          socket_.Close();
        socket_ = null;
      }
    }

    Socket socket_ = null;
    ByteBuffer inputBuffer_ = ByteBuffer.allocate(8000);
    // TODO: This belongs in the socket listener.
    private BinaryXmlElementReader elementReader_;
  }
}