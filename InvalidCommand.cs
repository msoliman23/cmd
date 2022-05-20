using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSProject
{
    class InvalidCommand : Exception
    {
        public InvalidCommand()
            : base("Invalid Command!!!\n")
        {
        }
        public InvalidCommand(string str)
            : base(str)
        {
        }
    }
}
