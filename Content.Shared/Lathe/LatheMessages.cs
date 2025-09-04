// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 Ilya246
// SPDX-FileCopyrightText: 2025 Whatstone
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Research.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe;

[Serializable, NetSerializable]
public sealed class LatheUpdateState : BoundUserInterfaceState
{
    public List<ProtoId<LatheRecipePrototype>> Recipes;

    public List<LatheRecipeBatch> Queue; // Frontier: LatheRecipePrototype<LatheRecipeBatch

    public LatheRecipePrototype? CurrentlyProducing;

    public bool Looping = false; // Mono
    public bool Skipping = false; // Mono

    public LatheUpdateState(List<ProtoId<LatheRecipePrototype>> recipes, List<LatheRecipeBatch> queue, LatheRecipePrototype? currentlyProducing = null, bool looping = false, bool skipping = false) // Frontier: change queue type // Mono
    {
        Recipes = recipes;
        Queue = queue;
        CurrentlyProducing = currentlyProducing;
        Looping = looping; // Mono
        Skipping = skipping; // Mono
    }
}

/// <summary>
///     Sent to the server to sync material storage and the recipe queue.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheSyncRequestMessage : BoundUserInterfaceMessage
{

}

/// <summary>
///     Sent to the server when a client queues a new recipe.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheQueueRecipeMessage : BoundUserInterfaceMessage
{
    public readonly string ID;
    public readonly int Quantity;
    public LatheQueueRecipeMessage(string id, int quantity)
    {
        ID = id;
        Quantity = quantity;
    }
}

// Mono
/// <summary>
///     Sent to the server when a client wants to change whether the lathe should loop.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheSetLoopingMessage : BoundUserInterfaceMessage
{
    public readonly bool ShouldLoop;
    public LatheSetLoopingMessage(bool shouldLoop)
    {
        ShouldLoop = shouldLoop;
    }
}

// Mono
/// <summary>
///     Sent to the server when a client wants to change whether the lathe should skip over unavailable recipes.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheSetSkipMessage : BoundUserInterfaceMessage
{
    public readonly bool ShouldSkip;
    public LatheSetSkipMessage(bool shouldSkip)
    {
        ShouldSkip = shouldSkip;
    }
}

// Mono
/// <summary>
///     Sent to the server when a client wants to de-queue a recipe from the lathe.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheRecipeCancelMessage : BoundUserInterfaceMessage
{
    public readonly int Index;
    public LatheRecipeCancelMessage(int index)
    {
        Index = index;
    }
}

[NetSerializable, Serializable]
public enum LatheUiKey
{
    Key,
}
