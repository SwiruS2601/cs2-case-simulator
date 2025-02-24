import { describe, it, expect } from 'vitest';
import mockCrateData from './mockCrateData.json';

describe('skin rarity distribution in mockCrateData', () => {
  it('should accumulate and log all distinct skin rarity_ids', () => {
    const tally = new Map<string, number>();
    const skins = mockCrateData.skins;

    for (const skin of skins) {
      const rarity = skin.rarity_id;
      tally.set(rarity, (tally.get(rarity) || 0) + 1);
    }

    console.log('All distinct skin rarities and their counts:');
    for (const [rarity, count] of tally) {
      console.log(`Rarity: ${rarity}, Count: ${count}`);
    }

    expect(tally.size).toBeGreaterThan(0);
  });
});
