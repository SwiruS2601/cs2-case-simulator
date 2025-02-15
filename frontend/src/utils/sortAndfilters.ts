import { RARITY_INDEX } from '@/constants';
import type { Skin } from '@/query/skins';

export function sortSkinByRarity(a: Skin, b: Skin) {
  return RARITY_INDEX[a.rarity] - RARITY_INDEX[b.rarity];
}

export function filterSkinsByOnlyGuns(skin: Skin) {
  return skin.weaponType !== 'Knife' && skin.type !== 'Gloves';
}

export function filterSkinsByOnlyKnives(skin: Skin) {
  return skin.weaponType === 'Knife';
}

export function filterSkinsByOnlyGloves(skin: Skin) {
  return skin.type === 'Gloves';
}

export function filterOnlyGlovesAndKnives(skin: Skin) {
  return filterSkinsByOnlyKnives(skin) || filterSkinsByOnlyGloves(skin);
}

export function sortSkinByName(a: Skin, b: Skin) {
  return a.name.localeCompare(b.name);
}
