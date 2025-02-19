import type { RARITY_MAPPED } from './constants';

export type Crate = {
  id: string;
  name: string;
  type: string;
  firstSaleDate: string;
  marketHashName: string;
  rental: boolean;
  image: string;
  modelPlayer: string;
  skins?: Skin[];
};

type Price = {
  sold: string;
  median: number;
  average: number;
  lowest_price: number;
  highest_price: number;
  standard_deviation: string;
};

type Prices = {
  '7_days': Price;
  '30_days': Price;
  '24_hours'?: Price;
  all_time: Price;
};

export type Wear = 'Battle-Scarred' | 'Well-Worn' | 'Field-Tested' | 'Minimal Wear' | 'Factory New';

export type Skin = {
  id: string;
  name: string;
  classid: string;
  type: string;
  weaponType: string;
  gunType: string;
  rarity: keyof typeof RARITY_MAPPED;
  rarityColor: string;
  prices: Record<Wear, Prices>;
  parsedPrices: string;
  firstSaleDate: string;
  knifeType: string;
  image: string;
  minFloat: number;
  maxFloat: number;
  stattrak: boolean;
  caseId: string;
  wearCategory?: Wear;
};
