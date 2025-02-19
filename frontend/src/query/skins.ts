import type { Skin } from '../types';
import { useQuery } from '@tanstack/vue-query';

export function useSkins() {
  return useQuery<Skin[]>({
    queryKey: ['skins'],
    queryFn: async () => fetch(`http://localhost:5015/api/skins`).then((res) => res.json()),
  });
}
