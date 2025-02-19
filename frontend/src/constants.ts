export const SLIDER_SIZE = 50;
export const WON_SKIN_INDEX = 42;

export const ERROR_MESSAGES = {
  CRATE_HAS_NO_SKINS: 'Crate has no skins',
  NO_SKINS_GENERATED: 'No skins generated for case opening',
  NO_SKIN_GENERATED: 'No skin generated for case opening',
};

export const REAL_RARITY_ODDS = {
  rare: 0.7992, // 79.92%
  mythical: 0.1598, // 15.98%
  legendary: 0.032, // 3.2%
  ancient: 0.0064, // 0.64%
  exceedinglyRare: 0.0026, // 0.26%
} as const;

export const FUN_ODDS = {
  rare: 0.05,
  mythical: 0.1,
  legendary: 0.4,
  ancient: 0.3,
  exceedinglyRare: 0.15,
} as const;

export const RARITY = {
  rare: ['Consumer Grade', 'Industrial Grade', 'Mil-Spec Grade'],
  mythical: ['Restricted'],
  legendary: ['Classified'],
  ancient: ['Covert', 'Extraordinary'],
  exceedinglyRare: ['Knife', 'Gloves', 'Contraband'],
};

export const RARITY_MAPPED = {
  'Consumer Grade': 'rare',
  'Industrial Grade': 'rare',
  'Mil-Spec Grade': 'rare',
  Restricted: 'mythical',
  Classified: 'legendary',
  Covert: 'ancient',
  Extraordinary: 'ancient',
  Knife: 'exceedinglyRare',
  Gloves: 'exceedinglyRare',
  Contraband: 'exceedinglyRare',
};

export const RARITY_INDEX = {
  'Consumer Grade': 0,
  'Industrial Grade': 1,
  'Mil-Spec Grade': 2,
  Restricted: 3,
  Classified: 4,
  Covert: 5,
  Extraordinary: 6,
} as const;

export const SKIN_WEAR_AND_FLOAT = {
  'Factory New': { floatRange: [0, 0.07], odds: 0.025 }, // ~2-3%
  'Minimal Wear': { floatRange: [0.07, 0.15], odds: 0.165 }, // ~15-18%
  'Field-Tested': { floatRange: [0.15, 0.38], odds: 0.725 }, // ~70-75%
  'Well-Worn': { floatRange: [0.38, 0.45], odds: 0.06 }, // ~5-7%
  'Battle-Scarred': { floatRange: [0.45, 1], odds: 0.005 }, // ~0.3-1%
} as const;
