// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 Ilya246
// SPDX-FileCopyrightText: 2025 shab00m
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.MachineLinking;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.Timing;

namespace Content.Client.MachineLinking.UI;

public sealed class SignalTimerBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private SignalTimerWindow? _window;

    public SignalTimerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<SignalTimerWindow>();
        _window.OnStartTimer += StartTimer;
        _window.OnCurrentTextChanged += OnTextChanged;
        _window.OnCurrentRepeatChanged += OnRepeatChanged; // Frontier
        _window.OnCurrentDelayChanged += OnDelayChanged; // Mono
    }

    public void StartTimer()
    {
        SendMessage(new SignalTimerStartMessage());
    }

    private void OnTextChanged(string newText)
    {
        SendMessage(new SignalTimerTextChangedMessage(newText));
    }

    // Frontier: Handle Repeat changed
    private void OnRepeatChanged(bool newRepeat)
    {
        SendMessage(new SignalTimerRepeatToggled(newRepeat));
    }
    //End Frontier

    private void OnDelayChanged(TimeSpan newDelay) // Mono
    {
        if (_window == null)
            return;
        SendMessage(new SignalTimerDelayChangedMessage(newDelay)); // Mono
    }

    /// <summary>
    /// Update the UI state based on server-sent info
    /// </summary>
    /// <param name="state"></param>
    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not SignalTimerBoundUserInterfaceState cast)
            return;

        _window.SetCurrentText(cast.CurrentText);
        _window.SetCurrentDelay(cast.CurrentDelay); // Mono
        _window.SetCurrentRepeat(cast.CurrentRepeat); // Frontier
        _window.SetShowText(cast.ShowText);
        _window.SetTriggerTime(cast.TriggerTime);
        _window.SetTimerStarted(cast.TimerStarted);
        _window.SetHasAccess(cast.HasAccess);
    }
}
