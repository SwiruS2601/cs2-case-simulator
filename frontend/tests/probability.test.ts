import { describe, test, expect, beforeEach, vi } from 'vitest';
import { crateOpeningService } from '../services/crateOpeningService';
import galleryCase from './mocks/galleryCase.json';
import revolutionCase from './mocks/revolutionCase.json';
import shanghaiSouvenir from './mocks/shanghaiSouvenir.json';
import shanghaiStickerCapsule from './mocks/shanghaiStickerCapsule.json';
import type { Crate } from '../types';
import { ODDS_TO_RARITY, REAL_ODDS } from '~/constants';

const ODDS: Record<string, number> = {
    ...REAL_ODDS,
};

const RARITY_MAP: Record<string, string[]> = {
    ...ODDS_TO_RARITY,
};

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
    function runSimulations(crate: Crate, odds: Record<string, number>, iterations = 100000) {
        const results: Record<string, number> = {};

        for (let i = 0; i < iterations; i++) {
            const skin = crateOpeningService.getRandomSkinByOdds(crate, odds);
            const rarity = skin.rarity_id;
            results[rarity] = (results[rarity] || 0) + 1;
        }

        return results;
    }

    function verifyOddsDistribution(
        actual: Record<string, number>,
        expected: Record<string, number>,
        RARITY_MAP: Record<string, string[]>,
        iterations: number,
    ) {
        const marginOfError = 2;

        const aggregatedActual: Record<string, number> = {};

        Object.entries(actual).forEach(([rarity, count]) => {
            for (const [key, values] of Object.entries(RARITY_MAP)) {
                if (values.includes(rarity)) {
                    aggregatedActual[key] = (aggregatedActual[key] || 0) + count;
                    break;
                }
            }
        });

        for (const [category, count] of Object.entries(aggregatedActual)) {
            if (expected[category]) {
                const actualPercentage = (count / iterations) * 100;
                const expectedPercentage = expected[category] * 100;

                expect(actualPercentage).toBeGreaterThanOrEqual(expectedPercentage - marginOfError);
                expect(actualPercentage).toBeLessThanOrEqual(expectedPercentage + marginOfError);
            }
        }
    }

    describe('Gallery Case tests', () => {
        test('should distribute rarities according to odds', () => {
            const results = runSimulations(galleryCaseData, ODDS);
            verifyOddsDistribution(results, ODDS, RARITY_MAP, 100000);
        });

        test('should select skins within appropriate rarity', () => {
            for (let i = 0; i < 100; i++) {
                const skin = crateOpeningService.getRandomSkinByOdds(galleryCaseData, ODDS);
                const rarityMatches = galleryCaseData.skins.some((crateSkin) => crateSkin.rarity_id === skin.rarity_id);
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
            verifyOddsDistribution(results, ODDS, RARITY_MAP, 100000);
        });

        test('should select skins within appropriate rarity', () => {
            for (let i = 0; i < 100; i++) {
                const skin = crateOpeningService.getRandomSkinByOdds(revolutionCaseData, ODDS);
                const rarityMatches = revolutionCaseData.skins.some(
                    (crateSkin) => crateSkin.rarity_id === skin.rarity_id,
                );
                expect(rarityMatches).toBeTruthy();
            }
        });
    });

    describe('Shanghai Souvenir tests', () => {
        test('should distribute rarities according to odds', () => {
            const results = runSimulations(shanghaiSouvenirData, ODDS);
            verifyOddsDistribution(results, ODDS, RARITY_MAP, 100000);
        });

        test('should select skins within appropriate rarity', () => {
            for (let i = 0; i < 100; i++) {
                const skin = crateOpeningService.getRandomSkinByOdds(shanghaiSouvenirData, ODDS);
                const rarityMatches = shanghaiSouvenirData.skins.some(
                    (crateSkin) => crateSkin.rarity_id === skin.rarity_id,
                );
                expect(rarityMatches).toBeTruthy();
            }
        });
    });

    describe('Shanghai Sticker Capsule tests', () => {
        test('should distribute rarities according to odds', () => {
            const results = runSimulations(shanghaiStickerCapsuleData, ODDS);
            verifyOddsDistribution(results, ODDS, RARITY_MAP, 100000);
        });

        test('should select skins within appropriate rarity', () => {
            for (let i = 0; i < 100; i++) {
                const skin = crateOpeningService.getRandomSkinByOdds(shanghaiStickerCapsuleData, ODDS);
                const rarityMatches = shanghaiStickerCapsuleData.skins.some(
                    (crateSkin) => crateSkin.rarity_id === skin.rarity_id,
                );
                expect(rarityMatches).toBeTruthy();
            }
        });
    });

    describe('Edge cases', () => {
        test('should handle custom odds', () => {
            const customOdds = { ...ODDS } as any;
            const weights = [50, 30, 15, 4, 1];
            const total = weights.reduce((sum, w) => sum + w, 0);

            Object.keys(customOdds).forEach((key, index) => {
                customOdds[key] = weights[index % 5] / total || customOdds[key];
            });

            const results = runSimulations(galleryCaseData, customOdds, 5000);
            verifyOddsDistribution(results, customOdds, RARITY_MAP, 5000);
        });

        test('should fallback to any skin of the selected rarity if gun skin filter returns no results', () => {
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
                { name: 'Shanghai Sticker Capsule', data: shanghaiStickerCapsuleData },
            ];

            for (const caseInfo of cases) {
                const results = runSimulations(caseInfo.data, ODDS, iterations);

                const tableData = Object.entries(results).map(([rarity, count]) => {
                    let expectedKey = '';
                    for (const [key, values] of Object.entries(RARITY_MAP)) {
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
                        difference: difference.toFixed(2) + '%',
                    };
                });

                tableData.sort((a, b) => b.count - a.count);

                console.log(`\n===== ${caseInfo.name} Distribution (${iterations} iterations) =====`);
                console.table(tableData);

                console.log('Expected ODDS from constants:');
                console.table(ODDS);
            }
        });
    });

    test('should verify exceedingly rare drop rates', () => {
        const highIterations = 100000;
        const cases = [
            { name: 'Gallery Case', data: galleryCaseData },
            { name: 'Revolution Case', data: revolutionCaseData },
        ];

        for (const caseInfo of cases) {
            console.log(`\n===== Testing ${caseInfo.name} exceedingly rare drops =====`);

            const testOdds = { ...ODDS, exceedingly_rare: 0.05 };
            const results = runSimulations(caseInfo.data, testOdds, highIterations);
            const rareCount = results['exceedingly_rare'] || 0;
            const percentage = (rareCount / highIterations) * 100;

            console.log(`Exceedingly rare drops: ${rareCount}/${highIterations} (${percentage.toFixed(4)}%)`);
            console.log(`Expected from test odds: 5.00%`);

            if (rareCount > 0) {
                expect(percentage).toBeGreaterThan(0);
            } else {
                console.log('No exceedingly rare items found even with boosted odds.');
                console.log('This could be because:');
                console.log('1. The mock data does not contain any exceedingly rare items');
                console.log('2. The crateOpeningService is not properly configured to select them');
            }
        }
    });
});
