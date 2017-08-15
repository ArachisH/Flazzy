using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Flazzy.IO;
using Flazzy.ABC;
using Flazzy.Tags;
using Flazzy.Sandbox.Utilities;

namespace Flazzy.Sandbox
{
    public class Program
    {
        public FileInfo Info { get; }
        public ShockwaveFlash Flash { get; }

        public Program(string[] args)
        {
            Info = new FileInfo(args[0]);
            Flash = new ShockwaveFlash(Info.FullName);
        }
        public static void Main(string[] args)
        {
            Console.Title = ("Flazzy v" +
                typeof(ShockwaveFlash).Assembly.GetName().Version);

            var app = new Program(args);
            app.Run();
        }

        public void Run()
        {
            Disassemble();
            Modify();
            Assemble();
        }

        private void Modify()
        {
            foreach (DoABCTag abcTag in Flash.Tags
                .Where(t => t.Kind == TagKind.DoABC))
            {
                var abc = new ABCFile(abcTag.ABCData);

                byte[] newData = abc.ToArray();
                byte[] oldData = abcTag.ABCData;

                int minSize = (Math.Min(oldData.Length, newData.Length));
                for (int i = 0; i < minSize; i++)
                {
                    if (newData[i] != abcTag.ABCData[i])
                    {
                        System.Diagnostics.Debugger.Break();
                        return;
                    }
                }
                if (oldData.Length != newData.Length)
                {
                    System.Diagnostics.Debugger.Break();
                    return;
                }
            }
        }
        private void Assemble()
        {
            ConsoleEx.WriteLineTitle("Assembling");

            string asmdPath = Path.Combine(Info.DirectoryName, ("asmd_" + Info.Name));
            using (var asmdFile = File.Open(asmdPath, FileMode.Create))
            using (var asmdStream = new FlashWriter(asmdFile))
            {
                Flash.Assemble(asmdStream, CompressionKind.None);
                Console.WriteLine("File Assembled: " + asmdPath);
            }
        }
        private void Disassemble()
        {
            ConsoleEx.WriteLineTitle("Disassembling");
            Flash.Disassemble();

            var productInfo = (ProductInfoTag)Flash.Tags
                .FirstOrDefault(t => t.Kind == TagKind.ProductInfo);

            if (productInfo != null)
            {
                Console.WriteLine("Compilation Date: {0}", (productInfo?.CompilationDate.ToString() ?? "?"));
            }
            IDictionary<TagKind, List<TagItem>> groups = GetTagGroups();
            foreach (KeyValuePair<TagKind, List<TagItem>> groupPair in groups)
            {
                Console.WriteLine($"{groupPair.Key}: {groupPair.Value.Count:n0}");
            }
        }

        private IDictionary<TagKind, List<TagItem>> GetTagGroups()
        {
            var groups = new Dictionary<TagKind, List<TagItem>>();
            foreach (TagItem tag in Flash.Tags)
            {
                if (!groups.TryGetValue(tag.Kind, out List<TagItem> group))
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