using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nex_PoliceEscort
{
    public class Config
    {
        // Max Following Vehicle Speed
        public static float VehicleFollowingSpeed { get; set; } = 100f; // Seems to be fast & safe.

        // Minimum Vehicle Infront Following Distance
        public static float VehicleFollowingDistanceInfront { get; set; } = 7f; // Seems to be fast & safe.

        // Minimum Vehicle Behind Following Distance
        public static float VehicleFollowingDistanceBehind { get; set; } = 32f; // Seems to be fast & safe.
    }
}
