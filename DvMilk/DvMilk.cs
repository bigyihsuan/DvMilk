using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityModManagerNet;
using HarmonyLib;

using DV.Logic.Job;

namespace DvMilk
{
	static class Main
	{
		public const CargoType MILK = (CargoType)3000;
		static void Load(UnityModManager.ModEntry modEntry)
		{
			modEntry.Logger.Log("Loaded DvMilk");

			if (AccessTools.Field(typeof(CargoTypes), "cargoTypeToSupportedCarContainer")?.GetValue(null)
					is Dictionary<CargoType, List<CargoContainerType>> ct2ct)
			{
				ct2ct[MILK] = new List<CargoContainerType>() { CargoContainerType.TankerChem, CargoContainerType.TankerGas };
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk container types");
			}

			if (AccessTools.Field(typeof(CargoTypes), "cargoTypeToCargoMassPerUnit")?.GetValue(null)
					is Dictionary<CargoType, float> ct2cmpu)
			{
				ct2cmpu[MILK] = 30_500f;
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk cargo mass");
			}

			if (AccessTools.Field(typeof(CargoTypes), "cargoSpecificDisplayName")?.GetValue(null)
					is Dictionary<CargoType, string> cspdn)
			{
				cspdn.Add(MILK, "Milk");
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk long name");
			}

			if (AccessTools.Field(typeof(CargoTypes), "cargoShortDisplayName")?.GetValue(null)
					is Dictionary<CargoType, string> cshdn)
			{
				cshdn[MILK] = "Milk";
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk short name");
			}

			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			modEntry.Logger.Log("Patched methods");
		}
	}

	[HarmonyPatch(typeof(WarehouseMachine), MethodType.Constructor)]
	static class WarehouseMachine_Constructor_Patch
	{
		static bool Prefix(ref Track WarehouseTrack, ref List<CargoType> SupportedCargoTypes)
		{
			if (WarehouseTrack.ID.yardId == "FF" || WarehouseTrack.ID.yardId == "FM")
			{
				SupportedCargoTypes.Add(Main.MILK);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(StationProceduralJobGenerator), MethodType.Constructor)]
	static class StationProcedualJobGenerator_Constructor_Patch
	{
		static void Prefix(ref StationController stationController)
		{
			if (stationController.stationInfo.YardID == "FF")
			{
				foreach (CargoGroup cargoGroup in stationController.proceduralJobsRuleset.inputCargoGroups)
				{
					if (cargoGroup.cargoTypes.Contains(CargoType.Corn))
					{
						cargoGroup.cargoTypes.Add(Main.MILK);
					}
				}
			}
			else if (stationController.stationInfo.YardID == "FM")
			{
				foreach (CargoGroup cargoGroup in stationController.proceduralJobsRuleset.outputCargoGroups)
				{
					if (cargoGroup.cargoTypes.Contains(CargoType.Corn))
					{
						cargoGroup.cargoTypes.Add(Main.MILK);
					}
				}
			}
		}
	}



}