import { useQuery } from '@tanstack/vue-query';
import type { Skin } from './skins';

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

export function useCreates() {
  return useQuery<Crate[]>({
    queryKey: ['crates'],
    queryFn: async () => fetch('http://localhost:5015/api/case').then((res) => res.json()),
  });
}

export function useCreate(id: string) {
  return useQuery<Crate>({
    queryKey: ['crate', id],
    queryFn: async () => fetch(`http://localhost:5015/api/case/${id}`).then((res) => res.json()),
  });
}
