using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using UnityEngine;
using UnityModManagerNet;
using HarmonyLib;

using DV.Logic.Job;

namespace DvMilk
{
	class Main
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

			if (AccessTools.Field(typeof(ResourceTypes), "cargoToFullCargoDamagePrice")?.GetValue(null)
					is Dictionary<CargoType, float> c2fcdp)
			{
				c2fcdp[MILK] = 0f;
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk full cargo damage price");
			}

			if (AccessTools.Field(typeof(ResourceTypes), "cargoToFullEnvironmentDamagePrice")?.GetValue(null)
					is Dictionary<CargoType, float> c2fedp)
			{
				c2fedp[MILK] = 16000f;
			}
			else
			{
				modEntry.Logger.Warning("Failed to add milk full cargo enviromental damage price");
			}

			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			modEntry.Logger.Log("Patched methods");
		}
	}

	[HarmonyPatch(typeof(WarehouseMachine), MethodType.Constructor)]
	[HarmonyPatch(new Type[] { typeof(Track), typeof(List<CargoType>) })]
	static class WarehouseMachine_Constructor_Patch
	{
		static void Postfix(ref Track WarehouseTrack, ref List<CargoType> SupportedCargoTypes)
		{
			if (WarehouseTrack.ID.yardId == "FF" || WarehouseTrack.ID.yardId == "FM")
			{
				SupportedCargoTypes.Add(Main.MILK);
				
			}
		}
	}

	[HarmonyPatch(typeof(WarehouseMachineController), "Start")]
	[HarmonyPriority(Priority.Low)]
	class WarhouseMachineController_Start_Patch
	{
		static void Postfix(WarehouseMachineController __instance, ref string ___supportedCargoTypesText)
		{
			string yardID = __instance.warehouseMachine.WarehouseTrack.ID.yardId;
			Console.WriteLine("[DvMilk] " + yardID);
			if (yardID == "FM" || yardID == "FF")
			{
				if (!__instance.supportedCargoTypes.Contains(Main.MILK))
				{
					__instance.supportedCargoTypes.Add(Main.MILK);
				}
				Console.WriteLine("[DvMilk] patching supported cargo types text");
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < __instance.supportedCargoTypes.Count; i++)
				{
					stringBuilder.AppendLine(__instance.supportedCargoTypes[i].GetCargoName());
				}
				stringBuilder.AppendLine("Milk");
				___supportedCargoTypesText = stringBuilder.ToString();
				Console.WriteLine("[DvMilk] appended Milk to warehouse machine");
				
			}
		}
	}

	[HarmonyPatch(typeof(StationProceduralJobGenerator), MethodType.Constructor)]
	[HarmonyPatch(new Type[] { typeof(StationController) })]
	static class StationProcedualJobGenerator_Constructor_Patch
	{
		static void Postfix(ref StationController stationController)
		{
			if (stationController.stationInfo.YardID == "FF")
			{
				foreach (CargoGroup cargoGroup in stationController.proceduralJobsRuleset.inputCargoGroups)
				{
					if (cargoGroup.cargoTypes.Contains(CargoType.Corn))
					{
						cargoGroup.cargoTypes.Add(Main.MILK);
						break;
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
						break;
					}
				}
			}
		}
	}



}