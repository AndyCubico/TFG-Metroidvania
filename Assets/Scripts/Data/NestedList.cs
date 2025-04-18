using System;
using System.Collections.Generic;

[Serializable]
public class NestedSubList<T>
{
    public List<T> subList;

    // Getter and Setter override
    // Allows NestedSubList[]
    public T this[int index]
    {
        get => subList[index];
        set => subList[index] = value;
    }
}

[Serializable]
public class NestedList<T>
{
    public List<NestedSubList<T>> list;

    // Allows NestedList[]
    public NestedSubList<T> this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }
}