import { RARITY, REAL_RARITY_ODDS } from '@/constants';
import type { Crate } from '@/query/crate';
import type { Skin } from '@/query/skins';

const commonTiers = ['Consumer Grade', 'Industrial Grade', 'Mil-Spec Grade'];
const defaultObj = { skins: [], wonSkin: null, wonSkinIndex: -1 };

function isSliderEligibleForSlider(skin: Skin) {
  return !(skin.type === 'Gloves' || skin.weaponType === 'Knife');
}

function determineRarity(odds: Record<string, number>, crate: Crate) {
  const rand = Math.random();
  let cumulative = 0;

  if (crate.type === 'Souvenir') {
    delete odds.exceedinglyRare;
  }

  for (const [rarity, probability] of Object.entries(odds)) {
    cumulative += probability;
    if (rand <= cumulative) {
      return rarity as keyof typeof odds;
    }
  }
  return 'rare';
}

function generateSkinsForCaseOpening(
  crate: Crate,
  _skins: Skin[],
  count = 50,
  odds: Record<string, number> = REAL_RARITY_ODDS,
) {
  if (!_skins.length) {
    console.error('No skins provided to generateSkinsForCaseOpening');
    return defaultObj;
  }

  if (!_skins[0]?.image || !_skins[0]?.name) {
    console.error('Skins are missing required properties:', _skins[0]);
    return defaultObj;
  }

  const commonSkins = _skins.filter((skin) => commonTiers.includes(skin.rarity) && isSliderEligibleForSlider(skin));
  const sliderEligibleSkins = _skins.filter(isSliderEligibleForSlider);

  const skins: Skin[] = [];
  for (let i = 0; i < count - 8; i++) {
    let selected: Skin;
    if (commonSkins.length && Math.random() < 0.85) {
      selected = commonSkins[Math.floor(Math.random() * commonSkins.length)];
    } else {
      selected = sliderEligibleSkins[Math.floor(Math.random() * sliderEligibleSkins.length)];
    }
    skins.push(selected);
  }

  const winningRarity = determineRarity(odds, crate);

  let wonSkin: Skin;

  if (winningRarity === 'exceedinglyRare') {
    const eligibleSkins = _skins.filter((skin) => skin.type === 'Gloves' || skin.weaponType === 'Knife');
    wonSkin = eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)];
  } else {
    const eligibleSkins = _skins.filter((skin) => RARITY[winningRarity].includes(skin.rarity));
    wonSkin = eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)];
  }

  if (!wonSkin) {
    console.error('No possible winning skins found for rarity:', winningRarity);
    return defaultObj;
  }

  skins.push(wonSkin);
  const wonSkinIndex = skins.length - 1;

  for (let i = 0; i < 7; i++) {
    skins.push(sliderEligibleSkins[Math.floor(Math.random() * sliderEligibleSkins.length)]);
  }

  return { skins, wonSkin, wonSkinIndex };
}

export const caseOpeningService = {
  generateSkinsForCaseOpening,
};
