import type { Skin } from '../types';

export function getSkinRarityColor(skin: Skin) {
  return skin.weaponType === 'Knife' || skin.type === 'Gloves' ? '#FFD700' : `#${skin?.rarityColor}`;
}
