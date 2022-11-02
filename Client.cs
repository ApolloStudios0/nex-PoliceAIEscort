using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.UI.Screen;
using static CitizenFX.Core.Native.API;

namespace nex_PoliceEscort
{
    public class Client : BaseScript
    {
        public static Vehicle PoliceCar1 { get; set; }
        public static Vehicle PoliceCar2 { get; set; }

        public static Ped PolicePed1_ { get; set; }
        public static Ped PolicePed2_ { get; set; }

        public Client()
        {
            // [!] Command Register
            RegisterCommand("PoliceEscort", new Action<dynamic, List<dynamic>, string>((dynamic source, List<dynamic> args, string rawCommand) =>
            {
                EscortMyVehicle();
            }), false);

            // [!] Command Register
            RegisterCommand("StopEscort", new Action<dynamic, List<dynamic>, string>((dynamic source, List<dynamic> args, string rawCommand) =>
            {
                StopEscort();
            }), false);
        }

        public static async void StopEscort()
        {
            TaskVehicleDriveWander(PolicePed1_.Handle, PoliceCar1.Handle, 60f, 60);
            TaskVehicleDriveWander(PolicePed2_.Handle, PoliceCar2.Handle, 60f, 60);

            await Delay(60000);

            PoliceCar1.Delete();
            PoliceCar2.Delete();
            PolicePed1_.Delete();
            PolicePed2_.Delete();
        }

        public static async void EscortMyVehicle()
        {
            // [!] Basic Vehicle Checks
            if (Game.PlayerPed.IsInVehicle())
            {
                // Is Driver?
                var veh = GetVehicle();
                if (Game.PlayerPed == veh.Driver)
                {
                    // Get Player Coords
                    var PlayerCoords = GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0, 25f, 0.1f) + new Vector3(0f, 0f, 1f);

                    // Spawn Police Car Nearby
                    await LoadModel((uint)VehicleHash.Police3);
                    Vehicle vehicle = new Vehicle(CreateVehicle((uint)VehicleHash.Police3, PlayerCoords.X, PlayerCoords.Y, PlayerCoords.Z, GetEntityHeading(Game.PlayerPed.Handle), true, false))
                    {
                        NeedsToBeHotwired = false,
                        PreviouslyOwnedByPlayer = true,
                        IsPersistent = true,
                        IsStolen = false,
                        IsWanted = false
                    };
                    PoliceCar1 = vehicle;
                    Vehicle vehicle2 = new Vehicle(CreateVehicle((uint)VehicleHash.Police3, PlayerCoords.X + 10f, PlayerCoords.Y + 10f, PlayerCoords.Z, GetEntityHeading(Game.PlayerPed.Handle), true, false))
                    {
                        NeedsToBeHotwired = false,
                        PreviouslyOwnedByPlayer = true,
                        IsPersistent = true,
                        IsStolen = false,
                        IsWanted = false
                    };
                    PoliceCar2 = vehicle2;

                    // Spawn & Set Ped Into Vehicle
                    RequestModel((uint)PedHash.Cop01SMY);
                    while (!HasModelLoaded((uint)PedHash.Cop01SMY))
                    {
                        await Delay(0);
                    }
                    var PolicePed = await World.CreatePed(PedHash.Cop01SMY, vehicle.Position);
                    PolicePed1_ = PolicePed;
                    var PolicePed2 = await World.CreatePed(PedHash.Cop01SMY, vehicle2.Position);
                    PolicePed2_ = PolicePed2;

                    // Set Ped As Driver
                    SetPedIntoVehicle(PolicePed.Handle, vehicle.Handle, -1);
                    SetPedIntoVehicle(PolicePed2.Handle, vehicle2.Handle, -1);
                    SetVehicleSiren(vehicle.Handle, true);
                    SetVehicleSiren(vehicle2.Handle, true);

                    // Escort Player
                    TaskVehicleEscort(PolicePed.Handle, vehicle.Handle, veh.Handle, -1, Config.VehicleFollowingSpeed, 262716, Config.VehicleFollowingDistanceBehind, 0, 20f); // behind
                    TaskVehicleEscort(PolicePed2.Handle, vehicle2.Handle, veh.Handle, 0, Config.VehicleFollowingSpeed, 262716, Config.VehicleFollowingDistanceInfront, 0, 20f); //front
                }
            }

            Vehicle GetVehicle(bool lastVehicle = false)
            {
                if (lastVehicle)
                {
                    return Game.PlayerPed.LastVehicle;
                }
                else
                {
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        return Game.PlayerPed.CurrentVehicle;
                    }
                }
                return null;
            }
        }

        private static async Task<bool> LoadModel(uint modelHash)
        {
            // Check if the model exists in the game.
            if (IsModelInCdimage(modelHash))
            {
                // Load the model.
                RequestModel(modelHash);
                // Wait until it's loaded.
                while (!HasModelLoaded(modelHash))
                {
                    await Delay(0);
                }
                // Model is loaded, return true.
                return true;
            }
            // Model is not valid or is not loaded correctly.
            else
            {
                // Return false.
                return false;
            }
        }
    }
}
