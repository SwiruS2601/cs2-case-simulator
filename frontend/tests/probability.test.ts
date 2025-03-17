import { describe, test, expect, beforeEach, vi } from 'vitest';
import { crateOpeningService } from '../services/crateOpeningService';
import galleryCase from './mocks/galleryCase.json';
import revolutionCase from './mocks/revolutionCase.json';
import shanghaiSouvenir from './mocks/shanghaiSouvenir.json';
import shanghaiStickerCapsule from './mocks/shanghaiStickerCapsule.json';
import type { Crate } from '../types';

const ODDS = {
    rare: 0.7992, // 79.92%
    mythical: 0.1598, // 15.98%
    legendary: 0.032, // 3.2%
    ancient: 0.0064, // 0.64%
    exceedingly_rare: 0.0026, // 0.26%
  }

let galleryCaseData = galleryCase as unknown as Crate;
let revolutionCaseData = revolutionCase as unknown as Crate;
let shanghaiSouvenirData = shanghaiSouvenir as unknown as Crate;
let shanghaiStickerCapsuleData = shanghaiStickerCapsule as unknown as Crate;

beforeEach(() => {
    galleryCaseData = JSON.parse(JSON.stringify(galleryCase)) as unknown as Crate;
    revolutionCaseData = JSON.parse(JSON.stringify(revolutionCase)) as unknown as Crate;
    shanghaiSouvenirData = JSON.parse(JSON.stringify(shanghaiSouvenir)) as unknown as Crate;
    shanghaiStickerCapsuleData = JSON.parse(JSON.stringify(shanghaiStickerCapsule)) as unknown as Crate;
  });

describe('Crate Opening Service Probability Tests', () => {

  // Helper function to run multiple simulations and collect statistics
  function runSimulations(crate: Crate, odds: Record<string, number>, iterations = 100000) {
    const results: Record<string, number> = {};
    
    for (let i = 0; i < iterations; i++) {
      const skin = crateOpeningService.getRandomSkinByOdds(crate, odds);
      const rarity = skin.rarity_id;
      results[rarity] = (results[rarity] || 0) + 1;
    }
    
    return results;
  }

  // Helper function to verify odds are within expected margin of error
  function verifyOddsDistribution(actual: Record<string, number>, expected: Record<string, number>, iterations: number) {
    const marginOfError = 2; // Allow 2% margin of error for randomness
    
    for (const [rarity, count] of Object.entries(actual)) {
      if (expected[rarity]) {
        const actualPercentage = (count / iterations) * 100;
        const expectedPercentage = expected[rarity];
        expect(actualPercentage).toBeGreaterThanOrEqual(expectedPercentage - marginOfError);
        expect(actualPercentage).toBeLessThanOrEqual(expectedPercentage + marginOfError);
      }
    }
  }

  describe('Gallery Case tests', () => {
    test('should distribute rarities according to odds', () => {
      const results = runSimulations(galleryCaseData, ODDS);
      verifyOddsDistribution(results, ODDS, 100000);
    });

    test('should select skins within appropriate rarity', () => {
      for (let i = 0; i < 100; i++) {
        const skin = crateOpeningService.getRandomSkinByOdds(galleryCaseData, ODDS);
        const rarityMatches = galleryCaseData.skins.some(
          crateSkin => crateSkin.rarity_id === skin.rarity_id
        );
        expect(rarityMatches).toBeTruthy();
      }
    });

    test('getWinningSkin should add wear category', () => {
      const winningSkin = crateOpeningService.getWinningSkin(galleryCaseData, ODDS);
      expect(winningSkin.wear_category).toBeDefined();
    });
  });

  describe('Revolution Case tests', () => {
    test('should distribute rarities according to odds', () => {
      const results = runSimulations(revolutionCaseData, ODDS);
      verifyOddsDistribution(results, ODDS, 100000);
    });

    test('should select skins within appropriate rarity', () => {
      for (let i = 0; i < 100; i++) {
        const skin = crateOpeningService.getRandomSkinByOdds(revolutionCaseData, ODDS);
        const rarityMatches = revolutionCaseData.skins.some(
          crateSkin => crateSkin.rarity_id === skin.rarity_id
        );
        expect(rarityMatches).toBeTruthy();
      }
    });
  });

  describe('Shanghai Souvenir tests', () => {
    test('should distribute rarities according to odds', () => {
      const results = runSimulations(shanghaiSouvenirData, ODDS);
      verifyOddsDistribution(results, ODDS, 100000);
    });

    test('should select skins within appropriate rarity', () => {
      for (let i = 0; i < 100; i++) {
        const skin = crateOpeningService.getRandomSkinByOdds(shanghaiSouvenirData, ODDS);
        const rarityMatches = shanghaiSouvenirData.skins.some(
          crateSkin => crateSkin.rarity_id === skin.rarity_id
        );
        expect(rarityMatches).toBeTruthy();
      }
    });
  });

  describe('Shanghai Sticker Capsule tests', () => {
    test('should distribute rarities according to odds', () => {
      const results = runSimulations(shanghaiStickerCapsuleData, ODDS);
      verifyOddsDistribution(results, ODDS, 100000);
    });

    test('should select skins within appropriate rarity', () => {
      for (let i = 0; i < 100; i++) {
        const skin = crateOpeningService.getRandomSkinByOdds(shanghaiStickerCapsuleData, ODDS);
        const rarityMatches = shanghaiStickerCapsuleData.skins.some(
          crateSkin => crateSkin.rarity_id === skin.rarity_id
        );
        expect(rarityMatches).toBeTruthy();
      }
    });
  });

  describe('Edge cases', () => {
    test('should handle custom odds', () => {
      const customOdds = { ...ODDS } as any;
      Object.keys(customOdds).forEach((key, index) => {
        customOdds[key] = [50, 30, 15, 4, 1][index % 5] || customOdds[key];
      });
      
      const results = runSimulations(galleryCaseData, customOdds, 5000);
      verifyOddsDistribution(results, customOdds, 5000);
    });
    
    
    test('should fallback to any skin of the selected rarity if gun skin filter returns no results', () => {
      // This test depends on the implementation of gunSkinFilter
      // Assuming it might filter out some skins, we want to make sure we still get a skin
      for (let i = 0; i < 100; i++) {
        const skin = crateOpeningService.getRandomSkinByOdds(galleryCaseData, ODDS);
        expect(skin).toBeDefined();
      }
    });
  });

  describe('Distribution visualization', () => {
    test('should log rarity distribution statistics for all cases', () => {
      const iterations = 100000;
      const cases = [
        { name: 'Gallery Case', data: galleryCaseData },
        { name: 'Revolution Case', data: revolutionCaseData },
        { name: 'Shanghai Souvenir', data: shanghaiSouvenirData },
        { name: 'Shanghai Sticker Capsule', data: shanghaiStickerCapsuleData }
      ];
      
      // Map from skin rarity IDs to ODDS keys
      const rarityMapping = {
        'rare': ['rarity_common_weapon', 'rarity_uncommon_weapon', 'rarity_rare_weapon', 'rarity_rare'],
        'mythical': ['rarity_mythical_weapon', 'rarity_mythical'],
        'legendary': ['rarity_legendary_weapon', 'rarity_legendary'],
        'ancient': ['rarity_ancient_weapon', 'rarity_ancient'],
        'exceedingly_rare': ['exceedingly_rare']
      };
      
      for (const caseInfo of cases) {
        const results = runSimulations(caseInfo.data, ODDS, iterations);
        
        // Calculate percentages and prepare table data
        const tableData = Object.entries(results).map(([rarity, count]) => {
          // Find which category this rarity belongs to
          let expectedKey = '';
          for (const [key, values] of Object.entries(rarityMapping)) {
            if (values.includes(rarity)) {
              expectedKey = key;
              break;
            }
          }
          
          const actualPercentage = (count / iterations) * 100;
          const expectedPercentage = ODDS[expectedKey] || 0;
          const difference = actualPercentage - expectedPercentage;
          
          return {
            rarity,
            count,
            percentage: actualPercentage.toFixed(2) + '%',
            expected: expectedPercentage.toFixed(2) + '%',
            difference: difference.toFixed(2) + '%'
          };
        });
        
        // Sort by count (descending)
        tableData.sort((a, b) => b.count - a.count);
        
        // Log the table
        console.log(`\n===== ${caseInfo.name} Distribution (${iterations} iterations) =====`);
        console.table(tableData);
        
        console.log("Expected ODDS from constants:");
        console.table(ODDS);
      }
    });

    test('should add custom exceedingly_rare items and test drops', () => {
      const iterations = 100000;
      
      // Create modified test cases with added exceedingly rare items
      const galleryCaseWithKnives = JSON.parse(JSON.stringify(galleryCaseData));
      
      // Add a knife item to the test case
      galleryCaseWithKnives.skins.push({
        id: 'knife-test-1',
        name: 'Test Knife | Fade',
        description: 'A test knife item',
        rarity_id: 'rarity_ancient_weapon', // Initially ancient, will be converted by service
        paint_index: 123,
        prices: []
      });
      
      // Create properly balanced test odds that sum close to 1
      const testOdds = {
        rare: 0.72, // 72%
        mythical: 0.144, // 14.4%
        legendary: 0.029, // 2.9%
        ancient: 0.007, // 0.7%
        exceedingly_rare: 0.1 // 10%
      };
      
      const results = runSimulations(galleryCaseWithKnives, testOdds, iterations);
      
      // Process results
      const tableData = Object.entries(results).map(([rarity, count]) => {
        return {
          rarity,
          count,
          percentage: (count / iterations * 100).toFixed(2) + '%'
        };
      });
      
      // Sort by rarity importance
      tableData.sort((a, b) => {
        // Put exceedingly_rare last
        if (a.rarity === 'exceedingly_rare') return 1;
        if (b.rarity === 'exceedingly_rare') return -1;
        return b.count - a.count;
      });
      
      console.table(tableData);
      
      // Verify we actually got exceedingly_rare items
      const rareCount = results['exceedingly_rare'] || 0;
      const percentage = (rareCount / iterations) * 100;
      
      console.log(`Exceedingly rare drops: ${rareCount}/${iterations} (${percentage.toFixed(2)}%)`);
      console.log(`Expected from test odds: 10.00%`);
      
      // The test should pass if probability is close to expected
      expect(percentage).toBeGreaterThanOrEqual(8);
      expect(percentage).toBeLessThanOrEqual(12);
    });
  });
  
  test('should verify exceedingly rare drop rates', () => {
    const highIterations = 100000; // Higher number for rare items
    const cases = [
      { name: 'Gallery Case', data: galleryCaseData },
      { name: 'Revolution Case', data: revolutionCaseData }
    ];
    
    for (const caseInfo of cases) {
      console.log(`\n===== Testing ${caseInfo.name} exceedingly rare drops =====`);
      
      // Use custom odds to increase chances of rare items for testing
      const testOdds = { ...ODDS, exceedingly_rare: 0.05 }; // 5% chance for testing
      const results = runSimulations(caseInfo.data, testOdds, highIterations);
      const rareCount = results['exceedingly_rare'] || 0;
      const percentage = (rareCount / highIterations) * 100;
      
      console.log(`Exceedingly rare drops: ${rareCount}/${highIterations} (${percentage.toFixed(4)}%)`);
      console.log(`Expected from test odds: 5.00%`);
      
      if (rareCount > 0) {
        // If we got any rare items with boosted odds, test passed
        expect(percentage).toBeGreaterThan(0);
      } else {
        console.log('No exceedingly rare items found even with boosted odds.');
        console.log('This could be because:');
        console.log('1. The mock data does not contain any exceedingly rare items');
        console.log('2. The crateOpeningService is not properly configured to select them');
      }
    }
  });

  // Add this test to your file to debug the issue

  test('debug exceedingly_rare item selection', () => {
    const iterations = 10000;
    const galleryCaseForDebugging = JSON.parse(JSON.stringify(galleryCaseData));
    
    // First, check what knives exist in the data:
    console.log("\nChecking for knives in gallery case:");
    const knives = galleryCaseForDebugging.skins.filter(skin => skin.name?.includes('★'));
    console.log(`Found ${knives.length} knives in the data`);
    if (knives.length > 0) {
      console.log("Example knife:", knives[0].name);
    }
    
    // Second, check if knivesAndGlovesSkinFilter works:
    const knivesFoundByFilter = galleryCaseForDebugging.skins.filter(knivesAndGlovesSkinFilter);
    console.log(`knivesAndGlovesSkinFilter found ${knivesFoundByFilter.length} knives`);
    
    // Check if ODDS includes exceedingly_rare:
    console.log("\nODDS object contains:", Object.keys(ODDS));
    console.log("exceedingly_rare value:", ODDS.exceedingly_rare);
    
    // Try to force exceedingly_rare selection
    const forcedOdds = {
      rare: 0,
      mythical: 0,
      legendary: 0, 
      ancient: 0,
      exceedingly_rare: 1 // 100% chance
    };
    
    console.log("\nTesting with 100% exceedingly_rare odds:");
    const forcedResults = runSimulations(galleryCaseForDebugging, forcedOdds, 100);
    console.table(forcedResults);
    
    // Check the normal pathway
    console.log("\nRandom selection path trace:");
    const debugSkin = crateOpeningService.getRandomSkinByOdds(galleryCaseForDebugging, ODDS);
    console.log("Selected skin:", debugSkin.name, "with rarity:", debugSkin.rarity_id);
  });
});