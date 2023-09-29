namespace Exlibris.Core.JSONs
{
    public class Key
    {
        public bool IsRoot { get; }
        public bool IsArrayItem { get; }
        public string Name { get; }
        public int ArrayIndex { get; }
        public bool HasNext { get; }

        public Key(bool isRoot, bool isArrayItem, string name, int arrayIndex, bool hasNext)
        {
            IsRoot = isRoot;
            IsArrayItem = isArrayItem;
            Name = name;
            ArrayIndex = arrayIndex;
            HasNext = hasNext;
        }

        public override string ToString()
            => IsArrayItem ? $"{Name}[{ArrayIndex}]" : Name;
    }
}