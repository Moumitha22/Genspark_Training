namespace PropFinderApi.Models
{
    public class GenericRangeModel<T> where T : struct, IComparable
    {
        public T? Min { get; set; }
        public T? Max { get; set; }
    }

}