using LibForge.Util;

namespace davesave.Saves
{
    public class SaveDetection
    {
        public enum SaveType
        {
            // Not a Harmonix RevisionStream or any save container type
            NotASave,
            // Harmonix game that we don't support
            Unsupported,
            // Harmonix game of some kind, but detection failed
            Failed,

            // Various Sony encryption layers people might try to provide
            PS4SaveContainer,
            PS4SealedKey,

            // Amplitude (2016)
            AmpPS4,
            AmpPS3,
            AmpPS4_Decrypted,
            AmpPS3_Decrypted,

            // Rock Band 4
            RB4Xbox,
            RB4XboxSystemOptions,
            RB4PS4,
            RB4Xbox_Decrypted,
            RB4XboxSystemOptions_Decrypted,
            RB4PS4_Decrypted,
        }

        public static SaveType DetectSaveType(byte[] readBuffer)
        {
            if (readBuffer.Length < 0x20)
                throw new Exception("Not enough data to detect save type.");
            if (readBuffer[0] == 0x7A)
            {
                // Harmonix RevisionStream
                uint revision = BitConverter.ToUInt32(readBuffer, 1);

                // Amplitude is revision 0x01
                // we detect PS3 vs PS4 based on the revision being stored as big endian
                if (revision == 0x01)
                    return SaveType.AmpPS4_Decrypted;
                else if (revision == 0x01000000) // big endian
                    return SaveType.AmpPS3_Decrypted;
                // Rock Band 4 is revision 0x19
                else if (revision == 0x19)
                {
                    // we detect PS4 or Xbox based on what comes after the RevisionStream header
                    // the PS4 has the RBSystemOptions embedded at the start of the file
                    if (readBuffer[5] == 0x7A && BitConverter.ToUInt32(readBuffer, 6) == 0x1C)
                        return SaveType.RB4PS4_Decrypted;
                    else
                        return SaveType.RB4Xbox_Decrypted;
                }
                // RBSystemOptions standalone is revision 0x1C
                else if (revision == 0x1C)
                    return SaveType.RB4XboxSystemOptions_Decrypted;
                // Encrypted RevisionStreams are always revision 0x00
                else if (revision == 0x00)
                {
                    MemoryStream ms = new MemoryStream(readBuffer);
                    byte[] encReadBuffer = new byte[0x10];
                    // attempt to read as a little endian RevisionStream
                    EncryptedReadRevisionStream ers = new EncryptedReadRevisionStream(ms, false);
                    ers.Read(encReadBuffer, 0, 0x10);
                    // if this fails, try big endian
                    if (encReadBuffer[0] != 0x7A)
                    {
                        ms.Position = 0;
                        ers = new EncryptedReadRevisionStream(ms, true);
                        ers.Read(encReadBuffer, 0, 0x10);
                        // bail out, we can't decrypt this
                        if (encReadBuffer[0] != 0x7A)
                            return SaveType.Failed;
                    }
                    uint revision_enc = BitConverter.ToUInt32(encReadBuffer, 1);
                    // Amplitude is revision 0x01
                    // we detect PS3 vs PS4 based on the revision being stored as big endian
                    if (revision_enc == 0x01)
                        return SaveType.AmpPS4;
                    else if (revision_enc == 0x01000000) // big endian
                        return SaveType.AmpPS3;
                    // Rock Band 4 is revision 0x19
                    else if (revision_enc == 0x19)
                    {
                        // we detect PS4 or Xbox based on what comes after the RevisionStream header
                        // the PS4 has the RBSystemOptions embedded at the start of the file
                        if (encReadBuffer[5] == 0x7A && BitConverter.ToUInt32(encReadBuffer, 6) == 0x1C)
                            return SaveType.RB4PS4;
                        else
                            return SaveType.RB4Xbox;
                    }
                    // RBSystemOptions standalone is revision 0x1C
                    else if (revision_enc == 0x1C)
                        return SaveType.RB4XboxSystemOptions;
                    else
                        return SaveType.Unsupported;
                }
                else
                    return SaveType.Unsupported;
            }
            else if (readBuffer[0] == 0x01 && BitConverter.ToInt32(readBuffer, 8) == 20130315)
            {
                // Encrypted PS4 save container / PFS file, we can't decrypt these
                return SaveType.PS4SaveContainer;
            }
            else if (BitConverter.ToUInt64(readBuffer, 0) == 0x79654B4B53736670) // pfsSKKey
            {
                // PS4 PSF sealed key, we can't do anything with these at the moment
                return SaveType.PS4SealedKey;
            }
            // TODO: read encrypted PS3 saves
            return SaveType.NotASave;
        }

        public static async Task<SaveType> DetectSaveTypeAsync(Stream stream)
        {
            // read the file header
            long startPos = stream.Position;
            byte[] readBuffer = new byte[0x20];
            await stream.ReadAsync(readBuffer, 0, 0x20);
            stream.Position = startPos;

            return DetectSaveType(readBuffer);
        }
    }
}
