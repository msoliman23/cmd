using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Prompt pmpt = new Prompt();
            Command cmd;
          
            while(true)
            {
                cmd = new Command(pmpt);
                Console.Write(pmpt);
                string input = Console.ReadLine();
                try
                {
                    cmd.Parse(input);
                }
                catch(InvalidCommand ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if ( cmd.Name.ToLower()=="exit")
                    break;
            }
        }
    }
}
