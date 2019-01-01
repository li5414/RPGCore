﻿using RPGCore.Behaviour;
using RPGCore.Behaviour.Connections;
using System;

namespace RPGCore
{
	[NodeInformation ("Item/Read Weapon Stat", "Attribute")]
	public class WeaponStatsNode : BehaviourNode
	{
		public WeaponStatEntry Stat;

		public ItemInput Target;
		public FloatOutput Value;

		protected override void OnSetup (IBehaviourContext context)
		{
			ConnectionEntry<ItemSurrogate> targetInput = Target[context];
			ConnectionEntry<float> valueInput = Value[context];

			Action updateListener = () =>
			{
				var weaponNode = targetInput.Value.Template.GetNode<WeaponInputNode> ();

				if (weaponNode == null)
				{
					valueInput.Value = 0;
					return;
				}

				FloatInput localStatInput = null;

				if (Stat.Index == 0)
					localStatInput = weaponNode.AttackDamage;
				else if (Stat.Index == 1)
					localStatInput = weaponNode.AttackSpeed;
				else if (Stat.Index == 2)
					localStatInput = weaponNode.CriticalChance;
				else if (Stat.Index == 3)
					localStatInput = weaponNode.CriticalMultiplier;

				valueInput.Value = localStatInput[targetInput.Value].Value;
			};

			if (targetInput.Value != null)
			{
				targetInput.Value.Template.GetNode<WeaponInputNode> ()
					.AttackDamage[targetInput.Value].OnAfterChanged += updateListener;

				updateListener ();
			}

			targetInput.OnBeforeChanged += () =>
			{
				if (targetInput.Value == null)
					return;

				targetInput.Value.Template.GetNode<WeaponInputNode> ()
					.AttackDamage[targetInput.Value].OnAfterChanged -= updateListener;
			};

			targetInput.OnAfterChanged += () =>
			{
				if (targetInput.Value == null)
					return;

				targetInput.Value.Template.GetNode<WeaponInputNode> ()
					.AttackDamage[targetInput.Value].OnAfterChanged += updateListener;
				updateListener ();
			};
		}

		protected override void OnRemove (IBehaviourContext context)
		{

		}
	}
}
