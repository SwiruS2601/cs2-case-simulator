import type { Crate, Skin } from '../types';

export function getSkinPrice(skin: Skin) {
  const foundSkin = skin.prices?.find((price) => price.wear_category === skin.wear_category);
  if (!foundSkin) return 0;

  const lastDay = foundSkin.steam_last_24h;
  if (lastDay) return lastDay;

  const lastWeek = foundSkin.steam_last_7d;
  if (lastWeek) return lastWeek;

  const lastMonth = foundSkin.steam_last_30d;
  if (lastMonth) return lastMonth;

  const lastThreeMonths = foundSkin.steam_last_90d;
  if (lastThreeMonths) return lastThreeMonths;

  return foundSkin.steam_last_ever || 0;
}

export function getCratePrice(crate: Crate | null) {
  if (!crate?.price) return 0;

  const lastDay = crate.price.steam_last_24h;
  if (lastDay) return lastDay;

  const lastWeek = crate.price.steam_last_7d;
  if (lastWeek) return lastWeek;

  const lastMonth = crate.price.steam_last_30d;
  if (lastMonth) return lastMonth;

  const lastThreeMonths = crate.price.steam_last_90d;
  if (lastThreeMonths) return lastThreeMonths;

  return crate.price.steam_last_ever || 0;
}
