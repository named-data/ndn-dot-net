# After running the translator, change directory to `ndn-dot-net/src/net` and enter:
#     ../patch-files.sh
# 
# This performs the following patches:
# Globally remove the conversion date so every file doesn't change.
# Globally capitalize the override methods `equals` and `toString`.
# Globally rename classes Signature and PublicKey in the System namespace which conflict.
# Globally fix the translation to FileReader.
# Globally fix the use of static System.Array.sort .
# Globally put test assertions in the Assert class.
# Fix .length and .parseInt in OID.cs .
# Fix the erroneous @ on strings with a backslash.
# Fix the erroneous translation to SqlCommand.
# Fix the erroneous conversion of BigInteger to Int64.
# Change IllegalBlockSizeException to the C# CryptographicException.'
# Include the namespace for generic IList where needed.
# In Name, added array operator for get(i).
# Remove unused generated enum Extension classes.
# Remove the generated TcpTransport.cs since we use src/tcp-transport.cs .
# Remove the generated Common.cs since we use src/util-common.cs .
# Move unit_tests and integration_tests to the top tests folder.

(unset LANG; find . -type f -exec sed -i '' 's/^\/\/ [0-9][0-9]*\/[0-9][0-9]*\/[0-9][0-9] [0-9][0-9]*:[0-9][0-9] [AP]M *$/\/\//g' {} +)
(unset LANG; find . -type f -exec sed -i '' 's/public override bool equals(Object other)/public override bool Equals(Object other)/g' {} +)
(unset LANG; find . -type f -exec sed -i '' 's/public override String toString()/public override String ToString()/g' {} +)
(unset LANG; find . -type f -exec sed -i '' 's/System\.Signature/System.SecuritySignature/g' {} +)
(unset LANG; find . -type f -exec sed -i '' 's/System\.PublicKey/System.SecurityPublicKey/g' {} +)
(unset LANG; find . -type f -exec sed -i '' 's/BufferedStream writer = new BufferedStream/var writer = /g' {} +)
(unset LANG; find . -type f -exec sed -i '' 's/BufferedStream certFile = new BufferedStream/var certFile = /g' {} +)
(unset LANG; find . -type f -exec sed -i '' 's/System\.Array\.sort/System.Array.Sort/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertArrayEquals(/Assert.AssertArrayEquals(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertEquals(/Assert.AssertEquals(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertFalse(/Assert.AssertFalse(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertTrue(/Assert.AssertTrue(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertNotNull(/Assert.AssertNotNull(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertNotSame(/Assert.AssertNotSame(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertNull(/Assert.AssertNull(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/AssertSame(/Assert.AssertSame(/g' {} +)
(unset LANG; find . -name "Test*.cs" -exec sed -i '' 's/Fail(/Assert.Fail(/g' {} +)
sed -i '' 's/\.length/.Length/g' named_data/jndn/encoding/OID.cs
sed -i '' 's/\.parseInt/.Parse/g' named_data/jndn/encoding/OID.cs
sed -i '' 's/@"\\0"/"\\0"/g' named_data/jndn/util/BoostInfoTree.cs
sed -i '' 's/@"/"/g' named_data/jndn/tests/unit_tests/TestRegex.cs
sed -i '' 's/@"/"/g' named_data/jndn/security/policy/ConfigPolicyManager.cs
sed -i '' 's/SqlCommand/Statement/g' named_data/jndn/security/identity/BasicIdentityStorage.cs
sed -i '' 's/SqlCommand/Statement/g' named_data/jndn/encrypt/Sqlite3*Db.cs
sed -i '' 's/new Int64//g' named_data/jndn/encrypt/algo/RsaAlgorithm.cs
sed -i '' 's/IllegalBlockSizeException/System.Security.Cryptography.CryptographicException/g' named_data/jndn/encrypt/algo/Encryptor.cs
sed -i '' 's/IList/System.Collections.Generic.IList/g' named_data/jndn/util/regex/NdnRegexMatcherBase.cs
sed -i '' 's/public void set/public Name.Component this[int i] { get { return get(i); } }  public void set/g' named_data/jndn/Name.cs
rm named_data/jndn/*Extension.cs named_data/jndn/encrypt/*Extension.cs named_data/jndn/encrypt/algo/*Extension.cs
rm named_data/jndn/security/*Extension.cs named_data/jndn/util/*Extension.cs
rm named_data/jndn/transport/TcpTransport.cs
rm named_data/jndn/util/Common.cs
rm -rf ../../tests/unit_tests
mv named_data/jndn/tests/unit_tests ../../tests
rm -rf ../../tests/integration_tests
mv named_data/jndn/tests/integration_tests/ ../../tests
