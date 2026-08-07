using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Auditor3;

/// <summary>
/// Parses GDB "ptype /o struct ..." output into a PrecLayout object.
/// </summary>
public sealed class PrecLayoutParser
{
    private static readonly Regex StructureRegex = new(
        @"type\s*=\s*struct\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);

    private static readonly Regex FieldRegex = new(
        @"^\s*/\*\s*(?<offset>[0-9]+)(?::(?<bit>[0-9]+))?\s*\|\s*(?<size>[0-9]+)\s*\*/\s*(?<declaration>.+?)\s*;?\s*$",
        RegexOptions.Compiled);

    private static readonly Regex HoleRegex = new(
        @"^\s*/\*\s*XXX\s*(?<size>[0-9]+)-byte\s+(?<kind>hole|padding)\s*\*/",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex TotalSizeRegex = new(
        @"total\s+size\s*\(\s*bytes\s*\)\s*:\s*(?<size>[0-9]+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public PrecLayout Parse(
        string content,
        string precType = "",
        string release = "",
        string sourceFile = "")
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException(
                "Layout content cannot be empty.",
                nameof(content));
        }

        var fields = new List<PrecLayoutField>();
        var structureName = string.Empty;
        int? totalSize = null;

        var lines = content.Replace("\r\n", "\n").Split('\n');

        foreach (var line in lines)
        {
            var structureMatch = StructureRegex.Match(line);

            if (structureMatch.Success)
            {
                structureName = structureMatch.Groups["name"].Value;
                continue;
            }

            var totalMatch = TotalSizeRegex.Match(line);

            if (totalMatch.Success &&
                int.TryParse(totalMatch.Groups["size"].Value, out var parsedTotal))
            {
                totalSize = parsedTotal;
                continue;
            }

            var holeMatch = HoleRegex.Match(line);

            if (holeMatch.Success &&
                int.TryParse(holeMatch.Groups["size"].Value, out var holeSize))
            {
                fields.Add(new PrecLayoutField
                {
                    Name = "padding",
                    Type = "padding",
                    Offset = CalculatePaddingOffset(fields),
                    Size = holeSize,
                    IsPadding = true,
                    SourceText = line
                });

                continue;
            }

            var fieldMatch = FieldRegex.Match(line);

            if (!fieldMatch.Success)
            {
                continue;
            }

            if (!int.TryParse(
                    fieldMatch.Groups["offset"].Value,
                    out var offset) ||
                !int.TryParse(
                    fieldMatch.Groups["size"].Value,
                    out var size))
            {
                continue;
            }

            var declaration = fieldMatch.Groups["declaration"].Value.Trim();
            var parsedDeclaration = ParseDeclaration(declaration);

            int? bitOffset = null;

            if (fieldMatch.Groups["bit"].Success &&
                int.TryParse(
                    fieldMatch.Groups["bit"].Value,
                    out var parsedBitOffset))
            {
                bitOffset = parsedBitOffset;
            }

            fields.Add(new PrecLayoutField
            {
                Name = parsedDeclaration.Name,
                Type = parsedDeclaration.Type,
                Offset = offset,
                Size = size,
                IsPadding = false,
                IsBitField = bitOffset.HasValue ||
                             declaration.Contains(":"),
                BitOffset = bitOffset,
                BitSize = ParseBitSize(declaration),
                SourceText = line
            });
        }

        if (string.IsNullOrWhiteSpace(structureName))
        {
            throw new FormatException(
                "The layout does not contain a recognizable structure name.");
        }

        return new PrecLayout
        {
            PrecType = string.IsNullOrWhiteSpace(precType)
                ? structureName.ToUpperInvariant()
                : precType,
            StructureName = structureName,
            Release = release,
            SourceFile = sourceFile,
            TotalSize = totalSize,
            Fields = fields
        };
    }

    public PrecLayout ParseFile(
        string filename,
        string precType = "",
        string release = "")
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentException(
                "Layout filename cannot be empty.",
                nameof(filename));
        }

        if (!File.Exists(filename))
        {
            throw new FileNotFoundException(
                "Layout file was not found.",
                filename);
        }

        var content = File.ReadAllText(filename);

        return Parse(content, precType, release, filename);
    }

    private static (string Type, string Name) ParseDeclaration(
        string declaration)
    {
        var text = declaration.Trim();

        var bitColon = text.IndexOf(':');

        if (bitColon >= 0)
        {
            text = text[..bitColon].Trim();
        }

        text = text.TrimEnd(';').Trim();

        var arrayStart = text.IndexOf('[');

        if (arrayStart >= 0)
        {
            var arrayEnd = text.IndexOf(']', arrayStart);

            if (arrayEnd >= 0)
            {
                var nameStart = FindNameStart(text, arrayStart);
                var name = text[nameStart..(arrayEnd + 1)];

                return (
                    text[..nameStart].Trim(),
                    name);
            }
        }

        var nameEnd = text.Length - 1;

        while (nameEnd >= 0 &&
               !char.IsLetterOrDigit(text[nameEnd]) &&
               text[nameEnd] != '_')
        {
            nameEnd--;
        }

        if (nameEnd < 0)
        {
            return (text, string.Empty);
        }

        var nameStartIndex = nameEnd;

        while (nameStartIndex >= 0 &&
               (char.IsLetterOrDigit(text[nameStartIndex]) ||
                text[nameStartIndex] == '_'))
        {
            nameStartIndex--;
        }

        nameStartIndex++;

        return (
            text[..nameStartIndex].Trim(),
            text[nameStartIndex..(nameEnd + 1)]);
    }

    private static int? ParseBitSize(string declaration)
    {
        var colon = declaration.IndexOf(':');

        if (colon < 0)
        {
            return null;
        }

        var bitText = declaration[(colon + 1)..]
            .Trim()
            .TrimEnd(';')
            .Trim();

        return int.TryParse(bitText, out var bitSize)
            ? bitSize
            : null;
    }

    private static int CalculatePaddingOffset(
        IReadOnlyList<PrecLayoutField> fields)
    {
        if (fields.Count == 0)
        {
            return 0;
        }

        var last = fields[^1];

        return last.Offset + last.Size;
    }

    private static int FindNameStart(string text, int arrayStart)
    {
        var index = arrayStart - 1;

        while (index >= 0 &&
               (char.IsLetterOrDigit(text[index]) ||
                text[index] == '_'))
        {
            index--;
        }

        return index + 1;
    }
}