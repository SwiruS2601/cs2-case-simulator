import { RARITY_COLORS } from '~/constants';
import type { Skin } from '../types';
import { knivesAndGlovesSkinFilter } from './sortAndfilters';
import type { InventoryItem } from '~/services/inventoryDb';

export function getItemRarityColor(item: Skin | InventoryItem) {
    return knivesAndGlovesSkinFilter(item) ? '#FFD700' : RARITY_COLORS[item.rarity_id];
}
