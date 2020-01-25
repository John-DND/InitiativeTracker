using System;

namespace InitiativeTracker.Data.Util
{
    public delegate void SettingsUpdatedEventHandler(object sender, SettingsUpdatedEventArgs e);

    public class SettingsUpdatedEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public SettingsUpdatedEventArgs(string name)
        {
            Name = name;
        }
    }

    /*
     * Simple event holder class that a few different parts of the program listen to. enables classes
     * to cache settings values and update them only when this event is raised. querying the program
     * settings is not a free operation!
     *
     * still, this is probably pointless. haven't bothered to test
     */
    public static class SettingsManager
    {
        public static event SettingsUpdatedEventHandler OnSettingsUpdate;

        public static void RaiseSettingsUpdate(string setting)
        {
            OnSettingsUpdate?.Invoke(null, new SettingsUpdatedEventArgs(setting));
        }
    }
}
