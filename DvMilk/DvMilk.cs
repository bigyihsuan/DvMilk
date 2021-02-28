using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityModManagerNet;
using HarmonyLib;

using DV.Logic.Job;

namespace DvMilk
{
	static class Main
	{
		static void Load(UnityModManager.ModEntry modEntry)
		{
			Debug.Log("[DvMilk] Loaded DvMilk");
			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			Debug.Log("[DvMilk] Patched methods");
		}
	}

	[HarmonyPatch(typeof(CargoTypes), "ContainerTypeToCarTypes", MethodType.Getter)]
	class ContainerTypeToCarTypes_Getter_Patch
	{
		// Before the method runs, add in Milk to the dictionary if it doesn't exist
		static bool Postfix(ref Dictionary<CargoContainerType, List<TrainCarType>> __result)
		{
			Debug.Log("[DvMilk] ContainerTypeToCarTypes getter called");
			if (!__result.TryGetValue((CargoContainerType)3000, out var value))
			{
				Debug.Log("[DvMilk] ContainerTypeToCarTypes getter: Milk not added");
				var milkCars = new List<TrainCarType>() { TrainCarType.TankBlue, TrainCarType.TankOrange };
				__result.Add((CargoContainerType)3000, milkCars);
				Debug.Log("[DvMilk] ContainerTypeToCarTypes getter: Added Milk");
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(CargoTypes), "GetCargoMass")]
	class GetCargoMass_Patch
	{
		
		static bool Prefix(CargoType cargoType, float cargoAmount)
		{
			if (cargoAmount == 3000) { }
			return true;
		}
	}
}