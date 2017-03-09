NDN-DOT-NET: A Named Data Networking client library for the .NET Framework
==========================================================================

NDN-DOT-NET has been tested with Mono and Xamarin Studio on:

 * OS X 10.10, OS X 10.11 and macOS 10.12
 * 64-bit Ubuntu 16.04 and 16.10.

NDN-DOT-NET has been tested with Microsoft .NET Framework and Visual Studio on:

 * Windows 7 64-bit

Prerequisites
=============

In your application project, you must add a reference to the following assemblies:

* System
* System.Data
* Mono.Data.Sqlite (if not using Microsoft .NET assemblies)

Build
=====

In the following, `<NDN-DOT-NET root>` is the root folder of the NDN-DOT-NET distribution.

## Using ndn-dot-net.dll

The easiest way to make you application is to add a reference to
`<NDN-DOT-NET root>/bin/ndn-dot-net.dll` . 

The DLL is built for .NET Framework 3.5 (for use in Unity). If your 
application is for a different .NET Framework 4.0 version then some 
assemblies may not load, in which case you should build from src as follows.

## Using the src files

Alternatively, if you want to experiment with changing the source code or you want
to statically link, then add all the *.cs files in the folder `<NDN-DOT-NET root>/src` 
(and all subfolders).

## Examples

To run an example, add `ndn-dot-net.dll` (or the src files, see above). Then add 
one of the files in the folder `<NDN-DOT-NET root>/examples` . For example, 
`<NDN-DOT-NET root>/examples/test-encode-decode-data.cs` .

## Unit tests

To run the unit tests, add `ndn-dot-net.dll` (or the src files, see above). Then add
all of the files in the folder `<NDN-DOT-NET root>/tests/unit_tests` .
Also add `<NDN-DOT-NET root>/tests/junit-dot-net.cs` (a JUnit utility) and 
`<NDN-DOT-NET root>/tests/run-unit-tests.cs` (which has the `main` method).

## Integration tests

To run the integration tests, The local NFD must be running. Add the src files (see above). 
Then add all of the files in the folder 
`<NDN-DOT-NET root>/tests/integration_tests` .
Also add `<NDN-DOT-NET root>/tests/junit-dot-net.cs` (a JUnit utility) and 
`<NDN-DOT-NET root>/tests/run-integration-tests.cs` (which has the `main` method).
Finally, the executable needs to access the test files. In a terminal change directory 
to the directory of the executable. Enter:

    mkdir tests
    ln -s <NDN-DOT-NET root>/eclipse/jndn/src tests/src

Java to C# Translation Prerequisites
====================================
These steps are only needed to do the translation from jNDN Java files to C#
(when jNDN is updated).

## OS X 10.10.2 and OS X 10.11

* Download and unzip Eclipse 3.6.2 (Helios SR2) for Mac Cocoa 64-bit from
  http://www.eclipse.org/downloads/download.php?file=/technology/epp/downloads/release/helios/SR2/eclipse-java-helios-SR2-macosx-cocoa-x86_64.tar.gz .
* Download the j2cstranslator plugin for Eclipse 3.6.0 from
  http://sourceforge.net/projects/j2cstranslator/files/j2cstranslator/Eclipse3.6/1.3.6.2001_05_19_01/
* Put the j2cstranslator plugin jar file in eclipse/plugins .

Java to C# Translation
======================
* Replace the snapshot in `ndn-dot-net/eclipse/jndn/src/net` with the updated jNDN
  files from `jndn/src/net` . Also copy `jndn/tests/src/net/named_data/jndn/tests` to
  `ndn-dot-net/eclipse/jndn/src/net/named_data/jndn/tests` .
* In a terminal, change directory to the new folder 
  `ndn-dot-net/eclipse/jndn/src/net/named_data/jndn/tests/integration_tests`, 
  run the following to fix the namespace.

    sed -i '' 's/package src\.net\.named_data\.jndn\.tests\.integration_tests/package net.named_data.jndn.tests.integration_tests/g' *.java

* Remove the old C# output folder `ndn-dot-net/src/net` so we can rebuild.
* Start Eclipse 3.6.0. (If necessary, follow the instructions to install the Java 6 runtime.)
  Note that the Eclipse project excludes Java files that we don't translate (such as Android code).
* Use the menu File->Switch Workspace to open `ndn-dot-net/eclipse`.
* In the Package Explorer on the left, you should see the jndn project. If not, click menu
  File->New->Java Project. Under "Project Name", type "jndn" and click Finish.
* Click menu File->Export. Select Other->ILOG Java to CSharp Translator. Click Next.
* Select the "jndn" project, click Next.
* Under "Renaming Options", set "Namespace naming behavior" and "Class Member naming behavior" to none.
* Under "Translation Destination Directory", click Browse and browse to `ndn-dot-net/src` .
* In the Translate Projects window, Click Finish. The output is in `ndn-dot-net/src/net` .
* (The translator creates an Eclipse project with temporary Java files, for example `translation_ndn-dot-net_Tue_Dec_22_08_27_23_PST_2015`. Delete it.)
* We need to patch errors in the translation process. In a terminal change directory to 
  `ndn-dot-net/src/net` and enter:

    ../patch-files.sh
