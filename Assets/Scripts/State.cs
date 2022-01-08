using System;

[Serializable]
public struct State
{
    public string uniqueProgramName;
    public string uniqueStageName;

    public static bool operator ==(State a, State b)
    {
        return a.uniqueProgramName == b.uniqueProgramName
            && a.uniqueStageName == b.uniqueStageName;
    }

    public static bool operator !=(State a, State b)
    {
        return !(a == b);
    }
}