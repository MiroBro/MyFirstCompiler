// See https://aka.ms/new-console-template for more information
using System.Diagnostics;


class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Here we go compiling!");
            Console.ForegroundColor = ConsoleColor.White;

            var ast = new Compiler();
            ast.Run(args[0]);
        }
        else 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Where's the argument? You have to have at least one expression for me to compile!");
            Console.ForegroundColor = ConsoleColor.White;
        }


    }

}