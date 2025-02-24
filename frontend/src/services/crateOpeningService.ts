import { ERROR_MESSAGES, ODDS_TO_RARITY, SKIN_WEAR_AND_FLOAT, SLIDER_SIZE, WON_SKIN_INDEX } from '../constants';
import type { Crate, Skin } from '../types';
import { gunSkinFilter, knivesAndGlovesSkinFilter } from '../utils/sortAndfilters';

export const crateOpeningService = {
  openCrate,
};

function openCrate(crate: Crate, odds: Record<string, number>) {
  if (!crate.skins) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  let wonSkin = getRandomSkinByOdds(crate, odds);

  if (!['Sticker Capsule', 'Autograph Capsule'].includes(crate.type)) {
    wonSkin = { ...wonSkin, wear_category: getSkinWearCategory(wonSkin) };
  }
  const sliderSkins = getSkinsForSlider(crate, SLIDER_SIZE, odds, wonSkin);

  if (!sliderSkins.length || !wonSkin) throw new CrateServiceError(ERROR_MESSAGES.NO_SKINS_GENERATED);

  sliderSkins[WON_SKIN_INDEX] = wonSkin;
  return { sliderSkins, wonSkin, wonSkinIndex: WON_SKIN_INDEX };
}

function getRandomSkinByOdds(crate: Crate, odds: Record<string, number>): Skin {
  const skins = crate.skins;
  if (!skins?.length) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const rarity = getRandomSkinRarityForCrateByOdds(crate, odds);

  if (rarity === 'exceedingly_rare') {
    const eligibleSkins = skins.filter(knivesAndGlovesSkinFilter);
    if (!eligibleSkins.length) {
      return getRandomSkinByOdds(crate, { ...odds, exceedingly_rare: 0 });
    }
    return eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)];
  }

  let eligibleSkins = skins.filter((skin) => skin.rarity_id === rarity);

  while (!eligibleSkins.length) {
    eligibleSkins = skins.filter((skin) => skin.rarity_id === rarity);
  }
  return eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)];
}

function getRandomSkinRarityForCrateByOdds(crate: Crate, odds: Record<string, number>): string {
  if (crate.type === 'Souvenir') {
    delete odds.exceedingly_rare;
  }

  const availableRarities = new Set(crate.skins.map((skin) => skin.rarity_id));

  const filteredOddsEntries = Object.entries(odds).filter(([bucketKey]) => {
    if (bucketKey === 'exceedingly_rare' && odds.exceedingly_rare) {
      return true;
    }
    const bucketRarities = ODDS_TO_RARITY[bucketKey];
    return bucketRarities.some((rarity) => availableRarities.has(rarity));
  });

  const totalProbability = filteredOddsEntries.reduce((sum, [, prob]) => sum + prob, 0);
  const rand = Math.random() * totalProbability;

  let cumulative = 0;
  for (const [oddsRarity, probability] of filteredOddsEntries) {
    cumulative += probability;
    if (rand <= cumulative) {
      const mappedRarities = ODDS_TO_RARITY[oddsRarity];
      if (oddsRarity === 'exceedingly_rare') {
        return 'exceedingly_rare';
      }
      const availableMapped = mappedRarities.filter((rarity) => availableRarities.has(rarity));
      if (availableMapped.length) {
        return availableMapped[Math.floor(Math.random() * availableMapped.length)];
      }
    }
  }
  return ODDS_TO_RARITY.rare[Math.floor(Math.random() * ODDS_TO_RARITY.rare.length)];
}

function getSkinsForSlider(crate: Crate, count: number, odds: Record<string, number>, wonSkin: Skin): Skin[] {
  const crateSkins = crate.skins;
  if (!crateSkins?.length) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const randomSkins: Skin[] = [];
  let previousSkinId: string | undefined;

  while (randomSkins.length < count) {
    const idx = randomSkins.length; // current index being filled
    const randomSkin = (function selectSkin(_prevId: string | undefined, index: number): Skin {
      const randomRarity = getRandomSkinRarityForCrateByOdds(crate, odds);
      const candidate = crateSkins[Math.floor(Math.random() * crateSkins.length)];
      if (
        candidate.id === _prevId ||
        !gunSkinFilter(candidate) ||
        candidate.rarity_id !== randomRarity ||
        ((index === WON_SKIN_INDEX - 1 || index === WON_SKIN_INDEX + 1) && candidate.id === wonSkin.id)
      ) {
        return selectSkin(_prevId, index);
      }
      return candidate;
    })(previousSkinId, idx);

    previousSkinId = randomSkin.id;
    randomSkins.push(randomSkin);
  }

  return randomSkins;
}

export function getSkinWearCategory(skin: Skin): keyof typeof SKIN_WEAR_AND_FLOAT {
  const diceRoll = Math.random();
  let cumulative = 0;

  const validWearEntries = Object.entries(SKIN_WEAR_AND_FLOAT).filter(([wearKey]) =>
    skin.prices.find((price) => price.wear_category === wearKey),
  );

  for (const [wearKey, { odds }] of validWearEntries) {
    cumulative += odds;

    if (diceRoll <= cumulative) {
      return wearKey as keyof typeof SKIN_WEAR_AND_FLOAT;
    }
  }

  if (validWearEntries.length > 0) {
    return validWearEntries[validWearEntries.length - 1][0] as keyof typeof SKIN_WEAR_AND_FLOAT;
  }

  const keys = Object.keys(SKIN_WEAR_AND_FLOAT) as Array<keyof typeof SKIN_WEAR_AND_FLOAT>;
  return keys[keys.length - 1];
}

class CrateServiceError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'CrateServiceError';
  }
}
