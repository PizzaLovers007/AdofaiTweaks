namespace AdofaiTweaks.Utils;

/// <summary>
/// Extension methods for comparing planets.
/// </summary>
public static class PlanetComparison {
    /// <summary>
    /// Checks if the planet is a red planet. Compares with <see cref="PlanetRenderer"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a red planet.</returns>
    public static bool IsRedPlanet(this PlanetRenderer planet) {
        return planet == PlanetGetter.RedPlanet.planetRenderer;
    }

    /// <summary>
    /// Checks if the planet is a blue planet. Compares with <see cref="PlanetRenderer"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a blue planet.</returns>
    public static bool IsBluePlanet(this PlanetRenderer planet) {
        return planet == PlanetGetter.BluePlanet.planetRenderer;
    }

    /// <summary>
    /// Checks if the planet is a green planet. Compares with <see cref="PlanetRenderer"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a green planet.</returns>
    public static bool IsGreenPlanet(this PlanetRenderer planet) {
        return planet == PlanetGetter.GreenPlanet.planetRenderer;
    }

    /// <summary>
    /// Checks if the planet is a red planet. Compares with <see cref="scrPlanet"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a red planet.</returns>
    public static bool IsRedPlanetLegacy(this scrPlanet planet) {
        return planet == PlanetGetter.RedPlanet;
    }

    /// <summary>
    /// Checks if the planet is a red planet. Compares with <see cref="scrPlanet"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a red planet.</returns>
    public static bool IsBluePlanetLegacy(this scrPlanet planet) {
        return planet == PlanetGetter.BluePlanet;
    }

    /// <summary>
    /// Checks if the planet is a green planet. Compares with <see cref="scrPlanet"/>.
    /// </summary>
    /// <param name="planet">Planet to check from.</param>
    /// <returns><c>true</c> if the planet is a green planet.</returns>
    public static bool IsGreenPlanetLegacy(this scrPlanet planet) {
        return planet == PlanetGetter.GreenPlanet;
    }
}