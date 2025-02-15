import { useQuery } from '@tanstack/vue-query';

export type Skin = {
  id: string;
  name: string;
  classid: string;
  type: string;
  weaponType: string;
  gunType: string;
  rarity: string;
  rarityColor: string;
  prices: string;
  parsedPrices: string;
  firstSaleDate: string;
  knifeType: string;
  image: string;
  minFloat: number;
  maxFloat: number;
  stattrak: boolean;
  caseId: string;
};

export function useSkins() {
  return useQuery<Skin[]>({
    queryKey: ['skins'],
    queryFn: async () => fetch(`http://localhost:5015/api/skins`).then((res) => res.json()),
  });
}
