// SPDX-FileCopyrightText: 2022 Chief-Engineer
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 Ilya246
//
// SPDX-License-Identifier: MPL-2.0

using System.Numerics;
using Content.Shared._Starlight.CollectiveMind; // Goobstation - Starlight collective mind port
using Content.Shared.Chat;

namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelSelectorButton : ChatPopupButton<ChannelSelectorPopup>
{
    public event Action<ChatSelectChannel>? OnChannelSelect;

    public ChatSelectChannel SelectedChannel { get; private set; }

    private const int SelectorDropdownOffset = 38;

    public ChannelSelectorButton()
    {
        Name = "ChannelSelector";

        Popup.Selected += OnChannelSelected;

        if (Popup.FirstChannel is { } firstSelector)
        {
            Select(firstSelector);
        }
    }

    protected override UIBox2 GetPopupPosition()
    {
        var globalLeft = GlobalPosition.X;
        var globalBot = GlobalPosition.Y + Height;
        return UIBox2.FromDimensions(
            new Vector2(globalLeft, globalBot),
            new Vector2(SizeBox.Width, SelectorDropdownOffset));
    }

    private void OnChannelSelected(ChatSelectChannel channel)
    {
        Select(channel);
    }

    public void Select(ChatSelectChannel channel)
    {
        if (Popup.Visible)
        {
            Popup.Close();
        }

        if (SelectedChannel == channel)
            return;
        SelectedChannel = channel;
        OnChannelSelect?.Invoke(channel);
    }

    public static string ChannelSelectorName(ChatSelectChannel channel)
    {
        return Loc.GetString($"hud-chatbox-select-channel-{channel}");
    }

    public Color ChannelSelectColor(ChatSelectChannel channel)
    {
        return channel switch
        {
            ChatSelectChannel.Radio => Color.LimeGreen,
            ChatSelectChannel.LOOC => Color.MediumTurquoise,
            ChatSelectChannel.OOC => Color.LightSkyBlue,
            ChatSelectChannel.Dead => Color.MediumPurple,
            ChatSelectChannel.Admin => Color.HotPink,
            _ => Color.DarkGray
        };
    }

    // Goobstation - Starlight collective mind port
    public void UpdateChannelSelectButton(ChatSelectChannel channel, Shared.Radio.RadioChannelPrototype? radio, CollectiveMindPrototype? collectiveMind = null)
    {
        if (radio != null)
        {
            Text = Loc.GetString(radio.Name);
            Modulate = radio?.Color ?? ChannelSelectColor(channel);
        }
        else if (collectiveMind != null)
        {
            Text = Loc.GetString(collectiveMind.Name);
            Modulate = collectiveMind.Color;
        }
        else
        {
            Text = ChannelSelectorName(channel);
            Modulate = ChannelSelectColor(channel);
        }
    }
}
