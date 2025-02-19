import { config } from '../config';
import type { Skin } from '../types';
import { useQuery } from '@tanstack/vue-query';

export function useSkins() {
  return useQuery<Skin[]>({
    queryKey: ['skins'],
    queryFn: async () => fetch(`${config.apiBaseUrl}/api/skins`).then((res) => res.json()),
  });
}
