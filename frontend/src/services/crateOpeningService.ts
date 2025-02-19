import { ERROR_MESSAGES, RARITY, SKIN_WEAR_AND_FLOAT, SLIDER_SIZE, WON_SKIN_INDEX } from '../constants';
import type { Crate, Skin } from '../types';

export const crateOpeningService = {
  openCrate,
  getRandomSkinsFromCrate,
  getRandomWonSkinFromCrateByOdds,
};

function openCrate(crate: Crate, odds: Record<string, number>) {
  if (!crate.skins) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const sliderSkins = getRandomSkinsFromCrate(crate, SLIDER_SIZE, odds);
  const wonSkin = getRandomWonSkinFromCrateByOdds(crate, odds);
  wonSkin.wearCategory = generateSkinWearCategory(wonSkin);

  if (!sliderSkins.length || !wonSkin) throw new CrateServiceError(ERROR_MESSAGES.NO_SKINS_GENERATED);

  sliderSkins[WON_SKIN_INDEX] = wonSkin;
  const wonSkinIndex = WON_SKIN_INDEX;

  return { sliderSkins, wonSkin, wonSkinIndex };
}

function isSliderEligibleForSlider(skin: Skin) {
  return !(skin.type === 'Gloves' || skin.weaponType === 'Knife');
}

function getRandomWonSkinFromCrateByOdds(crate: Crate, odds: Record<string, number>) {
  const skins = crate.skins;

  if (!skins?.length) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const rarity = getRandomSkinRarityForCrateByOdds(crate, odds);

  if (rarity === 'exceedinglyRare') {
    const eligibleSkins = skins.filter((skin) => skin.type === 'Gloves' || skin.weaponType === 'Knife');
    return { ...eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)] };
  }

  const eligibleSkins = skins.filter(
    (skin) => RARITY[rarity].includes(skin.rarity) && skin.type !== 'Gloves' && skin.weaponType !== 'Knife',
  );

  return { ...eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)] };
}

function getRandomSkinRarityForCrateByOdds(crate: Crate, odds: Record<string, number>) {
  if (crate.type === 'Souvenir') {
    delete odds.exceedinglyRare;
  }

  const rand = Math.random();
  let cumulative = 0;

  for (const [rarity, probability] of Object.entries(odds)) {
    cumulative += probability;
    if (rand <= cumulative) {
      return rarity as keyof typeof odds;
    }
  }

  return 'rare';
}

function getRandomSkinsFromCrate(crate: Crate, count: number, odds: Record<string, number>) {
  const crateSkins = crate.skins;

  if (!crateSkins?.length) throw new CrateServiceError(ERROR_MESSAGES.CRATE_HAS_NO_SKINS);

  const getRandomSkinRecursive = (_previousSkinId?: string) => {
    const randomSkinRarity = getRandomSkinRarityForCrateByOdds(crate, odds);
    const selectedSkin = crateSkins[Math.floor(Math.random() * crateSkins.length)];
    if (
      selectedSkin.id === _previousSkinId ||
      !isSliderEligibleForSlider(selectedSkin) ||
      !RARITY[randomSkinRarity].includes(selectedSkin.rarity)
    ) {
      return getRandomSkinRecursive(_previousSkinId);
    }
    return selectedSkin;
  };

  const randomSkins: Skin[] = [];
  let previousSkin: string | undefined;

  for (let i = 0; i < count; i++) {
    const randomSkin = getRandomSkinRecursive(previousSkin);
    previousSkin = randomSkin.id;
    randomSkins.push(randomSkin);
  }

  return randomSkins;
}

export function generateSkinWearCategory(skin: Skin) {
  const diceRoll = Math.random();
  let cumulative = 0;

  const validWearEntries = Object.entries(SKIN_WEAR_AND_FLOAT).filter(
    ([wearKey]) => skin.prices[wearKey as keyof typeof skin.prices],
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
