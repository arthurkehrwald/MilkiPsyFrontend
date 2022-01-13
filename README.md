# MilkiPsy Frontend
This repository contains a Unity frontend for creating psychomotor skills training applications. It works by parsing training programs further divided into stages from a local configuration folder. After selecting a program, the user will be guided through its stages. Detailed instructions can be specified using text, audio, videos or pictures for each stage. To provide feedback, a backend can be connected via TCP. The frontend will send information about the currently running program and stage. By evaluating real-time sensor data, the backend analyzes the user's performance and sends back a response. The frontend currently supports short popup texts and more detailed feedback with text and media in the same format as the instructions. The current stage can also be changed by the backend. This can be used to automatically continue when the user completes a stage. To see this functionality in action, use this [example backend](https://github.com/arthurkehrwald/MilkiPsyBackend "example backend"). It demonstrates how to connect to the frontend, receive status updates, and provides a command line interface for sending messages back to the client.

## Installation
The app is available for Windows, Linux, HoloLens 2, and Android. Executables can be found in the releases section of this  repository.

## Configuration
The frontend relies on data in a folder called 'MilkiPsyConfiguration' that must be present in the local Documents folder (Home directory on Linux). A working example can be found here. TODO: INSERT LINK

### Settings
The file 'ServerAdress.json' in the root configuration folder defines the adress of the server the app will attempt to connect to.

| Key | Type | Meaning | Required |
| - | - | - | - |
| ip | String | The app will attempt to connect to a backend running on this address. | Yes |
| port | Int | The app will attempt to connect to a backend running on this port. | Yes |

The file 'LoggingSettings.json' in the root configuration folder can be used to make troubleshooting easier by displaying technical information in the user interface.

| Key | Type | Meaning | Required |
| - | - | - | - |
| logDebugInfo | Boolean | Whether or not to display technical information such as when outdated server messages are ignored | Yes |
| logDebugError | Boolean | Whether or not to display messages related to errors such as file access or parsing errors | Yes |

### Programs
Each program is defined in its own .json file in 'MilkiPsyConfiguration/Programs'.

| Key | Type | Meaning | Required |
| ------------ | ------------ |------------ | ------------ |
| displayName | String | The name of the program as displayed to the user | Yes
| estimatedDurationMinutes | Float | The duration of the program as displayed in the program selection menu | No |
| stageFilenames | String Array | A list of .json filenames (including the '.json' at the end). Each entry refers to a stage configuration file. The files will be parsed into stages and added to the program. | Yes |

### Stages
Each stage is defined in its own .json file in 'MilkiPsyConfiguration/Stages'.

| Key | Type | Meaning | Required |
| - | - |- | - |
| displayName | String | The name of the stage as displayed to the user. Should  tell the user what to do. | Yes|
| instructionsFilename | String | The name of the .json file (including the '.json' at the end) that defines the detailed instructions to be displayed while the stage is running. | No |

### Instructions and Feedback
Each instruction is defined in its own .json file in 'MilkiPsyConfiguration/InstructionsAndFeedback'. The same applies to feedback that can be displayed as a response to a message from the server.

| Key | Type | Meaning | Required |
| - | - |- | - |
| text | string | The text to display to the user. Can use [markup syntax](http://digitalnativestudios.com/textmeshpro/docs/rich-text/). | Yes, unless 'media' is defined
| mediaFilename | string | The name of the media file to display, including the file ending. | Yes, unless 'text' is defined |

### Media

| Type| Supported Formats | Location |
| - | - |- |
| Image | .png, .jpeg | 'MilkiPsyConfiguration/Media/Images' |
| Video | .webm, .mp4* | 'MilkiPsyConfiguration/Media/Videos' |
| Audio | .wav, .mp3, .ogg | 'MilkiPsyConfiguration/Media/Audio' |

*not supported on Linux

### Popup Messages

The popup messages that can be triggered by the server must be defined in .json files located in 'MilkiPsyConfiguration/PopupMessages'.

| Key | Type | Meaning | Required |
| - | - |- | - |
| text | String | The text of the message | Yes |
| connotation | Int | 1 = Good, 2 = Neutral, 3 = Bad. The connotation is supposed to control the background color of the message, but this is not implemented yet. | Yes |

## Building
Building is straigthforward for every platform except HoloLens. To gain access to the local documents folder where the configuration files are, permission is needed. It is currently not possible to set this up in Unity, so the package manifest file must be manually edited after building. For instructions, refer to the section on 'Documents' under 'Restricted Capability List' in the [offical documentation](https://docs.microsoft.com/en-us/windows/uwp/packaging/app-capability-declarations "documentation").

## Further Development

The configuration process is very annoying. It requires perfect spelling and adherence to specific file and folder structures. It would be much better if the configuration files could be generated automatically by an editor with a graphical user interface and loaded into the frontend using an online content management system.
