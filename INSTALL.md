NDN-DOT-NET:  A Named Data Networking client library for the .NET Framework
===========================================================================

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
  files from `jndn/src/net` .
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
* We need to globally capitalize the override methods `equals` and `toString`, 
  rename classes Signathre and PublicKey in the System namespace which conflict,
  fix the use of DateTime.Now.Millisecond, fix .length and .parseInt in OID.cs,
  fix the erronous translation to @"\0",
  remove the generated TcpTransport.cs since we use src/tcp-transport.cs:

In a terminal change directory to `ndn-dot-net/src/net` and enter:

    (unset LANG; find . -type f -exec sed -i '' 's/public override bool equals(Object other)/public override bool Equals(Object other)/g' {} +)
    (unset LANG; find . -type f -exec sed -i '' 's/public override String toString()/public override String ToString()/g' {} +)
    (unset LANG; find . -type f -exec sed -i '' 's/System\.Signature/System.SecuritySignature/g' {} +)
    (unset LANG; find . -type f -exec sed -i '' 's/System\.PublicKey/System.SecurityPublicKey/g' {} +)
    sed -i '' 's/DateTime\.Now\.Millisecond/(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds/g' named_data/jndn/util/Common.cs
    sed -i '' 's/\.length/.Length/g' named_data/jndn/encoding/OID.cs
    sed -i '' 's/\.parseInt/.Parse/g' named_data/jndn/encoding/OID.cs
    sed -i '' 's/@"\\0"/"\\0"/g' named_data/jndn/util/BoostInfoTree.cs
    rm named_data/jndn/transport/TcpTransport.cs
