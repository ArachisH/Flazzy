namespace Flazzy.ABC
{
    public interface ISlotConstantTrait
    {
        ASMultiname Type { get; }
        int TypeIndex { get; set; }

        object Value { get; }
        int ValueIndex { get; set; }
        ConstantKind ValueKind { get; set; }

        int Id { get; set; }
    }
}