/**
 * Copyright (C) 2016 Regents of the University of California.
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
using System.Reflection;

namespace TestNdnDotNet {
  class RunUnitTests {
    /// <summary>
    /// Use reflection to find and run tests in the namespace 
    /// net.named_data.jndn.tests.unit_tests.
    /// </summary>
    static void Main(string[] args)
    {
      int nFailed = 0;

      // Use TestNameConventions just to get its Assembly.
      var assembly = typeof(net.named_data.jndn.tests.unit_tests.TestNameConventions).Assembly;
      // Find all types in the unit_tests namespace that start with "Test".
      foreach (var type in assembly.GetTypes()) {
        if (type.Namespace == "net.named_data.jndn.tests.unit_tests" &&
            type.Name.StartsWith("Test")) {
          var testInstance = type.GetConstructor(new Type[0]).Invoke(null);

          var setUpMethod = type.GetMethod("setUp", new Type[0]);
          var tearDownMethod = type.GetMethod("tearDown", new Type[0]);

          // Find each method that starts with "test" and takes no arguments.
          foreach (var methodInfo in type.GetMethods()) {
            if (!methodInfo.IsStatic && methodInfo.Name.StartsWith("test") &&
                methodInfo.GetParameters().Length == 0) {
              Console.Out.WriteLine("Running test " + type.Name + "." + methodInfo.Name);

              if (setUpMethod != null)
                setUpMethod.Invoke(testInstance, null);

              // Invoke the test and print any error.
              try {
                methodInfo.Invoke(testInstance, null);
              } catch (TargetInvocationException ex) {
                ++nFailed;
                Console.Out.WriteLine("FAIL: " + ex.InnerException.Message);
              }

              if (tearDownMethod != null)
                tearDownMethod.Invoke(testInstance, null);
            }
          }
        }
      }

      Console.Out.WriteLine("");
      if (nFailed > 0)
        Console.Out.WriteLine("FAILED " + nFailed + " tests.");
      else
        Console.Out.WriteLine("PASSED all tests.");
    }
  }
}
