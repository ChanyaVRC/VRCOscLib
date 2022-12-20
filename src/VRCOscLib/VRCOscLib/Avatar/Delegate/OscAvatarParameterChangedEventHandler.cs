namespace BuildSoft.VRChat.Osc.Avatar;

/// <summary>
/// Represents a delegate for handling changes to an <see cref="OscAvatarParameter"/>.
/// </summary>
/// <param name="parameter">The <see cref="OscAvatarParameter"/> that has changed.</param>
/// <param name="e">The <see cref="ValueChangedEventArgs"/> containing the old and new values.</param>
public delegate void OscAvatarParameterChangedEventHandler(OscAvatarParameter parameter, ValueChangedEventArgs e);
