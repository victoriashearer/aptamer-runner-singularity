using System.Text;

namespace AptamerRunner;

/// <summary>
/// Borrowed from https://www.codeproject.com/Articles/51488/Implementing-Word-Wrap-in-C
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Word wraps the given text to fit within the specified width.
    /// </summary>
    /// <param name="text">Text to be word wrapped</param>
    /// <param name="width">Width, in characters, to which the text
    /// should be word wrapped</param>
    /// <returns>The modified text</returns>
    public static string WordWrap(this string text, int width)
    {
        int pos, next;
        StringBuilder sb = new StringBuilder();

        // Lucidity check
        if (width < 1)
            return text;

        // Parse each line of text
        for (pos = 0; pos < text.Length; pos = next)
        {
            // Find end of line
            int eol = text.IndexOf(Environment.NewLine, pos);
            if (eol == -1)
                next = eol = text.Length;
            else
                next = eol + Environment.NewLine.Length;

            // Copy this line of text, breaking into smaller lines as needed
            if (eol > pos)
            {
                do
                {
                    int len = eol - pos;
                    if (len > width)
                        len = BreakLine(text, pos, width);
                    sb.Append(text, pos, len);
                    sb.Append(Environment.NewLine);

                    // Trim whitespace following break
                    pos += len;
                    while (pos < eol && Char.IsWhiteSpace(text[pos]))
                        pos++;
                } while (eol > pos);
            }
            else sb.Append(Environment.NewLine); // Empty line
        }

        return sb.ToString();
    }

    /// <summary>
    /// Locates position to break the given line so as to avoid
    /// breaking words.
    /// </summary>
    /// <param name="text">String that contains line of text</param>
    /// <param name="pos">Index where line of text starts</param>
    /// <param name="max">Maximum line length</param>
    /// <returns>The modified line length</returns>
    private static int BreakLine(string text, int pos, int max)
    {
        // Find last whitespace in line
        int i = max;
        while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
            i--;

        // If no whitespace found, break at maximum length
        if (i < 0)
            return max;

        // Find start of whitespace
        while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
            i--;

        // Return length of text before whitespace
        return i + 1;
    }
}
