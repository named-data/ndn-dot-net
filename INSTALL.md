NDN-DOT-NET:  A Named Data Networking client library for the .NET Framework
===========================================================================

Java to C# Translation Prerequisites
====================================
These steps are only needed to do the translation from jNDN Java files to C#
(when jNDN is updated).

## Mac OS X 10.10.2 and Mac OS X 10.11

* Download and unzip Eclipse 3.6.2 (Helios SR2) for Mac Cocoa 64-bit from
  http://www.eclipse.org/downloads/download.php?file=/technology/epp/downloads/release/helios/SR2/eclipse-java-helios-SR2-macosx-cocoa-x86_64.tar.gz .
* Download the j2cstranslator plugin for Eclipse 3.6.0 from
  http://sourceforge.net/projects/j2cstranslator/files/j2cstranslator/Eclipse3.6/1.3.6.2001_05_19_01/
* Put the j2cstranslator plugin jar file in eclipse/plugins .

Java to C# Translation
======================
* Remove the old C# files so we can rebuild. In a terminal, change directory to
  the NDN-DOT-NET root and enter:

    rm -rf src/cs/*

* Start Eclipse 3.6.0. (If necessary, follow the instructions to install the Java 6 runtime.)
* Use the menu File->Switch Workspace to open ndn-dot-net/eclipse.
* In the Package Explorer on the left, you should see the jndn project. If not, click menu
  File->New->Java Project. Under "Project Name", type "jndn" and click Finish.
* (The project jndn already has a snapshot of the jNDN Java files.)
* Click menu File->Export. Select Other->ILOG Java to CSharp Translator. Click Next.
* Select the "jndn" project, click Next.
* Under "Renaming Options", set "Namespace naming behavior" and "Class Member naming behavior" to none.
* Under "Translation Destination Directory", click Browse and browse to ndn-dot-net/src/cs .
* Click Finish. The output is in ndn-dot-net/src/cs .
* (The translator creates an Eclipse project with temporary Java files, for example "translation_ndn-dot-net_Tue_Dec_22_08_27_23_PST_2015". Delete it.)
