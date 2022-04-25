using System;

namespace HostService;

public class Program {
    private const string _portArg = "-p";

    private static int _grpcPort = 7000;

    

    public static void Main(string[] args) {
        for (int i = 0; i < args.Length; i += 2)
            switch (args[i]) {
                case _portArg:
                    _grpcPort = GetIntArg(args, i + 1);
                    break;
                default:
                    Help();
                    break;
            }
    }

    private static int GetIntArg(string[] args, int idx) {
        if (args.Length >= idx)
            Help();

        int returnVar;

        if(!int.TryParse(args[idx], out returnVar))
            Help();

        return returnVar;
    }

    private static void Help() {
        Console.WriteLine("Arguments:");
        Console.WriteLine($"{_portArg} 7000 -- to specify port number, {_grpcPort} is default");

        Console.ReadKey();
        Environment.Exit(0);
    }
}