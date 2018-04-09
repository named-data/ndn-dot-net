/**
 * Copyright (C) 2018 Regents of the University of California.
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

namespace net.named_data.jndn.transport
{
  /// <summary>
  /// An AsyncTcpTransport extends Transport to connect using a .NET 
  /// System.Net.Sockets.Socket over TCP using asynchronous communication.
  /// To use this, you do not need to call processEvents.
  /// </summary>
  public class AsyncTcpTransport : AsyncSocketTransport
  {
    /// <summary>
    /// Create an AsyncTcpTransport.
    /// </summary>
    public AsyncTcpTransport() : base(SocketType.Stream, ProtocolType.Tcp)
    {
    }

    /// <summary>
    /// An AsyncTcpTransport.ConnectionInfo extends Transport.ConnectionInfo to 
    /// hold the host and port info for the TCP connection.
    /// </summary>
    public new class ConnectionInfo : SocketTransport.ConnectionInfo
    {
      /// <summary>
      /// Create a ConnectionInfo with the given host and port.
      /// </summary>
      /// <param name="host">The host for the connection.</param>
      /// <param name="port">The port number for the connection.</param>
      public
      ConnectionInfo(String host, int port) : base(host, port)
      {
      }

      /// <summary>
      /// Create a ConnectionInfo with the given host and default port 6363.
      /// </summary>
      /// <param name="host">The host for the connection.</param>
      public
      ConnectionInfo(String host) : base(host)
      {
      }
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

    private bool isLocal_;
  }
}
