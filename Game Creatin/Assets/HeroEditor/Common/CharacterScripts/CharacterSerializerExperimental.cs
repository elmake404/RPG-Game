using System;
using System.Collections.Generic;
using System.Linq;
using HeroEditor.Common;
using HeroEditor.Common.Data;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor.Common.CharacterScripts
{
	public partial class Character
	{
		public override string ToJson()
		{
			var sc = SpriteCollection.Instance;

			if (sc == null) throw new Exception("SpriteCollection is missed on scene!");

			var description = new SerializableDictionary<string, string>
			{
				{ "Head", GetSpriteEntryId(sc.Head, Head) },
				{ "Body", GetSpriteEntryId(sc.Body, Body) },
				{ "Ears", GetSpriteEntryId(sc.Ears, Ears) },
				{ "Hair", GetSpriteEntryId(sc.Hair, Hair) },
				{ "Beard", GetSpriteEntryId(sc.Beard, Beard) },
				{ "Helmet", GetSpriteEntryId(sc.Helmet, Helmet) },
				{ "Glasses", GetSpriteEntryId(sc.Glasses, Glasses) },
				{ "Mask", GetSpriteEntryId(sc.Mask, Mask) },
				{ "Armor", GetSpriteEntryId(sc.Armor, Armor) },
				{ "PrimaryMeleeWeapon", GetSpriteEntryId(GetWeaponCollection(WeaponType), PrimaryMeleeWeapon) },
				{ "SecondaryMeleeWeapon", GetSpriteEntryId(GetWeaponCollection(WeaponType), SecondaryMeleeWeapon) },
				{ "Cape", GetSpriteEntryId(sc.Cape, Cape) },
				{ "Back", GetSpriteEntryId(sc.Back, Back) },
				{ "Shield", GetSpriteEntryId(sc.Shield, Shield) },
				{ "Bow", GetSpriteEntryId(sc.Bow, Bow) },
				{ "Firearms", GetSpriteEntryId(GetWeaponCollection(WeaponType), Firearms) },
				{ "FirearmParams", Firearm.Params.Name },
				{ "WeaponType", WeaponType.ToString() },
				{ "Expression", Expression }
			};

			foreach (var expression in Expressions)
			{
				description.Add(string.Format("Expression.{0}.Eyebrows", expression.Name), GetSpriteEntryId(sc.Eyebrows, expression.Eyebrows));
				description.Add(string.Format("Expression.{0}.Eyes", expression.Name), GetSpriteEntryId(sc.Eyes, expression.Eyes));
				description.Add(string.Format("Expression.{0}.Mouth", expression.Name), GetSpriteEntryId(sc.Mouth, expression.Mouth));
			}

			return JsonUtility.ToJson(description);
		}

		public override void LoadFromJson(string serialized)
		{
		}

		private static IEnumerable<SpriteGroupEntry> GetWeaponCollection(WeaponType weaponType)
		{
			switch (weaponType)
			{
				case WeaponType.Melee1H: return SpriteCollection.Instance.MeleeWeapon1H;
				case WeaponType.MeleePaired: return SpriteCollection.Instance.MeleeWeapon1H;
				case WeaponType.Melee2H: return SpriteCollection.Instance.MeleeWeapon2H;
				case WeaponType.Bow: return SpriteCollection.Instance.Bow;
				case WeaponType.Firearms1H: return SpriteCollection.Instance.Firearms1H;
				case WeaponType.FirearmsPaired: return SpriteCollection.Instance.Firearms1H;
				case WeaponType.Firearms2H: return SpriteCollection.Instance.Firearms2H;
				case WeaponType.Supplies: return SpriteCollection.Instance.Supplies;
				default:
					throw new NotSupportedException(weaponType.ToString());
			}
		}

		private static string GetSpriteEntryId(IEnumerable<SpriteGroupEntry> collection, Sprite sprite)
		{
			if (sprite == null) return null;

			var entry = collection.SingleOrDefault(i => i.Sprite == sprite);

			if (entry == null) throw new Exception(string.Format("Can't find {0} in SpriteCollection.", sprite.name));
			
			return entry.Id;
		}

		private static string GetSpriteEntryId(IEnumerable<SpriteGroupEntry> collection, List<Sprite> sprites)
		{
			if (sprites == null || sprites.Count == 0) return null;

			return GetSpriteEntryId(collection, sprites[0]);
		}

		private static Sprite FindSpriteById(IEnumerable<SpriteGroupEntry> collection, string id)
		{
			if (string.IsNullOrEmpty(id)) return null;

			var entries = collection.Where(i => i.Id == id).ToList();

			switch (entries.Count)
			{
				case 1:
					return entries[0].Sprite;
				case 0:
					throw new Exception(string.Format("Entry with id {0} not found in SpriteCollection.", id));
				default:
					throw new Exception(string.Format("Multiple entries with id {0} found in SpriteCollection.", id));
			}
		}

		private static List<Sprite> FindSpritesById(IEnumerable<SpriteGroupEntry> collection, string id)
		{
			if (string.IsNullOrEmpty(id)) return new List<Sprite>();

			var entries = collection.Where(i => i.Id == id).ToList();

			switch (entries.Count)
			{
				case 1:
					return entries[0].Sprites;
				case 0:
					throw new Exception(string.Format("Entry with id {0} not found in SpriteCollection.", id));
				default:
					throw new Exception(string.Format("Multiple entries with id {0} found in SpriteCollection.", id));
			}
		}
	}
}