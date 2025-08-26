// SPDX-FileCopyrightText: 2025 BombasterDS2
// SPDX-FileCopyrightText: 2025 GoobBot
// SPDX-FileCopyrightText: 2025 Ilya246
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client._Goobstation.Construction;
using Content.Client.Construction;
using Content.Shared._Goobstation.DragDrop;
using Content.Shared.DragDrop;
using Content.Shared.Interaction;
using Robust.Shared.Timing;

namespace Content.Client._Goobstation.DragDrop;

public sealed partial class GoobDragDropSystem : SharedGoobDragDropSystem
{
    [Dependency] private readonly ConstructionSystem _construction = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ConstructionComponent, DragDropTargetEvent>(OnDragDropConstruction);
        SubscribeLocalEvent<ConstructionComponent, CanDropTargetEvent>(CanDropTargetConstruction);

        SubscribeLocalEvent<DragDropTargetableComponent, DragDropTargetEvent>(OnDragDropTargetable);
        SubscribeLocalEvent<DragDropTargetableComponent, CanDropTargetEvent>(CanDropTargetTargetable);

        SubscribeLocalEvent<ConstructionGhostComponent, DragDropTargetEvent>(OnDragDropGhost);
        SubscribeLocalEvent<ConstructionGhostComponent, CanDropTargetEvent>(CanDropTargetGhost);
    }

    // this is cursed but making construction system code handle DragDropTargetEvent would be even more cursed
    // if it works it works
    private void OnDragDropConstruction(Entity<ConstructionComponent> ent, ref DragDropTargetEvent args)
    {
        OnDragDrop(ent, ref args);
    }

    private void CanDropTargetConstruction(Entity<ConstructionComponent> ent, ref CanDropTargetEvent args)
    {
        CanDropTarget(ent, ref args);
    }

    private void OnDragDropTargetable(Entity<DragDropTargetableComponent> ent, ref DragDropTargetEvent args)
    {
        OnDragDrop(ent, ref args);
    }

    private void CanDropTargetTargetable(Entity<DragDropTargetableComponent> ent, ref CanDropTargetEvent args)
    {
        CanDropTarget(ent, ref args);
    }

    private void OnDragDropGhost(Entity<ConstructionGhostComponent> ent, ref DragDropTargetEvent args)
    {
        if (!_timing.IsFirstTimePredicted || !CanDragDrop(args.User))
            return;

        _construction.TryStartConstruction(ent, ent.Comp, args.Dragged);
        args.Handled = true;
    }

    private void CanDropTargetGhost(Entity<ConstructionGhostComponent> ent, ref CanDropTargetEvent args)
    {
        CanDropTarget(ent, ref args);
    }
}
