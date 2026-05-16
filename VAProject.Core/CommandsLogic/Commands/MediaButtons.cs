using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Windows.Media.TextFormatting;
using VAProject.Core.Interfaces;
using VAProject.Core.Models.Notifications;

namespace VAProject.Core.CommandsLogic.Commands
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
                return new CommandResult
                {
                    Success = true,
                    LogMessage = "Toggled play/pause.",
                    TTSResponse = "Toggled playpause.",
                    NotificationPayload = new TextPayload
                    {
                        Text = "Toggled play/pause.",
                        AccentColor = System.Windows.Media.Colors.Green
                    }
                };
            }
            else if (cmdText.Contains("stop"))
            {
                keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                keybd_event(VK_MEDIA_STOP, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                return new CommandResult
                {
                    Success = true,
                    LogMessage = "Stopped playback.",
                    TTSResponse = "Stopped playback.",
                    NotificationPayload = new TextPayload
                    {
                        Text = "Stopped playback.",
                        AccentColor = System.Windows.Media.Colors.Green
                    }
                };
            }
            else if (cmdText.Contains("next") || cmdText.Contains("skip"))
            {
                keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                return new CommandResult
                {
                    Success = true,
                    LogMessage = "Skipped to next track.",
                    TTSResponse = "Skipped to next track.",
                    NotificationPayload = new TextPayload
                    {
                        Text = "Skipped to next track.",
                        AccentColor = System.Windows.Media.Colors.Green
                    }
                };
            }
            else if (cmdText.Contains("previous"))
            {
                keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
                keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
                return new CommandResult
                {
                    Success = true,
                    LogMessage = "Went back to previous track.",
                    TTSResponse = "Went back to previous track.",
                    NotificationPayload = new TextPayload
                    {
                        Text = "Went back to previous track.",
                        AccentColor = System.Windows.Media.Colors.Green
                    }
                };
            }
            else
            {
                return new CommandResult
                {
                    Success = false,
                    LogMessage = $"No command found for input: '{cmdText}'",
                    TTSResponse = "Sorry, I didn't understand that command.",
                    NotificationPayload = new TextPayload
                    {
                        Text = $"No command found for input: '{cmdText}'",
                        AccentColor = System.Windows.Media.Colors.Red
                    }
                };
            }
        }
    }
}
