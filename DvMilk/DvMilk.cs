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
		static bool Load(UnityModManager.ModEntry modEntry)
		{
			Debug.Log("[DvMilk] Loaded DvMilk");

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

			if (AccessTools.Field(typeof(CargoTypes), "cargoSpecficDisplayName")?.GetValue(null)
					is Dictionary<CargoType, string> cspdn)
			{
				cspdn[MILK] = "Milk";
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

			if (AccessTools.Field(typeof(ResourceTypes), "cargoToFullCargoDamagePrice")?.GetValue(null)
					is Dictionary<CargoType, float> cdpDict)
			{
				cdpDict[MILK] = 0f;
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk damage cost");
			}

			if (AccessTools.Field(typeof(ResourceTypes), "cargoToFullEnvironmentDamagePrice")?.GetValue(null)
					is Dictionary<CargoType, float> cedpDict)
			{
				cedpDict[MILK] = 0f;
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk enviromental damage cost");
			}

			Debug.Log("[DvMilk] Patched methods");

			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			return true;
		}
	}

	[HarmonyPatch(typeof(WarehouseMachine), MethodType.Constructor)]
	static class WarehouseMachine_Constructor_Patch
	{
		static bool Prefix(Track WarehouseTrack, List<CargoType> SupportedCargoTypes)
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
		static bool Prefix(StationController stationController)
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
				return true;
		}
	}
	

	
}