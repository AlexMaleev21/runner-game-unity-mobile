using System;
using System.Text;

public class PlayerProfileService
{
    public string Nickname { get; private set; }
    public string NicknameKey { get; private set; }
    public bool HasNickname => !string.IsNullOrEmpty(NicknameKey);

    public bool TrySetNickname(string nickname)
    {
        string normalizedNickname = NormalizeNickname(nickname);
        if (string.IsNullOrEmpty(normalizedNickname))
            return false;

        Nickname = normalizedNickname;
        NicknameKey = CreateDatabaseKey(normalizedNickname);
        return true;
    }

    private string NormalizeNickname(string nickname)
    {
        return string.IsNullOrWhiteSpace(nickname)
            ? string.Empty
            : nickname.Trim();
    }

    private string CreateDatabaseKey(string nickname)
    {
        string normalizedKey = nickname.ToLowerInvariant();
        byte[] bytes = Encoding.UTF8.GetBytes(normalizedKey);
        StringBuilder builder = new StringBuilder(bytes.Length);

        foreach (byte value in bytes)
        {
            bool isLetter = value >= 'a' && value <= 'z';
            bool isDigit = value >= '0' && value <= '9';
            bool isSafeSymbol = value == '_' || value == '-';

            if (isLetter || isDigit || isSafeSymbol)
            {
                builder.Append((char)value);
                continue;
            }

            builder.Append('%');
            builder.Append(value.ToString("X2"));
        }

        return builder.ToString();
    }
}
