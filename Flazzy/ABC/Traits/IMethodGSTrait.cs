namespace Flazzy.ABC
{
    public interface IMethodGSTrait
    {
        ASMethod Method { get; }
        int MethodIndex { get; set; }

        int Id { get; set; }
    }
}