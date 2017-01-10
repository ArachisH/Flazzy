using System;
using System.IO;
using System.Collections.Generic;

using Flazzy.ABC;
using Flazzy.Tags;

namespace Flazzy.Example
{
    public class Program
    {
        private static readonly string[] SizeSuffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public Program(string[] args)
        { }
        public static void Main(string[] args)
        {
            Console.Title = "Flazzy.Example";

            var app = new Program(args);
            app.Run();
        }

        public void Run()
        {
            string fileName = "Client.swf";
            var fileInfo = new FileInfo(fileName);

            // Path Setting: Project > Properties > Debug > Start Options > Working Directory
            using (var ffile = new ShockwaveFlash(fileInfo.FullName))
            {
                Console.WriteLine($"File Name: {fileName}({SizeSuffix(fileInfo.Length)})");
                Console.WriteLine($"Compression: {ffile.Compression}({ffile.GetSignature()})");
                Console.WriteLine("File Version: " + ffile.Version);
                Console.WriteLine($"File Size: {SizeSuffix(ffile.FileLength)}");

                ffile.Disassemble();
                var tagChunks = new Dictionary<TagKind, List<TagItem>>();
                foreach (TagItem tag in ffile.Tags)
                {
                    List<TagItem> chunks = null;
                    if (!tagChunks.TryGetValue(tag.Kind, out chunks))
                    {
                        chunks = new List<TagItem>();
                        tagChunks.Add(tag.Kind, chunks);
                    }
                    chunks.Add(tag);
                }

                Console.WriteLine("------------------------------");
                Console.WriteLine($"Tags Found: {ffile.Tags.Count:n0}");
                Console.WriteLine();
                foreach (TagKind kind in tagChunks.Keys)
                {
                    List<TagItem> chunks = tagChunks[kind];
                    Console.WriteLine($"  * {kind}: {chunks.Count:n0}");
                }

                for (int i = 0; i < ffile.Tags.Count; i++)
                {
                    TagItem tag = ffile.Tags[i];
                    if (tag.Kind != TagKind.DoABC) continue;

                    var doAbcTag = (DoABCTag)tag;
                    using (var abc = new ABCFile(doAbcTag.ABCData))
                    {
                        ASConstantPool pool = abc.Pool;

                        Console.WriteLine("------------------------------");
                        Console.WriteLine($"{doAbcTag.Name}.abc({SizeSuffix(doAbcTag.ABCData.Length)})");
                        Console.WriteLine();
                        Console.WriteLine($"  * Methods: {abc.Methods.Count:n0}");
                        Console.WriteLine($"  * Metadata: {abc.Metadata.Count:n0}");
                        Console.WriteLine($"  * Classes/Instances: {abc.Instances.Count:n0}");
                        Console.WriteLine($"  * Scripts: {abc.Scripts.Count:n0}");
                        Console.WriteLine($"  * Method Bodies: {abc.MethodBodies.Count:n0}");

                        Console.WriteLine();
                        Console.WriteLine($"  ~ Constant Pool");
                        Console.WriteLine($"       | Integers: {pool.Integers.Count:n0}");
                        Console.WriteLine($"       | UIntegers: {pool.UIntegers.Count:n0}");
                        Console.WriteLine($"       | Doubles: {pool.Doubles.Count:n0}");
                        Console.WriteLine($"       | Strings: {pool.Strings.Count:n0}");
                        Console.WriteLine($"       | Namespaces: {pool.Namespaces.Count:n0}");
                        Console.WriteLine($"       | NamespaceSets: {pool.NamespaceSets.Count:n0}");
                        Console.WriteLine($"       | Multinames: {pool.Multinames.Count:n0}");
                    }
                }
            }

            Console.WriteLine("------------------------------");
            Console.WriteLine("Complete!");
            Console.Read();
        }

        private static string SizeSuffix(long value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 Bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1}{1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}