import type { Skin } from '../types';
import { RARITY_INDEX } from '../constants';

export function sortSkinByRarity(a: Skin, b: Skin) {
  return RARITY_INDEX[a.rarity_id] - RARITY_INDEX[b.rarity_id];
}

export function gunSkinFilter(skin: Skin): boolean {
  return !skin?.name.includes('★');
}

export function knivesAndGlovesSkinFilter(skin: Skin): boolean {
  return skin?.name.includes('★');
}

export function filterSkinsByOnlyKnives(skin: Skin): boolean {
  return skin?.name.includes('Knife');
}

export function filterSkinsByOnlyGloves(skin: Skin): boolean {
  const gloveKeywords = ['Glove', 'Wrap'];
  return gloveKeywords.some((word) => skin.name.includes(word));
}

export function sortSkinByName(a: Skin, b: Skin): number {
  return a.name.localeCompare(b.name);
}
