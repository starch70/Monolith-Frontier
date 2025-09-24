// SPDX-FileCopyrightText: 2025 Ilya246
//
// SPDX-License-Identifier: MPL-2.0

using Content.Shared._Starlight.CollectiveMind;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.GameObjects;

namespace Content.Shared._Starlight.CollectiveMind;

public sealed class CollectiveMindUpdateSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    private static Dictionary<string, int> _currentId = new();

    public void UpdateCollectiveMind(EntityUid uid, CollectiveMindComponent collective)
    {
        foreach (var prototype in _prototypeManager.EnumeratePrototypes<CollectiveMindPrototype>())
        {
            if (!_currentId.ContainsKey(prototype.ID))
                _currentId[prototype.ID] = 0;

            bool hasChannel = collective.Channels.Contains(prototype.ID);
            bool alreadyAdded = collective.Minds.ContainsKey(prototype.ID);
            if (hasChannel && !alreadyAdded)
                collective.Minds.Add(prototype.ID, ++_currentId[prototype.ID]);
            else if (!hasChannel && alreadyAdded)
                collective.Minds.Remove(prototype.ID);
        }
    }
}
