# Vaisala HMP Series Humidity and Temperature Probe

The project (library and application) for a limited registers readings over serial port via MODBUS communication protocol from Vaisala HMP Series probe. Tested on Windows 10 20H2 and Raspberry Pi 3 B+ using setup below.

All Modbus registers are listed in [Vaisala User Guide](https://www.vaisala.com/sites/default/files/documents/HMP-Series-User-Guide-M212022EN.pdf)

NOTE: Unfinished (in-progress)

## Setup

* Vaisala HMP9 Humidity and Temperature Probe

* Indigo USB Adapter

## Usage

Obtain list of serial ports available via:

```csharp
SerialPort.GetPortNames();
```

Supply the appropriate port to ```CommunicationClass```:

```csharp

```

## Limitations

Only reading functions are implemented

## Cheers to

Big thanks to Andreas Müller for his library on [Modbus implementation](https://github.com/AndreasAmMueller/Modbus) on .NET Standard 2.0.

## License

Published under MIT License:

Copyright 2021 Kostya Kartavenka

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.