import type { Skin } from '@/query/skins';

export function getSkinPrice(skin: Skin) {
  return (
    skin?.prices['Field-Tested']?.all_time?.average ??
    skin?.prices['Minimal Wear']?.all_time?.average ??
    skin?.prices['Well-Worn']?.all_time?.average ??
    skin?.prices['Factory New']?.all_time?.average ??
    skin.prices['Battle-Scarred']?.all_time?.average
  );
}
