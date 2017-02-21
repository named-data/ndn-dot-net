/**
 * Copyright (C) 2017 Regents of the University of California.
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

namespace JUnitDotNet {
  public class JUnit {
    /// <summary>
    /// This imitates the behavior of JUnit. to use reflection to find all classes 
    /// in the given assembly in the given nameSpace where the class name starts with 
    /// "Test". Run methods which start with "test". If the class defines a static "setUpClass"
    /// method, run it once before all tests in the class. If the class defines a "setUp"
    /// method, run it before each test method. If the class defines a "tearDown"
    /// method, run it after each test method. If the class defines a static "tearDownClass"
    /// method, run it once after all tests in the class.
    /// Assume the test will use the Assert methods that may throw an exception.
    /// Print results to Console.Out.
    /// </summary>
    public static void 
    runTests(Assembly assembly, string nameSpace)
    {
      int nFailed = 0;

      // Find all types in the unit_tests namespace that start with "Test".
      foreach (var type in assembly.GetTypes()) {
        if (type.Namespace == nameSpace && type.Name.StartsWith("Test")) {
          var testInstance = type.GetConstructor(new Type[0]).Invoke(null);

          var setUpClassMethod = type.GetMethod("setUpClass", new Type[0]);
          var tearDownClassMethod = type.GetMethod("tearDownClass", new Type[0]);
          var setUpMethod = type.GetMethod("setUp", new Type[0]);
          var tearDownMethod = type.GetMethod("tearDown", new Type[0]);

          if (setUpClassMethod != null)
            setUpClassMethod.Invoke(null, null);

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

          if (tearDownClassMethod != null)
            tearDownClassMethod.Invoke(null, null);
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
