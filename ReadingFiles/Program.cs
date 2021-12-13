using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadingFiles
{
    class Program
    {
         

        static async Task Main(string[] args)
        {
            //Console.WriteLine("Creating Files...");
          // CreateFiles();
            //Console.WriteLine("Creation Files FINISHED...");
            //  MergeFilesSequencial();
            // await MergeFilesParallel();
           MergeFilesParallel2();
            Console.WriteLine("Hello World!");
        }


        private static void MergeFilesParallel2()
        {
            Stopwatch s = new Stopwatch();
            Console.WriteLine("Reading files parallel by cores...");
            s.Start();
            string path = @"C:\TestFiles\";
            List<Task> readers = new List<Task>();
            object o = new object();
            string[] fileContent = new string[Directory.GetFiles(path).Length];
            var options = new ParallelOptions { MaxDegreeOfParallelism = -1 };
            Parallel.ForEach(Directory.GetFiles(path).AsParallel(), options,(file,state,i) =>
             {
                 // Console.WriteLine(i);
             
                    var localContent = System.IO.File.ReadAllText(file);
                    fileContent[i] += localContent; //+ Environment.NewLine;
                 
             });
            string finalContent = string.Empty;

            foreach (var item in fileContent)
            {
                finalContent += item;
            }
     
            string fileName = @"C:\TestFiles\mergedParalelUsingPLINQ.txt";
            using (FileStream fs = File.Create(fileName))
            {
                // Add some text to file    
                Byte[] content = new UTF8Encoding(true).GetBytes(finalContent);
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
                //Task reader = ; //GetTaskReader(file);
                readers.Add(Task.Run(() => File.ReadAllText(file)));
            }
            //foreach (var t in readers)
            //{
            //    t.Start();
            //}
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
            int amountOfFiles = 3000;
            int linesPerFile = 10000;
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
