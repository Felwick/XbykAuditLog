# Xperience by Kentico - Web Page Item Event Logging Extension

This solution is an extension for [Xperience by Kentico](https://xperience.io/) that enhances logging capabilities by recording actions and changes made to web page items directly into the event log. It is built for .NET 8 and integrates seamlessly with your Xperience project.

## Features

- Logs create, update, and delete actions on web page items
- Captures detailed information about each change
- Stores logs in the Xperience event log for easy monitoring and auditing
- Designed for extensibility and minimal performance impact

## Getting Started

1. **Clone or download** this repository into your Xperience by Kentico solution.
2. **Build** the solution using Visual Studio 2022 or later.
3. **Configure** the extension as needed (see configuration section below).
4. **Deploy** to your Xperience environment.

## Configuration

- No special configuration is required for basic usage.
- Advanced options (such as filtering which page types to log) can be set in the extension's configuration file or via dependency injection.

## Usage

Once installed, any create, update, or delete action performed on web page items will be automatically logged in the Xperience event log. You can view these logs in the Xperience administration interface under **System > Event log**.

## Requirements

- Xperience by Kentico (latest version recommended)
- .NET 8
- Visual Studio 2022 or later

## Support

For issues or feature requests, please use the repository's issue tracker.

## License

This project is provided under the MIT License. See [LICENSE](LICENSE) for details.
