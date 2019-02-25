using FluidIrc.Model.Models;
using IrcDotNet;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;

namespace FluidIrc.Services
{
    public class FluidIrcClient : StandardIrcClient, IIrcClient
    {
        public event Action<IrcChannel> JoinedChannel;
        public event Action<IrcChannel> LeftChannel;

        public bool IsConnecting { get; private set; }
        public Server CurrentServer { get; private set; }

        public FluidIrcClient()
        {
            SendMessageInfo();

            Registered += (sender, args) =>
            {
                LocalUser.JoinedChannel += LocalUserOnJoinedChannel;
                LocalUser.LeftChannel += LocalUserOnLeftChannel;
            };

            Disconnected += OnDisconnected;
            Connected += OnConnected;
            ConnectFailed += OnConnectFailed;
        }

        private void OnConnectFailed(object sender, IrcErrorEventArgs e)
        {
            IsConnecting = false;
        }

        private void OnConnected(object sender, EventArgs e)
        {
            IsConnecting = false;
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            CurrentServer = null;
        }

        private void LocalUserOnLeftChannel(object sender, IrcChannelEventArgs e)
        {
            LeftChannel?.Invoke(e.Channel);
        }

        private void LocalUserOnJoinedChannel(object sender, IrcChannelEventArgs e)
        {
             JoinedChannel?.Invoke(e.Channel);
        }

        public void Connect(Server server)
        {
            IsConnecting = true;

            var userInfo = new IrcUserRegistrationInfo
            {
                NickName = server.UserProfile.Nickname,
                RealName = server.UserProfile.Nickname,
                UserName = server.UserProfile.Nickname
            };

            if (server.Port <= 0)
            {
                base.Connect(server.Address, server.SslEnabled, userInfo);
            }
            else
            {
                base.Connect(server.Address, server.Port, server.SslEnabled, userInfo);
            }

            CurrentServer = server;
        }

        public void SendMessage(IrcChannel channel, string message)
        {
            SendMessagePrivateMessage(new List<string> {channel.Name}, message);
        }

        public void SendCommand(string command, params string[] parameters)
        {
            SendRawMessage($"{command} {parameters.Join(" ")}");
        }

    }
}
