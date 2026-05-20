# Alex - Voice assistance

A modern desktop voice assistant for Windows, developed using **.NET (C# / WPF)**.

## Key Features:

* Offline Recognition 
* Wake Word Detection 
* Speech-to-Text (STT) 
* Text-to-Speech (TTS) 
* Modular Command System

## Project Structure

* VAProject.Core/ - System core, interfaces, models, and infrastructure (EventBus, Proxy, Analytics). 
* VAProject.Core/Commands/ - Business logic for specific voice commands (weather, open app, etc.).
* VAProject.UI/ - Graphical User Interface (WPF), settings windows, and system tray logic.

## Lab navigation
| № | Theme | Realization | Test |
|---|--------------------------|---------------------------------|----------------------|
| **1** | Generators and Iterators | `VAProject.Core/CommandsLogic/Commands/AlarmMode.cs` | `VAProject.Core/CommandsLogic/Commands/AlarmMode.cs` |
| **2** | Project Setup & Git | - | - |
| **3** | Memoization / Caching | `VAProject.Core/Utils/Memoization/LruCache.cs`<br>`VAProject.Core/Utils/Memoization/LfuCache.cs`<br>`VAProject.Core/Utils/Memoization/ActiveTtlCache.cs` | `VAProject.Core/CommandsLogic/Commands/WeatherCommand` |
| **4** | Priority Queue | `VAProject.Core/Utils/BiDirectionalPriorityQueue.cs` | `VAProject.Core/CommandsLogic/Commands/TestQueueCommand.cs` |
| **5** | Async Array Extensions | `VAProject.Core/Utils/AsyncArrayExtencions` | - |
| **6** | Data Streams (IAsyncEnumerable) | `VAProject.Core/Utils/LargeDataProcessor.cs` | `VAProject.Core/CommandsLogic/Commands/FindErrorsInLog.cs` |
| **7** | EventBus / PubSub | `VAProject.Core/Utils/EventBus/EventBus.cs` | `VAProject.Core/Audio/AudioCapturer.cs` |
| **8** | Authentication Proxy | `VAProject.Core/Utils/APIProxy/ApiKeyProxiHandler.cs` | `VAProject.Core/CommandsLogic/Commands/WeatherCommand` |
| **9** | Decorator Pattern | `VAProject.Core/CommandsLogic/CommandDecorators/AnalyticsDecorator.cs` | Any command. Stats save in: /Analytics/command_stats.json |

### Requirements:
* .NET 8.0 SDK (or later)
* Windows 10/11 OS
