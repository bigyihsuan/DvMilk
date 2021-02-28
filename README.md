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
		CargoTypes:
			cargoTypeToSupportedCarContainer
				* Milk => new List<CargoContainerType> {CargoContainerType.TankerChem}
			cargoTypeToCargoMassPerUnit
				* Milk => 30421f
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
			
```