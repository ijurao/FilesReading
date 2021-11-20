using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadingFiles
{
    class Program
    {
         static string fileContent = "";

        static async Task Main(string[] args)
        {
            //Console.WriteLine("Creating Files...");
            //CreateFiles();
            //Console.WriteLine("Creation Files FINISHED...");
          //  MergeFilesSequencial();
          await MergeFilesParallel();
         // MergeFilesParallel2();
            Console.WriteLine("Hello World!");
        }


        private static  void MergeFilesParallel2()
        {
            Stopwatch s = new Stopwatch();
            Console.WriteLine("Reading files parallel2222...");
            s.Start();
            string path = @"C:\TestFiles\";
            List<Task> readers = new List<Task>();
            object o = new object();
            Parallel.ForEach(Directory.GetFiles(path), file =>
             {
                 lock(o)
                   fileContent += System.IO.File.ReadAllText(file); //+ Environment.NewLine;
                 
             });
     
            string fileName = @"C:\TestFiles\mergedParalelUsingPLINQ.txt";
            using (FileStream fs = File.Create(fileName))
            {
                // Add some text to file    
                Byte[] content = new UTF8Encoding(true).GetBytes(fileContent.ToString());
                fs.Write(content, 0, content.Length);
         

            }

            s.Stop();
            Console.WriteLine("End Paralell2 - Lasted " + (s.ElapsedMilliseconds).ToString() + " miliseconds");


        }

        private static async Task MergeFilesParallel()
        {
            Stopwatch s = new Stopwatch();
            Console.WriteLine("Reading files parallel...");
            s.Start();
            string path = @"C:\TestFiles\";
            string fileContent = "";
            List<Task> readers = new List<Task>();
            foreach (var file in Directory.GetFiles(path))
            {
                // fileContent += System.IO.File.ReadAllText(file);
                Task reader = GetTaskReader(file);
                readers.Add(reader);
            }
            foreach (var t in readers)
            {
                t.Start();
            }
             await Task.WhenAll(readers);
            foreach (var task in readers)
            {
                
                var result = ((Task<string>)task).Result;
                fileContent += result;
            }
            string fileName = @"C:\TestFiles\mergedParalel.txt";
            using (FileStream fs = File.Create(fileName))
            {
                // Add some text to file    
                Byte[] content = new UTF8Encoding(true).GetBytes(fileContent.ToString());
                fs.Write(content, 0, content.Length);

            }

            s.Stop();
            Console.WriteLine("End Paralell - Lasted " + (s.ElapsedMilliseconds).ToString() + " miliseconds");


        }

        private static Task<string> GetTaskReader(string file)
        {
            return new Task<string>( () =>  File.ReadAllText(file) );
        }

        private static void MergeFilesSequencial()
        {
            Stopwatch s = new Stopwatch();
            Console.WriteLine("Reading files sequenciali...");
            s.Start();
            string path = @"C:\TestFiles\";
            string fileContent = "";
            foreach(var file in Directory.GetFiles(path))
            {
                 fileContent += System.IO.File.ReadAllText(file) + Environment.NewLine;
            }

            //create merged file
            string fileName = @"C:\Seq\mergedSeq.txt";
            using (FileStream fs = File.Create(fileName))
            {
                // Add some text to file    
                Byte[] content = new UTF8Encoding(true).GetBytes(fileContent.ToString());
                fs.Write(content, 0, content.Length);

            }

            s.Stop();
            Console.WriteLine("End Sequencial - Lasted " + (s.ElapsedMilliseconds).ToString() + " miliseconds");
        }

        private static void CreateFiles()
        {
            string path = @"C:\TestFiles\";
            int amountOfFiles = 500;
            int linesPerFile = 100;
            for (int i = 0; i < amountOfFiles; i++)
            {
                string fileName = string.Empty;
                var fileContent = new StringBuilder();
                for (int j = 0; j < linesPerFile; j++)
                {
                    fileContent.Append(i.ToString() + Environment.NewLine);
                }

                fileName = String.Format("{0}{1}{2}.txt", path, "TestFile", i.ToString());
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file    
                    Byte[] content = new UTF8Encoding(true).GetBytes(fileContent.ToString());
                    fs.Write(content, 0, content.Length);
                   
                }
            }
           
        }
    }
}
