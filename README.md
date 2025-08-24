# InfoWriter Converter

A C# console application that converts OBS InfoWriter plugin output into a clean, timestamp-formatted list.

## What is InfoWriter?

InfoWriter is an OBS plugin that allows content creators to take timestamped notes during live recording or streaming. This tool converts the raw output from InfoWriter into a clean, readable format perfect for video chapters, show notes, or content organization.

**Plugin Link**: [InfoWriter OBS Plugin](https://obsproject.com/forum/resources/infowriter.345/)

## Features

- ✅ Converts HOTKEY events to timestamped notes
- ✅ Handles recording pause/resume events  
- ✅ Filters out recording start/stop events
- ✅ Supports unlimited number of custom note types
- ✅ Robust error handling and validation
- ✅ Command-line interface for automation

## Input Format

The tool expects InfoWriter output in this format:
```
EVENT:START RECORDING @ 2025-08-20 20:43:06
0:00:00 Record Time Marker

HOTKEY:input-note-1 @ 2025-08-20 20:44:31
0:01:25 Record Time Marker

HOTKEY:input-note-2 @ 2025-08-20 20:46:33
0:03:27 Record Time Marker
```

## Output Format

The converted output looks like this:
```
0:01:25 input-note-1
0:03:27 input-note-2
0:06:14 input-note-3
```

## Requirements

- .NET 8.0 or later
- Windows, macOS, or Linux

## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/yourusername/infowriter-convert-script.git
   cd infowriter-convert-script
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

## Usage

### Basic Usage
Place your InfoWriter output file as `input.txt` in the project directory and run:
```bash
dotnet run
```

This will create `output.txt` with the converted format.

### Custom File Names
Specify custom input and output files using flags:
```bash
dotnet run -- -i "my-recording-notes.txt" -o "converted-notes.txt"
```

### Command Line Arguments
- `-i, --input <file>`: Input file path (default: `input.txt`)
- `-o, --output <file>`: Output file path (default: `output.txt`)
- `-h, --help`: Show help message

**Note**: When using `dotnet run` with arguments, you need to add `--` before your arguments to separate them from dotnet's arguments.

## Examples

### Example 1: Stream Notes
Convert your stream notes for YouTube chapters:
```bash
dotnet run -- -i "stream-2025-01-15.txt" -o "youtube-chapters.txt"
```

### Example 2: Using Long Form Arguments
```bash
dotnet run -- --input "recording.txt" --output "chapters.txt"
```

### Example 3: Batch Processing
Create a batch script to process multiple files:
```bash
for file in *.txt; do
    dotnet run -- -i "$file" -o "converted-$file"
done
```

### Example 4: Help
Show usage information:
```bash
dotnet run -- --help
```

## Supported Events

| InfoWriter Event | Action | Output |
|-----------------|--------|--------|
| `HOTKEY:your-note` | Convert | `timestamp your-note` |
| `EVENT:RECORDING PAUSED` | Convert | `timestamp RECORDING PAUSED` |
| `EVENT:RECORDING RESUMED` | Convert | `timestamp RECORDING RESUMED` |
| `EVENT:START RECORDING` | Remove | _(filtered out)_ |
| `EVENT:STOP RECORDING` | Remove | _(filtered out)_ |

## Troubleshooting

### Common Issues

**File not found error:**
```
✗ Error: Input file 'input-example.txt' not found.
```
- Ensure the input file exists in the specified location
- Check file permissions

**Timestamp parsing warning:**
```
Warning: Could not parse timestamp from line X
```
- The InfoWriter output may be malformed
- Check that each HOTKEY/EVENT line is followed by a timestamp line

**Note key parsing warning:**
```
Warning: Could not parse note key from line X
```
- The HOTKEY line format may be unexpected
- Ensure the format is: `HOTKEY:note-name @ timestamp`

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is open source and available under the [MIT License](LICENSE).

## Changelog

### v1.0.0
- Initial release
- Basic InfoWriter output conversion
- Support for custom note types
- Command-line interface
- Error handling and validation
