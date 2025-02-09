using System.Collections;
using System.Collections.Generic;
using SanguinePact.Common.KindredCommands.Data;
using SanguinePact.Common.Utils;
using ProjectM;
using Unity.Entities;

namespace SanguinePact.SanguinePact.Services
{
	internal class SanguinePactService
	{
		readonly HashSet<Entity> sanguinePactPlayers = [];

		public SanguinePactService()
		{
			foreach(var charEntity in Helper.GetEntitiesByComponentType<PlayerCharacter>(includeDisabled: true))
			{
				LoadSanguinePactPlayers(charEntity);
			}
		}

		public bool IsSanguinePactPlayer(Entity charEntity)
		{
			return sanguinePactPlayers.Contains(charEntity);
		}

		public IEnumerable<Entity> GetSanguinePactPlayers()
		{
			foreach (var charEntity in Helper.GetEntitiesByComponentType<PlayerCharacter>(includeDisabled: true))
			{
				if (IsSanguinePactPlayer(charEntity))
					yield return charEntity;
			}
		}

		public bool ToggleSanguinePact(Entity charEntity)
		{
			if (IsSanguinePactPlayer(charEntity))
			{
				sanguinePactPlayers.Remove(charEntity);
				return false;
			}
			sanguinePactPlayers.Add(charEntity);
			return true;
		}

		void LoadSanguinePactPlayers(Entity charEntity)
		{
			if (BuffUtility.TryGetBuff(Core.Server.EntityManager, charEntity, Prefabs.SanguinePactBuff, out var buffEntity) &&
			    buffEntity.Has<ModifyUnitStatBuff_DOTS>())
			{
				foreach (var buff in buffEntity.ReadBuffer<ModifyUnitStatBuff_DOTS>())
				{
					switch (buff.StatType)
					{
						case UnitStatType.ResistVsVampires:
							sanguinePactPlayers.Add(charEntity);
							break;
					}
				}
			}
		}

		public void UpdateSanguinePactPlayer(Entity charEntity)
		{
			if(!IsSanguinePactPlayer(charEntity))
			{
				ClearSanguinePactBuff(charEntity);
				return;
			}

			var userEntity = charEntity.Read<PlayerCharacter>().UserEntity;
			Core.StartCoroutine(RemoveAndAddSanguinePactBuff(userEntity, charEntity));
		}

		IEnumerator RemoveAndAddSanguinePactBuff(Entity userEntity, Entity charEntity)
		{
			Buffs.RemoveBuff(charEntity, Prefabs.SanguinePactBuff);
			while (BuffUtility.HasBuff(Core.EntityManager, charEntity, Prefabs.SanguinePactBuff))
				yield return null;

			Buffs.AddBuff(userEntity, charEntity, Prefabs.SanguinePactBuff, -1, true);
		}


		public void UpdateSanguinePactBuff(Entity buffEntity)
		{
			var charEntity = buffEntity.Read<EntityOwner>().Owner;
			var modifyStatBuffer = Core.EntityManager.AddBuffer<ModifyUnitStatBuff_DOTS>(buffEntity);
			modifyStatBuffer.Clear();

			if (IsSanguinePactPlayer(charEntity))
			{
				foreach (var sanguinePactBuff in sanguinePactBuffs)
				{
					var modifiedBuff = sanguinePactBuff;
					modifyStatBuffer.Add(modifiedBuff);
				}
			}

			long buffModificationFlags = 0;
			buffEntity.Add<BuffModificationFlagData>();
			var buffModificationFlagData = new BuffModificationFlagData()
			{
				ModificationTypes = buffModificationFlags,
				ModificationId = ModificationId.NewId(0),
			};
			buffEntity.Write(buffModificationFlagData);
		}

		void ClearSanguinePactBuff(Entity charEntity)
		{
			Buffs.RemoveBuff(charEntity, Prefabs.SanguinePactBuff);
		}

		#region Pact Buff Definitions
		private const float ResistStatMultiplier = -4.0f; // Take x5 more damage

		static ModifyUnitStatBuff_DOTS SPResistVsUndeads = new()
		{
			StatType = UnitStatType.ResistVsUndeads,
			Value = ResistStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPResistVsHumans = new()
		{
			StatType = UnitStatType.ResistVsHumans,
			Value = ResistStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPResistVsDemons = new()
		{
			StatType = UnitStatType.ResistVsDemons,
			Value = ResistStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPResistVsMechanical = new()
		{
			StatType = UnitStatType.ResistVsMechanical,
			Value = ResistStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPResistVsBeasts = new()
		{
			StatType = UnitStatType.ResistVsBeasts,
			Value = ResistStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPResistVsVampires = new()
		{
			StatType = UnitStatType.ResistVsVampires,
			Value = ResistStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		private const float DamageStatMultiplier = 2.0f; // Deal 100% more damage

		static ModifyUnitStatBuff_DOTS SPDamageVsUndeads = new()
		{
			StatType = UnitStatType.DamageVsUndeads,
			Value = DamageStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPDamageVsHumans = new()
		{
			StatType = UnitStatType.DamageVsHumans,
			Value = DamageStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPDamageVsDemons = new()
		{
			StatType = UnitStatType.DamageVsDemons,
			Value = DamageStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPDamageVsMechanical = new()
		{
			StatType = UnitStatType.DamageVsMechanical,
			Value = DamageStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPDamageVsBeasts = new()
		{
			StatType = UnitStatType.DamageVsBeasts,
			Value = DamageStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		static ModifyUnitStatBuff_DOTS SPDamageVsVampires = new()
		{
			StatType = UnitStatType.DamageVsVampires,
			Value = DamageStatMultiplier,
			ModificationType = ModificationType.Set,
			Modifier = 1,
			Id = ModificationId.NewId(0)
		};

		public static readonly List<ModifyUnitStatBuff_DOTS> sanguinePactBuffs =
		[
			SPDamageVsUndeads,
			SPDamageVsHumans,
			SPDamageVsDemons,
			SPDamageVsMechanical,
			SPDamageVsBeasts,
			SPDamageVsVampires,
			SPResistVsUndeads,
			SPResistVsHumans,
			SPResistVsDemons,
			SPResistVsMechanical,
			SPResistVsBeasts,
			SPResistVsVampires
		];
		#endregion
	}
}
