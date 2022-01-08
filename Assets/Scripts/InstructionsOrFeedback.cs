using System;

[Serializable]
public class InstructionsOrFeedback : IParseResult
{
    public string mediaFileName;
    public string text;

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(mediaFileName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        return true;
    }
}