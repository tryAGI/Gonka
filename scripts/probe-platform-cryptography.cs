using System.Security.Cryptography;
Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
try { using var key = ECDsa.Create(ECCurve.CreateFromFriendlyName("secp256k1")); Console.WriteLine("secp256k1: supported"); }
catch (Exception e) { Console.WriteLine($"secp256k1: {e.GetType().Name}: {e.Message}"); }
try { using var hash = IncrementalHash.CreateHash(new HashAlgorithmName("RIPEMD160")); hash.AppendData(new byte[] { 1 }); Console.WriteLine($"RIPEMD160: {Convert.ToHexString(hash.GetHashAndReset())}"); }
catch (Exception e) { Console.WriteLine($"RIPEMD160: {e.GetType().Name}: {e.Message}"); }
