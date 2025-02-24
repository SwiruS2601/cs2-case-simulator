import { describe, it, expect } from 'vitest';
import { getSkinWearCategory } from '../services/crateOpeningService';
import mockCrateData from './mockCrateData.json';
import type { Skin } from '../types';

describe('getSkinWearCategory', () => {
  it('should generate a frequency distribution over 10,000 iterations', () => {
    const iterations = 10000;
    const tally = new Map<string, number>();
    const randomSkin = mockCrateData.skins[Math.floor(Math.random() * mockCrateData.skins.length)] as Skin;

    for (let i = 0; i < iterations; i++) {
      const wear = getSkinWearCategory(randomSkin);
      tally.set(wear, (tally.get(wear) || 0) + 1);
    }

    expect(tally.size).toBeGreaterThan(0);
    for (const count of tally.values()) {
      expect(count).toBeGreaterThan(0);
    }

    console.log(randomSkin.name);
    console.log(tally);
  });
});
