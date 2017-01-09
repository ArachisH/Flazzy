namespace Flazzy.ABC
{
    public abstract class ConstantItem : FlashItem
    {
        protected ASConstantPool Pool { get; }

        public ConstantItem(ASConstantPool pool)
        {
            Pool = pool;
        }
    }
}
