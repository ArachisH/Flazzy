namespace Flazzy.ABC
{
    public abstract class AS3Item : FlashItem
    {
        public ABCFile ABC { get; }

        public AS3Item(ABCFile abc)
        {
            ABC = abc;
        }

        public virtual string ToAS3()
        {
            throw new NotSupportedException();
        }
    }
}