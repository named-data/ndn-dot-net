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
using System.Threading;
using net.named_data.jndn;
using net.named_data.jndn.util;

namespace net.named_data.jndn.transport
{
  /// <summary>
  /// ThreadPoolFace extends Face to provide the main methods for NDN 
  /// communication using the system ThreadPool. Most constructors create an
  /// AsyncTcpTransport for asynchronous communication. This also uses the 
  /// system ThreadPool to schedule the interest timeouts.
  /// </summary>
  public class ThreadPoolFace : Face
  {
    /// <summary>
    /// Create a new Face for communication with an NDN hub with the given
    /// Transport object and connectionInfo.
    /// </summary>
    /// <param name="transport"A Transport object used for communication. If you 
    /// do not want to call processEvents, then the transport should be an async 
    /// transport like AsyncTcpTransport.</param>
    /// <param name="connectionInfo">A Transport.ConnectionInfo to be used to 
    /// connect to the transport.</param>
    public ThreadPoolFace
      (Transport transport, Transport.ConnectionInfo connectionInfo)
    : base(transport, connectionInfo)
    {
    }

    /// <summary>
    /// Create a new Face for communication with an NDN hub at host:port using 
    /// an AsyncTcpTransport
    /// </summary>
    ///
    /// <param name="host">The host of the NDN hub.</param>
    /// <param name="port">The port of the NDN hub.</param>
    public ThreadPoolFace(String host, int port)
      : this(new AsyncTcpTransport(), 
             new AsyncTcpTransport.ConnectionInfo(host, port))
    {
    }

    /// <summary>
    /// Create a new Face for communication with an NDN hub at host using the
    /// default port 6363 and an AsyncTcpTransport.
    /// </summary>
    ///
    /// <param name="host">The host of the NDN hub.</param>
    public ThreadPoolFace(String host)
      : this(new AsyncTcpTransport(), 
        new AsyncTcpTransport.ConnectionInfo(host))
    {
    }

    /// <summary>
    /// Create a new Face for communication with an NDN hub at "localhost" using 
    /// the default port 6363 and an AsyncTcpTransport.
    /// </summary>
    ///
    public ThreadPoolFace()
      : this(new AsyncTcpTransport(), 
        new AsyncTcpTransport.ConnectionInfo("localhost"))
    {
    }

    /// <summary>
    /// Override to schedule in the system ThreadPool to call callback.run() 
    /// after the given delay. Even though this is public, it is not part of the 
    /// public API of Face.
    /// </summary>
    ///
    /// <param name="delayMilliseconds">The delay in milliseconds.</param>
    /// <param name="callback">This calls callback.run() after the delay.</param>
    public override void callLater(double delayMilliseconds, IRunnable callback)
    {
      RegisteredWaitHandle handle = null;
      handle = ThreadPool.RegisterWaitForSingleObject
        (new AutoResetEvent(false), 
         new WaitOrTimerCallback(delegate(object state, bool timedOut) {
           handle.Unregister(null);
           callback.run();
         }),
         null, (int)delayMilliseconds, true);
    }
  }
}
