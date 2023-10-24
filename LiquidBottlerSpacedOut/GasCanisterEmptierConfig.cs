using TUNING;
using UnityEngine;
using System.Collections.Generic;

namespace Alesseon.LiquidBottler.Building.Config
{
    class GasCanisterEmptierConfig: IBuildingConfig
    {
        public const string ID = "Alesseon.GasCanisterEmptier";

        public static readonly SimHashes[] enabledElements = new SimHashes[] {
            SimHashes.Steam, SimHashes.Oxygen, SimHashes.ContaminatedOxygen,
            SimHashes.CarbonDioxide, SimHashes.ChlorineGas, SimHashes.EthanolGas,
            SimHashes.Hydrogen, SimHashes.Fallout, SimHashes.PhosphorusGas,
            SimHashes.SourGas, SimHashes.SulfurGas, SimHashes.SuperCoolantGas
        };

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR2_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
            EffectorValues tieR2_2 = BUILDINGS.DECOR.PENALTY.TIER2;
            EffectorValues noise = tieR1;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                ID,
                1,
                3,
                "gas_emptying_station_kanim",
                30,
                60f,
                tieR2_1,
                refinedMetals,
                1600f,
                BuildLocationRule.OnFloor,
                tieR2_2,
                noise);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = true;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.OutputConduitType = ConduitType.Gas;
            buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
            buildingDef.ViewMode = OverlayModes.GasConduits.ID;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.storageFilters = STORAGEFILTERS.GASES;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.capacityKg = 100f;
            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<GasCanisterEmptier>();
            ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
            dispenser.conduitType = ConduitType.Gas;
            dispenser.elementFilter = GasCanisterEmptierConfig.enabledElements;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}
