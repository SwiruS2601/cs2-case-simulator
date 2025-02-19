import { describe, it, expect } from 'vitest';
import { crateOpeningService, generateSkinWearCategory } from '../services/crateOpeningService';
import { FUN_ODDS, REAL_RARITY_ODDS } from '../constants';
import mockCrateData from './mockCrateData.json';
import mockSouvenirData from './mockSouvenirData.json';
import type { Skin } from '../types';

describe('crateOpeningService', () => {
  describe('openCrate with normal crate', () => {
    it('should return valid sliderSkins, wonSkin and wonSkinIndex', () => {
      const crate = JSON.parse(JSON.stringify(mockCrateData));
      const result = crateOpeningService.openCrate(crate, REAL_RARITY_ODDS);

      expect(result).toHaveProperty('sliderSkins');
      expect(result).toHaveProperty('wonSkin');
      expect(result).toHaveProperty('wonSkinIndex');
      expect(result.wonSkinIndex).toBe(42);
      expect(result.sliderSkins[42]).toEqual(result.wonSkin);
    });
  });

  describe('openCrate with Souvenir crate', () => {
    it('should return valid sliderSkins, wonSkin and wonSkinIndex for Souvenir type', () => {
      const crate = JSON.parse(JSON.stringify(mockSouvenirData));
      const result = crateOpeningService.openCrate(crate, REAL_RARITY_ODDS);

      expect(result).toHaveProperty('sliderSkins');
      expect(result).toHaveProperty('wonSkin');
      expect(result).toHaveProperty('wonSkinIndex');
      expect(result.wonSkinIndex).toBe(42);
      expect(result.sliderSkins[42]).toEqual(result.wonSkin);
    });
  });

  describe('error handling', () => {
    it('should throw an error if no skins are present', () => {
      const crate = JSON.parse(JSON.stringify(mockCrateData));
      const badCrate = { ...crate, skins: [] };
      expect(() => crateOpeningService.openCrate(badCrate, FUN_ODDS)).toThrow();
    });
  });
});

describe('generateSkinWearCategory', () => {
  it('should generate a frequency distribution map over 10,000 iterations', () => {
    const tally = new Map<string, number>();
    const iterations = 10000;
    const randomSkin = mockCrateData.skins[Math.floor(Math.random() * mockCrateData.skins.length)] as unknown as Skin;

    for (let i = 0; i < iterations; i++) {
      const wear = generateSkinWearCategory(randomSkin);
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
