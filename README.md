```
Station IDs: FF, FM
	Food Factory, Farm
Farm generates Milk cargo, loadable from all loading tracks
Food Factory consnumes Milk cargo, unloadable from all loading tracks
Milk mass:
	Milk density ~ 1.03 kg/L
	Crude oil mass/unit = 26000f (assuming kg/car)
	Crude oil density ~ 0.88 kg/L
	Milk mass/unit = 26000 / 0.88 kg/L * 1.03 kg/L = 30421f

CargoType.Milk = 3000

Need to modify:
	DV.Logic.Job:
		CargoTypes
			cargoTypeToSupportedCarContainer
				* Milk => new List<CargoContainerType> {CargoContainerType.TankerChem}
			cargoTypeToCargoMassPerUnit
				* Milk => 30_500f
				* Note: is readonly, what to do about that?
			cargoSpecificDisplayName
				* Milk => "Milk"
			cargoShortDisplayName
				* Milk => "Milk"
			
			ContainerTypeToCarTypes Getter
				* Postfix: if Milk not in _containerTypeToCarTypes, add it to the dictionary
			GetCargoMass()
				* Prefix: if Milk not in cargoTypeToCargoMassPerUnit, add it in
			GetCargoUnitMass()
				* Prefix: if Milk not in cargoTypeToCargoMassPerUnit, add it in
			GetCargoName()
				* Prefix: if Milk not in cargoSpecficDisplayName, add it in
			GetShortCargoName()
				* Prefix: if Milk not in cargoShortDisplayName, add it in
		WarehouseMachine
			SupportedCargoTypes
				* Change in constructor
			WarehouseMachine()
				* check if WarehouseTrack.ID.yardID is FF or FM
				* if so, append to SupportedCargoTypes milk
				
		
	(root namespace)
		ResourceTypes
			cargoToFullCargoDamagePrice
				* Modify in GetFullDamagePriceForCargo()
			cargoToFullEnvironmentDamagePrice
				* Modify in GetFullEnvironmentDamagePriceForCargo()
			
			GetFullDamagePriceForCargo()
				* Postfix: Milk => 0f
			GetFullEnvironmentDamagePriceForCargo()
				* Postfix: Milk => 0f
	
		StationProceduralJobGenerator
			generationRuleset
				* Change in constructor
			StationProceduralJobGenerator()
				* Prefix:
				*	* Check if stationController.stationInfo.YardID is FF or FM
				*	* If FM, append Milk to the stationController.proceduralJobsRuleset.outputCargoGroups.cargoTypes
				*	* If FF, append Milk to the stationController.proceduralJobsRuleset.inputCargoGroups.cargoTypes

each station has a StationProceduralJobGenerator which contains a generationRuleSet
this generationRuleSet defines the station's rules for jobs through two List<CargoGroup>
each CargoGroup has a List<CargoType> and stations that it applies to, as well as licenses needed

so, to add in my own cargo to this chain, i need to append it to the List<CargoType>,
and make sure the stations its loaded/unloaded at are on the List<StationController>

see https://github.com/katycat5e/DVPassengerJobs/blob/127b90358ad622de7ec0474f97eef5d73af781b6/PassengerJobs.cs#L27
```