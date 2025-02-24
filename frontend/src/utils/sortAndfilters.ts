import type { Skin } from '../types';
import { RARITY_INDEX } from '../constants';

export function sortSkinByRarity(a: Skin, b: Skin) {
  return RARITY_INDEX[a.rarity_id] - RARITY_INDEX[b.rarity_id];
}

export function gunSkinFilter(skin: Skin) {
  return !['★'].includes(skin.name[0]);
}

export function knivesAndGlovesSkinFilter(skin: Skin) {
  return !gunSkinFilter(skin);
}

export function filterSkinsByOnlyKnives(skin: Skin) {
  return skin.name.includes('Knife');
}

export function filterSkinsByOnlyGloves(skin: Skin) {
  return ['Glove', 'Wrap'].some((word) => skin.name.includes(word));
}

export function sortSkinByName(a: Skin, b: Skin) {
  return a.name.localeCompare(b.name);
}
