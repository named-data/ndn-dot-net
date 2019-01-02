/**
 * Copyright (C) 2018-2019 Regents of the University of California.
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
using System.Threading;
using net.named_data.jndn;
using net.named_data.jndn.encoding;
using net.named_data.jndn.util;

namespace net.named_data.jndn.transport
{
  /// <summary>
  /// AsyncSocketTransport extends SocketTransport and is an abstract base class 
  /// for AsyncTcpTransport and AsyncUdpTransport to connect using a .NET 
  /// System.Net.Sockets.Socket for asynchronous communication.
  /// See https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
  /// </summary>
  public abstract class AsyncSocketTransport : SocketTransport
  {
    /// <summary>
    /// Create an AsyncSocketTransport for the given socket and protocol type
    /// </summary>
    /// <param name="socketType">The socket type.</param>
    /// <param name="protocolType">the protocol type.</param>
    public AsyncSocketTransport(SocketType socketType, ProtocolType protocolType)
    : base(socketType, protocolType)
    {
    }

    /// <summary>
    /// Connect according to the info in ConnectionInfo, and use elementListener.
    /// </summary>
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

      if (socket_ != null) {
        elementReader_ = new ElementReader(elementListener);

        if (onConnected != null)
          onConnected.run();

        // Begin a new receive.
        socket_.BeginReceive
          (inputBuffer_.array(), 0, inputBuffer_.capacity(), 0,
            new AsyncCallback(receiveCallback), null);
      }
    }

    /// <summary>
    /// Do nothing since we receive data using receiveCallback.
    /// </summary>
    public override void
    processEvents()
    {
    }

    private void 
    receiveCallback(IAsyncResult result)
    {
      try {
        // Read data from the remote device.
        int bytesRead = socket_.EndReceive(result);

        if (bytesRead > 0) {
          inputBuffer_.limit(bytesRead);
          inputBuffer_.position(0);
          elementReader_.onReceivedData(inputBuffer_);
        }
      } finally {
        // Begin a new receive.
        socket_.BeginReceive
          (inputBuffer_.array(), 0, inputBuffer_.capacity(), 0,
           new AsyncCallback(receiveCallback), null);
      }
    }
  }
}
