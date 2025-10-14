namespace davesave.Saves.RB4
{
    public class RBSetlist
    {
#pragma warning disable CS8618
        public string mName;
        public float mRuntimeMs;
        public int[] mSongList;
        public bool mShuffle;
        public HxGuid? mGuid;
#pragma warning restore CS8618

        public static RBSetlist ReadFromStream(Stream stream)
        {
            RBSetlist setlist = new();
            RevisionStream rev = new RevisionStream(stream, 0x3, 0x3);

            setlist.mName = rev.ReadLengthUTF8();
            setlist.mRuntimeMs = rev.ReadFloat();
            setlist.mSongList = new int[rev.ReadInt32LE()];
            for (int i = 0; i < setlist.mSongList.Length; i++)
                setlist.mSongList[i] = rev.ReadInt32LE();
            setlist.mShuffle = rev.ReadUInt8() != 0x00;
            setlist.mGuid = HxGuid.ReadFromStream(rev);

            rev.FinishReading();
            return setlist;
        }
    }

    public class RBSetlistCollection
    {
#pragma warning disable CS8618
        public RBSetlist[] mSetlists;
#pragma warning restore CS8618

        public static RBSetlistCollection ReadFromStream(Stream stream)
        {
            RBSetlistCollection collection = new RBSetlistCollection();
            RevisionStream rev = new RevisionStream(stream, 0x0, 0x0);

            collection.mSetlists = new RBSetlist[rev.ReadInt32LE()];
            for (int i = 0; i < collection.mSetlists.Length; i++)
                collection.mSetlists[i] = RBSetlist.ReadFromStream(rev);

            rev.FinishReading();
            return collection;
        }
    }
}
