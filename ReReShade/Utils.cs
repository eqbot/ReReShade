using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReReShade
{
    internal class Utils
    {
        public static void WriteErrorMessageAndPause(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }
}
