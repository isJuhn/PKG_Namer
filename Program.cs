using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace PKG_Namer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] { Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) };
            }

            if (args[0].Remove(0, args[0].Length - 4).ToLower() == ".pkg")
            {
                rename_pkg(args[0]);
            }
            else
            {
                string[] files = Directory.GetFiles(args[0]);
                foreach (string file in files)
                {
                    if (file.Remove(0, file.Length - 4).ToLower() == ".pkg")
                    {
                        if (!rename_pkg(file))
                        {
                            Console.WriteLine($"Failed to rename {file}");
                        }
                    }
                }
            }
        }

        static bool rename_pkg(string filepath)
        {
            string name = "";
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(0x37, SeekOrigin.Begin);
                byte[] buffer = new byte[0x24];
                fs.Read(buffer, 0, 0x9);
                fs.Seek(0x43, SeekOrigin.Begin);
                fs.Read(buffer, 0x9, 0x11);
                name = Encoding.UTF8.GetString(buffer).Trim('\0') + ".pkg";
                Console.WriteLine(name);
            }
            try
            {
                File.Move(filepath, filepath.Substring(0, filepath.LastIndexOf('/') + 1) + name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
    }
}
