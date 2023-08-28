using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat4osu
{
    public class Logger
    {
        public String data;
        public void Log(String text)
        {
            data = text;
        }
    }
}
