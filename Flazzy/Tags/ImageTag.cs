using System.Drawing;

using Flazzy.Records;

namespace Flazzy.Tags
{
    public abstract class ImageTag : TagItem
    {
        public ImageFormat Format { get; protected set; }

        public ImageTag(TagKind kind)
            : base(kind)
        { }
        public ImageTag(HeaderRecord header)
            : base(header)
        { }

        protected ImageFormat GetFormat(byte[] data)
        {
            if (BitConverter.ToInt32(data, 0) == -654321153 || BitConverter.ToInt16(data, 0) == -9985)
            {
                return ImageFormat.JPEG;
            }
            else if (BitConverter.ToInt64(data, 0) == 727905341920923785)
            {
                return ImageFormat.PNG;
            }
            else if (BitConverter.ToInt32(data, 0) == 944130375 && BitConverter.ToInt16(data, 4) == 24889)
            {
                return ImageFormat.GIF98a;
            }
            throw new ArgumentException("Provided data contains an unknown image format.");
        }

        public abstract Color[,] GetARGBMap();
        public abstract void SetARGBMap(Color[,] map);
    }
}