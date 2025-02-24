import type { Skin } from '../types';
import { knivesAndGlovesSkinFilter } from './sortAndfilters';

export function getSkinRarityColor(skin: Skin) {
  return knivesAndGlovesSkinFilter(skin) ? '#FFD700' : skin?.rarity.color;
}
