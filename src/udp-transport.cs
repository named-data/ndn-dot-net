/**
 * Copyright (C) 2017-2019 Regents of the University of California.
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
  /// A UdpTransport extends Transport to connect using a .NET 
  /// System.Net.Sockets.Socket over UDP.
  /// </summary>
  public class UdpTransport : SocketTransport
  {
    /// <summary>
    /// Create a TcpTransport.
    /// </summary>
    public UdpTransport() : base(SocketType.Dgram, ProtocolType.Udp)
    {
    }

    /// <summary>
    /// A UdpTransport::ConnectionInfo extends Transport::ConnectionInfo to hold 
    /// the host and port info for the UDP connection.
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
    /// Determine whether this transport connecting according to connectionInfo 
    /// is to a node on the current machine. According to 
    /// http://redmine.named-data.net/issues/2532#note-8, UDP transports are 
    /// always non-local.
    /// </summary>
    /// <param name="connectionInfo">This is ignored.</param>
    /// <returns>False because UDP transports are always non-local.</returns>
    public override bool 
    isLocal(Transport.ConnectionInfo connectionInfo)
    {
      return false;
    }
  }
}
