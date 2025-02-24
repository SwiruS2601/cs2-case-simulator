import { describe, it, expect } from 'vitest';
import { crateOpeningService } from '../services/crateOpeningService';
import { REAL_ODDS } from '../constants';
import mockCrateData from './mockCrateData.json';

describe('Odds overview', () => {
  it('should log expected odds, accumulated tally and frequencies (sorted by key)', () => {
    const iterations = 20000;
    const crate = JSON.parse(JSON.stringify(mockCrateData));
    const tally = new Map<string, number>();

    const expectedOddsSorted: Record<string, number> = {};
    Object.keys(REAL_ODDS)
      .sort()
      .forEach((key) => {
        expectedOddsSorted[key] = REAL_ODDS[key as keyof typeof REAL_ODDS];
      });
    console.log('Expected Odds (REAL_ODDS):');
    console.table(expectedOddsSorted);

    for (let i = 0; i < iterations; i++) {
      const result = crateOpeningService.openCrate(crate, REAL_ODDS);
      const bucket = result.wonSkin.rarity_id;
      tally.set(bucket, (tally.get(bucket) || 0) + 1);
    }

    const rawTally: Record<string, number> = {};
    Array.from(tally.entries())
      .sort(([aKey], [bKey]) => aKey.localeCompare(bKey))
      .forEach(([key, value]) => {
        rawTally[key] = value;
      });
    console.log('Accumulated Tally (raw counts, sorted):');
    console.table(rawTally);

    const frequencyMap: Record<string, string> = {};
    Array.from(tally.entries())
      .sort(([aKey], [bKey]) => aKey.localeCompare(bKey))
      .forEach(([key, count]) => {
        const frequency = (count / iterations) * 100;
        frequencyMap[key] = `${frequency.toFixed(2)}%`;
      });
    console.log('Frequency Distribution (sorted):');
    console.table(frequencyMap);

    expect(tally.size).toBeGreaterThan(0);
  });
});
