// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._Dumont.Medical.Holomaca;

/// <summary>
/// Maca dobravel que so funciona com carga. Desdobrada consome a celula e deixa
/// o metabolismo de quem esta deitado mais lento, dobrada vira item e nao gasta nada.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HolomacaComponent : Component
{
    /// <summary>
    /// Por quanto o metabolismo de quem esta deitado e multiplicado. Maior = mais lento.
    /// A cama de estase de verdade usa 10.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Multiplier = 5f;
}
