// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2025 Ilya246
// SPDX-FileCopyrightText: 2025 shab00m
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking;

[Serializable, NetSerializable]
public enum SignalTimerUiKey : byte
{
    Key
}

/// <summary>
/// Represents a SignalTimerComponent state that can be sent to the client
/// </summary>
[Serializable, NetSerializable]
public sealed class SignalTimerBoundUserInterfaceState : BoundUserInterfaceState
{
    public string CurrentText;
    public TimeSpan CurrentDelay; // Mono
    public bool CurrentRepeat; //Frontier
    public bool ShowText;
    public TimeSpan TriggerTime;
    public bool TimerStarted;
    public bool HasAccess;

    public SignalTimerBoundUserInterfaceState(string currentText,
        TimeSpan currentDelay, // Mono
        bool currentRepeat, //Frontier
        bool showText,
        TimeSpan triggerTime,
        bool timerStarted,
        bool hasAccess)
    {
        CurrentText = currentText;
        CurrentDelay = currentDelay; // Mono
        CurrentRepeat = currentRepeat; //Frontier
        ShowText = showText;
        TriggerTime = triggerTime;
        TimerStarted = timerStarted;
        HasAccess = hasAccess;
    }
}

[Serializable, NetSerializable]
public sealed class SignalTimerTextChangedMessage : BoundUserInterfaceMessage
{
    public string Text { get; }

    public SignalTimerTextChangedMessage(string text)
    {
        Text = text;
    }
}

//Frontier: SignalTimerRepeatToggled class
[Serializable, NetSerializable]
public sealed class SignalTimerRepeatToggled : BoundUserInterfaceMessage
{
    public bool Repeat { get; }

    public SignalTimerRepeatToggled(bool repeat)
    {
        Repeat = repeat;
    }
}
//End Frontier

[Serializable, NetSerializable]
public sealed class SignalTimerDelayChangedMessage : BoundUserInterfaceMessage
{
    public TimeSpan Delay { get; }
    public SignalTimerDelayChangedMessage(TimeSpan delay)
    {
        Delay = delay;
    }
}

[Serializable, NetSerializable]
public sealed class SignalTimerStartMessage : BoundUserInterfaceMessage
{

}
