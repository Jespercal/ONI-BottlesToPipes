﻿using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alesseon.LiquidBottler.Building
{
    [SerializationConfig(MemberSerialization.OptIn)]
    class LiquidBottleEmptier : StateMachineComponent<LiquidBottleEmptier.StatesInstance>, IGameObjectEffectDescriptor
    {
        public float emptyRate = 2f;
        [Serialize]
        public bool allowManualPumpingStationFetching;
        [SerializeField]
        public Color noFilterTint = TreeFilterable.NO_FILTER_TINT;
        [SerializeField]
        public Color filterTint = TreeFilterable.FILTER_TINT;
        private static readonly EventSystem.IntraObjectHandler<LiquidBottleEmptier> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LiquidBottleEmptier>((component, data) => component.OnRefreshUserMenu(data));
        private static readonly EventSystem.IntraObjectHandler<LiquidBottleEmptier> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LiquidBottleEmptier>((component, data) => component.OnCopySettings(data));

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi.StartSM();
            Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenuDelegate);
            Subscribe((int)GameHashes.CopySettings, OnCopySettingsDelegate);
        }

        public List<Descriptor> GetDescriptors(GameObject go) => null;

        private void OnChangeAllowManualPumpingStationFetching()
        {
            allowManualPumpingStationFetching = !allowManualPumpingStationFetching;
            smi.RefreshChore();
        }

        private void OnRefreshUserMenu(object data)
        {
            Game.Instance.userMenu.AddButton(gameObject, allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, new System.Action(OnChangeAllowManualPumpingStationFetching), tooltipText: (UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, new System.Action(OnChangeAllowManualPumpingStationFetching), tooltipText: (UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP)), 0.4f);
        }

        private void OnCopySettings(object data)
        {
            allowManualPumpingStationFetching = ((GameObject)data).GetComponent<LiquidBottleEmptier>().allowManualPumpingStationFetching;
            smi.RefreshChore();
        }

        public class StatesInstance :
          GameStateMachine<States, StatesInstance, LiquidBottleEmptier, object>.GameInstance
        {
            private FetchChore chore;

            public MeterController meter { get; private set; }

            public StatesInstance(LiquidBottleEmptier smi)
              : base(smi)
            {
                master.GetComponent<TreeFilterable>().OnFilterChanged += new Action<HashSet<Tag>>(OnFilterChanged);
                meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
                {
                    "meter_target",
                    "meter_arrow",
                    "meter_scale"
                });
                Subscribe(-1697596308, new Action<object>(OnStorageChange));
                Subscribe(644822890, new Action<object>(OnOnlyFetchMarkedItemsSettingChanged));
            }

            public void CreateChore()
            {
                KBatchedAnimController component1 = GetComponent<KBatchedAnimController>();
                HashSet<Tag> tags = GetComponent<TreeFilterable>().GetTags();
                if (tags == null || tags.Count == 0)
                {
                    component1.TintColour = master.noFilterTint;
                }
                else
                {
                    component1.TintColour = master.filterTint;
                    Tag[] forbidden_tags;
                    // HashSet<Tag> forbidden_tags = new HashSet<Tag>();
                    if (!master.allowManualPumpingStationFetching)
                        forbidden_tags = new Tag[1] {GameTags.LiquidSource};
                    else
                        forbidden_tags = new Tag[0];
                    foreach(Tag tag in forbidden_tags)
                    {
                        Console.WriteLine("Emptier Tag:" + tag.Name);
                    }
                    Storage component2 = GetComponent<Storage>();
                    chore = new FetchChore(
                        Db.Get().ChoreTypes.StorageFetch,
                        component2,
                        component2.Capacity(),
                        GetComponent<TreeFilterable>().GetTags(),
                        FetchChore.MatchCriteria.MatchID,
                        Tag.Invalid,
                        forbidden_tags
                    );
                }

            }

            public void CancelChore()
            {
                if (chore == null)
                    return;
                chore.Cancel("Storage Changed");
                chore = null;
            }

            public void RefreshChore() => GoTo(sm.unoperational);

            private void OnFilterChanged(HashSet<Tag> tags) => RefreshChore();

            private void OnStorageChange(object data)
            {
                Storage component = GetComponent<Storage>();
                meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
            }

            private void OnOnlyFetchMarkedItemsSettingChanged(object data) => RefreshChore();

            public void StartMeter()
            {
                PrimaryElement firstPrimaryElement = GetFirstPrimaryElement();
                if (firstPrimaryElement == null)
                    return;
                meter.SetSymbolTint(new KAnimHashedString("meter_fill"), firstPrimaryElement.Element.substance.colour);
                meter.SetSymbolTint(new KAnimHashedString("water1"), firstPrimaryElement.Element.substance.colour);
                GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), firstPrimaryElement.Element.substance.colour);
            }

            private PrimaryElement GetFirstPrimaryElement()
            {
                Storage component1 = GetComponent<Storage>();
                for (int idx = 0; idx < component1.Count; ++idx)
                {
                    GameObject gameObject = component1[idx];
                    if (!(gameObject == null))
                    {
                        PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
                        if (!(component2 == null))
                            return component2;
                    }
                }
                return null;
            }
        }

        public class States :
          GameStateMachine<LiquidBottleEmptier.States, LiquidBottleEmptier.StatesInstance, LiquidBottleEmptier>
        {
            private StatusItem statusItem;
            public State unoperational;
            public State waitingfordelivery;
            public State emptying;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = waitingfordelivery;
                statusItem = new StatusItem(nameof(LiquidBottleEmptier), "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
                statusItem.resolveStringCallback = ((str, data) =>
                {
                    LiquidBottleEmptier bottleEmptier = (LiquidBottleEmptier)data;
                    if (bottleEmptier == null)
                        return str;
                    return bottleEmptier.allowManualPumpingStationFetching ? BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME;
                });
                statusItem.resolveTooltipCallback = ((str, data) =>
                {
                    LiquidBottleEmptier bottleEmptier = (LiquidBottleEmptier)data;
                    if (bottleEmptier == null)
                        return str;
                    return bottleEmptier.allowManualPumpingStationFetching ? BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP;
                });
                root.ToggleStatusItem(statusItem, (smi => smi.master));
                unoperational.TagTransition(GameTags.Operational, waitingfordelivery).PlayAnim("off");

                waitingfordelivery
                    .TagTransition(GameTags.Operational, unoperational, true)
                    .EventTransition(
                        GameHashes.OnStorageChange,
                        emptying, 
                        (smi => !smi.GetComponent<Storage>().IsEmpty() && smi.GetComponent<Storage>().MassStored() > 0)
                    ).Enter(
                        "CreateChore",
                        (smi => smi.CreateChore())
                    ).Exit(
                        "CancelChore",
                        (smi => smi.CancelChore())
                    ).PlayAnim("on");

                emptying
                    .TagTransition(
                        GameTags.Operational,
                        unoperational,
                        true
                    ).EventTransition(
                        GameHashes.OnStorageChange,
                        waitingfordelivery,
                        (smi => smi.GetComponent<Storage>().IsEmpty() || smi.GetComponent<Storage>().MassStored() <= 0)
                    ).Enter(
                        "StartMeter",
                        (smi => smi.StartMeter())
                    ).PlayAnim(
                        "working_loop", 
                        KAnim.PlayMode.Loop
                    );
            }
        }
    }
}
