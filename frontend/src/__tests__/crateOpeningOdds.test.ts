import { describe, it, expect } from 'vitest';
import { crateOpeningService } from '../services/crateOpeningService';
import { REAL_ODDS, ODDS_TO_RARITY } from '../constants';
import mockCrateData from './mockCrateData.json';

function mapRarityIdToBucket(rarityId: string): keyof typeof REAL_ODDS | 'unknown' {
  for (const bucketKey of Object.keys(ODDS_TO_RARITY) as Array<keyof typeof ODDS_TO_RARITY>) {
    if (ODDS_TO_RARITY[bucketKey].includes(rarityId)) {
      return bucketKey;
    }
  }
  return 'unknown';
}

describe('crateOpeningService openCrate odds', () => {
  it('should provide wonSkin rarity distribution close to expected odds', () => {
    const iterations = 10000;
    const tolerance = 0.03;
    const crate = JSON.parse(JSON.stringify(mockCrateData));
    const tally = new Map<keyof typeof REAL_ODDS | 'unknown', number>();

    for (let i = 0; i < iterations; i++) {
      const result = crateOpeningService.openCrate(crate, REAL_ODDS);
      const rarityId = result.wonSkin.rarity_id;
      const bucket = mapRarityIdToBucket(rarityId);
      tally.set(bucket, (tally.get(bucket) || 0) + 1);
    }

    for (const [bucket, expected] of Object.entries(REAL_ODDS)) {
      const count = tally.get(bucket as keyof typeof REAL_ODDS) || 0;
      const frequency = count / iterations;
      expect(frequency).toBeGreaterThanOrEqual(expected - tolerance);
      expect(frequency).toBeLessThanOrEqual(expected + tolerance);
    }
  });
});
