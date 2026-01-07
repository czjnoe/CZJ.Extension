using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Models
{
    public class AppConfigOption
    {
        public Compileroptions compilerOptions { get; set; }
        public string[] exclude { get; set; }
    }

    public class Compileroptions
    {
        public bool noImplicitAny { get; set; }
        public bool noEmitOnError { get; set; }
        public bool removeComments { get; set; }
        public bool sourceMap { get; set; }
        public string target { get; set; }
    }
}
