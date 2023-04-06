﻿namespace PatternPal.Core.Checks;

internal interface ICheck
{
    bool Check(
        INode node);
}

internal class RootCheck : ICheck
{
    public bool Check(
        INode node)
    {
        return false;
    }
}
