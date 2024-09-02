// See https://aka.ms/new-console-template for more information
using davesave;
using LibForge.Util;

if (args.Length < 4)
{
    Console.WriteLine("davesave decrypt/encrypt ps3/360/ps4/xb1/pc input.bin output.bin");
    return;
}

string encryptVal = args[0].ToLower();
string platformVal = args[1].ToLower();
string inputFile = args[2];
string outputFile = args[3];

bool isBigEndian = platformVal == "ps3" || platformVal == "360";

FileStream instr = File.OpenRead(inputFile);
FileStream outstr = File.OpenWrite(outputFile);

if (encryptVal == "encrypt") {
    EncryptedWriteStream ews = new EncryptedWriteStream(outstr, 0x69696969, isBigEndian ? (byte)0x00 : (byte)0xFF, isBigEndian);
    instr.CopyTo(ews);
} else if (encryptVal == "decrypt") {
    EncryptedReadStream ers = new EncryptedReadStream(instr, isBigEndian ? (byte)0x00 : (byte)0xFF, isBigEndian);
    ers.CopyTo(outstr);
} else {
    Console.WriteLine("must be decrypt or encrypt");
    return;
}

outstr.Close();
instr.Close();
