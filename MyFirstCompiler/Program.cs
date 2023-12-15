// See https://aka.ms/new-console-template for more information
using System.Diagnostics;


class Program
{
    static void Main(string[] args)
    {
        int rax = 123;
        int messageAdress = 3141;
        bool loop = true;
        char[] toPrint = new char[10];
        //int[] toPrint = new int[10];
        int rdi = /* messageAdress */ + toPrint.Length - 1;
        while (loop)
        {
            int rdx = rax % 10;
            rax = rax / 10;
            toPrint[rdi] = (char) (0x30 + rdx);
            //toPrint[rdi] = rdx;
            rdi--;

            if (rax == 0 )
            {
                loop = false;
            }
        }

        foreach (char c in toPrint)
        {
            Console.Write(c);
        }
        Console.Write("\n");


















        if (args.Length > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Here we go compiling!");
            Console.ForegroundColor = ConsoleColor.White;

            var compiler = new Compiler();
            compiler.Run(args[0]);
        }
        else 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Where's the argument? You have to have at least one expression for me to compile!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}