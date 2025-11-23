using UnityEngine;

namespace Sjur.Buildings
{
    /// <summary>
    /// Defines the types of buildings that can be constructed
    /// </summary>
    public enum BuildingType
    {
        LoggingCamp,    // Generates Gold
        Mine,           // Generates Iron
        MageTower,      // Generates Arcane (requires Gold and Iron to build)
        Base,           // Main base structure
        CenterObjective // Center point that provides bonuses
    }
}
