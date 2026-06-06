// Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.

// Verifies that every staged text file is UTF-8 encoded, without BOM
// (except *.sln, which Visual Studio writes with a UTF-8 BOM — see .editorconfig).
// Run by the pre-commit hook via Husky.Net: file paths arrive in Args (${staged}).

using System.Text;

var errors = 0;

void Fail(string file, string message)
{
    Console.WriteLine($"ERROR: {file} {message}");
    errors++;
}

bool StartsWith(byte[] bytes, params byte[] prefix)
{
    if (bytes.Length < prefix.Length) return false;
    for (var i = 0; i < prefix.Length; i++)
        if (bytes[i] != prefix[i]) return false;
    return true;
}

foreach (var file in Args)
{
    if (!File.Exists(file)) continue; // deleted or renamed away

    var bytes = File.ReadAllBytes(file);
    if (bytes.Length == 0) continue;

    // UTF-16/UTF-32 must be rejected before the binary heuristic below:
    // their NUL bytes would otherwise make them pass as "binary".
    if (StartsWith(bytes, 0xFF, 0xFE, 0x00, 0x00) || StartsWith(bytes, 0x00, 0x00, 0xFE, 0xFF))
    {
        Fail(file, "is UTF-32 encoded (must be UTF-8)");
        continue;
    }
    if (StartsWith(bytes, 0xFF, 0xFE) || StartsWith(bytes, 0xFE, 0xFF))
    {
        Fail(file, "is UTF-16 encoded (must be UTF-8)");
        continue;
    }

    var hasBom = StartsWith(bytes, 0xEF, 0xBB, 0xBF);
    if (hasBom && !file.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
    {
        Fail(file, "has a UTF-8 BOM (charset = utf-8 means no BOM; only *.sln keeps one)");
        continue;
    }

    // Skip binary files — same heuristic as git: a NUL byte in the first 8000 bytes.
    if (Array.IndexOf(bytes, (byte)0, 0, Math.Min(bytes.Length, 8000)) >= 0) continue;

    try
    {
        var offset = hasBom ? 3 : 0;
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
            .GetString(bytes, offset, bytes.Length - offset);
    }
    catch (DecoderFallbackException)
    {
        Fail(file, "is not valid UTF-8 (probably Latin-1/Windows-1252)");
    }
}

if (errors > 0)
{
    Console.WriteLine();
    Console.WriteLine("Encoding check failed: convert the files above to UTF-8 (your IDE follows .editorconfig automatically).");
    return 1;
}

return 0;
