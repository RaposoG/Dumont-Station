// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Body.Events;
using Content.Shared.Body.Systems;
using Content.Shared.Buckle.Components;
using Content.Shared.Foldable;
using Content.Shared.PowerCell;

namespace Content.Shared._Dumont.Medical.Holomaca;

/// <summary>
/// Liga o estado dobrado da maca ao consumo de carga: desdobrar so funciona com
/// celula carregada, e a estase so vale enquanto ela estiver desdobrada e com carga.
/// </summary>
public sealed class HolomacaSystem : EntitySystem
{
    [Dependency] private readonly FoldableSystem _foldable = default!;
    [Dependency] private readonly SharedMetabolizerSystem _metabolizer = default!;
    [Dependency] private readonly SharedPowerCellSystem _cell = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<HolomacaComponent, FoldAttemptEvent>(OnFoldAttempt);
        SubscribeLocalEvent<HolomacaComponent, FoldedEvent>(OnFolded);
        SubscribeLocalEvent<HolomacaComponent, StrappedEvent>(OnStrapped);
        SubscribeLocalEvent<HolomacaComponent, UnstrappedEvent>(OnUnstrapped);
        SubscribeLocalEvent<HolomacaComponent, PowerCellSlotEmptyEvent>(OnCellEmpty);

        SubscribeLocalEvent<HolomacaBuckledComponent, GetMetabolicMultiplierEvent>(OnGetMultiplier);
    }

    private void OnFoldAttempt(Entity<HolomacaComponent> ent, ref FoldAttemptEvent args)
    {
        if (args.Cancelled || !args.Comp.IsFolded)
            return;

        if (!_cell.HasDrawCharge(ent.Owner))
            args.Cancelled = true;
    }

    private void OnFolded(Entity<HolomacaComponent> ent, ref FoldedEvent args)
    {
        _cell.SetDrawEnabled(ent.Owner, !args.IsFolded);
        UpdateMetabolisms(ent.Owner);
    }

    private void OnStrapped(Entity<HolomacaComponent> ent, ref StrappedEvent args)
    {
        EnsureComp<HolomacaBuckledComponent>(args.Buckle);
        _metabolizer.UpdateMetabolicMultiplier(args.Buckle);
    }

    private void OnUnstrapped(Entity<HolomacaComponent> ent, ref UnstrappedEvent args)
    {
        RemComp<HolomacaBuckledComponent>(args.Buckle);
        _metabolizer.UpdateMetabolicMultiplier(args.Buckle);
    }

    private void OnCellEmpty(Entity<HolomacaComponent> ent, ref PowerCellSlotEmptyEvent args)
    {
        if (!TryComp<FoldableComponent>(ent, out var foldable) || foldable.IsFolded)
            return;

        _foldable.TrySetFolded(ent.Owner, foldable, true);
    }

    private void OnGetMultiplier(Entity<HolomacaBuckledComponent> ent, ref GetMetabolicMultiplierEvent args)
    {
        if (!TryComp<BuckleComponent>(ent, out var buckle) || buckle.BuckledTo is not { } bed)
            return;

        if (!TryComp<HolomacaComponent>(bed, out var holomaca))
            return;

        if (TryComp<FoldableComponent>(bed, out var foldable) && foldable.IsFolded)
            return;

        if (!_cell.HasDrawCharge(bed))
            return;

        args.Multiplier *= holomaca.Multiplier;
    }

    private void UpdateMetabolisms(Entity<StrapComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        foreach (var buckled in ent.Comp.BuckledEntities)
        {
            _metabolizer.UpdateMetabolicMultiplier(buckled);
        }
    }
}
