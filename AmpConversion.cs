using davesave.Saves;
using davesave.Saves.Amp2016;
using LibForge.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace davesave
{
    internal class AmpConversion
    {
        public static void ConvertAmpSavePS3toPS4(Stream inStream, Stream outStream, bool encrypt = true)
        {
            Console.WriteLine("Converting PS3 Amplitude save to PS4...");
            // start the stream we're reading from
            Stream realIn = inStream;
            EncryptedReadRevisionStream? ers = null;
            if (encrypt)
            {
                Console.WriteLine("(encryption enabled)");
                ers = new EncryptedReadRevisionStream(inStream, true);
                realIn = ers;
            }
            // load the profile in from the stream
            AmpProfile profile = AmpProfile.ReadFromStream(realIn, true);
            if (ers != null) ers.FinishReading(); // we've finished reading, do this as a sanity check
            // change some values to properly convert
            profile.mVersion = 0x19;
            // start an output stream
            Stream realOut = outStream;
            EncryptedWriteRevisionStream? ews = null;
            if (encrypt)
            {
                ews = new EncryptedWriteRevisionStream(outStream, 0x69696969, false);
                realOut = ews;
            }
            // write our profile to it
            profile.WriteToStream(realOut, false);
            if (ews != null) ews.FinishWriting();
            // conversion successful?
            Console.WriteLine("Conversion completed!");
        }

        public static void ConvertAmpSavePS4toPS3(Stream inStream, Stream outStream, bool encrypt = true)
        {
            Console.WriteLine("Converting PS4 Amplitude save to PS3...");
            // start the stream we're reading from
            Stream realIn = inStream;
            EncryptedReadRevisionStream? ers = null;
            if (encrypt)
            {
                Console.WriteLine("(encryption enabled)");
                ers = new EncryptedReadRevisionStream(inStream, false);
                realIn = ers;
            }
            // load the profile in from the stream
            AmpProfile profile = AmpProfile.ReadFromStream(realIn, false);
            if (ers != null) ers.FinishReading(); // we've finished reading, do this as a sanity check
            // change some values to properly convert
            profile.mVersion = 0x1A;
            profile.mCampaignData.mUnknownPS3Byte = 0x1;
            // start an output stream
            Stream realOut = outStream;
            EncryptedWriteRevisionStream? ews = null;
            if (encrypt)
            {
                ews = new EncryptedWriteRevisionStream(outStream, 0x69696969, true);
                realOut = ews;
            }
            // write our profile to it
            profile.WriteToStream(realOut, true);
            if (ews != null) ews.FinishWriting();
            // conversion successful?
            Console.WriteLine("Conversion completed!");
        }

        public static async Task ConvertAmpSave(string input, string output)
        {
            FileStream inStream = File.OpenRead(input);
            SaveDetection.SaveType type = await SaveDetection.DetectSaveTypeAsync(inStream);

            bool encrypted = false;
            bool ps3tops4 = false;

            if (type == SaveDetection.SaveType.AmpPS4 || type == SaveDetection.SaveType.AmpPS4_Decrypted)
            {
                ps3tops4 = false;
                encrypted = (type == SaveDetection.SaveType.AmpPS4);
            } else if (type == SaveDetection.SaveType.AmpPS3 || type == SaveDetection.SaveType.AmpPS3_Decrypted)
            {
                ps3tops4 = true;
                encrypted = (type == SaveDetection.SaveType.AmpPS3);
            } else
            {
                Console.WriteLine("Unsupported input file.");
                return;
            }

            FileStream outStream = File.OpenWrite(output);
            outStream.SetLength(0);
            if (ps3tops4)
                ConvertAmpSavePS3toPS4(inStream, outStream, encrypted);
            else
                ConvertAmpSavePS4toPS3(inStream, outStream, encrypted);
            inStream.Close();
            outStream.Close();
        }
    }
}
