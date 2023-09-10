using PlayState.Enums;
using PlayState.Models;

namespace PlayState.Events
{
    public class OnGameStatusSwitchedArgs
    {
        /// <summary>
        /// Gets PlayState Data initiating the event.
        /// </summary>
        public PlayStateData PlayStateData { get; internal set; }

        /// <summary>
        /// Gets the Notification Type initiating the event.
        /// </summary>
        public NotificationTypes NotificationType { get; internal set; }
    }
}