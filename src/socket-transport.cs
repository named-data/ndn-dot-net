/**
 * Copyright (C) 2017-2018 Regents of the University of California.
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
  /// SocketTransport extends Transport and is an abstract base class for 
  /// TcpTransport and UdpTransport to connect using a .NET 
  /// System.Net.Sockets.Socket.
  /// </summary>
  public abstract class SocketTransport : Transport
  {
    /// <summary>
    /// Create a SocketTransport for the given socket and protocol type
    /// </summary>
    /// <param name="socketType">The socket type.</param>
    /// <param name="protocolType">the protocol type.</param>
    public SocketTransport(SocketType socketType, ProtocolType protocolType)
    {
      socketType_ = socketType;
      protocolType_ = protocolType;
    }

    /// <summary>
    /// A SocketTransport.ConnectionInfo extends Transport.ConnectionInfo to hold 
    /// the host and port info for the socket connection.
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
    protected Socket
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
        var tempSocket = new Socket(ipe.AddressFamily, socketType_, protocolType_);
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
    /// Send data to the host.
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
        if (socket_.Connected) {
          socket_.Shutdown(SocketShutdown.Both);
          socket_.Close();
        }
        socket_ = null;
      }
    }

    /// <summary>
    /// Override to return false since connect does not need to use the onConnected
    /// callback.
    /// </summary>
    ///
    /// <returns>False.</returns>
    public override bool isAsync() 
    {
      return false;
    }

    protected SocketType socketType_;
    protected ProtocolType protocolType_;
    protected Socket socket_ = null;
    protected ByteBuffer inputBuffer_ = ByteBuffer.allocate(8000);
    protected ElementReader elementReader_;
    protected ConnectionInfo connectionInfo_;
  }
}
