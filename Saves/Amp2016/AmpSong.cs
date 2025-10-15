﻿namespace davesave.Saves.Amp2016
{
    public class AmpSongPersistentData
    {
#pragma warning disable CS8618
        public string mSongName;
        public ushort mScore;
        public byte mDifficulty;
        public byte mHighestStreak; // always 0 on co-op scores
        public byte mPercentCleared; // always 0 on co-op scores
        public byte mScoreType; // 1 for singleplayer, 2 for co-op?
        public byte mProgress; // always 100
#pragma warning restore CS8618

        public void WriteToStream(Stream stream, bool isBE = false)
        {
            stream.WriteLengthUTF8(mSongName, isBE);
            if (isBE)
                stream.WriteUInt16BE(mScore);
            else
                stream.WriteUInt16LE(mScore);
            stream.WriteUInt8(mDifficulty);
            stream.WriteUInt8(mHighestStreak);
            stream.WriteUInt8(mPercentCleared);
            stream.WriteUInt8(mScoreType);
            stream.WriteUInt8(mProgress);
        }

        public static AmpSongPersistentData ReadFromStream(Stream stream, bool isBE = false)
        {
            AmpSongPersistentData song = new();

            song.mSongName = stream.ReadLengthUTF8(isBE);
            if (isBE)
                song.mScore = stream.ReadUInt16BE();
            else
                song.mScore = stream.ReadUInt16LE();
            song.mDifficulty = stream.ReadUInt8();
            song.mHighestStreak = stream.ReadUInt8();
            song.mPercentCleared = stream.ReadUInt8();
            song.mScoreType = stream.ReadUInt8();
            song.mProgress = stream.ReadUInt8();

            return song;
        }
    }
}
