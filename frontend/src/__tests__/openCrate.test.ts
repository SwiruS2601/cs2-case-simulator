import { describe, it, expect } from 'vitest';
import { crateOpeningService, getSkinWearCategory } from '../services/crateOpeningService';
import { FUN_ODDS, REAL_ODDS } from '../constants';
import mockCrateData from './mockCrateData.json';
import mockSouvenirData from './mockSouvenirData.json';

describe('crateOpeningService', () => {
  describe('openCrate with normal crate', () => {
    it('should return valid sliderSkins, wonSkin and wonSkinIndex', () => {
      const crate = JSON.parse(JSON.stringify(mockCrateData));
      const result = crateOpeningService.openCrate(crate, REAL_ODDS);

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
      const result = crateOpeningService.openCrate(crate, REAL_ODDS);

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
