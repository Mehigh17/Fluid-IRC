using FluidIrc.Model.Models;
using IrcDotNet;
using System;

namespace FluidIrc.Services
{
    public interface IIrcClient
    {

        #region Properties
        IrcLocalUser LocalUser { get; }
        bool IsConnected { get; }
        bool IsConnecting { get; }
        Server CurrentServer { get; }
        string MessageOfTheDay { get; }
        #endregion Properties

        #region Methods
        void Connect(Server server);
        void SendMessage(IrcChannel channel, string message);
        void SendCommand(string command, params string[] parameters);
        #endregion Methods

        #region Events
        event EventHandler<EventArgs> Connected;
        event EventHandler<IrcErrorEventArgs> ConnectFailed;
        event EventHandler<EventArgs> Disconnected;
        event EventHandler<IrcErrorEventArgs> Error;
        event EventHandler<IrcProtocolErrorEventArgs> ProtocolError;
        event EventHandler<IrcErrorMessageEventArgs> ErrorMessageReceived;
        event EventHandler<EventArgs> Registered;
        event EventHandler<EventArgs> ClientInfoReceived;
        event EventHandler<IrcServerInfoEventArgs> ServerBounce;
        event EventHandler<EventArgs> ServerSupportedFeaturesReceived;
        event EventHandler<EventArgs> MotdReceived;
        event EventHandler<IrcCommentEventArgs> NetworkInformationReceived;
        event EventHandler<IrcServerVersionInfoEventArgs> ServerVersionInfoReceived;
        event EventHandler<IrcServerTimeEventArgs> ServerTimeReceived;
        event EventHandler<IrcServerLinksListReceivedEventArgs> ServerLinksListReceived;
        event EventHandler<IrcServerStatsReceivedEventArgs> ServerStatsReceived;
        event EventHandler<IrcNameEventArgs> WhoReplyReceived;
        event EventHandler<IrcUserEventArgs> WhoIsReplyReceived;
        event EventHandler<IrcUserEventArgs> WhoWasReplyReceived;
        event EventHandler<IrcChannelListReceivedEventArgs> ChannelListReceived;

        event Action<IrcChannel> JoinedChannel;
        event Action<IrcChannel> LeftChannel;
        #endregion Events

    }
}
