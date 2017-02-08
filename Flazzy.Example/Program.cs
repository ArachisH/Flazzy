using System;
using System.IO;
using System.Collections.Generic;

using Flazzy.IO;
using Flazzy.Tags;
using Flazzy.Example.Utilities;
using System.Security.Cryptography;

namespace Flazzy.Example
{
    public class Program
    {
        public FileInfo Info { get; }
        public ShockwaveFlash Flash { get; private set; }

        public Program(string[] args)
        {
            Info = new FileInfo(args[0]);
            Flash = new ShockwaveFlash(Info.FullName);
        }
        public static void Main(string[] args)
        {
            Console.Title = ("Flazzy v" +
                typeof(ShockwaveFlash).Assembly.GetName().Version);

            new Program(args).Run();
        }

        public void Run()
        {
            Disassemble();

            Modify();

            Assemble();
        }
        private void Modify()
        { }
        private void Assemble()
        {
            ConsoleEx.WriteLineTitle("Assembling");

            string fileName = "Assembled.swf";
            using (var asmFile = File.Open(fileName, FileMode.Create))
            using (var asmStream = new FlashWriter(asmFile))
            {
                Flash.Assemble(asmStream, CompressionKind.None);

                asmFile.Position = 0;
                Console.WriteLine("SHA1: " + GetSHA1(asmFile));
                Console.WriteLine("File Assembled: " + Path.Combine(Info.DirectoryName, fileName));
            }
        }
        private void Disassemble()
        {
            ConsoleEx.WriteLineTitle("Disassembling");
            Flash.Disassemble();

            Console.WriteLine("SHA1: " + GetSHA1(Info.FullName));
            Console.WriteLine();

            IDictionary<TagKind, List<TagItem>> groups = GetTagGroups();
            foreach (KeyValuePair<TagKind, List<TagItem>> groupPair in groups)
            {
                Console.WriteLine($"{groupPair.Key}: {groupPair.Value.Count:n0}");
            }
            Console.WriteLine();
        }

        public string GetSHA1(string path)
        {
            using (var input = File.OpenRead(path))
            {
                return GetSHA1(input);
            }
        }
        public string GetSHA1(Stream input)
        {
            using (var sha1 = new SHA1Managed())
            {
                return BitConverter.ToString(sha1.ComputeHash(input))
                    .Replace("-", string.Empty)
                    .ToLower();
            }
        }
        private IDictionary<TagKind, List<TagItem>> GetTagGroups()
        {
            var groups = new Dictionary<TagKind, List<TagItem>>();
            foreach (TagItem tag in Flash.Tags)
            {
                List<TagItem> group = null;
                if (!groups.TryGetValue(tag.Kind, out group))
                {
                    group = new List<TagItem>();
                    groups.Add(tag.Kind, group);
                }
                group.Add(tag);
            }
            return groups;
        }
    }
}