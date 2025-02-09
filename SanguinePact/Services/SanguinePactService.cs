using System.Collections;
using System.Collections.Generic;
using System.IO;
using SanguinePact.Common.KindredCommands.Data;
using SanguinePact.Common.Utils;
using ProjectM;
using Unity.Entities;

namespace SanguinePact.SanguinePact.Services
{
	internal class SanguinePactService
	{
		private List<ModifyUnitStatBuff_DOTS> sanguinePactBuffs;
		readonly HashSet<Entity> sanguinePactPlayers = [];

		public SanguinePactService()
		{
			foreach(var charEntity in Helper.GetEntitiesByComponentType<PlayerCharacter>(includeDisabled: true))
			{
				sanguinePactBuffs = CreateSanguinePactBuffs();
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
		public static List<ModifyUnitStatBuff_DOTS> CreateSanguinePactBuffs()
		{
			var resistMultiplier = (Plugin.ResistMultiplier.Value - 1) * -1;
			var damageMultiplier = Plugin.DamageMultiplier.Value;

			ModifyUnitStatBuff_DOTS spResistVsUndeads = new()
			{
				StatType = UnitStatType.ResistVsUndeads,
				Value = resistMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spResistVsHumans = new()
			{
				StatType = UnitStatType.ResistVsHumans,
				Value = resistMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spResistVsDemons = new()
			{
				StatType = UnitStatType.ResistVsDemons,
				Value = resistMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spResistVsMechanical = new()
			{
				StatType = UnitStatType.ResistVsMechanical,
				Value = resistMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spResistVsBeasts = new()
			{
				StatType = UnitStatType.ResistVsBeasts,
				Value = resistMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spResistVsVampires = new()
			{
				StatType = UnitStatType.ResistVsVampires,
				Value = resistMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spDamageVsUndeads = new()
			{
				StatType = UnitStatType.DamageVsUndeads,
				Value = damageMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spDamageVsHumans = new()
			{
				StatType = UnitStatType.DamageVsHumans,
				Value = damageMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spDamageVsDemons = new()
			{
				StatType = UnitStatType.DamageVsDemons,
				Value = damageMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spDamageVsMechanical = new()
			{
				StatType = UnitStatType.DamageVsMechanical,
				Value = damageMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spDamageVsBeasts = new()
			{
				StatType = UnitStatType.DamageVsBeasts,
				Value = damageMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			ModifyUnitStatBuff_DOTS spDamageVsVampires = new()
			{
				StatType = UnitStatType.DamageVsVampires,
				Value = damageMultiplier,
				ModificationType = ModificationType.Set,
				Modifier = 1,
				Id = ModificationId.NewId(0)
			};

			List<ModifyUnitStatBuff_DOTS> sanguinePactBuffs =
			[
				spDamageVsUndeads,
				spDamageVsHumans,
				spDamageVsDemons,
				spDamageVsMechanical,
				spDamageVsBeasts,
				spDamageVsVampires,
				spResistVsUndeads,
				spResistVsHumans,
				spResistVsDemons,
				spResistVsMechanical,
				spResistVsBeasts,
				spResistVsVampires
			];

			return sanguinePactBuffs;
		}
	}
}
