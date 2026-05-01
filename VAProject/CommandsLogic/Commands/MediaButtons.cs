using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace VAProject.CommandsLogic.Commands
{
    internal class MediaButtons : IVoiceCommand
    {
        public List<string> Triggers { get; } = new List<string>()
        {
            "play",
            "stop",
            "next",
            "skip",
            "previous"
        };

        public string TTSResponse { get; }

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const byte VK_MEDIA_STOP = 0xB2;
        private const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        private const byte VK_MEDIA_PREV_TRACK = 0xB1;
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        public CommandResult OnExecute(string cmdText)
        {
            if (cmdText.Contains("play") || cmdText.Contains("pause"))
            {
                keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                return new CommandResult ( true, "Toggled play/pause.",  "Toggled playpause.");
            }
            else if (cmdText.Contains("stop"))
            {
                keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                return new CommandResult(true, "Stoped playback", "Stoped playback.");
            }
            else if (cmdText.Contains("next") || cmdText.Contains("skip"))
            {
                keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                return new CommandResult(true, "Skiped to next track", "Skiped");
            }
            else if (cmdText.Contains("previous"))
            {
                keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                return new CommandResult(true, "Went back to previous track.", "Went back to previous track.");
            }
            else
            {
                return new CommandResult
                (
                    success: false,
                    logMessage: $"No command found for input: '{cmdText}'",
                    ttsResponse: "Sorry, I didn't understand that command.",
                    commandType: CommandType.Unknown
                );
            }
        }
    }
}
