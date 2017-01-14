using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Flazzy.IO;
using Flazzy.Tags;

namespace Flazzy.Example
{
    public class Program
    {
        private static readonly string[] SizeSuffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private readonly Stopwatch _watch;

        public string FilePath { get; }
        public ShockwaveFlash Flash { get; }

        public Program(string[] args)
        {
            _watch = new Stopwatch();

            FilePath = Path.GetFullPath(args[0]);
            Flash = new ShockwaveFlash(FilePath);
            UpdateTitle(this);
        }
        public static void Main(string[] args)
        {
            Console.Title = ("Flazzy v" + GetVersion());
            new Program(args).Run();
        }

        public void Run()
        {
            /* Step #1 - Disassembling */
            Disassemble();

            /* Step #1.5(Optional) - Modifying/Extracting */
            Modify();

            /* Step #2 - Assembling */
            Assemble(CompressionKind.None);
        }
        private void Modify()
        {
            //var doABCTags = Flash.Tags.OfType<DoABCTag>();
            //foreach (DoABCTag abcTag in doABCTags)
            //{
            //    using (var abc = new ABCFile(abcTag.ABCData))
            //    {
            //        // Magic here.
            //        // abcTag.ABCData = abc.ToArray();
            //    }
            //}

            //Flash.Tags.OfType<DefineBitsLossless2Tag>().AsParallel().ForAll(bitsTag =>
            //{
            //    using (Bitmap asset = bitsTag.GenerateAsset())
            //    {
            //        asset.Save(/* File Path */);
            //    }
            //    Console.WriteLine($"Asset saved/generated {bitsTag.Id}.");
            //});

            //var defineBinaryDataTags = Flash.Tags.OfType<DefineBinaryDataTag>();
            //foreach (DefineBinaryDataTag binTag in defineBinaryDataTags)
            //{
            //    // Magic here.
            //}

            // etc...
        }
        private void Disassemble()
        {
            _watch.Restart();
            Console.Write("Disassembling...");

            Flash.Disassemble();
            Console.WriteLine($" | Tags: {Flash.Tags.Count:n0} | {GetLap()}");
            Console.WriteLine();

            IDictionary<TagKind, List<TagItem>> tagGroups = GetTagGroups();
            foreach (TagKind kind in tagGroups.Keys)
            {
                int totalSize = 0;
                List<TagItem> group = tagGroups[kind];
                foreach (TagItem item in group)
                {
                    totalSize += item.Header.Length;
                    totalSize += (item.Header.IsLongTag ? 6 : 2);
                }
                Console.WriteLine($"  ~  [{kind}]: {group.Count:n0}({SizeSuffix(totalSize)})");
            }
            Console.WriteLine();
        }
        private void Assemble(CompressionKind compression)
        {
            _watch.Restart();
            Console.Write("Assembling...");

            if (File.Exists("Assembled.swf"))
            {
                File.Delete("Assembled.swf");
            }
            using (var asmFile = File.OpenWrite("Assembled.swf"))
            using (var asmStream = new FlashWriter(asmFile))
            {
                Flash.Assemble(asmStream, compression);
                Console.WriteLine($" | Assembled.swf({SizeSuffix(asmStream.Length)}) | {GetLap()}");
            }
        }

        private string GetLap()
        {
            _watch.Stop();
            return (_watch.Elapsed.ToString("s\\.ff") + " Seconds");
        }
        private IDictionary<TagKind, List<TagItem>> GetTagGroups()
        {
            var tagChunks = new Dictionary<TagKind, List<TagItem>>();
            foreach (TagItem tag in Flash.Tags)
            {
                List<TagItem> chunks = null;
                if (!tagChunks.TryGetValue(tag.Kind, out chunks))
                {
                    chunks = new List<TagItem>();
                    tagChunks.Add(tag.Kind, chunks);
                }
                chunks.Add(tag);
            }
            return tagChunks;
        }

        private static Version GetVersion()
        {
            return typeof(ShockwaveFlash).Assembly.GetName().Version;
        }
        private static string SizeSuffix(long value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 Bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        private static void UpdateTitle(Program program)
        {
            var titleBuilder = new StringBuilder();
            titleBuilder.Append(" | ");
            titleBuilder.Append(Path.GetFileName(program.FilePath));
            titleBuilder.Append("(" + SizeSuffix(program.Flash.FileLength) + ")");
            titleBuilder.Append(" | ");
            titleBuilder.Append("Compression: " + program.Flash.Compression);
            Console.Title += titleBuilder;
        }
    }
}