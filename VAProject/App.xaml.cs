using System.Windows;
using System.IO;
using VAProject.UI;
using VAProject.Core;
using VAProject.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using VAProject.Core.Interfaces;
using VAProject.Core.CommandsLogic;
using VAProject.Core.Utils.EventBus;
using VAProject.Core.Utils.EventBus.Events;
using VAProject.Core.Utils.APIProxy;
using VAProject.Core.CommandsLogic.CommandDecorators;
using System.Reflection;

namespace VAProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private VACore _VACore;

        private NotifyIcon? _trayIcon;
        private MainWindow? _settingsWindow;
        private NotificationWindow? _notificationWindow;

        private ISubscription _micStateSubscription;

        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // di-container
            ServiceCollection services = new ServiceCollection();

            _notificationWindow = new NotificationWindow();
            services.AddSingleton<INotificationService>(new NotificationService(_notificationWindow));
            services.AddSingleton<EventBus>(new EventBus());

            services.AddTransient<ApiKeyProxiHandler>();
            services.AddHttpClient("WeatherApi").AddHttpMessageHandler<ApiKeyProxiHandler>();

            services.AddSingleton<IStatisticTracker, JsonStatisticsTracker>();

            Assembly coreAssembly = typeof(IVoiceCommand).Assembly;
            IEnumerable<Type> commandTypes = coreAssembly.GetTypes()
                .Where(t => typeof(IVoiceCommand).IsAssignableFrom(t) 
                    && !t.IsInterface 
                    && !t.IsAbstract
                    && t != typeof(AnalyticsDecorator));
            foreach (var type in commandTypes)
            {
                services.AddTransient(typeof(IVoiceCommand), provider => CreateAndDecorateCommand(provider, type));
            }

            services.AddSingleton<CommandRouter>();
            services.AddSingleton<VACore>();

            ServiceProvider = services.BuildServiceProvider();

            _VACore = ServiceProvider.GetRequiredService<VACore>();

            InitializeTrayIcon();

            EventBus eventBus = ServiceProvider.GetRequiredService<EventBus>();
            _micStateSubscription = eventBus.Subscribe<MicStateChangedEvent>((msg) => HandleMicStateChange(msg.State));

            _VACore.Start(); 
        }

        private AnalyticsDecorator CreateAndDecorateCommand(IServiceProvider provider, Type type)
        {
            IVoiceCommand originalCommand = (IVoiceCommand)ActivatorUtilities.CreateInstance(provider, type);

            IStatisticTracker tracker = provider.GetRequiredService<IStatisticTracker>();

            return new AnalyticsDecorator(originalCommand, tracker);
        }

        private void InitializeTrayIcon()
        {
            Icon icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons", "icon.ico"));

            _trayIcon = new NotifyIcon
            {
                Icon = icon,
                Visible = true,
                Text = "Voice Assistant"
            };

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            
            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += ShowSettingsWindow;

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += ExitApp;

            contextMenu.Items.Add(settingsItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenuStrip = contextMenu;

            _trayIcon.DoubleClick += ShowSettingsWindow;
        }

        private void ShowSettingsWindow(object sender, EventArgs e)
        {
            if (_settingsWindow == null || !_settingsWindow.IsLoaded)
            {
                _settingsWindow = new MainWindow();
            }

            _settingsWindow.Show();
            _settingsWindow.Activate();
        }

        private void ExitApp(object sender, EventArgs e)
        {
            if(_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }

            _VACore.Stop();


            Environment.Exit(0);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _micStateSubscription.Dispose();
            _VACore.Stop();

            base.OnExit(e);
        }

        private void HandleMicStateChange(MicStates micState)
        {
            Current.Dispatcher.Invoke(() =>
            {
                if (_settingsWindow != null && _settingsWindow.IsLoaded)
                {
                    _settingsWindow?.SetMicrophoneStatus(micState);
                }
            });
        }
    }
}
