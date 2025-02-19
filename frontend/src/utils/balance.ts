import type { Skin } from '@/types';

export function getSkinPrice(skin: Skin) {
  return skin?.wearCategory ? skin.prices[skin?.wearCategory].all_time.average : 0;
}
