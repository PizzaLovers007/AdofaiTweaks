using JetBrains.Annotations;

namespace AdofaiTweaks.Utils;

/// <summary>
/// Extension methods for comparing planets.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PlanetComparison {
    /// <summary>
    /// Checks if the planet is a red planet. Compares with <see cref="PlanetRenderer"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a red planet.</returns>
    public static bool IsRedPlanet(this PlanetRenderer planet) {
        return planet && planet == PlanetGetter.RedPlanet?.planetRenderer;
    }

    /// <summary>
    /// Checks if the planet is a blue planet. Compares with <see cref="PlanetRenderer"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a blue planet.</returns>
    public static bool IsBluePlanet(this PlanetRenderer planet) {
        return planet && planet == PlanetGetter.BluePlanet?.planetRenderer;
    }

    /// <summary>
    /// Checks if the planet is a green planet. Compares with <see cref="PlanetRenderer"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a green planet.</returns>
    public static bool IsGreenPlanet(this PlanetRenderer planet) {
        return planet && planet == PlanetGetter.GreenPlanet?.planetRenderer;
    }

    /// <summary>
    /// Checks if the planet is a red planet. Compares with <see cref="scrPlanet"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a red planet.</returns>
    public static bool IsRedPlanetLegacy(this scrPlanet planet) {
        return planet && planet == PlanetGetter.RedPlanet;
    }

    /// <summary>
    /// Checks if the planet is a red planet. Compares with <see cref="scrPlanet"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a red planet.</returns>
    public static bool IsBluePlanetLegacy(this scrPlanet planet) {
        return planet && planet == PlanetGetter.BluePlanet;
    }

    /// <summary>
    /// Checks if the planet is a green planet. Compares with <see cref="scrPlanet"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a green planet.</returns>
    public static bool IsGreenPlanetLegacy(this scrPlanet planet) {
        return planet && planet == PlanetGetter.GreenPlanet;
    }

    /// <summary>
    /// Checks if the planet is a fake planet (not red, blue, or green). Compares with <see cref="PlanetRenderer"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a fake planet.</returns>
    public static bool IsFake(PlanetRenderer planet) {
        return !planet || (!planet.IsRedPlanet() && !planet.IsBluePlanet() && !planet.IsGreenPlanet());
    }

    /// <summary>
    /// Checks if the planet is a fake planet (not red, blue, or green). Compares with <see cref="scrPlanet"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a fake planet.</returns>
    public static bool IsFakeLegacy(scrPlanet planet) {
        return !planet || (!planet.IsRedPlanetLegacy() && !planet.IsBluePlanetLegacy() && !planet.IsGreenPlanetLegacy());
    }
}