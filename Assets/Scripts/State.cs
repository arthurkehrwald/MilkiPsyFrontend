using System;

[Serializable]
public class State : IParseResult
{
    public string uniqueProgramName;
    public string uniqueStageName;

    public State()
    {
        uniqueProgramName = "none";
        uniqueStageName = "none";
    }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(uniqueProgramName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(uniqueStageName))
        {
            return false;
        }

        return true;
    }

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