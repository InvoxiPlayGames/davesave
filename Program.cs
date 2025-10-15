using davesave;
using davesave.Saves;
using LibForge.Util;

static void PrintUsage(bool showInfo = true)
{
    if (showInfo)
    {
        Console.WriteLine("davesave - 2024-2025 Emma / InvoxiPlayGames");
        Console.WriteLine("  uses parts of DtxCS and LibForge by maxton");
        Console.WriteLine("  https://github.com/InvoxiPlayGames/davesave");
    }
    Console.WriteLine();
    Console.WriteLine("available commands: info, decrypt, encrypt");
    Console.WriteLine("supported games: Amplitude 2016 (1.0/1.1), Rock Band 4 (2.3.7)");
    Console.WriteLine();
    Console.WriteLine("- davesave info [filename]");
    Console.WriteLine("    will display savegame info from the file provided");
    Console.WriteLine();
    Console.WriteLine("- davesave decrypt [input file] [output file] (optional: ps3/360/ps4/xb1/pc)");
    Console.WriteLine("    will decrypt the encrypted save from the input file into the output");
    Console.WriteLine("    platform must be provided if working with an unsupported game");
    Console.WriteLine();
    Console.WriteLine("- davesave encrypt [input file] [output file] (optional: ps3/360/ps4/xb1/pc)");
    Console.WriteLine("    will encrypt a decrypted save from the input file into the output");
    Console.WriteLine("    platform must be provided if working with an unsupported game");
    Console.WriteLine();
}

if (args.Length < 1)
{
    PrintUsage(true);
    return;
}

string verb = args[0].ToLower();

if (verb == "help" || verb == "about" || verb == "-h" || verb == "/?")
{
    PrintUsage(true);
    return;
}
else if (verb == "info")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Error: no filename provided.");
        PrintUsage(false);
        return;
    }
    await InfoPrint.SaveInfo(args[1]);
    return;
}
else if (verb == "decrypt")
{
    if (args.Length < 3)
    {
        Console.WriteLine("Error: missing arguments.");
        PrintUsage(false);
        return;
    }
    FileStream inStream = File.OpenRead(args[1]);
    SaveDetection.SaveType type = await SaveDetection.DetectSaveTypeAsync(inStream);
    string platform = InfoPrint.SaveDescriptions[type];
    bool isBigEndian = false;
    if (type == SaveDetection.SaveType.AmpPS3 || type == SaveDetection.SaveType.AmpPS3_Decrypted)
    {
        isBigEndian = true;
    } else if (type == SaveDetection.SaveType.NotASave || type == SaveDetection.SaveType.Unsupported ||
        type == SaveDetection.SaveType.Failed)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Game type couldn't be detected and no platform was provided.");
            PrintUsage(false);
            return;
        }
        platform = args[3].ToLower();
        isBigEndian = platform == "ps3" || platform == "360";
        if (!isBigEndian && platform != "xb1" && platform != "ps4" && platform != "pc")
        {
            Console.WriteLine("Invalid platform provided (valid options are: xb1, 360, ps3, ps4, pc)");
            PrintUsage(false);
            return;
        }
    }
    Console.WriteLine("Decrypting {0} endian {1} save...", isBigEndian ? "big" : "little", platform);

    FileStream outStream = File.OpenWrite(args[2]);
    EncryptedReadRevisionStream ers = new EncryptedReadRevisionStream(inStream, isBigEndian);
    ers.CopyTo(outStream);
    outStream.Close();
    ers.FinishReading();
    ers.Close();
    return;
}
else if (verb == "encrypt")
{
    if (args.Length < 3)
    {
        Console.WriteLine("Error: missing arguments.");
        PrintUsage(false);
        return;
    }
    FileStream inStream = File.OpenRead(args[1]);
    SaveDetection.SaveType type = await SaveDetection.DetectSaveTypeAsync(inStream);
    bool isBigEndian = false;
    string platform = InfoPrint.SaveDescriptions[type];
    if (type == SaveDetection.SaveType.AmpPS3 || type == SaveDetection.SaveType.AmpPS3_Decrypted)
    {
        isBigEndian = true;
    }
    else if (type == SaveDetection.SaveType.NotASave || type == SaveDetection.SaveType.Unsupported ||
        type == SaveDetection.SaveType.Failed)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Game type couldn't be detected and no platform was provided.");
            PrintUsage(false);
            return;
        }
        platform = args[3].ToLower();
        isBigEndian = platform == "ps3" || platform == "360";
        if (!isBigEndian && platform != "xb1" && platform != "ps4" && platform != "pc")
        {
            Console.WriteLine("Invalid platform provided (valid options are: xb1, 360, ps3, ps4, pc)");
            PrintUsage(false);
            return;
        }
    }
    Console.WriteLine("Encrypting {0} endian {1} save...", isBigEndian ? "big" : "little", platform);

    FileStream outStream = File.OpenWrite(args[2]);
    EncryptedWriteRevisionStream ews = new EncryptedWriteRevisionStream(outStream, 0x69696969, isBigEndian);
    inStream.CopyTo(ews);
    inStream.Close();
    ews.FinishWriting();
    ews.Close();
    outStream.Close();
    return;
}
else if (verb == "convert")
{
    if (args.Length < 3)
    {
        Console.WriteLine("Error: not enough filenames provided.");
        PrintUsage(false);
        return;
    }
    await AmpConversion.ConvertAmpSave(args[1], args[2]);
    return;
}
else
{
    Console.WriteLine("Error: unknown command");
    PrintUsage(false);
    return;
}