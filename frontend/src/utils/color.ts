import type { Skin } from '@/query/skins';

export function getSkinRarityColor(skin: Skin) {
  return skin.weaponType === 'Knife' || skin.type === 'Gloves' ? '#FFD700' : `#${skin?.rarityColor}`;
}
