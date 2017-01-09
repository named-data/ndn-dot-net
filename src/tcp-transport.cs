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

using ILOG.J2CsMapping.NIO;
using System;
using System.Net;
using System.Net.Sockets;
using net.named_data.jndn;
using net.named_data.jndn.encoding;
using net.named_data.jndn.util;

namespace net.named_data.jndn.transport
{
  /// <summary>
  /// A TcpTransport extends Transport to connect using a .NET 
  /// System.Net.Sockets.Socket over TCP.
  /// </summary>
  public class TcpTransport : Transport
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
    /// Determine whether this transport connecting according to connectionInfo is
    /// to a node on the current machine. This affects the processing of
    /// Face.registerPrefix(): if the NFD is local, registration occurs with the
    /// '/localhost/nfd...' prefix; if non-local, the library will attempt to use
    /// remote prefix registration using '/localhop/nfd...'
    /// </summary>
    ///
    /// <param name="connectionInfo">A ConnectionInfo with the host to check.</param>
    /// <returns>True if the host is local, false if not.</returns>
    public override bool 
    isLocal(Transport.ConnectionInfo connectionInfo)
    {
      if (connectionInfo_ == null || 
          ((ConnectionInfo)connectionInfo).getHost() !=  connectionInfo_.getHost()) {
        isLocal_ = getIsLocal(((ConnectionInfo)connectionInfo).getHost());
        connectionInfo_ = (ConnectionInfo)connectionInfo;
      }

      return isLocal_;
    }

    /// <summary>
    /// Override to return false since connect does not need to use the onConnected
    /// callback.
    /// </summary>
    ///
    /// <returns>False.</returns>
    public override bool isAsync() {
      return false;
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
    ///
    /// <param name="connectionInfo">A TcpTransport.ConnectionInfo.</param>
    /// <param name="elementListener"></param>
    /// <param name="onConnected"></param>
    /// <exception cref="IOException">For I/O error.</exception>
    public override void 
    connect(Transport.ConnectionInfo connectionInfo,
      ElementListener elementListener, IRunnable onConnected) 
    {
      close();

      socket_ = connectSocket
        (((ConnectionInfo)connectionInfo).getHost(),
         ((ConnectionInfo)connectionInfo).getPort());

      elementReader_ = new ElementReader(elementListener);

      if (onConnected != null)
        onConnected.run();
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

    /// <summary>
    /// A static method to determine whether the host is on the current machine.
    /// Results are not cached. According to
    /// http://redmine.named-data.net/projects/nfd/wiki/ScopeControl#local-face,
    /// TCP transports with a loopback address are local. If connectionInfo
    /// contains a host name, InetAddress will do a blocking DNS lookup; otherwise
    /// it will parse the IP address and examine the first octet to determine if
    /// it is a loopback address (e.g. first octet == 127).
    /// </summary>
    ///
    /// <param name="host">The host to check.</param>
    /// <returns>True if the host is local, False if not.</returns>
    public static bool 
    getIsLocal(string host) 
    {
      // TODO: Implement the lookup as in TcpTransport.
      return false;
    }

    Socket socket_ = null;
    ByteBuffer inputBuffer_ = ByteBuffer.allocate(8000);
    private ElementReader elementReader_;
    private ConnectionInfo connectionInfo_;
    private bool isLocal_;
  }
}
