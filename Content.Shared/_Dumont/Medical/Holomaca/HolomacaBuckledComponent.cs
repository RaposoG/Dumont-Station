// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Dumont.Medical.Holomaca;

/// <summary>
/// Marca quem esta deitado numa <see cref="HolomacaComponent"/>, pra receber o
/// evento de metabolismo. Adicionado e removido pelo <see cref="HolomacaSystem"/>.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(HolomacaSystem))]
public sealed partial class HolomacaBuckledComponent : Component;
