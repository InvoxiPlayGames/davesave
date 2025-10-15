﻿namespace davesave.Saves.Amp2016
{
    public class AmpProfileFooter
    {
#pragma warning disable CS8618
        public byte mUnknown1; // 0x6
        public byte mUnknown2; // 0x1
        public int mUnknown3;
#pragma warning restore CS8618

        public void WriteToStream(Stream stream, bool isBE = false)
        {
            stream.WriteUInt8(mUnknown1);
            stream.WriteUInt8(mUnknown2);
            if (isBE)
                stream.WriteUInt32BE((uint)mUnknown3);
            else
                stream.WriteInt32LE(mUnknown3);
        }

        public static AmpProfileFooter ReadFromStream(Stream stream, bool isBE = false)
        {
            AmpProfileFooter footer = new();

            footer.mUnknown1 = stream.ReadUInt8();
            footer.mUnknown2 = stream.ReadUInt8();
            if (isBE)
                footer.mUnknown3 = stream.ReadInt32BE();
            else
                footer.mUnknown3 = stream.ReadInt32LE();

            return footer;
        }
    }

    public class AmpProfile
    {
#pragma warning disable CS8618
        public byte mVersion; // 0x19 on PS4, 0x1A on PS3
        public string mUnused; // always empty
        public AmpSongPersistentData[] mSongData;
        public AmpSystemOptionsPersistentData mOptions;
        public byte mUnknown; // always 0?
        public string[] mUnlocks;
        public AmpCampaignPersistentData mCampaignData;
        public AmpProfileFooter mFooter;
#pragma warning restore CS8618

        public void WriteToStream(Stream stream, bool isPS3 = false)
        {
            RevisionStream rev = new RevisionStream(stream, 0x1, isPS3);

            rev.WriteUInt8(mVersion);
            rev.WriteLengthUTF8(mUnused, isPS3);
            if (isPS3)
                rev.WriteUInt32BE((uint)mSongData.Length);
            else
                rev.WriteInt32LE(mSongData.Length);
            for (int i = 0; i < mSongData.Length; i++)
                mSongData[i].WriteToStream(rev, isPS3);

            mOptions.WriteToStream(rev, isPS3);
            rev.WriteUInt8(mUnknown);

            if (isPS3)
                rev.WriteUInt32BE((uint)mUnlocks.Length);
            else
                rev.WriteInt32LE(mUnlocks.Length);
            for (int i = 0; i < mUnlocks.Length; i++)
                rev.WriteLengthUTF8(mUnlocks[i], isPS3);

            mCampaignData.WriteToStream(rev, isPS3);
            mFooter.WriteToStream(rev, isPS3);

            // PS3 pads the save file
            if (isPS3)
            {
                // TODO: is this correct?
                long numPadding = 0x7FFE - (rev.Position) - 5 - 4;
                byte[] bytesToWrite = new byte[numPadding];
                rev.Write(bytesToWrite, 0, (int)numPadding);
            }

            rev.FinishWriting();
        }

        public static AmpProfile ReadFromStream(Stream stream, bool isPS3 = false)
        {
            AmpProfile profile = new();
            RevisionStream rev = new RevisionStream(stream, 0x1, 0x1, isPS3);

            profile.mVersion = rev.ReadUInt8();
            profile.mUnused = rev.ReadLengthUTF8(isPS3);

            if (isPS3)
                profile.mSongData = new AmpSongPersistentData[rev.ReadInt32BE()];
            else
                profile.mSongData = new AmpSongPersistentData[rev.ReadInt32LE()];
            for (int i = 0; i < profile.mSongData.Length; i++)
                profile.mSongData[i] = AmpSongPersistentData.ReadFromStream(rev, isPS3);

            profile.mOptions = AmpSystemOptionsPersistentData.ReadFromStream(rev, isPS3);
            profile.mUnknown = rev.ReadUInt8();

            if (isPS3)
                profile.mUnlocks = new string[rev.ReadInt32BE()];
            else
                profile.mUnlocks = new string[rev.ReadInt32LE()];
            for (int i = 0; i < profile.mUnlocks.Length; i++)
                profile.mUnlocks[i] = rev.ReadLengthUTF8(isPS3);

            profile.mCampaignData = AmpCampaignPersistentData.ReadFromStream(rev, isPS3);
            profile.mFooter = AmpProfileFooter.ReadFromStream(rev, isPS3);

            // PS3 pads the save file
            if (isPS3)
            {
                // TODO: is this correct?
                long numPadding = 0x7FFE - (rev.Position) - 5 - 4;
                rev.Position += numPadding;
            }

            rev.FinishReading();
            return profile;
        }
    }
}
