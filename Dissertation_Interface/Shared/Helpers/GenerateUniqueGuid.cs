namespace Shared.Helpers;

public static class GenerateUniqueGuid
{
    private static readonly object GuidLock = new();

    public static Guid Generate()
    {
        lock (GuidLock) // Ensure thread safety using a lock
        {
            // Generate a random GUID
            var randomGuid = Guid.NewGuid();

            // Get the current timestamp as a byte array
            var timestampBytes = BitConverter.GetBytes(DateTime.UtcNow.Ticks);

            // If BitConverter.IsLittleEndian is true, reverse the byte order
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timestampBytes);

            // XOR the first 8 bytes of the random GUID with the timestamp bytes
            var modifiedGuidBytes = new byte[16];
            Buffer.BlockCopy(randomGuid.ToByteArray(), 0, modifiedGuidBytes, 0, 8);
            for (var i = 0; i < 8; i++)
                modifiedGuidBytes[i] ^= timestampBytes[i];

            // Create a new GUID from the modified byte array
            var uniqueGuid = new Guid(modifiedGuidBytes);

            return uniqueGuid;
        }
    }
}