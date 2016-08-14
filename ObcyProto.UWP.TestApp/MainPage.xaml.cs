using System;
using Windows.UI.Core;
using ObcyProto.UWP.Client.Identity;
using ObcyProto.UWP.Events;

namespace ObcyProto.UWP.TestApp
{
    public sealed partial class MainPage
    {
        private readonly Connection _connection;

        public MainPage()
        {
            InitializeComponent();

            _connection = new Connection();
            _connection.JsonRead += _connection_JSONRead;
            _connection.JsonWrite += _connection_JsonWrite;
            _connection.StrangerFound += _connection_StrangerFound;
            _connection.ConversationEnded += _connection_ConversationEnded;
            _connection.ConnectionAccepted += _connection_ConnectionAccepted;

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await _connection.ConnectAsync(); });
        }

        private async void _connection_JsonWrite(object sender, JsonEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DebugTextBox.Text += $"{e.JsonString}\n";
            });
        }

        private void _connection_ConnectionAccepted(object sender, ConnectionAcceptedEventArgs e)
        {
            _connection.SearchForStranger(Location.WholePoland);
        }

        private void _connection_ConversationEnded(object sender, ConversationEndedEventArgs e)
        {
            _connection.SearchForStranger(Location.WholePoland);
        }

        private void _connection_StrangerFound(object sender, StrangerFoundEventArgs e)
        {
            _connection.SendMessage("Yayayayayayayayayayayayayayayayayayayayayayayay.");
        }

        private async void _connection_JSONRead(object sender, JsonEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DebugTextBox.Text += $"{e.JsonString}\n";
            });
        }
    }
}
