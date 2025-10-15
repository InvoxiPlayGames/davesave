namespace davesave.Saves.Amp2016
{
    public class AmpCampaignScore
    {
#pragma warning disable CS8618
        public ushort mScore;
        public byte mZero;
        public byte mDifficulty;
#pragma warning restore CS8618

        public void WriteToStream(Stream stream, bool isBE = false)
        {
            if (isBE)
                stream.WriteUInt16BE(mScore);
            else
                stream.WriteUInt16LE(mScore);
            stream.WriteUInt8(mZero);
            stream.WriteUInt8(mDifficulty);
        }

        public static AmpCampaignScore ReadFromStream(Stream stream, bool isBE = false)
        {
            AmpCampaignScore score = new();

            if (isBE)
                score.mScore = stream.ReadUInt16BE();
            else
                score.mScore = stream.ReadUInt16LE();
            score.mZero = stream.ReadUInt8();
            score.mDifficulty = stream.ReadUInt8();

            return score;
        }
    }

    public class AmpCampaignPersistentData
    {
#pragma warning disable CS8618
        public byte mUnknown1; // possibly bool mCompletedCampaign or bool mStartedCampaign?
        public byte mUnknown2;
        public byte mUnknownPS3Byte; // only exists in PS3 saves, 0x01?
        public uint mUnknown3; // 127 on a complete save, 0 on fresh
        public byte mUnknown4; // 0 on both
        public byte mUnknown5; // 255 on a complete save, 2 on fresh
        public byte mUnknown6;
        public string mNextSong;
        public byte mSongIndex;
        public byte mUnknown7; // always 0xFF
        public AmpCampaignScore[] mScores;
#pragma warning restore CS8618

        public void WriteToStream(Stream stream, bool isPS3 = false)
        {
            stream.WriteUInt8(mUnknown1);
            stream.WriteUInt8(mUnknown2);
            if (isPS3)
                stream.WriteUInt8(mUnknownPS3Byte);
            if (isPS3)
                stream.WriteUInt32BE(mUnknown3);
            else
                stream.WriteUInt32LE(mUnknown3);
            stream.WriteUInt8(mUnknown4);
            stream.WriteUInt8(mUnknown5);
            stream.WriteUInt8(mUnknown6);
            stream.WriteLengthUTF8(mNextSong, isPS3);
            stream.WriteUInt8(mSongIndex);
            stream.WriteUInt8(mUnknown7);
            for (int i = 0; i < 15; i++)
                mScores[i].WriteToStream(stream, isPS3);
        }

        public static AmpCampaignPersistentData ReadFromStream(Stream stream, bool isPS3 = false)
        {
            AmpCampaignPersistentData campaign = new();

            campaign.mUnknown1 = stream.ReadUInt8();
            campaign.mUnknown2 = stream.ReadUInt8();
            if (isPS3)
                campaign.mUnknownPS3Byte = stream.ReadUInt8();
            if (isPS3)
                campaign.mUnknown3 = stream.ReadUInt32BE();
            else
                campaign.mUnknown3 = stream.ReadUInt32LE();
            campaign.mUnknown4 = stream.ReadUInt8();
            campaign.mUnknown5 = stream.ReadUInt8();
            campaign.mUnknown6 = stream.ReadUInt8();
            campaign.mNextSong = stream.ReadLengthUTF8(isPS3);
            campaign.mSongIndex = stream.ReadUInt8();
            campaign.mUnknown7 = stream.ReadUInt8();
            // scores are hardcoded to 15
            campaign.mScores = new AmpCampaignScore[15];
            for (int i = 0; i < 15; i++)
                campaign.mScores[i] = AmpCampaignScore.ReadFromStream(stream, isPS3);

            return campaign;
        }
    }
}
