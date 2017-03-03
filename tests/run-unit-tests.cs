/**
 * Copyright (C) 2016-2017 Regents of the University of California.
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
using JUnitDotNet;

namespace TestNdnDotNet {
  class RunUnitTests {
    /// <summary>
    /// Run tests in the namespace net.named_data.jndn.tests.unit_tests.
    /// </summary>
    static void Main(string[] args)
    {
      // Use TestNameConventions just to get its Assembly.
      var assembly = typeof(net.named_data.jndn.tests.unit_tests.TestNameConventions).Assembly;
      JUnit.runTests(assembly, "net.named_data.jndn.tests.unit_tests");
    }
  }
}
