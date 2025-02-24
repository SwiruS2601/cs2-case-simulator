import { ERROR_MESSAGES, ODDS_TO_RARITY, SKIN_WEAR_AND_FLOAT, SLIDER_SIZE, WON_SKIN_INDEX } from '../constants';
import type { Crate, Skin } from '../types';
import { gunSkinFilter, knivesAndGlovesSkinFilter } from '../utils/sortAndfilters';

export const crateOpeningService = {
  openCrate,
};

function openCrate(crate: Crate, odds: Record<string, number>) {
  if (!crate.skins) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const wonSkin = getRandomSkinByOdds(crate, odds);
  wonSkin.wear_category = getSkinWearCategory(wonSkin);
  const sliderSkins = getSkinsForSlider(crate, SLIDER_SIZE, odds, wonSkin);

  if (!sliderSkins.length || !wonSkin) throw new CrateServiceError(ERROR_MESSAGES.NO_SKINS_GENERATED);

  sliderSkins[WON_SKIN_INDEX] = wonSkin;
  const wonSkinIndex = WON_SKIN_INDEX;

  return { sliderSkins, wonSkin, wonSkinIndex };
}

function getRandomSkinByOdds(crate: Crate, odds: Record<string, number>) {
  const skins = crate.skins;

  if (!skins?.length) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const rarity = getRandomSkinRarityForCrateByOdds(crate, odds);

  if (rarity === 'exceedingly_rare') {
    const eligibleSkins = skins.filter(knivesAndGlovesSkinFilter);
    return { ...eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)] };
  }

  const eligibleSkins = skins.filter((skin) => {
    return gunSkinFilter(skin) && skin.rarity_id.includes(rarity.replace('_weapon', ''));
  });

  if (!eligibleSkins.length) {
    return getRandomSkinByOdds(crate, odds);
  }

  return { ...eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)] };
}

function getRandomSkinRarityForCrateByOdds(crate: Crate, odds: Record<string, number>) {
  if (crate.type === 'Souvenir') {
    delete odds.exceedingly_rare;
  }

  const rand = Math.random();
  let cumulative = 0;

  for (let [oddsRarity, probability] of Object.entries(odds)) {
    cumulative += probability;
    if (rand <= cumulative) {
      const mappedRarity = ODDS_TO_RARITY[oddsRarity];
      if (mappedRarity) {
        return mappedRarity[Math.floor(Math.random() * mappedRarity.length)];
      }
    }
  }

  return ODDS_TO_RARITY.rare[Math.floor(Math.random() * ODDS_TO_RARITY.rare.length)];
}

function getSkinsForSlider(crate: Crate, count: number, odds: Record<string, number>, wonSkin: Skin) {
  const crateSkins = crate.skins;

  if (!crateSkins?.length) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const getRandomSkinRecursive = (_previousSkinId?: string) => {
    const randomSkinRarity = getRandomSkinRarityForCrateByOdds(crate, odds);
    const selectedSkin = crateSkins[Math.floor(Math.random() * crateSkins.length)];

    if (
      selectedSkin.id === _previousSkinId ||
      !gunSkinFilter(selectedSkin) ||
      randomSkinRarity !== selectedSkin.rarity_id
    ) {
      return getRandomSkinRecursive(_previousSkinId);
    }

    return selectedSkin;
  };

  const randomSkins: Skin[] = [];
  let previousSkin: string | undefined;

  for (let i = 0; i < count; i++) {
    if (i === WON_SKIN_INDEX - 1 || i === WON_SKIN_INDEX + 1) {
      previousSkin = wonSkin.id;
    }
    const randomSkin = getRandomSkinRecursive(previousSkin);
    previousSkin = randomSkin.id;
    randomSkins.push(randomSkin);
  }

  return randomSkins;
}

export function getSkinWearCategory(skin: Skin) {
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
