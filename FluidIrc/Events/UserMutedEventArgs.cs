using IrcDotNet;

namespace FluidIrc.Events
{
    public class UserMutedEventArgs
    {
        public UserMutedEventArgs(IrcUser user)
        {
            User = user;
        }

        public IrcUser User { get; }

    }
}
