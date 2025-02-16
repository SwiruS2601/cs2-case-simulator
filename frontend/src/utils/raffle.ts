import { REAL_RARITY_ODDS, RARITY } from '@/constants';
import type { Skin } from '@/query/skins';

export function determineRarity(): keyof typeof REAL_RARITY_ODDS {
  const rand = Math.random();
  let cumulative = 0;

  for (const [rarity, probability] of Object.entries(REAL_RARITY_ODDS)) {
    cumulative += probability;
    if (rand <= cumulative) {
      return rarity as keyof typeof REAL_RARITY_ODDS;
    }
  }

  return 'rare';
}

function isSliderEligible(skin: Skin): boolean {
  return !(skin.type === 'Gloves' || skin.weaponType === 'Knife');
}

export function generateRaffleItems(skins: Skin[], count = 50) {
  if (!skins.length) {
    console.error('No skins provided to generateRaffleItems');
    return { items: [], wonItem: null, winningIndex: -1 };
  }

  if (!skins[0]?.image || !skins[0]?.name) {
    console.error('Skins are missing required properties:', skins[0]);
    return { items: [], wonItem: null, winningIndex: -1 };
  }

  const commonTiers = ['Consumer Grade', 'Industrial Grade', 'Mil-Spec Grade'];
  const commonSkins = skins.filter((skin) => commonTiers.includes(skin.rarity) && isSliderEligible(skin));
  const eligibleSkins = skins.filter(isSliderEligible);

  const items: Skin[] = [];
  for (let i = 0; i < count - 8; i++) {
    let selected: Skin;
    if (commonSkins.length && Math.random() < 0.85) {
      selected = commonSkins[Math.floor(Math.random() * commonSkins.length)];
    } else {
      selected = eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)];
    }
    items.push(selected);
  }

  const winningRarity = determineRarity();
  const possibleWins = skins.filter(
    (skin) =>
      RARITY[winningRarity].includes(skin.rarity) ||
      RARITY[winningRarity].includes(skin.type) ||
      RARITY[winningRarity].includes(skin.weaponType),
  );

  if (!possibleWins.length) {
    console.error('No possible winning skins found for rarity:', winningRarity);
    return { items: [], wonItem: null, winningIndex: -1 };
  }

  const wonItem = possibleWins[Math.floor(Math.random() * possibleWins.length)];

  const winningIndex = items.length;
  items.push(wonItem);

  for (let i = 0; i < 7; i++) {
    items.push(eligibleSkins[Math.floor(Math.random() * eligibleSkins.length)]);
  }

  return { items, wonItem, winningIndex };
}
